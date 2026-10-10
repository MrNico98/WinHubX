using Microsoft.Win32;
using Mono.Unix.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO.Compression;
using System.Management;
using System.Security.Policy;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using WinHubX.Forms.Base;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.ImpostazioniApp
{
    public partial class FormImpostazioniApp : Form
    {
        private static bool IsExpectedApplicationUpdateFailure(Exception exception) =>
            exception is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or System.ComponentModel.Win32Exception
                or HttpRequestException
                or System.Text.Json.JsonException
                or System.Security.Cryptography.CryptographicException
                or InvalidOperationException
                or OperationCanceledException
                or TimeoutException
                or ArgumentException;

        private string selectedTheme = "";
        private string lingua = "";
        private static readonly HttpClient client = CreateHttpClient();
        private string? latestVersion;
        private string? latestUpdateUrl;
        private string? latestReleaseNotes;
        private string? latestUpdateSha256;
        public bool UpdateDetectedAtStartup { get; private set; } = false;

        private static HttpClient CreateHttpClient()
        {
            var handler = new SocketsHttpHandler
            {
                AllowAutoRedirect = false,
                MaxConnectionsPerServer = 4,
                PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                AutomaticDecompression = System.Net.DecompressionMethods.None
            };

            return new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public FormImpostazioniApp()
        {
            InitializeComponent();
            LoadCurrentTheme();
            LoadCurrentLingua();
            labelversione.Text = AppConfig.CurrentVersion;
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            btnAggiornamento.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Controlla aggiornamenti",
                "en" => "  Check for updates",
                _ => btnAggiornamento.Content
            };
            btnApplicaVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Applica",
                "en" => "  Apply",
                _ => btnApplicaVerdi.Content
            };
        }

        private void radioButton_temachiaro_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_temachiaro.Checked)
            {
                selectedTheme = "chiaro";
            }
        }

        private void radioButton_temascuro_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_temascuro.Checked)
            {
                selectedTheme = "scuro";
            }
        }

        private void radioButton_temadisistema_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_temadisistema.Checked)
            {
                selectedTheme = "sistema";
            }
        }

        private void btnInstallaVerdi_Click(object sender, EventArgs e)
        {
            bool temaSelezionato = !string.IsNullOrEmpty(selectedTheme);
            bool linguaSelezionata = !string.IsNullOrEmpty(lingua);

            if (!temaSelezionato && !linguaSelezionata)
            {
                MessageBox.Show("Seleziona un tema e/o una lingua prima di applicare.", "Attenzione",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (temaSelezionato)
            {
                ApplySelectedTheme();
            }

            if (linguaSelezionata)
            {
                ApplySelectedLingua();
            }


            RestartApplication();
        }

        private void ApplySelectedLingua()
        {
            var config = ThemeConfig.Load();

            switch (lingua)
            {
                case "en":
                    config.Language = "en";
                    config.LanguageManuallySet = true;
                    break;
                case "it":
                    config.Language = "it";
                    config.LanguageManuallySet = true;
                    break;
                case "sistema":
                    config.LanguageManuallySet = false;
                    break;
            }

            config.Save();
        }

        private void ApplySelectedTheme()
        {
            var config = ThemeConfig.Load();

            switch (selectedTheme)
            {
                case "chiaro":
                    config.DarkTheme = false;
                    config.ThemeManuallySet = true;
                    break;
                case "scuro":
                    config.DarkTheme = true;
                    config.ThemeManuallySet = true;
                    break;
                case "sistema":
                    config.ThemeManuallySet = false;
                    break;
            }

            config.Save();
        }

        private void RestartApplication()
        {
            var result = MessageBox.Show(
                LanguageManager.GetTranslation("FormImpostazioni", "riavvio_msg"),
                LanguageManager.GetTranslation("FormImpostazioni", "riavvio_title"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                this.Close();
                var timer = new System.Windows.Forms.Timer();
                timer.Interval = 500;
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    timer.Dispose();

                    string applicationPath = Application.ExecutablePath;
                    Process.Start(applicationPath);
                    Environment.Exit(0);
                };
                timer.Start();
            }
        }


        private void LoadCurrentTheme()
        {
            var config = ThemeConfig.Load();

            if (!config.ThemeManuallySet)
            {
                radioButton_temadisistema.Checked = true;
                selectedTheme = "sistema";
            }
            else if (config.DarkTheme)
            {
                radioButton_temascuro.Checked = true;
                selectedTheme = "scuro";
            }
            else
            {
                radioButton_temachiaro.Checked = true;
                selectedTheme = "chiaro";
            }
        }

        private void LoadCurrentLingua()
        {
            var config = ThemeConfig.Load();

            if (!config.LanguageManuallySet)
            {
                radioButton_sistemalingua.Checked = true;
                lingua = "sistema";
            }
            else if (config.Language == "en")
            {
                radioButton_ingleselingua.Checked = true;
                lingua = "en";
            }
            else
            {
                radioButton_italianolinuga.Checked = true;
                lingua = "it";
            }
        }

        private void radioButton_sistemalingua_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_sistemalingua.Checked)
            {
                lingua = "sistema";
            }
        }

        private void radioButton_ingleselingua_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_ingleselingua.Checked)
            {
                lingua = "en";
            }
        }

        private void radioButton_italianolinuga_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_italianolinuga.Checked)
            {
                lingua = "it";
            }
        }

        private async void btnAggiornamento_Click(object sender, EventArgs e)
        {

            if (btnAggiornamento.Content == "  Aggiorna" && latestUpdateUrl != null && latestVersion != null)
            {
                btnAggiornamento.Content = LanguageManager.CurrentLanguage switch
                {
                    "it" => "  Aggiorna",
                    "en" => "  Update",
                    _ => btnAggiornamento.Content
                };
                await DownloadAndUpdate(latestUpdateUrl, latestVersion, latestUpdateSha256);
                return;
            }
            var result = await CheckForUpdatesAsync();

            if (result.UpdateAvailable)
            {
                latestVersion = result.LatestVersion;
                latestUpdateUrl = result.UpdateUrl;
                latestReleaseNotes = result.ReleaseNotes;
                latestUpdateSha256 = result.Sha256;
                btnAggiornamento.Content = LanguageManager.CurrentLanguage switch
                {
                    "it" => "  Aggiorna",
                    "en" => "  Update",
                    _ => btnAggiornamento.Content
                };
                btnAggiornamento.Image = Properties.Resources.pngScaricaOffice;
                string versioneTesto = AppConfig.CurrentVersion;
                versioneTesto += " - Aggiornamento disponibile";
                btnAggiornamento.Content = LanguageManager.CurrentLanguage switch
                {
                    "it" => "  Aggiorna",
                    "en" => "  Update",
                    _ => btnAggiornamento.Content
                };
                labelversione.Text = "Versione: " + versioneTesto;
                MessageBox.Show(
                    $"È disponibile la versione {latestVersion}\n\n" +
                    $"{latestReleaseNotes}",
                    "Aggiornamento disponibile",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return;
            }
            btnAggiornamento.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Controlla aggiornamenti",
                "en" => "  Check for updates",
                _ => btnAggiornamento.Content
            };
            btnAggiornamento.Image = Properties.Resources.pngclick;
            MessageBox.Show(
                "Nessun aggiornamento disponibile.",
                "WinHubX",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }


        private void AggiornaTestoBottoneAggiornamenti(bool updateAvailable)
        {
            if (!AppConfig.CheckUpdatesOnStartup)
            {
                btnAggiornamento.Content = LanguageManager.CurrentLanguage switch
                {
                    "it" => "  Controlla aggiornamenti",
                    "en" => "  Check for updates",
                    _ => btnAggiornamento.Content
                };
                btnAggiornamento.Image = Properties.Resources.pngclick;
                return;
            }

            btnAggiornamento.Content = updateAvailable ? "  Aggiorna" : "  Controlla aggiornamenti";
            btnAggiornamento.Image = Properties.Resources.pngScaricaOffice;
        }

        private void switch_aggiornamentoavvio_CheckedChanged(object sender, EventArgs e)
        {
            AppConfig.CheckUpdatesOnStartup = switch_aggiornamentoavvio.Checked;
            AppConfig.SaveSettings();
        }

        public async Task<bool> VerificaAggiornamentiAutomaticiAsync()
        {
            if (!AppConfig.CheckUpdatesOnStartup)
            {
                AggiornaTestoBottoneAggiornamenti(false);
                UpdateDetectedAtStartup = false;
                return false;
            }

            var result = await CheckForUpdatesAsync(reportErrors: false);

            if (result.UpdateAvailable)
            {
                latestVersion = result.LatestVersion;
                latestUpdateUrl = result.UpdateUrl;
                latestReleaseNotes = result.ReleaseNotes;
                latestUpdateSha256 = result.Sha256;

                UpdateDetectedAtStartup = true;
            }
            else
            {
                UpdateDetectedAtStartup = false;
            }

            AggiornaTestoBottoneAggiornamenti(result.UpdateAvailable);
            return result.UpdateAvailable;
        }


        private async Task<UpdateInfoResult> CheckForUpdatesAsync(bool reportErrors = true)
        {
            const string updateManifestUrl = "https://raw.githubusercontent.com/LightYagami28/WinHubX/refs/heads/main/update.json";
            string currentVersion = AppConfig.CurrentVersion;

            try
            {
                string response = await GetTrustedResponseStringAsync(updateManifestUrl, "manifest aggiornamenti");
                JObject updateInfo = JObject.Parse(response);

                ValidatedUpdateManifest manifest = UpdateManifestValidator.Validate(
                    updateInfo["version"]?.Value<string>(),
                    updateInfo["updateUrl"]?.Value<string>(),
                    updateInfo["sha256"]?.Value<string>());

                string userLang = Thread.CurrentThread.CurrentUICulture
                    .TwoLetterISOLanguageName.ToUpper();

                string releaseNotes = GetReleaseNotesByLanguage(updateInfo["releaseNotes"], userLang);

                return new UpdateInfoResult
                {
                    UpdateAvailable = IsNewerVersion(manifest.Version, currentVersion),
                    LatestVersion = manifest.Version,
                    UpdateUrl = manifest.UpdateUrl,
                    ReleaseNotes = releaseNotes,
                    Sha256 = manifest.Sha256
                };
            }
            catch (Exception ex) when (IsExpectedApplicationUpdateFailure(ex))
            {
                System.Diagnostics.Debug.WriteLine($"Controllo aggiornamenti non riuscito: {ex}");
                if (reportErrors && !IsDisposed)
                    MessageBox.Show($"Impossibile controllare gli aggiornamenti: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new UpdateInfoResult { UpdateAvailable = false };
            }
        }

        private async Task<string> GetTrustedResponseStringAsync(string url, string description)
        {
            UpdateManifestValidator.EnsureTrustedHttpsUrl(url, description);
            return await TrustedHttpsClient.GetStringAsync(client, url);
        }

        private static bool IsNewerVersion(string candidate, string current)
        {
            return Version.TryParse(candidate, out Version? candidateVersion)
                && Version.TryParse(current, out Version? currentVersion)
                && candidateVersion > currentVersion;
        }

        private string GetReleaseNotesByLanguage(JToken? releaseNotesObject, string language)
        {
            try
            {
                var notes = releaseNotesObject?[language];
                if (notes == null)
                    return "Nessuna nota disponibile.";

                StringBuilder sb = new StringBuilder();
                foreach (var note in notes)
                {
                    sb.AppendLine("• " + note);
                }

                return sb.ToString().Trim();
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or Newtonsoft.Json.JsonException)
            {
                Debug.WriteLine($"Lettura note di rilascio non riuscita: {ex}");
                return "Note di rilascio non disponibili per la lingua selezionata.";
            }
        }

        private async Task DownloadAndUpdate(string updateUrl, string version, string? expectedSha256)
        {
            string updateFilePath = Path.Join(Path.GetTempPath(), $"WinHubX-{version}-{Guid.NewGuid():N}.exe");
            using (var progressForm = new ProgressForm())
            {
                progressForm.Show();
                progressForm.SetMarquee();
                try
                {
                    if (!UpdateManifestValidator.IsValidSha256(expectedSha256))
                        throw new InvalidOperationException("Aggiornamento interrotto: SHA-256 assente o non valido.");

                    await DownloadFileWithProgress(updateUrl, updateFilePath, progressForm);
                    await using FileStream downloadedFile = File.OpenRead(updateFilePath);
                    byte[] actualHash = await SHA256.HashDataAsync(downloadedFile);
                    string actualSha256 = Convert.ToHexString(actualHash);
                    if (!actualSha256.Equals(expectedSha256, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("Il controllo SHA-256 del pacchetto aggiornamento non è riuscito.");
                    await UpdateInstaller.InstallAndStartAsync(
                        updateFilePath,
                        Application.ExecutablePath,
                        expectedSha256,
                        StartUpdatedProcessAndWaitForWindowAsync);
                    Application.Exit();
                }
                catch (Exception ex) when (IsExpectedApplicationUpdateFailure(ex))
                {
                    _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    progressForm.CompleteOperation();
                    try
                    {
                        if (File.Exists(updateFilePath))
                            File.Delete(updateFilePath);
                    }
                    catch (IOException cleanupException)
                    {
                        System.Diagnostics.Debug.WriteLine($"File temporaneo aggiornamento non rimosso: {cleanupException.Message}");
                    }
                }
            }
        }

        private static async Task<bool> StartUpdatedProcessAndWaitForWindowAsync(string executablePath, CancellationToken cancellationToken)
        {
            using Process process = Process.Start(new ProcessStartInfo(executablePath) { UseShellExecute = false })
                ?? throw new InvalidOperationException("Windows non ha avviato il processo aggiornato.");
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromSeconds(20));

            try
            {
                while (!timeout.IsCancellationRequested)
                {
                    if (process.HasExited)
                    {
                        return false;
                    }

                    process.Refresh();
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        return true;
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(200), timeout.Token);
                }
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                // La nuova versione non ha mostrato la finestra entro il timeout: arrestarla prima del rollback.
            }
            catch
            {
                StopFailedUpdateProcess(process);
                throw;
            }

            StopFailedUpdateProcess(process);
            return false;
        }

        private static void StopFailedUpdateProcess(Process process)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                    process.WaitForExit();
                }
            }
            catch (Exception exception) when (exception is InvalidOperationException or System.ComponentModel.Win32Exception)
            {
                System.Diagnostics.Debug.WriteLine($"Arresto processo aggiornato non riuscito: {exception}");
            }
        }

        private async Task DownloadFileWithProgress(string url, string filePath, ProgressForm progressForm)
        {
            UpdateManifestValidator.EnsureTrustedHttpsUrl(url, "pacchetto aggiornamento");
            using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(10));
            using var response = await TrustedHttpsClient.GetAsync(client, url, timeout.Token);
            UpdateManifestValidator.EnsureTrustedHttpsUrl(response.RequestMessage?.RequestUri?.ToString(), "reindirizzamento pacchetto aggiornamento");
            _ = response.EnsureSuccessStatusCode();
            var totalBytes = response.Content.Headers.ContentLength.GetValueOrDefault();
            if (totalBytes <= 0)
                throw new InvalidOperationException("Il pacchetto aggiornamento non espone una dimensione valida.");
            using var contentStream = await response.Content.ReadAsStreamAsync(timeout.Token);
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
            var buffer = new byte[8192];
            long bytesRead = 0;
            int read;
            while ((read = await contentStream.ReadAsync(buffer, timeout.Token)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, read), timeout.Token);
                bytesRead += read;
                progressForm.Invoke(new Action(() =>
                    progressForm.SetStatus("Download...", (int)Math.Clamp((bytesRead * 100) / totalBytes, 0, 100))));
            }
        }

        private class UpdateInfoResult
        {
            public bool UpdateAvailable { get; set; }
            public string LatestVersion { get; set; } = string.Empty;
            public string UpdateUrl { get; set; } = string.Empty;
            public string ReleaseNotes { get; set; } = string.Empty;
            public string? Sha256 { get; set; }
        }

        private async void FormImpostazioniApp_Load(object sender, EventArgs e)
        {
            AppConfig.LoadSettings();
            switch_aggiornamentoavvio.Checked = AppConfig.CheckUpdatesOnStartup;
            string versioneTesto = AppConfig.CurrentVersion;

            if (AppConfig.CheckUpdatesOnStartup)
            {
                bool updateAvailable = await VerificaAggiornamentiAutomaticiAsync();

                if (updateAvailable)
                {
                    versioneTesto += " - " + LanguageManager.GetTranslation("FormImpostazioni", "aggiornamento_disponibile");
                    btnAggiornamento.Content = "  " + LanguageManager.GetTranslation("FormImpostazioni", "aggiornamento_disponibile");
                    btnAggiornamento.Image = Properties.Resources.pngScaricaOffice;
                    MessageBox.Show(
                        LanguageManager.FormatTranslation(
                            "FormImpostazioni", "aggiornamento_disponibile_msg", latestVersion, latestReleaseNotes),
                        LanguageManager.GetTranslation("FormImpostazioni", "aggiornamento_disponibile"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    versioneTesto += " - " + LanguageManager.GetTranslation("FormImpostazioni", "nessun_aggiornamento");
                }
            }
            else
            {
                versioneTesto += " - " + LanguageManager.GetTranslation("FormImpostazioni", "nessun_aggiornamento");
            }
            labelversione.Text = LanguageManager.FormatTranslation("FormImpostazioni", "labelversione", versioneTesto);
        }
    }
}
