using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Text.Json;
using WinHubX.Dialog;
using WinHubX.Forms.CreaISO;

namespace WinHubX.Forms.Base
{
    public partial class FormCreaISO : Form
    {
        private Form1 form1;
        private string selectedFile = string.Empty;

        public FormCreaISO(Form1 form1)
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            this.form1 = form1;
            groupBox7.Hide();
            groupBox6.Hide();
            pictureBox7.Hide();
            pictureBox4.Hide();
            this.FormClosing += FormCreaISO_FormClosing;
            this.ActiveControl = btn_browser;
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }

        string IsoMountLetter = string.Empty;
        string installwimpath = string.Empty;

        public void ExecuteCommand(string command, bool ShowMessage)
        {
            if (!ShowMessage)
            {
                var startInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = command,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = Process.Start(startInfo))
                {
                    process.WaitForExit();

                    var output = process.StandardOutput.ReadToEnd();
                    var error = process.StandardError.ReadToEnd();
                }
            }
            else if (ShowMessage)
            {
            }
        }
        private async Task ScaricaFileAsync(string url, string destinazione)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    _ = response.EnsureSuccessStatusCode();
                    using (FileStream fs = new FileStream(destinazione, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fs);
                    }
                }
            }
        }

        private async Task<string> GetZipUrlFromJsonAsync(string jsonUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string jsonResponse = await client.GetStringAsync(jsonUrl);
                    using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                    {
                        JsonElement root = doc.RootElement;
                        string? zipUrl = root.GetProperty("CreaISOWIN").GetProperty("creaiso").GetString();
                        return zipUrl ?? string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return string.Empty;
                }
            }
        }

        private async void btn_CreaISO_Click(object sender, EventArgs e)
        {

            string zipUrl = await GetZipUrlFromJsonAsync("https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json");
            string zipFilePath = Path.Combine(Path.GetTempPath(), "RisorseCreaISO.zip");

            try
            {
                if (!File.Exists(zipFilePath))
                {
                    await ScaricaFileAsync(zipUrl, zipFilePath);
                }

                string tempPath = Path.Combine(Path.GetTempPath(), "RisorseCreaISO");
                if (!Directory.Exists(tempPath))
                {
                    _ = Directory.CreateDirectory(tempPath);
                }

                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string destinazioneFile = Path.Combine(tempPath, entry.FullName);
                        if (string.IsNullOrEmpty(entry.Name))
                        {
                            continue;
                        }
                        string directoryPath = Path.GetDirectoryName(destinazioneFile);
                        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                        {
                            _ = Directory.CreateDirectory(directoryPath);
                        }

                        entry.ExtractToFile(destinazioneFile, overwrite: true);
                    }
                }
            }
            catch (Exception)
            {

                return;
            }

            ExecuteCommand("Dismount-DiskImage -ImagePath \"" + selectedFile + "\"", false);

            string ComboSelected = "";
            string comboxstr = comboBox1.Text;
            int index = comboxstr.IndexOf(' ');
            if (index > 0)
            {
                ComboSelected = comboxstr.Substring(0, index);
            }

            string windowsVersion = Win10Rad.Checked ? "10" : Win11Rad.Checked ? "11" : "";
            string edgeRemovalPreference = RemEdgeRad.Checked ? "RemoveEdge" : NotRemEdgeRad.Checked ? "SiEdge" : "";
            string defenderPreference = DisWindDefRad.Checked ? "DisableWindowsDefender" : NotDisWinDefRad.Checked ? "SiDefender" : "";
            string Processi = RemProcRad.Checked ? "RimuoviProcessi" : NotRemProcRad.Checked ? "NonRimuovereProcessi" : "";
            string Unattend = Win11BypassRad.Checked ? "Bypass" : Win11StockRad.Checked ? "Stock" : "";
            string Architettura = SixforArchRad.Checked ? "x64" : ThirTwoRad.Checked ? "x32" : "";
            string DebloatApp = DebAppRad.Checked ? "Debloat" : StockAppRad.Checked ? "NonDebloat" : "";
            string ImportaSettaggiWinhubx = ImpSettWinHub.Checked ? "SiImporta" : NonImportWinhub.Checked ? "NonImporta" : "";
            string DriverWin = DriverCartella.Checked ? "DriverCartella" : DriverQuestoPC.Checked ? "DriverQuestoPC" : NoDriver.Checked ? "NoDriver" : "";
            string TipoOttimizzazione = IsoLite.Checked ? "IsoLite" : IsoLavorWork.Checked ? "LavorWork" : IsoGaming.Checked ? "IsoGaming" : "";

            var parametri = new Dictionary<string, string>
    {
        { "windowsVersion", windowsVersion },
        { "edgeRemovalPreference", edgeRemovalPreference },
        { "defenderPreference", defenderPreference },
        { "Processi", Processi },
        { "Unattend", Unattend },
        { "Architettura", Architettura },
        { "DebloatApp", DebloatApp },
        { "ComboSelected", ComboSelected },
        { "SelectedFile", selectedFile },
        { "ImportaSettaggiWinhubx", ImportaSettaggiWinhubx },
        { "DriverWin", DriverWin },
        { "TipoOttimizzazione", TipoOttimizzazione },
    };

            FormCreazioneISO nuovaForm = new FormCreazioneISO(form1, this)
            {
                ParametriISO = parametri
            };
            string lbltitle = LanguageManager.GetTranslation("FormCreaISO", "creazioneiso");
            form1.lblPanelTitle.Text = lbltitle;
            form1.PnlFormLoader.Controls.Clear();
            nuovaForm.Dock = DockStyle.Fill;
            nuovaForm.TopLevel = false;
            nuovaForm.TopMost = true;
            nuovaForm.FormBorderStyle = FormBorderStyle.None;
            form1.PnlFormLoader.Controls.Add(nuovaForm);
            nuovaForm.Show();
            this.Close();
        }

        private void btn_browser_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "ISO Files (*.iso)|*.iso|All files (*.*)|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFile = openFileDialog.FileName;
                textBox1.Text = selectedFile;
                var mountInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"Mount-DiskImage -ImagePath '{selectedFile}'\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = Process.Start(mountInfo))
                {
                    process.WaitForExit();
                }
                var startInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"(Get-DiskImage -ImagePath '{selectedFile}' | Get-Volume).DriveLetter\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    var output = process.StandardOutput.ReadToEnd();
                    IsoMountLetter = output.Trim();
                }

                if (string.IsNullOrWhiteSpace(IsoMountLetter))
                {
                    _ = MessageBox.Show("Impossibile trovare la lettera di unità montata.");
                    return;
                }
                if (File.Exists($"{IsoMountLetter}:\\sources\\install.wim"))
                {
                    installwimpath = $"{IsoMountLetter}:\\sources\\install.wim";
                }
                else if (File.Exists($"{IsoMountLetter}:\\sources\\install.esd"))
                {
                    installwimpath = $"{IsoMountLetter}:\\sources\\install.esd";
                }
                else
                {
                    _ = MessageBox.Show("Impossibile trovare install.wim o install.esd.");
                    return;
                }
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    var dismInfo = new ProcessStartInfo()
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-Command \"dism /english /Get-WimInfo /WimFile:'{installwimpath}'\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    using (var process = Process.Start(dismInfo))
                    {
                        process.WaitForExit();

                        var output = process.StandardOutput.ReadToEnd();

                        string input = output;

                        int indiceNome = 0;
                        int primoind = 1;
                        while ((indiceNome = input.IndexOf("Name", indiceNome)) != -1)
                        {
                            int indiceFineValore = input.IndexOfAny(new char[] { '\r', '\n' }, indiceNome + 6);
                            if (indiceFineValore != -1)
                            {
                                string valoreNome = input.Substring(indiceNome + 6, indiceFineValore - indiceNome - 6).Trim();
                                comboBox1.Invoke(new Action(() =>
                                    comboBox1.Items.Add($"{primoind} - {valoreNome.Replace(":", "")}")
                                ));
                                primoind++;
                            }
                            indiceNome = indiceFineValore + 1;
                        }
                    }
                }).Start();
            }
        }


        private void Win10Rad_CheckedChanged_1(object sender, EventArgs e)
        {
            groupBox6.Show();
            groupBox7.Hide();
            pictureBox7.Hide();
            pictureBox4.Show();
        }

        private void Win11Rad_CheckedChanged_1(object sender, EventArgs e)
        {
            groupBox6.Hide();
            groupBox7.Show();
            pictureBox7.Show();
            pictureBox4.Hide();
        }

        private void FormCreaISO_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (!string.IsNullOrEmpty(IsoMountLetter))
            {
                ExecuteCommand("Dismount-DiskImage -ImagePath " + "\"" + selectedFile + "\"", false);
            }
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            string description = LanguageManager.GetTranslation("FormCreaISO", "domanda");

            InfoDialog creaisodescription = new InfoDialog(description)
            {
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen
            };
            creaisodescription.Show();
        }
    }
}