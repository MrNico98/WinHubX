using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Net;
using WinHubX.Dialog;
using WinHubX.Forms.Personalizzazione_office;

namespace WinHubX
{
    public partial class FormOffice : Form
    {
        private Form1 form1;
        private NotifyIcon notifyIcon;
        private string link32_2019, link64_2019, fileName_2019, sha256_2019;
        private string link32_2021, link64_2021, fileName_2021, sha256_2021;
        private string link32_2024, link64_2024, fileName_2024, sha256_2024;
        private string link32_365, link64_365, fileName_365, sha256_365;

        public FormOffice(Form1 form1)
        {
            InitializeComponent();
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = false
            };
            this.form1 = form1;
        }

        #region Link
        private async Task LoadOfficeLinks()
        {
            string jsonUrl = "https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json";

            try
            {
                var jsonData = await GetJsonDataAsync(jsonUrl);

                // Office 2019
                link32_2019 = jsonData["Office2019"]["Officex32"]?.ToString();
                link64_2019 = jsonData["Office2019"]["Officex64"]?.ToString();
                fileName_2019 = jsonData["Office2019"]["Offline2019"]?.ToString();
                sha256_2019 = jsonData["Office2019"]["sha2562019"]?.ToString();

                // Office 2021
                link32_2021 = jsonData["Office2021"]["Officex32"]?.ToString();
                link64_2021 = jsonData["Office2021"]["Officex64"]?.ToString();
                fileName_2021 = jsonData["Office2021"]["Offline2021"]?.ToString();
                sha256_2021 = jsonData["Office2021"]["sha2562021"]?.ToString();

                // Office 2024
                link32_2024 = jsonData["Office2024"]["Officex32"]?.ToString();
                link64_2024 = jsonData["Office2024"]["Officex64"]?.ToString();
                fileName_2024 = jsonData["Office2024"]["Offline2024"]?.ToString();
                sha256_2024 = jsonData["Office2024"]["sha2562024"]?.ToString();

                // Office 365
                link32_365 = jsonData["Office365"]["Officex32"]?.ToString();
                link64_365 = jsonData["Office365"]["Officex64"]?.ToString();
                fileName_365 = jsonData["Office365"]["Offline365"]?.ToString();
                sha256_365 = jsonData["Office365"]["sha256365"]?.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<JObject> GetJsonDataAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                return JObject.Parse(response);
            }
        }

        #endregion



        #region Office2019

        private void btn2019Online_MouseUp(object sender, MouseEventArgs e)
        {
            OfficeDialog officeDialog = new OfficeDialog()
            {
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen
            };
            officeDialog.openDialog(lblOffice2019, link32_2019, link64_2019);
            officeDialog.ShowDialog();
        }

        private void btn2019Offline_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Clipboard.SetText(sha256_2019);
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("Global", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("Global", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = fileName_2019,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Office2021

        private void btn2021Online_MouseUp(object sender, MouseEventArgs e)
        {
            OfficeDialog officeDialog = new OfficeDialog()
            {
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen
            };
            officeDialog.openDialog(lblOffice2021, link32_2021, link64_2021);
            officeDialog.ShowDialog();
        }

        private void btn2021Offline_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Clipboard.SetText(sha256_2021);
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("FormOffice", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("FormOffice", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = fileName_2021,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region Office365

        private void btn365Online_MouseUp(object sender, MouseEventArgs e)
        {
            OfficeDialog officeDialog = new OfficeDialog()
            {
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen
            };
            officeDialog.openDialog(lblOffice365, link32_365, link64_365);
            officeDialog.ShowDialog();
        }

        private void btn365Offline_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Clipboard.SetText(sha256_365);
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("FormOffice", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("FormOffice", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = fileName_365,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Office2024


        private void btn2024Online_MouseUp(object sender, MouseEventArgs e)
        {
            OfficeDialog officeDialog = new OfficeDialog()
            {
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen
            };
            officeDialog.openDialog(lblOffice2024, link32_2024, link64_2024);
            officeDialog.ShowDialog();
        }

        private void btn2024Offline_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Clipboard.SetText(sha256_2024);
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("FormOffice", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("FormOffice", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = fileName_2024,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

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
                    MessageBox.Show(
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
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = tempFilePath,
                        UseShellExecute = true,
                        Verb = "runas"
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                byte[] scriptBytes = Properties.Resources.TSforge_Activation_Office;

                File.WriteAllBytes(scriptPath, scriptBytes);
                Process.Start(new ProcessStartInfo
                {
                    FileName = scriptPath,
                    UseShellExecute = true,
                    Verb = "runas"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                Directory.CreateDirectory(tempFolder);

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

                process.Start();
                await Task.Run(() => process.WaitForExit());
                await Task.Run(() => AttendiScrubberConTitolo("Office Scrubber v12"));
                Directory.Delete(tempFolder, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task AttendiScrubberConTitolo(string titolo, int timeoutMs = 10 * 60 * 1000)
        {
            int waited = 0;
            Process scrubberProc = null;

            while (waited < timeoutMs)
            {
                scrubberProc = Process.GetProcessesByName("powershell")
                    .FirstOrDefault(p => p.MainWindowTitle.Contains(titolo));

                if (scrubberProc != null)
                    break;

                await Task.Delay(1000);
                waited += 1000;
            }

            if (scrubberProc != null)
            {
                scrubberProc.WaitForExit();
            }
        }

        private void btnPersonalizzaOffice_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = LanguageManager.GetTranslation("FormOffice", "paneltitle");
            form1.PnlFormLoader.Controls.Clear();
            PersonalizzazioneOffice formPersonalizzazioneOffice = new PersonalizzazioneOffice(form1, this) { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            formPersonalizzazioneOffice.FormBorderStyle = FormBorderStyle.None;
            form1.PnlFormLoader.Controls.Add(formPersonalizzazioneOffice);
            formPersonalizzazioneOffice.Show();
        }

        private async void FormOffice_Load(object sender, EventArgs e)
        {
            await LoadOfficeLinks();
        }
    }
}
