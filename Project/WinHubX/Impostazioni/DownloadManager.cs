namespace WinHubX.Impostazioni
{
    public static class DownloadManager
    {
        public static int ProgressPercentage { get; private set; }
        public static bool IsDownloading { get; private set; }
        public static event Action<int>? ProgressChanged;
        public static event Action<bool>? DownloadStateChanged;

        private static readonly HttpClient _httpClient = CreateHttpClient();
        private static long _totalDownloadedBytes = 0;

        private static readonly SemaphoreSlim _downloadSemaphore = new(1, 1);
        private static CancellationTokenSource? _globalCts;
        private static readonly object _stateLock = new();

        private static HttpClient CreateHttpClient()
        {
            var handler = new SocketsHttpHandler
            {
                AllowAutoRedirect = false,
                MaxConnectionsPerServer = 8,
                AutomaticDecompression = System.Net.DecompressionMethods.None,
                PooledConnectionLifetime = TimeSpan.FromMinutes(5)
            };

            return new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMinutes(10)
            };
        }

        public static void ForceStopDownload()
        {
            lock (_stateLock)
            {
                _globalCts?.Cancel();
            }
        }

        public static async Task DownloadFileAsync(string url, string savePath, CancellationToken token, bool autoParallel = true, int maxChunks = 4, bool useBits = true)
        {
            _ = TrustedHttpsClient.ValidateUri(url, "download");
            if (string.IsNullOrWhiteSpace(savePath))
                throw new ArgumentException("Il percorso di destinazione è obbligatorio.", nameof(savePath));
            _ = Math.Clamp(maxChunks, 2, 8);
            string? directory = Path.GetDirectoryName(Path.GetFullPath(savePath));
            if (directory is not null)
                Directory.CreateDirectory(directory);

            string temporaryPath = $"{savePath}.{Guid.NewGuid():N}.download";
            await _downloadSemaphore.WaitAsync(token);
            CancellationTokenSource? globalCts = null;
            bool stateAnnounced = false;

            try
            {
                using CancellationTokenSource operationCts = new();
                globalCts = operationCts;
                lock (_stateLock)
                {
                    _globalCts = operationCts;
                    IsDownloading = true;
                    ProgressPercentage = 0;
                    _totalDownloadedBytes = 0;
                }

                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, operationCts.Token);
                CancellationToken linkedToken = linkedCts.Token;
                stateAnnounced = true;
                PublishDownloadState(true);
                PublishProgress(0);

                try
                {
                    linkedToken.ThrowIfCancellationRequested();

                    if (useBits)
                    {
                        await BitsTransferDownloader.DownloadFileAsync(
                            url,
                            temporaryPath,
                            progress =>
                            {
                                ProgressPercentage = progress;
                                PublishProgress(progress);
                            },
                            linkedToken);
                    }
                    else if (autoParallel && await SupportsParallelDownload(url, linkedToken))
                    {
                        await DownloadParallelAutoAsync(url, temporaryPath, linkedToken, maxChunks);
                    }
                    else
                    {
                        await DownloadSequentialAsync(url, temporaryPath, linkedToken);
                    }

                    File.Move(temporaryPath, savePath, true);
                }
                catch
                {
                    DeleteTemporaryFile(temporaryPath);
                    throw;
                }
            }
            finally
            {
                try
                {
                    lock (_stateLock)
                    {
                        IsDownloading = false;
                        if (ReferenceEquals(_globalCts, globalCts))
                            _globalCts = null;
                    }

                    if (stateAnnounced)
                        PublishDownloadState(false);
                }
                finally
                {
                    _downloadSemaphore.Release();
                }
            }
        }

        private static void PublishProgress(int progress)
        {
            if (ProgressChanged is not { } handlers)
                return;

            foreach (Action<int> handler in handlers.GetInvocationList())
            {
                try
                {
                    handler(progress);
                }
                catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
                {
                    System.Diagnostics.Debug.WriteLine($"Handler progresso download non riuscito: {ex}");
                }
            }
        }

        private static void PublishDownloadState(bool isDownloading)
        {
            if (DownloadStateChanged is not { } handlers)
                return;

            foreach (Action<bool> handler in handlers.GetInvocationList())
            {
                try
                {
                    handler(isDownloading);
                }
                catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
                {
                    System.Diagnostics.Debug.WriteLine($"Handler stato download non riuscito: {ex}");
                }
            }
        }

        private static void DeleteTemporaryFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                System.Diagnostics.Debug.WriteLine($"File temporaneo download non eliminato: {ex}");
                // Il file temporaneo non è mai esposto come output valido; il cleanup può
                // essere completato dal sistema operativo al successivo avvio.
            }
        }


        private static async Task<bool> SupportsParallelDownload(string url, CancellationToken token)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Head, url);
                using (var headResponse = await TrustedHttpsClient.SendAsync(_httpClient, request, token))
                {
                    headResponse.EnsureSuccessStatusCode();
                    // Verifica: 1) Supporta ranges, 2) Ha content length, 3) È abbastanza grande
                    bool supportsRanges = headResponse.Headers.AcceptRanges.Contains("bytes");
                    long contentLength = headResponse.Content.Headers.ContentLength ?? -1;

                    return supportsRanges && contentLength > 1024 * 1024; // > 1MB
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                // Se fallisce la verifica, usa il metodo sequenziale
                return false;
            }
        }

        private static async Task DownloadParallelAutoAsync(string url, string savePath, CancellationToken token, int maxChunks)
        {
            // Prima otteniamo le info complete sul file
            long totalBytes;
            using var request = new HttpRequestMessage(HttpMethod.Head, url);
            using (var headResponse = await TrustedHttpsClient.SendAsync(_httpClient, request, token))
            {
                headResponse.EnsureSuccessStatusCode();
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
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using (var response = await TrustedHttpsClient.SendAsync(_httpClient, request, token))
            {
                response.EnsureSuccessStatusCode();
                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                var canReportProgress = totalBytes != -1;

                using (var contentStream = await response.Content.ReadAsStreamAsync(token))
                using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write,
                    FileShare.None, 65536, FileOptions.Asynchronous | FileOptions.SequentialScan))
                {
                    var buffer = new byte[65536];
                    long totalRead = 0;
                    int bytesRead;
                    int lastReportedProgress = -1;

                    while ((bytesRead = await contentStream.ReadAsync(buffer.AsMemory(), token)) > 0)
                    {
                        token.ThrowIfCancellationRequested();
                        await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), token);
                        totalRead += bytesRead;

                        if (canReportProgress)
                        {
                            int progress = (int)((totalRead * 100) / totalBytes);

                            if (progress != lastReportedProgress && progress % 2 == 0) // Report ogni 2%
                            {
                                ProgressPercentage = progress;
                                PublishProgress(progress);
                                lastReportedProgress = progress;
                            }
                        }
                    }
                }
            }

            ProgressPercentage = 100;
            PublishProgress(100);
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
            PublishProgress(100);
        }

        private static async Task DownloadChunkAsync(string url, string savePath, long start, long end, int chunkIndex, object progressLock, long totalBytes, CancellationToken token)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(start, end);

            using (var response = await TrustedHttpsClient.SendAsync(_httpClient, request, token))
            {
                response.EnsureSuccessStatusCode();
                if (response.StatusCode != System.Net.HttpStatusCode.PartialContent)
                {
                    throw new HttpRequestException("Il server non ha rispettato l'intervallo richiesto per il download.");
                }

                long expectedChunkBytes = end - start + 1;
                var contentRange = response.Content.Headers.ContentRange;
                if (contentRange is null ||
                    !string.Equals(contentRange.Unit, "bytes", StringComparison.OrdinalIgnoreCase) ||
                    contentRange.From != start ||
                    contentRange.To != end ||
                    contentRange.Length != totalBytes)
                {
                    throw new HttpRequestException("L'intervallo restituito dal server non corrisponde a quello richiesto.");
                }

                if (response.Content.Headers.ContentLength is long contentLength && contentLength != expectedChunkBytes)
                {
                    throw new EndOfStreamException("La dimensione del blocco scaricato non corrisponde a quella attesa.");
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(token))
                using (var fileStream = new FileStream(savePath, FileMode.Open, FileAccess.Write, FileShare.Write, 65536, true))
                {
                    var buffer = new byte[65536];
                    int bytesRead;
                    long chunkDownloaded = 0;

                    while ((bytesRead = await contentStream.ReadAsync(buffer.AsMemory(), token)) > 0)
                    {
                        token.ThrowIfCancellationRequested();
                        fileStream.Seek(start + chunkDownloaded, SeekOrigin.Begin);
                        await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), token);
                        chunkDownloaded += bytesRead;

                        // Aggiorna il progresso tramite un metodo thread-safe
                        UpdateDownloadProgress(bytesRead, totalBytes, progressLock);
                    }

                    if (chunkDownloaded != expectedChunkBytes)
                    {
                        throw new EndOfStreamException("Il download del blocco è terminato prima di ricevere tutti i byte previsti.");
                    }
                }
            }
        }

        private static void UpdateDownloadProgress(long bytesDownloaded, long totalBytes, object progressLock)
        {
            int? progressToPublish = null;
            lock (progressLock)
            {
                // Usiamo una variabile statica per tracciare il totale
                _totalDownloadedBytes += bytesDownloaded;
                int progress = (int)((_totalDownloadedBytes * 100) / totalBytes);

                if (progress != ProgressPercentage && progress % 2 == 0)
                {
                    ProgressPercentage = progress;
                    progressToPublish = progress;
                }
            }

            // Gli handler UI possono reentrare: non invocarli mentre il lock è detenuto.
            if (progressToPublish.HasValue)
                PublishProgress(progressToPublish.Value);
        }
    }
}
