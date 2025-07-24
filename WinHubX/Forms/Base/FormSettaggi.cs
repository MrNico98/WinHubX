using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Management;
using System.Reflection;
using WinHubX.Dialog;
using WinHubX.Forms.Settaggi;

namespace WinHubX.Forms.Base
{
    public partial class FormSettaggi : Form
    {
        private Form1 form1;
        private string? wsa11x64;
        private string? wsa11arm64;
        private string? wsa10x64;
        private FormPersonalizzazione? formPersonalizzazione;
        public FormSettaggi(Form1 form1)
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            this.form1 = form1;
            LoadJsonLinks();
            LoadPcSpec();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }
        private void EnsureFormPersonalizzazioneIsCreated()
        {
            if (formPersonalizzazione == null || formPersonalizzazione.IsDisposed)
            {
                formPersonalizzazione = new FormPersonalizzazione(this, form1);
            }
        }

        private void LoadPcSpec()
        {
            try
            {
                DriveInfo systemDrive = GetSystemDrive();
                string driveType = GetDriveType(systemDrive);
                bool isSSD = driveType == "SSD" || driveType == "NVME";
                string ramSize = GetSystemRAM();

                EnsureFormPersonalizzazioneIsCreated();

                if (formPersonalizzazione != null)
                {
                    formPersonalizzazione.ImpostaSpecifichePC(driveType, ramSize, isSSD);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DriveInfo GetSystemDrive()
        {
            string systemDirectory = Environment.SystemDirectory;
            string root = Path.GetPathRoot(systemDirectory);

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name.Equals(root, StringComparison.OrdinalIgnoreCase))
                {
                    return drive;
                }
            }

            throw new Exception("Impossibile trovare il disco di sistema");
        }

        private string GetDriveType(DriveInfo drive)
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive"))
                {
                    foreach (ManagementObject disk in searcher.Get())
                    {
                        if (disk["MediaType"] != null)
                        {
                            string mediaType = disk["MediaType"].ToString();
                            string model = disk["Model"]?.ToString() ?? "";

                            if (mediaType.Contains("Fixed hard disk media"))
                            {
                                if (model.Contains("SSD") || mediaType.Contains("SSD"))
                                    return "SSD";
                                if (model.Contains("NVMe") || model.Contains("NVME"))
                                    return "NVME";
                                return "HDD";
                            }
                        }
                    }
                }

                string driveLetter = drive.Name.Substring(0, 1);
                using (var searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_LogicalDisk WHERE DeviceID = '{driveLetter}:'"))
                {
                    foreach (ManagementObject disk in searcher.Get())
                    {
                        if (disk["MediaType"] != null && disk["MediaType"].ToString() == "12")
                        {
                            return "SSD";
                        }
                    }
                }

                return "HDD";
            }
            catch
            {
                return "Sconosciuto";
            }
        }

        private string GetSystemRAM()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
                {
                    foreach (ManagementObject item in searcher.Get())
                    {
                        double totalMemory = Convert.ToDouble(item["TotalPhysicalMemory"]) / (1024 * 1024 * 1024);
                        return $"{Math.Round(totalMemory)} GB";
                    }
                }
            }
            catch
            {
                return "Sconosciuto";
            }

            return "Sconosciuto";
        }


        private void btnPrivacy_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = "Windows Privacy";
            form1.PnlFormLoader.Controls.Clear();
            FormPrivacy formPrivacy = new FormPrivacy(this, form1) { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            formPrivacy.FormBorderStyle = FormBorderStyle.None;
            form1.PnlFormLoader.Controls.Add(formPrivacy);
            formPrivacy.Show();
        }

        private void btnUtility_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = "Utility";
            form1.PnlFormLoader.Controls.Clear();
            FormUtility formUtility = new FormUtility(this, form1) { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            formUtility.FormBorderStyle = FormBorderStyle.None;
            form1.PnlFormLoader.Controls.Add(formUtility);
            formUtility.Show();
        }

        private void btnDefender_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = "Windows Defender";
            form1.PnlFormLoader.Controls.Clear();
            FormDefender formDefender = new FormDefender(this, form1) { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            formDefender.FormBorderStyle = FormBorderStyle.None;
            form1.PnlFormLoader.Controls.Add(formDefender);
            formDefender.Show();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = "Windows Update";
            form1.PnlFormLoader.Controls.Clear();
            FormUpdate formUpdate = new FormUpdate(this, form1) { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            formUpdate.FormBorderStyle = FormBorderStyle.None;
            form1.PnlFormLoader.Controls.Add(formUpdate);
            formUpdate.Show();
        }

        private void btnRipristinaSO_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = LanguageManager.GetTranslation("FormSettaggi", "restoreos");
            form1.PnlFormLoader.Controls.Clear();
            FormRipristinoSO formRipristinoSO = new FormRipristinoSO(this, form1) { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            formRipristinoSO.FormBorderStyle = FormBorderStyle.None;
            form1.PnlFormLoader.Controls.Add(formRipristinoSO);
            formRipristinoSO.Show();
        }

        private async void LoadJsonLinks()
        {
            string url = "https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync(url);
                    JObject data = JObject.Parse(json);

                    wsa11x64 = data["WSA"]["win11x64"]?.ToString();
                    wsa11arm64 = data["WSA"]["win11arm64"]?.ToString();
                    wsa10x64 = data["WSA"]["win10x64"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnAttivaWSA_Click(object sender, EventArgs e)
        {
            string systemType = GetSystemType();
            string downloadUrl = "";
            string zipFileName = "";

            if (systemType.Contains("Windows 11"))
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    downloadUrl = wsa11x64;
                    zipFileName = "WSAwin11x64.zip";
                }
                else
                {
                    downloadUrl = wsa11arm64;
                    zipFileName = "WSAwin11arm64.zip";
                }
            }
            else if (systemType.Contains("Windows 10"))
            {
                downloadUrl = wsa10x64;
                zipFileName = "WSAwin10x64.zip";
            }

            if (string.IsNullOrEmpty(downloadUrl))
            {
                _ = MessageBox.Show(LanguageManager.GetTranslation("FormSettaggi", "downloadlinknotfound"), "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FormOperazioni progressForm = new FormOperazioni();
            progressForm.Show();

            try
            {
                progressForm.SetStatus("Downloading...", 0);
                string downloadPath = Path.Combine(Path.GetTempPath(), zipFileName);
                string extractPath = Path.Combine(Path.GetTempPath(), "WSA");

                using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromMinutes(30) })
                using (var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    _ = response.EnsureSuccessStatusCode();
                    var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    var canReportProgress = totalBytes != -1;

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        var buffer = new byte[81920];
                        long totalRead = 0;
                        int bytesRead;

                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalRead += bytesRead;

                            if (canReportProgress)
                            {
                                int progress = (int)((totalRead * 100) / totalBytes);
                                progressForm.SetStatus("Downloading...", progress);
                            }
                        }
                    }
                }

                progressForm.SetStatus("Extraction in progress...", 100);
                ZipFile.ExtractToDirectory(downloadPath, extractPath, true);

                string batFilePath = Path.Combine(extractPath, "Run.bat");
                if (File.Exists(batFilePath))
                {
                    var process = Process.Start(new ProcessStartInfo(batFilePath) { UseShellExecute = true });
                    process?.WaitForExit();
                }
                else
                {
                    _ = MessageBox.Show(LanguageManager.GetTranslation("FormSettaggi", "runbatnotfound"), "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                progressForm.CompleteOperation();
            }
            catch (Exception ex)
            {
                progressForm.SetStatus($"Errore: {ex.Message}");
                _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressForm.Close();
            }
            PacManDialog pacManDialog = new PacManDialog
            {
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen
            };
            _ = pacManDialog.ShowDialog();
        }


        private string GetSystemType()
        {
            string osName = "";
            string osArchitecture = Environment.Is64BitOperatingSystem ? "x64" : "ARM64";

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                osName = os["Caption"].ToString();
                break;
            }

            return $"{osName} {osArchitecture}";
        }

        private void btnAttivaWSL_Click(object sender, EventArgs e)
        {
            try
            {
                string assemblyName1 = Assembly.GetExecutingAssembly().GetName().Name;
                string resourcePath1 = $"{assemblyName1}.Resources.WinHubXWSL.ps1";
                byte[] exeBytes1 = LoadEmbeddedResource1(resourcePath1);
                string ps1FilePath1 = Path.Combine(Path.GetTempPath(), "WinHubXWSL.ps1");
                File.WriteAllBytes(ps1FilePath1, exeBytes1);

                StartPowerShell1(ps1FilePath1);
            }
            finally { }
        }

        private byte[] LoadEmbeddedResource1(string resourcePath)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Error: {resourcePath}");
                }
                byte[] buffer = new byte[stream.Length];
                _ = stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        private void StartPowerShell1(string scriptFilePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Bypass -File \"{scriptFilePath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                _ = process.Start();
                string output = process.StandardOutput.ReadToEnd();
            }
        }

        private void btnPersonalizzazione_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = LanguageManager.GetTranslation("FormSettaggi", "customization");
            form1.PnlFormLoader.Controls.Clear();

            formPersonalizzazione = new FormPersonalizzazione(this, form1)
            {
                Dock = DockStyle.Fill,
                TopLevel = false,
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None
            };

            form1.PnlFormLoader.Controls.Add(formPersonalizzazione);
            formPersonalizzazione.Show();
            LoadPcSpec();
        }


        private void btnEsportaSettaggi_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Title = LanguageManager.GetTranslation("FormSettaggi", "exporttitle");
                dlg.Filter = "Dat file (*.dat)|*.dat|Tutti i file (*.*)|*.*";
                dlg.FileName = "config.dat";
                dlg.InitialDirectory = Application.StartupPath;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string exportPath = dlg.FileName;
                    string keyToExport = @"HKEY_CURRENT_USER\Software\WinHubX";

                    var process = new Process();
                    process.StartInfo.FileName = "reg.exe";
                    process.StartInfo.Arguments = $"export \"{keyToExport}\" \"{exportPath}\" /y";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;

                    try
                    {
                        _ = process.Start();
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                        {
                            _ = MessageBox.Show(
                                string.Format(LanguageManager.GetTranslation("FormSettaggi", "exportsuccess"), exportPath),
                                LanguageManager.GetTranslation("FormSettaggi", "exportdone"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                        else
                        {
                            _ = MessageBox.Show(
                                string.Format(LanguageManager.GetTranslation("FormSettaggi", "exporterrorcode"), process.ExitCode),
                                LanguageManager.GetTranslation("FormSettaggi", "error"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(
                            string.Format(LanguageManager.GetTranslation("FormSettaggi", "exportexception"), ex.Message),
                            LanguageManager.GetTranslation("FormSettaggi", "exception"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        private void btnImportaSettaggi_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Seleziona file di registro da importare";
                dlg.Filter = "Dat file (*.dat)|*.dat|Tutti i file (*.*)|*.*";
                dlg.InitialDirectory = Application.StartupPath;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string filePath = dlg.FileName;

                    var process = new Process();
                    process.StartInfo.FileName = "reg.exe";
                    process.StartInfo.Arguments = $"import \"{filePath}\"";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;

                    try
                    {
                        _ = process.Start();
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                        {
                            _ = MessageBox.Show("Settaggi importati correttamente dal file .dat.",
                                "Importazione completata", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IstanziaEAvviaFormSelezionati();
                        }
                        else
                        {
                            _ = MessageBox.Show($"Errore durante l'importazione. Codice uscita: {process.ExitCode}",
                                "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show($"Si è verificato un errore:\n{ex.Message}",
                            "Eccezione", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        public void ImportaSettaggiDaPercorso(string filePath)
        {
            var process = new Process();
            process.StartInfo.FileName = "reg.exe";
            process.StartInfo.Arguments = $"import \"{filePath}\"";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;

            try
            {
                _ = process.Start();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine("Settaggi importati correttamente dal file .dat.");
                    IstanziaEAvviaFormSelezionati();
                }
                else
                {
                    Console.WriteLine($"Errore durante l'importazione. Codice uscita: {process.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Si è verificato un errore:\n{ex.Message}");
            }
        }



        private void IstanziaEAvviaFormSelezionati()
        {
            var formList = new List<Form>
    {
        new FormPrivacy(this, form1),
        new FormUtility(this, form1),
        new FormDefender(this, form1),
        new FormUpdate(this, form1),
        new FormPersonalizzazione(this, form1)
    };

            foreach (Form form in formList)
            {
                form.TopLevel = false;
                form.TopMost = true;
                form.FormBorderStyle = FormBorderStyle.None;
                form.Dock = DockStyle.Fill;
                form.CreateControl();
                form.Show();
                var metodo = form.GetType().GetMethod("btnAvviaSelezionati_Click", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var bottone = form.Controls.Find("btnAvviaSelezionati", true).FirstOrDefault();

                if (metodo != null && bottone != null)
                {
                    _ = metodo.Invoke(form, new object[] { bottone, EventArgs.Empty });
                }
                form.Close();
            }
        }

    }
}
