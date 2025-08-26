using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Net;

namespace WinHubX
{
    public partial class FormWin : Form
    {
        private Form1 form1;
        private string selectedOS;
        private string? selectedArch;
        private string selectedVersion;
        private string selectedLanguage;
        public FormWin(Form1 form1)
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            this.form1 = form1;
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }

        private async void btnAttivaWin_Click(object sender, EventArgs e)
        {
            string configUrl = "https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json";
            string primaryURL = string.Empty;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                // Otteniamo informazioni sul sistema operativo
                OperatingSystem os = Environment.OSVersion;
                string osName = GetWindowsVersionName(os);

                // Stop immediato per Windows 7, 8, 8.1
                if (osName == "Windows 7" || osName == "Windows 8" || osName == "Windows 8.1")
                {
                    ExtractAndExecuteLocalScript();
                    return;
                }

                // Per Windows 10/11 o versioni più recenti
                if (IsInternetAvailable())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var jsonResponse = await client.GetStringAsync(configUrl);
                        var jsonObject = JObject.Parse(jsonResponse);
                        primaryURL = jsonObject["FormWin"]["attivatorewin"].ToString();
                    }
                    await ExecuteScriptFromUrl(primaryURL);
                }
                else
                {
                    _ = MessageBox.Show(
                        LanguageManager.GetTranslation("Global", "nointernet"),
                        "WinHubX",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    // Windows 10 più vecchi (< 19041)
                    if (osName == "Windows 10")
                    {
                        ExtractAndExecuteLocalScript();
                    }
                    else
                    {
                        ExtractAndExecuteLocalScriptKMS38();
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Metodo helper per ottenere il nome Windows
        private string GetWindowsVersionName(OperatingSystem os)
        {
            Version v = os.Version;
            switch (v.Major)
            {
                case 6:
                    switch (v.Minor)
                    {
                        case 1: return "Windows 7";
                        case 2: return "Windows 8";
                        case 3: return "Windows 8.1";
                    }
                    break;
                case 10:
                    return "Windows 10";
                case 11:
                    return "Windows 11"; // approssimativo, perché Environment.OSVersion potrebbe non distinguere Windows 11 correttamente
            }
            return "Unknown";
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
                byte[] scriptBytes = Properties.Resources.TSforge_Activation;
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
                _ = MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ExtractAndExecuteLocalScriptKMS38()
        {
            try
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string scriptPath = Path.Combine(documentsPath, "TSforge_Activation.cmd");
                byte[] scriptBytes = Properties.Resources.TSforge_Activation;
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
                _ = MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public static async Task ExecuteScriptFromUrl(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var scriptContent = await client.GetStringAsync(url);
                    string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "HWID_Activation.cmd");
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
                _ = MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnCambioEdizione_Click(object sender, EventArgs e)
        {
            string tempScript = Path.Combine(Path.GetTempPath(), "tempScript.bat");
            string logFile = Path.Combine(Path.GetTempPath(), "ScriptExecution.log");
            string configUrl = "https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json";
            string primaryURL = string.Empty;

            if (File.Exists(tempScript))
            {
                File.Delete(tempScript);
            }

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (HttpClient client = new HttpClient())
                {
                    var jsonResponse = await client.GetStringAsync(configUrl);
                    var jsonObject = JObject.Parse(jsonResponse);
                    primaryURL = jsonObject["FormWin"]["cambiowin"].ToString();
                }
                using (HttpClient client = new HttpClient())
                {
                    byte[] fileBytes = await client.GetByteArrayAsync(primaryURL);
                    await File.WriteAllBytesAsync(tempScript, fileBytes);
                }
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{tempScript}\"",
                    UseShellExecute = true,
                    CreateNoWindow = false
                };

                using (Process process = Process.Start(startInfo))
                {
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFile, ex.Message);
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedOS = comboBox1.SelectedItem?.ToString();
            comboBox_SelezionaLingua.Visible = true;
            lblSelezionaLingua.Visible = true;
            comboBoxArchitettura.SelectedItem = null;
            comboBoxVersione.SelectedItem = null;
            _ = Task.Delay(3000);
            switch (selectedOS)
            {
                case "Windows 7":
                    pictureBox4.Image = Properties.Resources.pngWin7;
                    break;
                case "Windows 8.1":
                    pictureBox4.Image = Properties.Resources.pngWin8dot1;
                    break;
                case "Windows 10":
                    pictureBox4.Image = Properties.Resources.pngWin10;
                    break;
                case "Windows 11":
                    pictureBox4.Image = Properties.Resources.pngWindows11;
                    break;
                default:
                    pictureBox4.Image = null;
                    break;
            }
            comboBoxArchitettura.Items.Clear();
            comboBoxVersione.Items.Clear();
            if (selectedOS == "Windows Live" || selectedOS == "Windows Server")
            {
                btnDownload.Visible = true;
                lblSelezionaLingua.Visible = false;
                comboBox_SelezionaLingua.Visible = false;
                labelArchitettura.Visible = false;
                comboBoxArchitettura.Visible= false;
                labelVersione.Visible = false;
                comboBoxVersione.Visible = false;
            }
            else if (selectedOS == "Windows 7" || selectedOS == "Windows 8.1")
            {
                labelVersione.Visible = true;
                comboBoxVersione.Visible = true;
                labelArchitettura.Visible = true;
                comboBoxArchitettura.Visible = true;
                _ = comboBoxArchitettura.Items.Add("x32");
                _ = comboBoxArchitettura.Items.Add("x64");
                _ = comboBoxVersione.Items.Add("Stock");
                _ = comboBoxVersione.Items.Add("Lite");
            }
            else if (selectedOS == "Windows 10")
            {
                labelArchitettura.Visible = true;
                comboBoxArchitettura.Visible = true;
                labelVersione.Visible = true;
                comboBoxVersione.Visible = true;
                _ = comboBoxArchitettura.Items.Add("x32");
                _ = comboBoxArchitettura.Items.Add("x64");
                _ = comboBoxArchitettura.Items.Add("arm64");
                _ = comboBoxVersione.Items.Add("ltsc");
                _ = comboBoxVersione.Items.Add("Stock");
                _ = comboBoxVersione.Items.Add("Lite");
            }
            else if (selectedOS == "Windows 11")
            {
                labelArchitettura.Visible = true;
                comboBoxArchitettura.Visible = true;
                labelVersione.Visible = true;
                comboBoxVersione.Visible = true;
                _ = comboBoxArchitettura.Items.Add("x64 - 23H2");
                _ = comboBoxArchitettura.Items.Add("x64 - 24H2");
                _ = comboBoxArchitettura.Items.Add("arm64");
                _ = comboBoxVersione.Items.Add("ltsc");
                _ = comboBoxVersione.Items.Add("Stock");
                _ = comboBoxVersione.Items.Add("Lite");
            }
            else
            {
                labelArchitettura.Visible = false;
                comboBoxArchitettura.Visible = false;
                labelVersione.Visible = false;
                comboBoxVersione.Visible = false;
                selectedArch = null;
            }

            AggiornaDescrizione();
        }
        private void comboBox_SelezionaLingua_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedLanguage = comboBox_SelezionaLingua.SelectedItem.ToString();
            if (selectedLanguage == "EN")
            {
                int liteIndex = comboBoxVersione.FindStringExact("Lite");
                if (liteIndex != -1)
                {
                    comboBoxVersione.Items.RemoveAt(liteIndex);
                }
            }
            else
            {
                if (comboBoxVersione.FindStringExact("Lite") == -1 && selectedOS != null)
                {
                    if (selectedOS == "Windows 7" || selectedOS == "Windows 8.1" || selectedOS == "Windows 10" || selectedOS == "Windows 11")
                    {
                        _ = comboBoxVersione.Items.Add("Lite");
                    }
                }
            }

            AggiornaDescrizione();
        }

        private void comboBoxArchitettura_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedArch = comboBoxArchitettura.SelectedItem?.ToString();
            if (selectedArch != null && selectedArch.ToLower().Contains("arm64"))
            {
                int index = comboBoxVersione.FindStringExact("stock");
                if (index >= 0)
                {
                    comboBoxVersione.SelectedIndex = index;
                    comboBoxVersione.Enabled = false;
                }
                else
                {

                }
            }
            else
            {
                comboBoxVersione.Enabled = true;
            }

            AggiornaDescrizione();
        }

        private void comboBoxVersione_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedVersion = comboBoxVersione.SelectedItem?.ToString();
            AggiornaDescrizione();
            btnDownload.Visible = true;
        }

        private void AggiornaDescrizione()
        {
            richTextBoxDescription.Clear();
            richTextBoxInfo.Clear();

            if (string.IsNullOrEmpty(selectedOS))
            {
                richTextBoxDescription.AppendText(LanguageManager.GetTranslation("FormWin", "nointernet"), Color.OrangeRed, FontStyle.Bold);
                return;
            }

            richTextBoxDescription.AppendText(LanguageManager.GetTranslation("FormWin", "configurazioneSelezionata") + "\n", Color.DodgerBlue, FontStyle.Bold | FontStyle.Underline, 12);

            richTextBoxDescription.AppendText("🖥️ ", Color.DodgerBlue, FontStyle.Regular, 11);
            richTextBoxDescription.AppendText(LanguageManager.GetTranslation("FormWin", "sistemaOperativo") + ": ", Color.Gray, FontStyle.Bold, 11);
            richTextBoxDescription.AppendText($"{selectedOS}\n", Color.Orange, FontStyle.Regular, 11);

            richTextBoxDescription.AppendText("🌍 ", Color.DodgerBlue, FontStyle.Regular, 11);
            richTextBoxDescription.AppendText(LanguageManager.GetTranslation("FormWin", "lingua") + ": ", Color.Gray, FontStyle.Bold, 11);
            richTextBoxDescription.AppendText($"{selectedLanguage}\n", Color.Orange, FontStyle.Regular, 11);

            string versioneSelezionata = null;

            if (!string.IsNullOrWhiteSpace(comboBoxVersione?.Text))
            {
                versioneSelezionata = comboBoxVersione.Text.ToLower();
            }

            // Scrive informazioni aggiuntive nel richTextBoxInfo
            richTextBoxInfo.AppendText("ℹ️ " + LanguageManager.GetTranslation("FormWin", "versionInfoLabel") + ":\n\n", Color.DodgerBlue, FontStyle.Bold, 11);

            if (selectedOS != "Windows Server" && selectedOS != "Windows Live")
            {
                if (!string.IsNullOrEmpty(selectedArch))
                {
                    richTextBoxDescription.AppendText("🏗️ ", Color.DodgerBlue, FontStyle.Regular, 11);
                    richTextBoxDescription.AppendText(LanguageManager.GetTranslation("FormWin", "architettura") + ": ", Color.Gray, FontStyle.Bold, 11);
                    richTextBoxDescription.AppendText($"{selectedArch}\n", Color.Orange, FontStyle.Regular, 11);
                }

                richTextBoxDescription.AppendText("📦 ", Color.DodgerBlue, FontStyle.Regular, 11);
                richTextBoxDescription.AppendText(LanguageManager.GetTranslation("FormWin", "versione") + ": ", Color.Gray, FontStyle.Bold, 11);

                if (selectedOS == "Windows 7")
                {
                    if (versioneSelezionata == "stock")
                    {
                        richTextBoxDescription.AppendText($"{selectedVersion} - HomeBasic, HomePremium, Professional, Enterprise e Ultimate\n", Color.Orange, FontStyle.Regular, 11);
                        richTextBoxInfo.AppendText("• " + LanguageManager.GetTranslation("FormWin", "stockMeaning") + " (Windows 7)\n", Color.Gray, FontStyle.Regular, 10);
                    }
                    else if (versioneSelezionata == "lite")
                    {
                        richTextBoxDescription.AppendText($"{selectedVersion} - Ultimate\n", Color.Orange, FontStyle.Regular, 11);
                        richTextBoxInfo.AppendText("• " + LanguageManager.GetTranslation("FormWin", "liteMeaning") + " (Windows 7)\n", Color.Gray, FontStyle.Regular, 10);
                    }
                }
                else if (selectedOS == "Windows 8.1")
                {
                    if (versioneSelezionata == "stock")
                    {
                        richTextBoxDescription.AppendText($"{selectedVersion} - Core, Enterprise\n", Color.Orange, FontStyle.Regular, 11);
                        richTextBoxInfo.AppendText("• " + LanguageManager.GetTranslation("FormWin", "stockMeaning") + " (Windows 8.1)\n", Color.Gray, FontStyle.Regular, 10);
                    }
                    else if (versioneSelezionata == "lite")
                    {
                        richTextBoxDescription.AppendText($"{selectedVersion} - Enterprise\n", Color.Orange, FontStyle.Regular, 11);
                        richTextBoxInfo.AppendText("• " + LanguageManager.GetTranslation("FormWin", "liteMeaning") + " (Windows 8.1)\n", Color.Gray, FontStyle.Regular, 10);
                    }
                }

                if (selectedArch != null && selectedArch.ToLower().Contains("arm64") && versioneSelezionata == "stock")
                {
                    richTextBoxDescription.AppendText("Stock - Pro, Enterprise\n", Color.Orange, FontStyle.Regular, 11);
                    richTextBoxInfo.AppendText("• " + LanguageManager.GetTranslation("FormWin", "stockMeaning") + " (ARM64)\n", Color.Gray, FontStyle.Regular, 10);
                }
                else if (selectedOS == "Windows 10")
                {
                    if (versioneSelezionata == "stock")
                    {
                        richTextBoxDescription.AppendText($"{selectedVersion} - Consumer\n", Color.Orange, FontStyle.Regular, 11);
                        richTextBoxInfo.AppendText("• " + LanguageManager.GetTranslation("FormWin", "consumerMeaning") + " (Windows 10)\n", Color.Gray, FontStyle.Regular, 10);
                    }
                    else if (versioneSelezionata == "lite" || versioneSelezionata == "ltsc")
                    {
                        richTextBoxDescription.AppendText($"{selectedVersion} - LTSC\n", Color.Orange, FontStyle.Regular, 11);
                        richTextBoxInfo.AppendText("• " + LanguageManager.GetTranslation("FormWin", "ltscMeaning") + " (Windows 10)\n", Color.Gray, FontStyle.Regular, 10);
                    }
                }
                else if (selectedOS == "Windows 11")
                {
                    if (versioneSelezionata == "lite")
                    {
                        string architettura = comboBoxArchitettura.SelectedItem?.ToString();

                        if (architettura == "x64 - 23H2")
                        {
                            richTextBoxDescription.AppendText($"{selectedVersion} - PRO\n", Color.Orange, FontStyle.Regular, 11);
                            richTextBoxInfo.AppendText("• " + LanguageManager.GetTranslation("FormWin", "liteMeaning") + " (Windows 11 23H2)\n", Color.Gray, FontStyle.Regular, 10);
                        }
                        else if (architettura == "x64 - 24H2")
                        {
                            richTextBoxDescription.AppendText($"{selectedVersion} - LTSC\n", Color.Orange, FontStyle.Regular, 11);
                            richTextBoxInfo.AppendText("• " + LanguageManager.GetTranslation("FormWin", "ltscMeaning") + " (Windows 11 24H2)\n", Color.Gray, FontStyle.Regular, 10);
                        }
                    }
                    else if (versioneSelezionata == "stock")
                    {
                        richTextBoxDescription.AppendText($"{selectedVersion} - Consumer\n", Color.Orange, FontStyle.Regular, 11);
                        richTextBoxInfo.AppendText("• " + LanguageManager.GetTranslation("FormWin", "consumerMeaning") + " (Windows 11)\n", Color.Gray, FontStyle.Regular, 10);
                    }
                    else if (versioneSelezionata == "ltsc")
                    {
                        selectedArch = "x64";
                        richTextBoxDescription.AppendText($"{selectedArch}\n", Color.Orange, FontStyle.Regular, 11);
                        richTextBoxInfo.AppendText("• " + LanguageManager.GetTranslation("FormWin", "ltscMeaning") + " (Windows 11)\n", Color.Gray, FontStyle.Regular, 10);
                    }
                }

                if (string.IsNullOrEmpty(selectedArch) && string.IsNullOrEmpty(selectedVersion))
                {
                    richTextBoxDescription.AppendText(LanguageManager.GetTranslation("FormWin", "selezionaOpzioneValida"), Color.OrangeRed, FontStyle.Bold, 11);
                }
            }
            else
            {
                // Informazioni per Windows Server e Windows Live
                richTextBoxInfo.AppendText($"• {selectedOS}: " + LanguageManager.GetTranslation("FormWin", "serverMeaning") + "\n", Color.Gray, FontStyle.Regular, 10);
            }

            // Note sulla configurazione
            richTextBoxInfo.AppendText("🔹 " + LanguageManager.GetTranslation("FormWin", "infoconfigurationLabel") + "\n", Color.Gray, FontStyle.Bold, 10);
            richTextBoxInfo.AppendText("• " + LanguageManager.GetTranslation("FormWin", "consumerMeaning") + "\n", Color.Gray, FontStyle.Regular, 10);
            richTextBoxInfo.AppendText("• " + LanguageManager.GetTranslation("FormWin", "enterpriseMeaning") + "\n", Color.Gray, FontStyle.Regular, 10);
            richTextBoxInfo.AppendText("• " + LanguageManager.GetTranslation("FormWin", "ltscMeaning") + "\n", Color.Gray, FontStyle.Regular, 10);
            richTextBoxInfo.AppendText("• " + LanguageManager.GetTranslation("FormWin", "stockMeaning") + "\n", Color.Gray, FontStyle.Regular, 10);
            richTextBoxInfo.AppendText("• " + LanguageManager.GetTranslation("FormWin", "liteMeaning") + "\n", Color.Gray, FontStyle.Regular, 10);

            richTextBoxDescription.AppendText("\n────────────────────────\n", Color.LightGray, FontStyle.Regular, 11);
            richTextBoxDescription.AppendText(LanguageManager.GetTranslation("FormWin", "verificaImpostazioni"), Color.SteelBlue, FontStyle.Italic, 10);
        }

        private async Task<DownloadConfig> ScaricaConfigurazioneAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync("https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json");
                return JsonConvert.DeserializeObject<DownloadConfig>(json);
            }
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            ScaricaISO();
        }

        private async void ScaricaISO()
        {
            if (selectedOS == "Windows Live" || selectedOS == "Windows Server")
            {
                var config = await ScaricaConfigurazioneAsync();
                var altreIso = config.AltreIso;

                string key = selectedOS.Contains("Live") ? "Live" : "Server";

                if (altreIso.TryGetValue(key, out string url))
                {
                    _ = Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else
                {
                    _ = MessageBox.Show(LanguageManager.GetTranslation("FormWin", "noisoLinkFound").Replace("{os}", selectedOS));
                }
                return;
            }

            if (string.IsNullOrEmpty(selectedOS) || string.IsNullOrEmpty(selectedVersion) || string.IsNullOrEmpty(selectedArch))
            {
                _ = MessageBox.Show(LanguageManager.GetTranslation("FormWin", "missingSelection"));
                return;
            }

            string prefix = selectedOS switch
            {
                "Windows 11" => "11",
                "Windows 10" => "10",
                "Windows 8.1" => "8",
                "Windows 7" => "7",
                _ => ""
            };

            if (string.IsNullOrEmpty(prefix))
            {
                _ = MessageBox.Show(LanguageManager.GetTranslation("FormWin", "unsupportedOS"));
                return;
            }

            string formKey = "FormWin" + prefix;

            string arch = selectedArch.ToLower();
            string versionPart = "";

            if (arch.Contains("x64"))
            {
                if (arch.Contains("24h2"))
                {
                    versionPart = "24h2";
                    arch = "x64";
                }
                else
                {
                    versionPart = "";
                    arch = "x64";
                }
            }
            else if (arch.Contains("arm64"))
            {
                arch = "Arm64";
            }

            string version = selectedVersion;
            string chiave = arch == "Arm64"
                ? prefix + arch
                : $"{prefix}{version}{arch.ToLower()}{versionPart}";

            _ = MostraInfoSHA256(selectedOS, formKey, selectedVersion, selectedArch, versionPart);

            var configNormale = await ScaricaConfigurazioneAsync();
            var dictLingua = typeof(DownloadConfig).GetProperty(formKey)?.GetValue(configNormale) as Dictionary<string, Dictionary<string, string>>;

            if (dictLingua != null && dictLingua.TryGetValue(selectedLanguage, out var dict))
            {
                if (dict.TryGetValue(chiave, out string urlNormale))
                {
                    _ = Process.Start(new ProcessStartInfo(urlNormale) { UseShellExecute = true });
                }
                else
                {
                    _ = MessageBox.Show(LanguageManager.GetTranslation("FormWin", "combinationNotAvailable").Replace("{combination}", chiave));
                }
            }
            else
            {
                _ = MessageBox.Show(LanguageManager.GetTranslation("FormWin", "noLanguageFound").Replace("{lang}", selectedLanguage));
            }
        }

        private async Task MostraInfoSHA256(string prefix, string formKey, string version, string arch, string versionPart)
        {
            richTextBoxInfo.Clear();

            string prefixPulito = prefix.Replace("Windows ", "");
            string chiave = arch == "Arm64"
                ? $"{prefixPulito}Sha256{arch}"
                : $"{prefixPulito}Sha256{version}{arch}{versionPart}";
            var configNormale = await ScaricaConfigurazioneAsync();
            var dictLingua = typeof(DownloadConfig).GetProperty(formKey)?.GetValue(configNormale) as Dictionary<string, Dictionary<string, string>>;

            if (dictLingua != null && dictLingua.TryGetValue(selectedLanguage, out var dict))
            {
                richTextBoxInfo.AppendText(LanguageManager.GetTranslation("FormWin", "integrityCheckHeader"), Color.DodgerBlue, FontStyle.Bold | FontStyle.Underline, 12);

                if (dict.TryGetValue(chiave, out string sha256))
                {
                    richTextBoxInfo.AppendTextInfo("\n✅ ", Color.Green, FontStyle.Bold, 10);
                    richTextBoxInfo.AppendTextInfo(LanguageManager.GetTranslation("FormWin", "validSha256Found"), Color.DarkGreen, FontStyle.Bold, 10);

                    richTextBoxInfo.AppendTextInfo("\n🔹 " + LanguageManager.GetTranslation("FormWin", "configurationLabel"), Color.Gray, FontStyle.Bold, 10);
                    richTextBoxInfo.AppendTextInfo($"{prefix}{version}{arch}\n", Color.Orange, FontStyle.Regular, 10);

                    richTextBoxInfo.AppendTextInfo("\n📋 " + LanguageManager.GetTranslation("FormWin", "sha256CodeLabel"), Color.DodgerBlue, FontStyle.Bold, 10);
                    richTextBoxInfo.AppendTextInfo($"{sha256}\n", Color.DarkSlateBlue, FontStyle.Bold, 10, new FontFamily("Consolas"));
                    richTextBoxInfo.AppendTextInfo("\nℹ️ " + LanguageManager.GetTranslation("FormWin", "sha256InfoBox"), Color.SteelBlue, FontStyle.Italic, 9);
                }
                else
                {
                    richTextBoxInfo.AppendTextInfo("\n❌ ", Color.Red, FontStyle.Bold, 11);
                    richTextBoxInfo.AppendTextInfo(LanguageManager.GetTranslation("FormWin", "sha256NotFound"), Color.DarkRed, FontStyle.Bold, 9);

                    richTextBoxInfo.AppendTextInfo("\n🔹 " + LanguageManager.GetTranslation("FormWin", "requiredConfigLabel"), Color.Gray, FontStyle.Bold, 9);
                    richTextBoxInfo.AppendTextInfo($"{chiave}\n", Color.Black, FontStyle.Regular, 9);

                    richTextBoxInfo.AppendTextInfo("\n⚠️ " + LanguageManager.GetTranslation("FormWin", "possibleCausesLabel"), Color.Orange, FontStyle.Bold, 9);
                    richTextBoxInfo.AppendTextInfo("\n• " + LanguageManager.GetTranslation("FormWin", "unsupportedConfig"), Color.DarkOrange, FontStyle.Regular, 9);
                    richTextBoxInfo.AppendTextInfo("\n• " + LanguageManager.GetTranslation("FormWin", "dataRetrievalError"), Color.DarkOrange, FontStyle.Regular, 9);
                    richTextBoxInfo.AppendTextInfo("\n• " + LanguageManager.GetTranslation("FormWin", "updateRequired"), Color.DarkOrange, FontStyle.Regular, 9);
                }
            }
            else
            {
                _ = MessageBox.Show(LanguageManager.GetTranslation("FormWin", "noLanguageFound").Replace("{lang}", selectedLanguage));
            }

            richTextBoxInfo.AppendText("\n\n────────────────────────────\n", Color.LightGray, FontStyle.Regular, 10);
        }
    }

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color, FontStyle style = FontStyle.Regular, float size = 9)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.SelectionFont = new Font(box.Font.FontFamily, size, style);
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
    public static class RichTextBoxExtensionsInfo
    {
        public static void AppendTextInfo(this RichTextBox box, string text, Color color,
                                    FontStyle style = FontStyle.Regular, float size = 11,
                                    FontFamily? fontFamily = null)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.SelectionFont = new Font(fontFamily ?? box.Font.FontFamily, size, style);
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
    public class DownloadConfig
    {
        public required Dictionary<string, Dictionary<string, string>> FormWin10 { get; set; }
        public required Dictionary<string, Dictionary<string, string>> FormWin11 { get; set; }
        public required Dictionary<string, Dictionary<string, string>> FormWin8 { get; set; }
        public required Dictionary<string, Dictionary<string, string>> FormWin7 { get; set; }
        public Dictionary<string, string> AltreIso { get; set; } = new Dictionary<string, string>();
    }

}
