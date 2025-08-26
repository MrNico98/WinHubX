using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Net;
using WinHubX.Forms.Personalizzazione_office;

namespace WinHubX
{
    public partial class FormOffice : Form
    {
        private Form1 form1;
        private NotifyIcon notifyIcon;
        private string selectedInstallationType;
        private string selectedOfficeVersion;
        private string selectedLanguage;

        public FormOffice(Form1 form1)
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = false
            };
            this.form1 = form1;
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }

        #region AttivaOffice
        private async void btnAttivaOffice_Click(object sender, EventArgs e)
        {
            string configUrl = "https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json";
            string primaryURL = string.Empty;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                if (IsInternetAvailable())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var jsonResponse = await client.GetStringAsync(configUrl);
                        var jsonObject = JObject.Parse(jsonResponse);
                        primaryURL = jsonObject["AttivatoreOffice"]["primaryURL"].ToString();
                    }
                    await ExecuteScriptFromUrl(primaryURL);
                }
                else
                {
                    _ = MessageBox.Show(
                        LanguageManager.GetTranslation("Global", "nointernet"),
                        "Errore",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    ExtractAndExecuteLocalScript();
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static async Task ExecuteScriptFromUrl(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var scriptContent = await client.GetStringAsync(url);
                    string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Ohook_Activation_AIO.cmd");

                    System.IO.File.WriteAllText(tempFilePath, scriptContent);

                    _ = Process.Start(new ProcessStartInfo
                    {
                        FileName = tempFilePath,
                        UseShellExecute = true,
                        Verb = "runas"
                    });
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsInternetAvailable()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var result = client.GetAsync("https://www.google.com").Result;
                    return result.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        private void ExtractAndExecuteLocalScript()
        {
            try
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string scriptPath = Path.Combine(documentsPath, "TSforge_Activation.cmd");
                byte[] scriptBytes = Properties.Resources.Ohook_Activation_AIO;

                File.WriteAllBytes(scriptPath, scriptBytes);
                _ = Process.Start(new ProcessStartInfo
                {
                    FileName = scriptPath,
                    UseShellExecute = true,
                    Verb = "runas"
                });
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private async Task<string> OttiniURL(string jsonUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(jsonUrl);
                var json = JObject.Parse(response);
                return json["FormOffice"]["scrubber"].ToString();
            }
        }

        private async void btnScrubber_Click(object sender, EventArgs e)
        {
            try
            {
                string jsonUrl = "https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json";
                string zipFileUrl = await OttiniURL(jsonUrl);

                string tempFolder = Path.Combine(Path.GetTempPath(), "OfficeScrubber");
                string tempZipPath = Path.Combine(tempFolder, "OfficeScrubber.zip");

                if (Directory.Exists(tempFolder))
                    Directory.Delete(tempFolder, true);
                _ = Directory.CreateDirectory(tempFolder);

                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(zipFileUrl))
                using (FileStream fs = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write))
                {
                    await response.Content.CopyToAsync(fs);
                }

                ZipFile.ExtractToDirectory(tempZipPath, tempFolder);

                string cmdPath = Path.Combine(tempFolder, "OfficeScrubber.cmd");
                if (!File.Exists(cmdPath))
                {
                    return;
                }

                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/c \"{cmdPath}\"";
                process.StartInfo.WorkingDirectory = tempFolder;
                process.StartInfo.Verb = "runas";
                process.StartInfo.UseShellExecute = true;

                _ = process.Start();
                await Task.Run(() => process.WaitForExit());
                await Task.Run(() => AttendiScrubberConTitolo("Office Scrubber v12"));
                Directory.Delete(tempFolder, true);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Errore: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task AttendiScrubberConTitolo(string titolo, int timeoutMs = 10 * 60 * 1000)
        {
            int waited = 0;
            Process? scrubberProc = null;

            while (waited < timeoutMs)
            {
                scrubberProc = Process.GetProcessesByName("powershell")
                    .FirstOrDefault(p => p.MainWindowTitle.Contains(titolo));

                if (scrubberProc != null)
                    break;

                await Task.Delay(1000);
                waited += 1000;
            }

            scrubberProc?.WaitForExit();
        }

        private void btnPersonalizzaOffice_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = LanguageManager.GetTranslation("FormOffice", "paneltitle");
            form1.PnlFormLoader.Controls.Clear();
            PersonalizzazioneOffice formPersonalizzazioneOffice = new PersonalizzazioneOffice(form1, this) { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            formPersonalizzazioneOffice.FormBorderStyle = FormBorderStyle.None;
            form1.PnlFormLoader.Controls.Add(formPersonalizzazioneOffice);
            ThemeManager.ApplyThemeToControl(formPersonalizzazioneOffice, ThemeManager.IsDarkTheme);
            formPersonalizzazioneOffice.Show();
        }

        private void comboBoxVerOffice_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_Lingua.Visible = true;
            lblSelezionLingua.Visible = true;
            string selectedVersion = comboBoxVerOffice.SelectedItem.ToString();

            switch (selectedVersion)
            {
                case "Office 365":
                    pictureBox4.Image = Properties.Resources.png365;
                    break;
                case "Office 2019":
                case "Office 2021":
                    pictureBox4.Image = Properties.Resources.pngOffice;
                    break;
                case "Office 2024":
                    pictureBox4.Image = Properties.Resources.pngOfficeHome;
                    break;
                default:
                    pictureBox4.Image = null;
                    break;
            }
        }

        private void comboBox_Lingua_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedLanguage = comboBox_Lingua.SelectedItem?.ToString();
            labelTipoInstallazione.Visible = true;
            comboBoxInstallazione.Visible = true;
            if (!string.IsNullOrEmpty(selectedInstallationType) && !string.IsNullOrEmpty(selectedOfficeVersion))
            {
                comboBoxInstallazione_SelectedIndexChanged(null, null);
            }
        }

        private void comboBoxInstallazione_SelectedIndexChanged(object? sender, EventArgs? e)
        {
            string selezione = comboBoxInstallazione.SelectedItem?.ToString();
            btnDownload.Visible = true;
            richTextBoxInfo.Clear();

            if (selezione.Contains("Online"))
            {
                richTextBoxInfo.AppendText(LanguageManager.GetTranslation("FormOffice", "paneltitle"), Color.DodgerBlue, FontStyle.Bold | FontStyle.Underline, 12);
                richTextBoxInfo.AppendText("\n📦 ", Color.DodgerBlue, FontStyle.Regular, 11);
                richTextBoxInfo.AppendText(LanguageManager.GetTranslation("FormOffice", "onlineDescription"), Color.Orange, FontStyle.Regular, 11);
                richTextBoxInfo.AppendText("\n✅ " + LanguageManager.GetTranslation("FormOffice", "advantagesTitle") + "\n", Color.Green, FontStyle.Bold, 11);
                richTextBoxInfo.AppendText(LanguageManager.GetTranslation("FormOffice", "onlineAdvantage1"), Color.DarkGreen, FontStyle.Regular, 10);
                richTextBoxInfo.AppendText(LanguageManager.GetTranslation("FormOffice", "onlineAdvantage2"), Color.DarkGreen, FontStyle.Regular, 10);
                richTextBoxInfo.AppendText("\n⚠️ " + LanguageManager.GetTranslation("FormOffice", "requirementsTitle") + "\n", Color.Orange, FontStyle.Bold, 11);
                richTextBoxInfo.AppendText(LanguageManager.GetTranslation("FormOffice", "onlineRequirement1"), Color.DarkOrange, FontStyle.Regular, 10);
                richTextBoxInfo.AppendText("\n────────────────────\n", Color.LightGray, FontStyle.Regular, 11);
            }
            else if (selezione.Contains("Offline"))
            {
                richTextBoxInfo.AppendText(LanguageManager.GetTranslation("FormOffice", "offlinePanelTitle"), Color.Purple, FontStyle.Bold | FontStyle.Underline, 12);
                richTextBoxInfo.AppendText("\n🗂️ ", Color.Purple, FontStyle.Regular, 11);
                richTextBoxInfo.AppendText(LanguageManager.GetTranslation("FormOffice", "offlineDescription"), Color.Orange, FontStyle.Regular, 11);
                richTextBoxInfo.AppendText("\n✅ " + LanguageManager.GetTranslation("FormOffice", "advantagesTitle") + "\n", Color.Green, FontStyle.Bold, 11);
                richTextBoxInfo.AppendText(LanguageManager.GetTranslation("FormOffice", "offlineAdvantage1"), Color.DarkGreen, FontStyle.Regular, 10);
                richTextBoxInfo.AppendText(LanguageManager.GetTranslation("FormOffice", "offlineAdvantage2"), Color.DarkGreen, FontStyle.Regular, 10);
                richTextBoxInfo.AppendText(LanguageManager.GetTranslation("FormOffice", "offlineAdvantage3"), Color.DarkGreen, FontStyle.Regular, 10);
                richTextBoxInfo.AppendText("\n⚠️ " + LanguageManager.GetTranslation("FormOffice", "requirementsTitle") + "\n", Color.Orange, FontStyle.Bold, 11);
                richTextBoxInfo.AppendText(LanguageManager.GetTranslation("FormOffice", "offlineRequirement1"), Color.DarkOrange, FontStyle.Regular, 10);
                richTextBoxInfo.AppendText(LanguageManager.GetTranslation("FormOffice", "offlineRequirement2"), Color.DarkOrange, FontStyle.Regular, 10);
                richTextBoxInfo.AppendText("\n────────────────────\n", Color.LightGray, FontStyle.Regular, 11);
                richTextBoxInfo.AppendText(LanguageManager.GetTranslation("FormOffice", "offlineTip"), Color.DodgerBlue, FontStyle.Bold, 10);
                richTextBoxInfo.AppendText(LanguageManager.GetTranslation("FormOffice", "offlineTipText"), Color.Gray, FontStyle.Italic, 10);
            }

            selectedInstallationType = comboBoxInstallazione.SelectedItem?.ToString();
            selectedOfficeVersion = comboBoxVerOffice.SelectedItem?.ToString();

            btnDownload.Visible = !string.IsNullOrEmpty(selectedInstallationType) &&
                                   !string.IsNullOrEmpty(selectedOfficeVersion);
            richTextBoxDescription.Clear();

            if (!string.IsNullOrEmpty(selectedInstallationType) && !string.IsNullOrEmpty(selectedOfficeVersion))
            {
                richTextBoxDescription.AppendText(LanguageManager.GetTranslation("FormOffice", "configurationTitle"), Color.DodgerBlue, FontStyle.Bold | FontStyle.Underline, 12);
                richTextBoxDescription.AppendText("\n────────────────────────────\n", Color.LightGray, FontStyle.Regular, 9);

                richTextBoxDescription.AppendText("\n🏷️ " + LanguageManager.GetTranslation("FormOffice", "officeVersion") + ": ", Color.Gray, FontStyle.Bold, 10);
                richTextBoxDescription.AppendText($"{selectedOfficeVersion}\n", Color.Orange, FontStyle.Regular, 11);

                richTextBoxDescription.AppendText("🌍 " + LanguageManager.GetTranslation("FormOffice", "lingua") + ": ", Color.Gray, FontStyle.Bold, 10);
                richTextBoxDescription.AppendText($"{selectedLanguage}\n", Color.Orange, FontStyle.Regular, 11);

                richTextBoxDescription.AppendText("📦 " + LanguageManager.GetTranslation("FormOffice", "installationType") + ": ", Color.Gray, FontStyle.Bold, 10);
                richTextBoxDescription.AppendText($"{selectedInstallationType}\n", Color.Orange, FontStyle.Regular, 11);

                if (selectedInstallationType.Contains("Online"))
                {
                    richTextBoxDescription.AppendText("\nℹ️ " + LanguageManager.GetTranslation("FormOffice", "noteOnline"), Color.SteelBlue, FontStyle.Bold, 10);
                    richTextBoxDescription.AppendText(LanguageManager.GetTranslation("FormOffice", "noteOnlineText"), Color.SteelBlue, FontStyle.Regular, 10);
                }
                else
                {
                    richTextBoxDescription.AppendText("\nℹ️ " + LanguageManager.GetTranslation("FormOffice", "noteOffline"), Color.SteelBlue, FontStyle.Bold, 10);
                    richTextBoxDescription.AppendText(LanguageManager.GetTranslation("FormOffice", "noteOfflineText"), Color.SteelBlue, FontStyle.Regular, 10);
                }
            }
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedInstallationType) || string.IsNullOrEmpty(selectedOfficeVersion))
            {
                _ = MessageBox.Show(LanguageManager.GetTranslation("FormOffice", "seleziona_versione_tipo_installazione"));
                return;
            }

            try
            {
                var config = await ScaricaConfigurazioneAsync();
                string key = selectedOfficeVersion.Replace(" ", "");

                Dictionary<string, OfficeLanguage> officeData = key switch
                {
                    "Office2019" => config.Office2019,
                    "Office2021" => config.Office2021,
                    "Office2024" => config.Office2024,
                    "Office365" => config.Office365,
                    _ => null
                };

                if (officeData == null || !officeData.TryGetValue(selectedLanguage, out var languageData))
                {
                    richTextBoxDescription.AppendText("\n❌ ", Color.Red, FontStyle.Bold, 11);
                    richTextBoxDescription.AppendText(LanguageManager.GetTranslation("FormOffice", "configurazione_non_trovata"), Color.DarkRed, FontStyle.Regular, 11);
                    return;
                }

                string offlineProperty = $"Offline{key.Substring(6)}";
                string url = selectedInstallationType.ToLower() switch
                {
                    var x when x.Contains("offline") => languageData.GetType().GetProperty(offlineProperty)?.GetValue(languageData)?.ToString(),
                    var x when x.Contains("online") => selectedInstallationType.Contains("x64") ? languageData.Officex64 : languageData.Officex32,
                    _ => null
                };

                if (!string.IsNullOrEmpty(url))
                {
                    if (selectedInstallationType.ToLower().Contains("offline"))
                    {
                        if (config.OfficeHashes.TryGetValue(selectedLanguage, out var hashDict))
                        {
                            string shaKey = $"Sha256{key}";
                            if (hashDict.TryGetValue(shaKey, out string sha256))
                            {
                                richTextBoxDescription.AppendText("\n🔒 ", Color.Green, FontStyle.Bold, 11);
                                richTextBoxDescription.AppendText(LanguageManager.GetTranslation("FormOffice", "sha256_verifica") + ":\n", Color.DarkRed, FontStyle.Bold, 11);
                                richTextBoxDescription.AppendTextInfo($"{sha256}\n", Color.DarkSlateBlue, FontStyle.Regular, 9, new FontFamily("Consolas"));
                            }
                        }
                    }

                    _ = Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else
                {
                    richTextBoxDescription.AppendText("\n❌ ", Color.Red, FontStyle.Bold, 11);
                    richTextBoxDescription.AppendText(LanguageManager.GetTranslation("FormOffice", "url_non_trovato"), Color.DarkRed, FontStyle.Regular, 11);
                }
            }
            catch (Exception ex)
            {
                richTextBoxDescription.AppendText("\n❌ ", Color.Red, FontStyle.Bold, 11);
                richTextBoxDescription.AppendText($"{LanguageManager.GetTranslation("FormOffice", "errore")} {ex.Message}\n", Color.DarkRed, FontStyle.Regular, 11);
            }
        }

        private async Task<DownloadConfigOffice> ScaricaConfigurazioneAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync("https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json");
                return JsonConvert.DeserializeObject<DownloadConfigOffice>(json)!;
            }
        }

        public class DownloadConfigOffice
        {
            public required Dictionary<string, OfficeLanguage> Office2019 { get; set; }
            public required Dictionary<string, OfficeLanguage> Office2021 { get; set; }
            public required Dictionary<string, OfficeLanguage> Office2024 { get; set; }
            public required Dictionary<string, OfficeLanguage> Office365 { get; set; }
            public required Dictionary<string, Dictionary<string, string>> OfficeHashes { get; set; }
        }

        public class OfficeLanguage
        {
            public required string Officex64 { get; set; }
            public required string Officex32 { get; set; }
            public string? Offline2019 { get; set; }
            public string? Offline2021 { get; set; }
            public string? Offline2024 { get; set; }
            public string? Offline365 { get; set; }
        }

        private void btnAggRimAppOffice_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = LanguageManager.GetTranslation("FormOffice", "aggiungirimuoviapp");
            form1.PnlFormLoader.Controls.Clear();
            FormAggiungiRimuoviAppOffice formaggiungirimuoviappoffice = new FormAggiungiRimuoviAppOffice(form1, this) { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            formaggiungirimuoviappoffice.FormBorderStyle = FormBorderStyle.None;
            form1.PnlFormLoader.Controls.Add(formaggiungirimuoviappoffice);
            ThemeManager.ApplyThemeToControl(formaggiungirimuoviappoffice, ThemeManager.IsDarkTheme);
            formaggiungirimuoviappoffice.Show();
        }
    }
}
