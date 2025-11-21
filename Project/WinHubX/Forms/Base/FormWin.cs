using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using WinHubX.Forms.Base;
using WinHubX.Forms.DriverRST;
using WinHubX.Forms.ImpostazioniApp;
using WinHubX.Impostazioni;

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
            LanguageManager.LoadLanguageFromSettings();
            InitializeComponent();
            btnAttivaWindowsPrinci.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Attiva Windows",
                "en" => "  Activate Windows",
                _ => btnAttivaWindowsPrinci.Content
            };
            btnCambiaEdizionePrinci.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Cambia edizione",
                "en" => "  Change edition",
                _ => btnCambiaEdizionePrinci.Content
            };
            btnCreaIsoPrinci.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Crea ISO",
                "en" => "  Create ISO",
                _ => btnCreaIsoPrinci.Content
            };
            this.form1 = form1;
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }

        private async void btnAttivaWin_Click(object sender, EventArgs e)
        {
            string primaryURL = string.Empty;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                OperatingSystem os = Environment.OSVersion;
                string osName = GetWindowsVersionName(os);
                if (IsWindowsServer())
                {
                    string serverScriptUrl = "https://github.com/MrNico98/WinHubX-Resource/releases/download/WinHubX-Risorse/TSforge_Activation.cmd";
                    await ExecuteScriptFromUrl(serverScriptUrl);
                    return;
                }
                if (osName == "Windows 7" || osName == "Windows 8" || osName == "Windows 8.1")
                {
                    ExtractAndExecuteLocalScript();
                    return;
                }

                if (IsInternetAvailable())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var jsonResponse = await client.GetStringAsync(Dipendenze.GitHubConfigUrl);
                        var jsonObject = JObject.Parse(jsonResponse);

                        primaryURL = jsonObject["FormWin"]?["attivatorewin"]?.ToString() ?? string.Empty;
                    }

                    if (string.IsNullOrWhiteSpace(primaryURL))
                    {
                        MessageBox.Show("Impossibile trovare 'attivatorewin' in Dipendenze.json", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
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

                    if (osName == "Windows 10")
                        ExtractAndExecuteLocalScript();
                    else
                        ExtractAndExecuteLocalScriptKMS38();
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsWindowsServer()
        {
            try
            {
                string productName = string.Empty;
                using (var baseKey64 = Microsoft.Win32.RegistryKey.OpenBaseKey(
                    Microsoft.Win32.RegistryHive.LocalMachine,
                    Microsoft.Win32.RegistryView.Registry64))
                {
                    using (var subKey = baseKey64.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                    {
                        productName = subKey?.GetValue("ProductName")?.ToString() ?? string.Empty;
                    }
                }
                if (string.IsNullOrEmpty(productName))
                {
                    using (var baseKey32 = Microsoft.Win32.RegistryKey.OpenBaseKey(
                        Microsoft.Win32.RegistryHive.LocalMachine,
                        Microsoft.Win32.RegistryView.Registry32))
                    {
                        using (var subKey = baseKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                        {
                            productName = subKey?.GetValue("ProductName")?.ToString() ?? string.Empty;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(productName) &&
                    productName.Contains("Server", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                string osName = GetWindowsVersionName(Environment.OSVersion);
                if (osName.Contains("Server", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            catch
            {

            }

            return false;
        }

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
                    return "Windows 11"; 
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
                    var jsonResponse = await client.GetStringAsync(Dipendenze.GitHubConfigUrl);
                    var jsonObject = JObject.Parse(jsonResponse);
                    primaryURL = jsonObject["FormWin"]?["cambiowin"]?.ToString() ?? string.Empty;
                }
                if (string.IsNullOrWhiteSpace(primaryURL))
                {
                    File.AppendAllText(logFile, "primaryURL non trovato nel JSON.");
                    return;
                }
                using (HttpClient client = new HttpClient())
                {
                    byte[] fileBytes = await client.GetByteArrayAsync(primaryURL);
                    await File.WriteAllBytesAsync(tempScript, fileBytes);
                }
                var startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{tempScript}\"",
                    UseShellExecute = true,
                    CreateNoWindow = false
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFile, ex.Message);
            }
        }

        private void btnCreaIso_Click(object sender, EventArgs e)
        {
            panel50.Controls.Clear();
            Form1 mainForm = Application.OpenForms["Form1"] as Form1;
            if (mainForm == null) return;
            mainForm.pictureBox3.Visible = true;
            mainForm.pictureBox3.Click += (s, ev) =>
            {
                mainForm.pictureBox3.Visible = false;
                mainForm.LoadForm(new FormWin(mainForm), mainForm.btnWin, LanguageManager.GetTranslation("FormMain", "Windows"));
            };
            mainForm.pictureBoxlblalto.Image = btnCreaIsoPrinci.Image;
            mainForm.lblPanelTitle.Text = LanguageManager.GetTranslation("FormWin", "CreaISO");
            FormCreaISO formCreaIso = new FormCreaISO(mainForm);
            formCreaIso.TopLevel = false;
            formCreaIso.FormBorderStyle = FormBorderStyle.None;
            formCreaIso.Dock = DockStyle.Fill;
            panel50.Controls.Add(formCreaIso);
            formCreaIso.Show();
        }

        private void btnDownloadIsoPrinci_Click(object sender, EventArgs e)
        {
            string url = "https://mrnico98.github.io/ISODownloader/";

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossibile aprire il browser: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDriverRSTPrinci_Click(object sender, EventArgs e)
        {
            FormDriverRST formDriverRST = new FormDriverRST();
            formDriverRST.Show();
        }
    }
}
