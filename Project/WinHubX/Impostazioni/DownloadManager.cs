namespace WinHubX.Impostazioni
{
    public static class DownloadManager
    {
        public static int ProgressPercentage { get; private set; }
        public static bool IsDownloading { get; private set; }
        public static event Action<int> ProgressChanged;
        public static event Action<bool> DownloadStateChanged;

        private static readonly HttpClient _httpClient = new HttpClient();
        private static long _totalDownloadedBytes = 0;

        // 🔥 AGGIUNGI: CancellationTokenSource statico per gestire la cancellazione globale
        private static CancellationTokenSource _globalCts;

        // 🔥 AGGIUNGI: Metodo per forzare l'interruzione
        public static void ForceStopDownload()
        {
            _globalCts?.Cancel();
            IsDownloading = false;
            DownloadStateChanged?.Invoke(false);
        }

        public static async Task DownloadFileAsync(string url, string savePath, CancellationToken token, bool autoParallel = true, int maxChunks = 4)
        {
            // 🔥 CREA UN LINKED TOKEN SOURCE per combinare token esterno e globale
            _globalCts = new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, _globalCts.Token);
            var linkedToken = linkedCts.Token;

            IsDownloading = true;
            DownloadStateChanged?.Invoke(true);
            ProgressPercentage = 0;
            ProgressChanged?.Invoke(0);

            try
            {
                linkedToken.ThrowIfCancellationRequested();

                if (autoParallel)
                {
                    bool useParallel = await SupportsParallelDownload(url, linkedToken);
                    if (useParallel)
                    {
                        await DownloadParallelAutoAsync(url, savePath, linkedToken, maxChunks);
                        return;
                    }
                }

                await DownloadSequentialAsync(url, savePath, linkedToken);
            }
            catch (OperationCanceledException)
            {
                if (File.Exists(savePath))
                    File.Delete(savePath);
                throw;
            }
            catch (Exception)
            {
                if (File.Exists(savePath))
                    File.Delete(savePath);
                throw;
            }
            finally
            {
                IsDownloading = false;
                DownloadStateChanged?.Invoke(false);
                _globalCts?.Dispose();
                _globalCts = null;
            }
        }


        private static async Task<bool> SupportsParallelDownload(string url, CancellationToken token)
        {
            try
            {
                using (var headResponse = await _httpClient.SendAsync(
                    new HttpRequestMessage(HttpMethod.Head, url), token))
                {
                    // Verifica: 1) Supporta ranges, 2) Ha content length, 3) È abbastanza grande
                    bool supportsRanges = headResponse.Headers.AcceptRanges.Contains("bytes");
                    long contentLength = headResponse.Content.Headers.ContentLength ?? -1;

                    return supportsRanges && contentLength > 1024 * 1024; // > 1MB
                }
            }
            catch
            {
                // Se fallisce la verifica, usa il metodo sequenziale
                return false;
            }
        }

        private static async Task DownloadParallelAutoAsync(string url, string savePath, CancellationToken token, int maxChunks)
        {
            // Prima otteniamo le info complete sul file
            long totalBytes;
            using (var headResponse = await _httpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Head, url), token))
            {
                totalBytes = headResponse.Content.Headers.ContentLength ?? -1;
            }

            // Calcola il numero ottimale di chunks
            int optimalChunks = CalculateOptimalChunks(totalBytes, maxChunks);

            await DownloadWithRanges(url, savePath, totalBytes, optimalChunks, token);
        }

        private static int CalculateOptimalChunks(long fileSize, int maxChunks)
        {
            if (fileSize <= 5 * 1024 * 1024) return 2;          // < 5MB: 2 chunks
            if (fileSize <= 20 * 1024 * 1024) return 3;         // < 20MB: 3 chunks  
            if (fileSize <= 100 * 1024 * 1024) return 4;        // < 100MB: 4 chunks
            return Math.Min(maxChunks, 8);                      // > 100MB: max 8 chunks
        }

        private static async Task DownloadSequentialAsync(string url, string savePath, CancellationToken token)
        {
            using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token))
            {
                response.EnsureSuccessStatusCode();
                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                var canReportProgress = totalBytes != -1;

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true))
                {
                    var buffer = new byte[65536];
                    long totalRead = 0;
                    int bytesRead;
                    int lastReportedProgress = -1;

                    while ((bytesRead = await contentStream.ReadAsync(buffer, token)) > 0)
                    {
                        token.ThrowIfCancellationRequested();
                        await fileStream.WriteAsync(buffer, 0, bytesRead, token);
                        totalRead += bytesRead;

                        if (canReportProgress)
                        {
                            int progress = (int)((totalRead * 100) / totalBytes);

                            if (progress != lastReportedProgress && progress % 2 == 0) // Report ogni 2%
                            {
                                ProgressPercentage = progress;
                                ProgressChanged?.Invoke(progress);
                                lastReportedProgress = progress;
                            }
                        }
                    }
                }
            }

            ProgressPercentage = 100;
            ProgressChanged?.Invoke(100);
        }

        private static async Task DownloadWithRanges(string url, string savePath, long totalBytes, int chunks, CancellationToken token)
        {
            // Reset del progresso totale
            _totalDownloadedBytes = 0;

            // Pre-alloca il file
            using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true))
            {
                fileStream.SetLength(totalBytes);
            }

            var tasks = new List<Task>();
            var progressLock = new object();

            for (int i = 0; i < chunks; i++)
            {
                var start = i * (totalBytes / chunks);
                var end = (i == chunks - 1) ? totalBytes - 1 : start + (totalBytes / chunks) - 1;

                // Rimuovi il ref dal parametro
                tasks.Add(DownloadChunkAsync(url, savePath, start, end, i, progressLock, totalBytes, token));
            }

            await Task.WhenAll(tasks);
            ProgressPercentage = 100;
            ProgressChanged?.Invoke(100);
        }

        private static async Task DownloadChunkAsync(string url, string savePath, long start, long end, int chunkIndex, object progressLock, long totalBytes, CancellationToken token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(start, end);

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token))
            {
                response.EnsureSuccessStatusCode();

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(savePath, FileMode.Open, FileAccess.Write, FileShare.Write, 65536, true))
                {
                    var buffer = new byte[65536];
                    int bytesRead;
                    long chunkDownloaded = 0;

                    while ((bytesRead = await contentStream.ReadAsync(buffer, token)) > 0)
                    {
                        token.ThrowIfCancellationRequested();
                        fileStream.Seek(start + chunkDownloaded, SeekOrigin.Begin);
                        await fileStream.WriteAsync(buffer, 0, bytesRead, token);
                        chunkDownloaded += bytesRead;

                        // Aggiorna il progresso tramite un metodo thread-safe
                        UpdateDownloadProgress(bytesRead, totalBytes, progressLock);
                    }
                }
            }
        }

        private static void UpdateDownloadProgress(long bytesDownloaded, long totalBytes, object progressLock)
        {
            lock (progressLock)
            {
                // Usiamo una variabile statica per tracciare il totale
                _totalDownloadedBytes += bytesDownloaded;
                int progress = (int)((_totalDownloadedBytes * 100) / totalBytes);

                if (progress != ProgressPercentage && progress % 2 == 0)
                {
                    ProgressPercentage = progress;
                    ProgressChanged?.Invoke(progress);
                }
            }
        }
    }
}