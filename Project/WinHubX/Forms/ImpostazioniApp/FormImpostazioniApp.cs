using Microsoft.Win32;
using Mono.Unix.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO.Compression;
using System.Management;
using System.Security.Policy;
using System.Text;
using WinHubX.Forms.Base;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.ImpostazioniApp
{
    public partial class FormImpostazioniApp : Form
    {
        private string selectedTheme = "";
        private string lingua = "";
        private static readonly HttpClient client = new HttpClient();
        private string latestVersion = null;
        private string latestUpdateUrl = null;
        private string latestReleaseNotes = null;
        public bool UpdateDetectedAtStartup { get; private set; } = false;
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

            string jsonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WinHubX", "Impostazioni", "Tema.json");

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

            string jsonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WinHubX", "Impostazioni", "Tema.json");

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

            if (btnAggiornamento.Content == "  Aggiorna" && latestUpdateUrl != null)
            {
                btnAggiornamento.Content = LanguageManager.CurrentLanguage switch
                {
                    "it" => "  Aggiorna",
                    "en" => "  Update",
                    _ => btnAggiornamento.Content
                };
                await DownloadAndUpdate(latestUpdateUrl, latestVersion);
                return;
            }
            var result = await CheckForUpdatesAsync();

            if (result.UpdateAvailable)
            {
                latestVersion = result.LatestVersion;
                latestUpdateUrl = result.UpdateUrl;
                latestReleaseNotes = result.ReleaseNotes;
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

            var result = await CheckForUpdatesAsync();

            if (result.UpdateAvailable)
            {
                latestVersion = result.LatestVersion;
                latestUpdateUrl = result.UpdateUrl;
                latestReleaseNotes = result.ReleaseNotes;

                UpdateDetectedAtStartup = true;
            }
            else
            {
                UpdateDetectedAtStartup = false;
            }

            AggiornaTestoBottoneAggiornamenti(result.UpdateAvailable);
            return result.UpdateAvailable;
        }


        private async Task<UpdateInfoResult> CheckForUpdatesAsync()
        {
            string configUrl = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/Dipendenze.json";
            string currentVersion = AppConfig.CurrentVersion;

            try
            {
                var configResponse = await client.GetStringAsync(configUrl);
                dynamic configData = JsonConvert.DeserializeObject(configResponse);
                string updateInfoUrl = configData.Form1.updateInfoUrl;

                var response = await client.GetStringAsync(updateInfoUrl);
                dynamic updateInfo = JsonConvert.DeserializeObject(response);

                string latestVersion = (string)updateInfo.version;
                string updateUrl = (string)updateInfo.updateUrl;

                string userLang = Thread.CurrentThread.CurrentUICulture
                    .TwoLetterISOLanguageName.ToUpper();

                string releaseNotes = GetReleaseNotesByLanguage(updateInfo.releaseNotes, userLang);

                return new UpdateInfoResult
                {
                    UpdateAvailable = latestVersion != currentVersion,
                    LatestVersion = latestVersion,
                    UpdateUrl = updateUrl,
                    ReleaseNotes = releaseNotes
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new UpdateInfoResult { UpdateAvailable = false };
            }
        }

        private string GetReleaseNotesByLanguage(dynamic releaseNotesObject, string language)
        {
            try
            {
                var notes = releaseNotesObject[language];
                if (notes == null)
                    return "Nessuna nota disponibile.";

                StringBuilder sb = new StringBuilder();
                foreach (var note in notes)
                {
                    sb.AppendLine("• " + note.ToString());
                }

                return sb.ToString().Trim();
            }
            catch
            {
                return "Note di rilascio non disponibili per la lingua selezionata.";
            }
        }

        private async Task DownloadAndUpdate(string updateUrl, string version)
        {
            string updateFilePath = Path.Combine(Path.GetTempPath(), $"WinHubX{version}.exe");
            using (var progressForm = new ProgressForm())
            {
                progressForm.Show();
                progressForm.SetMarquee();
                try
                {
                    await DownloadFileWithProgress(updateUrl, updateFilePath, progressForm);
                    string currentExecutablePath = Application.ExecutablePath;
                    File.Move(currentExecutablePath, Path.ChangeExtension(currentExecutablePath, ".old"), true);
                    File.Move(updateFilePath, currentExecutablePath);
                    _ = Process.Start(currentExecutablePath);
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    progressForm.CompleteOperation();
                }
            }
        }

        private async Task DownloadFileWithProgress(string url, string filePath, ProgressForm progressForm)
        {
            string localPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WinHubX");
            if (File.Exists(localPath))
            {
                try
                {
                    File.Delete(localPath);
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show($"Error:\n{ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            _ = response.EnsureSuccessStatusCode();
            var totalBytes = response.Content.Headers.ContentLength.GetValueOrDefault();
            using var contentStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
            var buffer = new byte[8192];
            long bytesRead = 0;
            int read;
            while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, read);
                progressForm.Invoke(new Action(() =>
                progressForm.SetStatus("Download...", (int)((bytesRead * 100) / totalBytes))));
            }
        }

        private class UpdateInfoResult
        {
            public bool UpdateAvailable { get; set; }
            public string LatestVersion { get; set; }
            public string UpdateUrl { get; set; }
            public string ReleaseNotes { get; set; }
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
                        string.Format(
                            LanguageManager.GetTranslation("FormImpostazioni", "aggiornamento_disponibile_msg"),
                            latestVersion,
                            latestReleaseNotes
                        ),
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
            labelversione.Text = string.Format(
                LanguageManager.GetTranslation("FormImpostazioni", "labelversione"),
                versioneTesto
            );
        }
    }
}