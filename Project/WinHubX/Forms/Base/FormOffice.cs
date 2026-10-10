using HartUI.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using WinHubX.Forms.Personalizzazione_office;
using WinHubX.Impostazioni;

namespace WinHubX
{
    public partial class FormOffice : Form
    {
        private static bool IsExpectedOfficeOperationFailure(Exception exception) =>
            exception is IOException
                or HttpRequestException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or System.ComponentModel.Win32Exception
                or System.Runtime.InteropServices.COMException
                or System.Management.ManagementException
                or InvalidOperationException
                or NotSupportedException
                or OperationCanceledException
                or TimeoutException
                or ArgumentException;

        private readonly Form1 form1;
        private readonly NotifyIcon notifyIcon;
        private List<OfficeVersion> officeVersions = new();
        private string selectedOfficeVersion = string.Empty;
        private string selectedLanguage = string.Empty;
        private string selectedInstallationType = string.Empty;
        private string percorsoCompleto = string.Empty;
        private CancellationTokenSource? _cts;
        private readonly CancellationTokenSource _lifetimeCancellation = new();
        private int _activeLifetimeOperations;
        private int _lifetimeDisposalRequested;
        private int _lifetimeDisposed;
        private static readonly HttpClient ResourceClient = CreateResourceClient();
        private readonly Action<int> _downloadProgressHandler;
        private readonly Action<bool> _downloadStateHandler;

        private static HttpClient CreateResourceClient()
        {
            var handler = new SocketsHttpHandler
            {
                AllowAutoRedirect = false,
                MaxConnectionsPerServer = 4,
                AutomaticDecompression = System.Net.DecompressionMethods.None,
                PooledConnectionLifetime = TimeSpan.FromMinutes(5)
            };
            return new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(2) };
        }

        public FormOffice(Form1 form1)
        {
            InitializeComponent();
            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = false
            };
            this.form1 = form1;
            _downloadProgressHandler = HandleDownloadProgressChanged;
            _downloadStateHandler = HandleDownloadStateChanged;
            FormClosed += FormOffice_FormClosed;

            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            string downloadPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            labelpercorso.Text = $"{downloadPath}";
            toolTip1.SetToolTip(labelpercorso, downloadPath);
            percorsoCompleto = downloadPath;
            AggiornaPercorsoLabel(downloadPath);

            WinHubX.Impostazioni.DownloadManager.ProgressChanged += _downloadProgressHandler;
            WinHubX.Impostazioni.DownloadManager.DownloadStateChanged += _downloadStateHandler;
            SetDownloadButtonStyle(WinHubX.Impostazioni.DownloadManager.IsDownloading);
            if (WinHubX.Impostazioni.DownloadManager.IsDownloading)
            {
                UpdateProgress(WinHubX.Impostazioni.DownloadManager.ProgressPercentage);
            }

            LanguageManager.LoadLanguageFromSettings();
            btnAttivaOfficePrinci.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Attiva Office",
                "en" => "  Activate Office",
                _ => btnAttivaOfficePrinci.Content
            };
            btnScrubberPrinci.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Disinstalla Office",
                "en" => "  Scrubber Office",
                _ => btnScrubberPrinci.Content
            };
            btnPersonalizzaOfficePrinci.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => " Crea versione personalizzata",
                "en" => " Create custom version",
                _ => btnPersonalizzaOfficePrinci.Content
            };
            btnAggRimAppOfficePrinci.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => " Aggiungi/Rimuovi app",
                "en" => "Add/Remove apps",
                _ => btnAggRimAppOfficePrinci.Content
            };
        }

        private void HandleDownloadProgressChanged(int progress)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action(() => UpdateProgress(progress)));
                }
                catch (InvalidOperationException ex)
                {
                    Debug.WriteLine($"Aggiornamento progresso Office ignorato dopo la chiusura: {ex.Message}");
                }
                return;
            }

            UpdateProgress(progress);
        }

        private void HandleDownloadStateChanged(bool isDownloading)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action(() => SetDownloadButtonStyle(isDownloading)));
                }
                catch (InvalidOperationException ex)
                {
                    Debug.WriteLine($"Aggiornamento stato Office ignorato dopo la chiusura: {ex.Message}");
                }
                return;
            }

            SetDownloadButtonStyle(isDownloading);
        }

        private void FormOffice_FormClosed(object? sender, FormClosedEventArgs e)
        {
            if (Interlocked.Exchange(ref _lifetimeDisposalRequested, 1) == 0)
                _lifetimeCancellation.Cancel();
            _cts?.Cancel();
            WinHubX.Impostazioni.DownloadManager.ProgressChanged -= _downloadProgressHandler;
            WinHubX.Impostazioni.DownloadManager.DownloadStateChanged -= _downloadStateHandler;
            notifyIcon.Dispose();
            DisposeLifetimeCancellationIfIdle();
        }

        private CancellationToken BeginLifetimeOperation()
        {
            if (Volatile.Read(ref _lifetimeDisposalRequested) != 0)
                throw new OperationCanceledException("La finestra Office è in chiusura.");

            Interlocked.Increment(ref _activeLifetimeOperations);
            return _lifetimeCancellation.Token;
        }

        private void EndLifetimeOperation()
        {
            if (Interlocked.Decrement(ref _activeLifetimeOperations) == 0)
                DisposeLifetimeCancellationIfIdle();
        }

        private void DisposeLifetimeCancellationIfIdle()
        {
            if (Volatile.Read(ref _lifetimeDisposalRequested) != 0 &&
                Volatile.Read(ref _activeLifetimeOperations) == 0 &&
                Interlocked.Exchange(ref _lifetimeDisposed, 1) == 0)
            {
                _lifetimeCancellation.Dispose();
            }
        }
        private void UpdateProgress(int progress)
        {
            progressBar1.Visible = true;
            progressBar1.Value = Math.Min(progress, 100);
            label2.Visible = true;
            label2.Text = $"{progress}%";
        }
        #region AttivaOffice
        private void btnAttivaOffice_Click(object? sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://account.microsoft.com/services") { UseShellExecute = true });
            }
            catch (Exception ex) when (IsExpectedOfficeOperationFailure(ex))
            {
                MessageBox.Show($"Impossibile aprire la gestione ufficiale dell’abbonamento Office: {ex.Message}",
                    "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static async Task<bool> IsInternetAvailableAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                using var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken,
                    timeout.Token);
                using HttpResponseMessage result = await TrustedHttpsClient.GetAsync(
                    ResourceClient, "https://www.microsoft.com/", linkedToken.Token);
                return result.IsSuccessStatusCode;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (IsExpectedOfficeOperationFailure(ex))
            {
                Debug.WriteLine($"Verifica connettività Office non riuscita: {ex}");
                return false;
            }
        }

        #endregion

        private async Task<string> OttieniURL(string jsonUrl, CancellationToken cancellationToken)
        {
            var response = await TrustedHttpsClient.GetStringAsync(ResourceClient, jsonUrl, cancellationToken);
            var json = JObject.Parse(response);
            string url = json["FormOffice"]?["scrubber"]?.Value<string>()
                ?? throw new InvalidOperationException("URL scrubber non presente nella configurazione.");
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri) || uri.Scheme != Uri.UriSchemeHttps)
                throw new InvalidOperationException("URL scrubber non valido: è richiesto HTTPS.");
            return uri.ToString();
        }

        private async void btnScrubber_Click(object? sender, EventArgs e)
        {
            string? tempFolder = null;
            CancellationToken cancellationToken;
            try
            {
                cancellationToken = BeginLifetimeOperation();
            }
            catch (OperationCanceledException)
            {
                return;
            }

            try
            {
                string zipFileUrl = string.Empty;

                if (await IsInternetAvailableAsync(cancellationToken))
                {
                    zipFileUrl = await OttieniURL(Dipendenze.GitHubConfigUrl, cancellationToken);

                    if (string.IsNullOrEmpty(zipFileUrl))
                        throw new Exception(LanguageManager.GetTranslation("FormOffice", "url_non_trovato_github"));
                }
                else
                {
                    MessageBox.Show(
                        LanguageManager.GetTranslation("Global", "nointernet"),
                        LanguageManager.GetTranslation("FormOffice", "errore"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
                tempFolder = PrivateUserWorkspace.CreateSession();
                string tempZipPath = Path.Join(tempFolder, "OfficeScrubber.zip");
                string extractionFolder = Path.Join(tempFolder, "extracted");

                Directory.CreateDirectory(tempFolder);
                await DownloadManager.DownloadFileAsync(zipFileUrl, tempZipPath,
                    cancellationToken, autoParallel: false);
                SafeZipExtractor.ExtractToFreshDirectory(tempZipPath, extractionFolder);
                string cmdPath = Path.Join(extractionFolder, "OfficeScrubber.cmd");

                if (!File.Exists(cmdPath))
                {
                    MessageBox.Show(
                        LanguageManager.GetTranslation("FormOffice", "file_non_trovato"),
                        LanguageManager.GetTranslation("FormOffice", "errore"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                using Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = Path.Join(Environment.SystemDirectory, "cmd.exe"),
                        WorkingDirectory = extractionFolder,
                        Verb = "runas",
                        UseShellExecute = true
                    }
                };
                process.StartInfo.ArgumentList.Add("/d");
                process.StartInfo.ArgumentList.Add("/c");
                process.StartInfo.ArgumentList.Add(cmdPath);

                process.Start();
                // Il processo usa gli script estratti: attendi prima di rimuovere la directory temporanea.
                await process.WaitForExitAsync(CancellationToken.None);
                await AttendiScrubberConTitolo("Office Scrubber v12", CancellationToken.None);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                Debug.WriteLine("Operazione Scrubber annullata durante la chiusura della finestra Office.");
            }
            catch (Exception ex) when (IsExpectedOfficeOperationFailure(ex))
            {
                MessageBox.Show(
                    $"{LanguageManager.GetTranslation("FormOffice", "errore_generico_scrubber")} {ex.Message}",
                    LanguageManager.GetTranslation("FormOffice", "errore"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                try
                {
                    if (tempFolder is not null)
                    {
                        try
                        {
                            if (Directory.Exists(tempFolder))
                                Directory.Delete(tempFolder, recursive: true);
                        }
                        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                        {
                            Debug.WriteLine($"Directory temporanea Scrubber non rimossa: {ex}");
                        }
                    }
                }
                finally
                {
                    EndLifetimeOperation();
                }
            }
        }

        private async Task AttendiScrubberConTitolo(
            string titolo,
            CancellationToken cancellationToken,
            int timeoutMs = 10 * 60 * 1000)
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < timeoutMs)
            {
                int? processId = await Task.Run(
                    () => FindScrubberProcessId(titolo),
                    cancellationToken);
                if (processId is int id)
                {
                    try
                    {
                        using Process scrubberProcess = Process.GetProcessById(id);
                        await scrubberProcess.WaitForExitAsync(cancellationToken);
                    }
                    catch (ArgumentException)
                    {
                        // Il processo può terminare fra la ricerca e il recupero del PID.
                    }
                    return;
                }

                await Task.Delay(1000, cancellationToken);
            }

            throw new TimeoutException($"Il processo {titolo} non è comparso entro il tempo previsto.");
        }

        private static int? FindScrubberProcessId(string title)
        {
            foreach (Process process in Process.GetProcessesByName("powershell"))
            {
                using (process)
                {
                    try
                    {
                        if (process.MainWindowTitle.Contains(title, StringComparison.OrdinalIgnoreCase))
                        {
                            return process.Id;
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        Debug.WriteLine($"Processo PowerShell terminato durante l'ispezione: {ex.Message}");
                    }
                    catch (System.ComponentModel.Win32Exception ex)
                    {
                        Debug.WriteLine($"Titolo finestra PowerShell non accessibile: {ex.Message}");
                    }
                }
            }

            return null;
        }

        private void PictureBox3_Click_BackToOffice(object? sender, EventArgs e)
        {
            Form1? mainForm = Application.OpenForms["Form1"] as Form1;
            if (mainForm == null) return;

            mainForm.pictureBox3.Visible = false;
            mainForm.LoadForm(new FormOffice(mainForm), mainForm.btnOffice, "Office");
        }

        private async Task<List<OfficeVersion>> CaricaOfficeVersions(
            string jsonUrl,
            CancellationToken cancellationToken)
        {
            List<OfficeVersion> loadedOfficeVersions = new();

            {
                string jsonResponse = await TrustedHttpsClient.GetStringAsync(
                    ResourceClient,
                    jsonUrl,
                    cancellationToken);
                var jsonObject = JObject.Parse(jsonResponse);
                foreach (var prop in jsonObject.Properties().Where(p => p.Name.StartsWith("Office")))
                {
                    string nomeOffice = prop.Name;
                    nomeOffice = Regex.Replace(nomeOffice, @"^Office(\d+)$", "Office $1");

                    var office = new OfficeVersion
                    {
                        Nome = nomeOffice,
                        Lingue = new Dictionary<string, Dictionary<string, string>>()
                    };

                    var officeObj = (JObject)prop.Value;

                    foreach (var lang in officeObj.Properties())
                    {
                        var links = new Dictionary<string, string>();

                        foreach (var kvp in (JObject)lang.Value)
                        {
                            string chiave = kvp.Key;
                            string? url = kvp.Value?.ToString();
                            if (string.IsNullOrWhiteSpace(url))
                                continue;
                            if (chiave.Equals("Officex64", StringComparison.OrdinalIgnoreCase) ||
                                chiave.Equals("Officex32", StringComparison.OrdinalIgnoreCase))
                                chiave = "Online";
                            else if (chiave.StartsWith("Offline", StringComparison.OrdinalIgnoreCase))
                                chiave = "Offline";
                            else if (chiave.Equals("officehash", StringComparison.OrdinalIgnoreCase))
                                continue;

                            links[chiave] = url;
                        }

                        office.Lingue.Add(lang.Name, links);
                    }

                    loadedOfficeVersions.Add(office);
                }
            }

            return loadedOfficeVersions;
        }


        private void comboBoxVerOffice_SelectedIndexChanged(object? sender, EventArgs e)
        {
            selectedOfficeVersion = comboBoxVerOffice.SelectedItem?.ToString() ?? string.Empty;

            comboBox_Lingua.Items.Clear();
            if (!string.IsNullOrEmpty(selectedOfficeVersion))
            {
                var office = officeVersions.FirstOrDefault(o => o.Nome == selectedOfficeVersion);
                if (office != null)
                {
                    comboBox_Lingua.Items.AddRange(office.Lingue.Keys.ToArray());
                }
            }
        }


        private void comboBox_Lingua_SelectedIndexChanged(object? sender, EventArgs e)
        {
            selectedLanguage = comboBox_Lingua.SelectedItem?.ToString() ?? string.Empty;

            comboBoxInstallazione.Items.Clear();
            if (!string.IsNullOrEmpty(selectedOfficeVersion) && !string.IsNullOrEmpty(selectedLanguage))
            {
                var office = officeVersions.First(o => o.Nome == selectedOfficeVersion);
                if (office.Lingue.TryGetValue(selectedLanguage, out var options))
                {
                    comboBoxInstallazione.Items.AddRange(options.Keys.ToArray());
                }
            }
        }
        private void comboBoxInstallazione_SelectedIndexChanged(object? sender, EventArgs? e)
        {
            string selezione = comboBoxInstallazione.SelectedItem?.ToString() ?? "";
            selectedInstallationType = selezione;
            selectedOfficeVersion = comboBoxVerOffice.SelectedItem?.ToString() ?? string.Empty;
            labelavviso.Visible = selezione.Equals("Online", StringComparison.OrdinalIgnoreCase);
            Checkbox_Salva.Visible = selezione.Equals("Offline", StringComparison.OrdinalIgnoreCase);
            Checkbox_Installa.Visible = selezione.Equals("Offline", StringComparison.OrdinalIgnoreCase);
            labelpercorso.Visible = selezione.Equals("Offline", StringComparison.OrdinalIgnoreCase);
            label1.Visible = selezione.Equals("Offline", StringComparison.OrdinalIgnoreCase);
            btn_cambiaBianco.Visible = selezione.Equals("Offline", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetHardwareArchitecture()
        {
            try
            {
                string hwPath = Path.Join(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "WinHubX", "Computer", "osehardware.json");

                if (File.Exists(hwPath))
                {
                    var json = File.ReadAllText(hwPath);
                    var hwInfo = JsonConvert.DeserializeObject<WinHubX.Impostazioni.HardwareInfo>(json);

                    string? arch = hwInfo?.Architettura?.Trim()?.ToLowerInvariant();
                    if (arch != null)
                    {
                        if (arch.Contains("arm64")) return "x64";
                        if (arch.Contains("64")) return "x64";
                        if (arch.Contains("32")) return "x32";
                    }
                }
            }
            catch (Exception ex) when (IsExpectedOfficeOperationFailure(ex))
            {
                Debug.WriteLine($"Rilevamento architettura Office non riuscito; uso l'architettura del sistema: {ex}");
            }

            return Environment.Is64BitOperatingSystem ? "x64" : "x32";
        }

        private async void btnDownload_Click(object? sender, EventArgs e)
        {
            string hardwarePath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WinHubX", "Computer", "osehardware.json");

            if (!File.Exists(hardwarePath))
            {
                var popup = new WinHubX.DialogBlock.Form_DialogBlock(form1);
                var panelCenter = panel50.PointToScreen(new Point(panel50.Width / 2, panel50.Height / 2));
                popup.StartPosition = FormStartPosition.Manual;
                popup.Location = new Point(
                    panelCenter.X - popup.Width / 2,
                    panelCenter.Y - popup.Height / 2
                );

                popup.ShowDialog();
                return;
            }

            if (WinHubX.Impostazioni.DownloadManager.IsDownloading)
            {
                _cts?.Cancel();
                WinHubX.Impostazioni.DownloadManager.ForceStopDownload();
                await Task.Delay(1000, CancellationToken.None);
                SetDownloadButtonStyle(false);
                return;
            }

            if (string.IsNullOrEmpty(selectedInstallationType) ||
                string.IsNullOrEmpty(selectedOfficeVersion) ||
                string.IsNullOrEmpty(selectedLanguage))
            {
                MessageBox.Show(LanguageManager.GetTranslation("FormOffice", "seleziona_versione_tipo_installazione"));
                return;
            }

            string? tempFile = null;
            string? privateWorkspacePath = null;
            string? savePath = null;
            SetDownloadButtonStyle(true);
            using CancellationTokenSource downloadCancellation = new();
            _cts = downloadCancellation;

            try
            {
                downloadCancellation.Token.ThrowIfCancellationRequested();

                var office = officeVersions.FirstOrDefault(o => o.Nome == selectedOfficeVersion);
                if (office == null)
                    throw new Exception(LanguageManager.GetTranslation("FormOffice", "versione_non_trovata"));

                if (!office.Lingue.TryGetValue(selectedLanguage, out var links))
                    throw new Exception(LanguageManager.GetTranslation("FormOffice", "lingua_non_trovata"));

                string arch = GetHardwareArchitecture();
                string? url = null;

                if (selectedInstallationType.Equals("Offline", StringComparison.OrdinalIgnoreCase))
                {
                    url = links.GetValueOrDefault("Offline");
                }
                else if (selectedInstallationType.Equals("Online", StringComparison.OrdinalIgnoreCase))
                {
                    string architectureToken = arch == "x64" ? "x64" : "x32";
                    url = links.FirstOrDefault(link => link.Key.Contains(architectureToken, StringComparison.OrdinalIgnoreCase)).Value
                          ?? links.GetValueOrDefault("Online");
                }

                if (string.IsNullOrEmpty(url))
                    throw new Exception(LanguageManager.GetTranslation("FormOffice", "link_non_trovato"));

                progressBar1.Visible = true;
                progressBar1.Value = 0;
                label2.Visible = true;
                label2.Text = "0%";

                if (selectedInstallationType.Contains("Offline"))
                {
                    bool salva = Checkbox_Salva.Checked;
                    bool installa = Checkbox_Installa.Checked;
                    if (!salva && !installa)
                        throw new Exception("Seleziona almeno una delle opzioni: Salva o Installa.");
                    savePath = Path.Join(percorsoCompleto, Path.GetFileName(url));
                    await WinHubX.Impostazioni.DownloadManager.DownloadFileAsync(url, savePath, downloadCancellation.Token);
                    downloadCancellation.Token.ThrowIfCancellationRequested();
                    if (installa)
                    {
                        WinHubX.Impostazioni.OfficeSettings.LastDownloadedFile = savePath;
                        WinHubX.Impostazioni.OfficeSettings.HasPendingInstallation = true;
                        WinHubX.Impostazioni.OfficeSettings.InstallationType = "Offline";
                        await StartInstallation(savePath, salva);
                    }
                }

                else
                {
                    privateWorkspacePath = PrivateUserWorkspace.CreateSession();
                    string tempDir = privateWorkspacePath;
                    string platform = arch == "x32" ? "x86" : "x64";

                    url = links.FirstOrDefault(k => k.Key.Contains($"Officex{(platform == "x64" ? "64" : "32")}", StringComparison.OrdinalIgnoreCase)).Value
                          ?? links.GetValueOrDefault("Online");

                    if (string.IsNullOrEmpty(url))
                        throw new Exception("Link di download non trovato per questa configurazione.");

                    Uri uri = new Uri(url);
                    string query = uri.Query.TrimStart('?');
                    var parameters = System.Web.HttpUtility.ParseQueryString(query);

                    string product = parameters["ProductreleaseID"] ?? selectedOfficeVersion;
                    string lang = parameters["language"] ?? selectedLanguage;
                    string version = parameters["version"] ?? "O16GA";

                    string cleanFileName = $"OfficeSetup_{product}_{platform}_{lang}_{version}.exe";
                    tempFile = SafePathResolver.ResolveContainedPath(tempDir, cleanFileName);
                    await WinHubX.Impostazioni.DownloadManager.DownloadFileAsync(url, tempFile, downloadCancellation.Token);
                    downloadCancellation.Token.ThrowIfCancellationRequested();
                    WinHubX.Impostazioni.OfficeSettings.LastDownloadedFile = tempFile;
                    WinHubX.Impostazioni.OfficeSettings.HasPendingInstallation = true;
                    WinHubX.Impostazioni.OfficeSettings.InstallationType = "Online";
                    await StartOnlineInstallation(tempFile);
                }
            }
            catch (OperationCanceledException)
            {
                WinHubX.Impostazioni.OfficeSettings.HasPendingInstallation = false;
                WinHubX.Impostazioni.OfficeSettings.LastDownloadedFile = null;
                MessageBox.Show("Download annullato dall'utente.");
            }
            catch (Exception ex) when (IsExpectedOfficeOperationFailure(ex))
            {
                WinHubX.Impostazioni.OfficeSettings.HasPendingInstallation = false;
                WinHubX.Impostazioni.OfficeSettings.LastDownloadedFile = null;
                MessageBox.Show($"{LanguageManager.GetTranslation("FormOffice", "errore")}:\n{ex.Message}");
            }
            finally
            {
                try
                {
                    if (!IsDisposed && !Disposing)
                    {
                        progressBar1.Visible = false;
                        label2.Visible = false;
                        SetDownloadButtonStyle(false);
                    }

                    try
                    {
                        if (!string.IsNullOrEmpty(tempFile) && File.Exists(tempFile))
                            File.Delete(tempFile);
                        if (privateWorkspacePath is not null)
                            PrivateUserWorkspace.DeleteSession(privateWorkspacePath);
                        if (!IsDisposed && !Disposing &&
                            !string.IsNullOrEmpty(savePath) && File.Exists(savePath) &&
                            !WinHubX.Impostazioni.DownloadManager.IsDownloading &&
                            selectedInstallationType.Contains("Offline"))
                        {
                            bool salva = Checkbox_Salva.Checked;
                            bool installa = Checkbox_Installa.Checked;
                            if (!salva && !installa)
                            {
                                File.Delete(savePath);
                            }
                        }
                    }
                    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Security.SecurityException)
                    {
                        Debug.WriteLine($"Pulizia file temporaneo/destinazione Office non riuscita: {ex}");
                    }
                }
                finally
                {
                    if (ReferenceEquals(_cts, downloadCancellation))
                        _cts = null;
                }
            }
        }
        private async Task StartInstallation(string savePath, bool salva)
        {
            bool mounted = false;
            bool installationSucceeded = false;
            try
            {
                if (!await MountIsoAsync(savePath))
                {
                    MessageBox.Show("Errore durante il montaggio dell'immagine ISO.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                mounted = true;

                string? driveLetter = null;
                for (int i = 0; i < 10; i++)
                {
                    _cts?.Token.ThrowIfCancellationRequested();
                    driveLetter = await GetIsoDriveLetterAsync(savePath);
                    if (!string.IsNullOrWhiteSpace(driveLetter)) break;
                    await Task.Delay(1000, _cts?.Token ?? CancellationToken.None);
                }

                if (string.IsNullOrWhiteSpace(driveLetter))
                {
                    MessageBox.Show("Impossibile determinare la lettera di unità dell'immagine montata.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string drivePath = driveLetter + @":\";
                string[] possibleSetups = await Task.Run(() => FindSetupExecutables(drivePath), _cts?.Token ?? CancellationToken.None);
                if (possibleSetups.Length == 0)
                {
                    MessageBox.Show("Nessun file di installazione trovato.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string setupExe = possibleSetups[0];
                using Process setupProcess = Process.Start(new ProcessStartInfo(setupExe)
                {
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(setupExe)
                }) ?? throw new InvalidOperationException("Impossibile avviare il setup trovato nell'immagine.");
                // Non interrompere l'attesa dopo l'avvio: il processo può continuare e usare il supporto montato.
                await setupProcess.WaitForExitAsync(CancellationToken.None);
                if (setupProcess.ExitCode != 0)
                    throw new InvalidOperationException($"Il setup è terminato con codice {setupProcess.ExitCode}.");

                installationSucceeded = true;
            }
            catch (Exception ex) when (IsExpectedOfficeOperationFailure(ex))
            {
                Debug.WriteLine($"Installazione offline Office non riuscita: {ex}");
                MessageBox.Show($"Errore durante l'installazione: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (mounted)
                {
                    bool dismounted = false;
                    try
                    {
                        int exitCode = await RunPowerShellAsync($"Dismount-DiskImage -ImagePath '{EscapePowerShellLiteral(savePath)}'");
                        if (exitCode != 0)
                        {
                            MessageBox.Show($"L'immagine Office è ancora montata (codice DISM {exitCode}).", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            dismounted = true;
                        }
                    }
                    catch (Exception ex) when (IsExpectedOfficeOperationFailure(ex))
                    {
                        Debug.WriteLine($"Smontaggio immagine Office non riuscito: {ex}");
                        MessageBox.Show($"Impossibile smontare l'immagine Office: {ex.Message}", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    if (installationSucceeded && dismounted)
                    {
                        bool cleaned = salva || TryDeleteOfficeFile(savePath, "immagine Office temporanea");
                        if (cleaned)
                        {
                            WinHubX.Impostazioni.OfficeSettings.HasPendingInstallation = false;
                            WinHubX.Impostazioni.OfficeSettings.LastDownloadedFile = null;
                        }
                        else
                        {
                            MessageBox.Show("Installazione completata, ma l'immagine ISO non è stata rimossa.", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }

        private static string[] FindSetupExecutables(string drivePath)
        {
            return Directory.EnumerateFiles(drivePath, "*.exe", SearchOption.AllDirectories)
                .Where(path =>
                {
                    string fileName = Path.GetFileNameWithoutExtension(path);
                    return fileName.Contains("setup", StringComparison.OrdinalIgnoreCase)
                        || fileName.Contains("install", StringComparison.OrdinalIgnoreCase)
                        || fileName.Contains("autorun", StringComparison.OrdinalIgnoreCase);
                })
                .OrderByDescending(path => string.Equals(Path.GetFileName(path), "setup.exe", StringComparison.OrdinalIgnoreCase))
                .ThenBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private async Task StartOnlineInstallation(string tempFile)
        {
            try
            {
                using Process setup = Process.Start(new ProcessStartInfo(tempFile) { UseShellExecute = true })
                    ?? throw new InvalidOperationException("Impossibile avviare il programma di installazione di Office.");
                // Mantieni il file disponibile finché il setup avviato non è realmente terminato.
                await setup.WaitForExitAsync(CancellationToken.None);
                if (setup.ExitCode != 0)
                    throw new InvalidOperationException($"Il setup Office è terminato con codice {setup.ExitCode}.");

                if (TryDeleteOfficeFile(tempFile, "installer Office temporaneo"))
                {
                    WinHubX.Impostazioni.OfficeSettings.HasPendingInstallation = false;
                    WinHubX.Impostazioni.OfficeSettings.LastDownloadedFile = null;
                }
                else
                {
                    MessageBox.Show("Installazione completata, ma l'installer temporaneo non è stato rimosso.", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex) when (IsExpectedOfficeOperationFailure(ex))
            {
                MessageBox.Show($"Errore durante l'installazione: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDownloadButtonStyle(bool isStop)
        {
            if (isStop)
            {
                btnDownloadVerdi.Image = Properties.Resources.pngCloseCreazioneISO;
                btnDownloadVerdi.CheckedBackground = Color.FromArgb(192, 0, 0);
                btnDownloadVerdi.CheckedForeColor = Color.FromArgb(192, 0, 0);
                btnDownloadVerdi.CheckedImageTint = Color.FromArgb(192, 0, 0);
                btnDownloadVerdi.CheckedOutline = Color.FromArgb(192, 0, 0);
                btnDownloadVerdi.HoverBackground = Color.FromArgb(192, 0, 0);
                btnDownloadVerdi.HoverOutline = Color.FromArgb(192, 0, 0);
                btnDownloadVerdi.NormalOutline = Color.FromArgb(192, 0, 0);
                btnDownloadVerdi.PressedBackground = Color.FromArgb(192, 0, 0);
                btnDownloadVerdi.PressedOutline = Color.FromArgb(192, 0, 0);
                btnDownloadVerdi.Content = "  STOP";
                label1.Visible = true;
                labelpercorso.Visible = true;
                Checkbox_Installa.Visible = true;
                Checkbox_Installa.Checked = WinHubX.Impostazioni.OfficeSettings.Installa;
                Checkbox_Salva.Visible = true;
                Checkbox_Salva.Checked = WinHubX.Impostazioni.OfficeSettings.SalvaFile;
            }
            else
            {
                btnDownloadVerdi.Image = Properties.Resources.pngScaricaOffice;
                btnDownloadVerdi.CheckedBackground = Color.FromArgb(46, 125, 60);
                btnDownloadVerdi.CheckedForeColor = Color.FromArgb(46, 125, 60);
                btnDownloadVerdi.CheckedImageTint = Color.FromArgb(46, 125, 60);
                btnDownloadVerdi.CheckedOutline = Color.FromArgb(46, 125, 60);
                btnDownloadVerdi.HoverBackground = Color.FromArgb(46, 125, 60);
                btnDownloadVerdi.HoverOutline = Color.FromArgb(46, 125, 60);
                btnDownloadVerdi.NormalOutline = Color.FromArgb(46, 125, 60);
                btnDownloadVerdi.PressedBackground = Color.FromArgb(46, 125, 60);
                btnDownloadVerdi.PressedOutline = Color.FromArgb(46, 125, 60);
                btnDownloadVerdi.Content = "  Download";
            }
        }

        private void Checkbox_Salva_CheckedChanged(object? sender, EventArgs e)
        {
            WinHubX.Impostazioni.OfficeSettings.SalvaFile = Checkbox_Salva.Checked;
        }

        private void Checkbox_Installa_CheckedChanged(object? sender, EventArgs e)
        {
            WinHubX.Impostazioni.OfficeSettings.Installa = Checkbox_Installa.Checked;
        }

        private async Task CheckPendingInstallation(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (WinHubX.Impostazioni.OfficeSettings.HasPendingInstallation &&
                !string.IsNullOrEmpty(WinHubX.Impostazioni.OfficeSettings.LastDownloadedFile) &&
                File.Exists(WinHubX.Impostazioni.OfficeSettings.LastDownloadedFile))
            {
                var result = MessageBox.Show(
                    "Trovata un'installazione di Office in sospeso. Vuoi completarla ora?",
                    "Installazione in sospeso",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    SetDownloadButtonStyle(true); 
                    using CancellationTokenSource pendingInstallationCancellation =
                        CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    if (_cts is not null)
                        throw new InvalidOperationException("È già in corso un'operazione Office.");
                    _cts = pendingInstallationCancellation;
                    try
                    {
                        if (WinHubX.Impostazioni.OfficeSettings.InstallationType == "Offline")
                        {
                            bool salva = WinHubX.Impostazioni.OfficeSettings.SalvaFile;
                            await StartInstallation(WinHubX.Impostazioni.OfficeSettings.LastDownloadedFile, salva);
                        }
                        else
                        {
                            await StartOnlineInstallation(WinHubX.Impostazioni.OfficeSettings.LastDownloadedFile);
                        }
                    }
                    finally
                    {
                        _cts = null;
                        if (!IsDisposed && !Disposing)
                            SetDownloadButtonStyle(false);
                    }
                }
                else
                {
                    WinHubX.Impostazioni.OfficeSettings.HasPendingInstallation = false;
                    WinHubX.Impostazioni.OfficeSettings.LastDownloadedFile = null;
                }
            }
        }
        private static async Task<bool> MountIsoAsync(string isoPath)
        {
            return await RunPowerShellAsync($"Mount-DiskImage -ImagePath '{EscapePowerShellLiteral(isoPath)}'") == 0;
        }

        private static async Task<string> GetIsoDriveLetterAsync(string isoPath)
        {
            string result = await RunPowerShellOutputAsync(
                $"(Get-DiskImage -ImagePath '{EscapePowerShellLiteral(isoPath)}' | Get-Volume).DriveLetter"
            );
            return result.Trim();
        }
        private static async Task<int> RunPowerShellAsync(string command)
        {
            using var process = new Process
            {
                StartInfo = CreatePowerShellStartInfo(command)
            };
            process.Start();
            Task<string> standardOutput = process.StandardOutput.ReadToEndAsync();
            Task<string> standardError = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            string[] capturedOutput = await Task.WhenAll(standardOutput, standardError);
            if (process.ExitCode != 0)
            {
                Debug.WriteLine($"PowerShell terminato con codice {process.ExitCode}: {capturedOutput[1]}");
            }

            return process.ExitCode;
        }

        private static async Task<string> RunPowerShellOutputAsync(string command)
        {
            using var process = new Process
            {
                StartInfo = CreatePowerShellStartInfo(command)
            };
            process.Start();
            Task<string> standardOutput = process.StandardOutput.ReadToEndAsync();
            Task<string> standardError = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            string[] capturedOutput = await Task.WhenAll(standardOutput, standardError);

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"PowerShell terminato con codice {process.ExitCode}: {capturedOutput[1]}");
            }

            return capturedOutput[0];
        }

        private static ProcessStartInfo CreatePowerShellStartInfo(string command)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-NonInteractive");
            startInfo.ArgumentList.Add("-Command");
            startInfo.ArgumentList.Add(command);
            return startInfo;
        }

        private static string EscapePowerShellLiteral(string value) => value.Replace("'", "''", StringComparison.Ordinal);

        private static bool TryDeleteOfficeFile(string path, string description)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                Debug.WriteLine($"Rimozione {description} non riuscita: {ex}");
                return false;
            }
        }

        private async void FormOffice_Load(object? sender, EventArgs e)
        {
            CancellationToken cancellationToken;
            try
            {
                cancellationToken = BeginLifetimeOperation();
            }
            catch (OperationCanceledException)
            {
                return;
            }

            try
            {
                try
                {
                    officeVersions = await CaricaOfficeVersions(
                        Dipendenze.GitHubConfigUrl,
                        cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex) when (IsExpectedOfficeOperationFailure(ex))
                {
                    Debug.WriteLine($"Caricamento catalogo Office non riuscito: {ex}");
                    if (!IsDisposed && !Disposing)
                    {
                        MessageBox.Show(
                            $"Impossibile caricare il catalogo Office. Verifica la connessione e riprova.\n{ex.Message}",
                            "WinHubX",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }

                    return;
                }

                if (cancellationToken.IsCancellationRequested || IsDisposed || Disposing)
                    return;

                comboBoxVerOffice.Items.Clear();

                foreach (var office in officeVersions)
                {
                    comboBoxVerOffice.Items.Add(office.Nome);
                }

                Checkbox_Salva.Checked = WinHubX.Impostazioni.OfficeSettings.SalvaFile;
                Checkbox_Installa.Checked = WinHubX.Impostazioni.OfficeSettings.Installa;
                await CheckPendingInstallation(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                Debug.WriteLine("Caricamento Office annullato durante la chiusura della finestra.");
            }
            finally
            {
                EndLifetimeOperation();
            }
        }

        private void AggiornaPercorsoLabel(string downloadPath)
        {
            if (string.IsNullOrEmpty(percorsoCompleto) || percorsoCompleto != downloadPath)
                percorsoCompleto = downloadPath;
            int maxWidth = btn_cambiaBianco.Left - labelpercorso.Left - 10;
            int fullWidth = TextRenderer.MeasureText(percorsoCompleto, labelpercorso.Font).Width;
            if (fullWidth <= maxWidth)
            {
                labelpercorso.Text = percorsoCompleto;
                toolTip1.SetToolTip(labelpercorso, percorsoCompleto);
                return;
            }
            string path = percorsoCompleto;
            while (TextRenderer.MeasureText(path + "...", labelpercorso.Font).Width > maxWidth && path.Contains("\\"))
            {
                int lastSlash = path.LastIndexOf('\\');
                if (lastSlash <= 0) break;
                path = path.Substring(0, lastSlash);
            }

            labelpercorso.Text = path + "...";
        }
        private void btn_cambia_Click(object? sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    AggiornaPercorsoLabel(dialog.SelectedPath);
                }
            }
        }

        private void panel50_Resize(object? sender, EventArgs e) => AggiornaPercorsoLabel(percorsoCompleto);
        private void panel70_Resize(object? sender, EventArgs e) => AggiornaPercorsoLabel(percorsoCompleto);
        private void tableLayoutPanel50_Resize(object? sender, EventArgs e) => AggiornaPercorsoLabel(percorsoCompleto);
        private void FormOffice_Resize(object? sender, EventArgs e) => AggiornaPercorsoLabel(percorsoCompleto);

        private void btnAggRimAppOffice_Click(object? sender, EventArgs e)
        {
            MostraFormInPanel<FormAggiungiRimuoviAppOffice>("AggiungiRimuoviApp", btnAggRimAppOfficePrinci);
        }

        private void btnPersonalizzaOffice_Click(object? sender, EventArgs e)
        {
            string hardwarePath = Path.Join(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "WinHubX", "Computer", "osehardware.json");

            if (!File.Exists(hardwarePath))
            {
                var popup = new WinHubX.DialogBlock.Form_DialogBlock(form1);
                var panelCenter = panel50.PointToScreen(new Point(panel50.Width / 2, panel50.Height / 2));
                popup.StartPosition = FormStartPosition.Manual;
                popup.Location = new Point(
                    panelCenter.X - popup.Width / 2,
                    panelCenter.Y - popup.Height / 2
                );

                popup.ShowDialog();
                return;
            }
            MostraFormInPanel<PersonalizzazioneOffice>("PersonalizzazioneTitoloForm", btnPersonalizzaOfficePrinci);
        }

        private void MostraFormInPanel<T>(string titoloTraduzione, cuiButton button) where T : Form
        {
            panel50.Controls.Clear();

            Form1? mainForm = Application.OpenForms["Form1"] as Form1;
            if (mainForm == null) return;

            mainForm.pictureBox3.Visible = true;

            mainForm.pictureBox3.Click -= PictureBox3_Click_BackToOffice;
            mainForm.pictureBox3.Click += PictureBox3_Click_BackToOffice;

            mainForm.lblPanelTitle.Text = LanguageManager.GetTranslation("FormPersonallizatoOffice", titoloTraduzione);
            mainForm.pictureBoxlblalto.Image = button.Image;
            Form form = Activator.CreateInstance(typeof(T), mainForm, this) as Form
                ?? throw new InvalidOperationException($"Impossibile creare il form {typeof(T).Name}.");
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            panel50.Controls.Add(form);
            form.Show();
        }
    }
}
