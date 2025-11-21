using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using WinHubX.Forms.CreaISO;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Base
{
    public partial class FormCreaISO : Form
    {
        private Form1 form1;
        private string selectedFile = string.Empty;
        private string percorsoCompletoISO;
        public FormCreaISO(Form1 form1)
        {
            InitializeComponent();
            form1 = form1;
            groupBox7.Hide();
            groupBox6.Hide();
            pictureBox7.Hide();
            pictureBox4.Hide();
            ActiveControl = btn_browserBianco;
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            labelpercorso.Text = $"{downloadPath}";
            percorsoCompletoISO = downloadPath;
            btn_CreaISOVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Crea ISO",
                "en" => "  Create ISO",
                _ => btn_CreaISOVerdi.Content
            };
            btn_browserBianco.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Seleziona",
                "en" => "  Select",
                _ => btn_browserBianco.Content
            };
            btn_cambiaBianco.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "Cambia",
                "en" => "Change",
                _ => btn_cambiaBianco.Content
            };
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
        private async Task<string> GetZipUrlFromGitHubConfigAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var json = await client.GetStringAsync(Dipendenze.GitHubConfigUrl);
                    var obj = JObject.Parse(json);
                    string? url = obj["FormWin"]?["creaISOzip"]?.ToString();

                    if (string.IsNullOrWhiteSpace(url))
                        throw new Exception("URL ZIP non trovato in Dipendenze.json");

                    return url;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Impossibile ottenere zipUrl: " + ex.Message);
            }
        }

        private async void btn_CreaISO_Click(object sender, EventArgs e)
        {
            string comboxstr = comboBox1.Text.Trim();
            bool selezioniValide =
                (RemProcRad.Checked || NotRemProcRad.Checked) &&
                (DebAppRad.Checked || StockAppRad.Checked) &&
                (NotDisWinDefRad.Checked || DisWindDefRad.Checked) &&
                (NotRemEdgeRad.Checked || RemEdgeRad.Checked) &&
                (IsoLite.Checked || IsoLavorWork.Checked || IsoGaming.Checked) && 
                (DriverCartella.Checked || DriverQuestoPC.Checked || NoDriver.Checked);
            if (comboxstr.Contains("10"))
            {
                selezioniValide &= (SixforArchRad.Checked || ThirTwoRad.Checked);
            }
            else if (comboxstr.Contains("11"))
            {
                selezioniValide &= (Win11StockRad.Checked || Win11BypassRad.Checked);
            }
            if (!selezioniValide)
            {
                MessageBox.Show(
                    LanguageManager.GetTranslation("FormCreaISO", "selezione_obbligatoria_msg"),
                    LanguageManager.GetTranslation("FormCreaISO", "selezione_obbligatoria_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            string zipUrl = await GetZipUrlFromGitHubConfigAsync();
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
            AppState.IsoMontata = false;
            AppState.IsoPath = null;

            string ComboSelected = "";
            string windowsVersion = "";
            if (comboxstr.Contains("10"))
            {
                windowsVersion = "10";
            }
            else if (comboxstr.Contains("11"))
            {
                windowsVersion = "11";
            }
            else
            {
                windowsVersion = "Sconosciuto";
            }
            int index = comboxstr.IndexOf(' ');
            if (index > 0)
            {
                ComboSelected = comboxstr.Substring(0, index);
            }

            string edgeRemovalPreference = RemEdgeRad.Checked ? "RemoveEdge" : NotRemEdgeRad.Checked ? "SiEdge" : "";
            string defenderPreference = DisWindDefRad.Checked ? "DisableWindowsDefender" : NotDisWinDefRad.Checked ? "SiDefender" : "";
            string Processi = RemProcRad.Checked ? "RimuoviProcessi" : NotRemProcRad.Checked ? "NonRimuovereProcessi" : "";
            string Unattend = Win11BypassRad.Checked ? "Bypass" : Win11StockRad.Checked ? "Stock" : "";
            string Architettura = SixforArchRad.Checked ? "x64" : ThirTwoRad.Checked ? "x32" : "";
            string DebloatApp = DebAppRad.Checked ? "Debloat" : StockAppRad.Checked ? "NonDebloat" : "";
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
            Close();
        }

        private async void btn_browser_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "ISO Files (*.iso)|*.iso|All files (*.*)|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            selectedFile = openFileDialog.FileName;
            textBox10.Text = selectedFile;
            try
            {
                Cursor = Cursors.WaitCursor;
                btn_browserBianco.Enabled = false;

                if (!await MountIsoAsync(selectedFile))
                {
                    MessageBox.Show("Errore durante il montaggio dell'immagine ISO.");
                    return;
                }

                IsoMountLetter = await GetIsoDriveLetterAsync(selectedFile);

                if (string.IsNullOrWhiteSpace(IsoMountLetter))
                {
                    MessageBox.Show("Impossibile trovare la lettera di unità montata.");
                    AppState.IsoMontata = false;
                    AppState.IsoPath = null;
                    return;
                }
                AppState.IsoMontata = true;
                AppState.IsoPath = selectedFile;

                installwimpath = GetInstallImagePath(IsoMountLetter);

                if (installwimpath == null)
                {
                    MessageBox.Show("Impossibile trovare install.wim o install.esd.");
                    return;
                }

                await LoadWimInfoAsync(installwimpath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore: {ex.Message}");
            }
            finally
            {
                Cursor = Cursors.Default;
                btn_browserBianco.Enabled = true;
            }
        }

        private static async Task<bool> MountIsoAsync(string isoPath)
        {
            return await RunPowerShellAsync($"Mount-DiskImage -ImagePath '{isoPath}'") == 0;
        }

        private static async Task<string> GetIsoDriveLetterAsync(string isoPath)
        {
            string result = await RunPowerShellOutputAsync(
                $"(Get-DiskImage -ImagePath '{isoPath}' | Get-Volume).DriveLetter"
            );
            return result.Trim();
        }

        private static string GetInstallImagePath(string driveLetter)
        {
            string basePath = $"{driveLetter}:\\sources";
            string wimPath = Path.Combine(basePath, "install.wim");
            string esdPath = Path.Combine(basePath, "install.esd");

            if (File.Exists(wimPath)) return wimPath;
            if (File.Exists(esdPath)) return esdPath;
            return null;
        }

        private async Task LoadWimInfoAsync(string wimPath)
        {
            string output = await RunPowerShellOutputAsync(
                $"dism /english /Get-WimInfo /WimFile:'{wimPath}'"
            );

            var matches = Regex.Matches(output, @"Name\s*:\s*(.+)");

            comboBox1.Items.Clear();
            int index = 1;

            foreach (Match match in matches)
            {
                comboBox1.Items.Add($"{index++} - {match.Groups[1].Value.Trim()}");
            }

            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
        }

        private static async Task<int> RunPowerShellAsync(string command)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"{command}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            process.Start();
            await process.WaitForExitAsync();
            return process.ExitCode;
        }

        private static async Task<string> RunPowerShellOutputAsync(string command)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"{command}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            return output;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string comboxstr = comboBox1.Text.Trim();
            if (comboxstr.Contains("10"))
            {
                groupBox6.Show();
                groupBox7.Hide();
                pictureBox7.Hide();
                pictureBox4.Show();

            }
            else if (comboxstr.Contains("11"))
            {
                groupBox6.Hide();
                groupBox7.Show();
                pictureBox7.Show();
                pictureBox4.Hide();
            }
        }

        private void AggiornaPercorsoLabel(string downloadPath)
        {
            if (string.IsNullOrEmpty(percorsoCompletoISO) || percorsoCompletoISO != downloadPath)
                percorsoCompletoISO = downloadPath;
            int maxWidth = btn_cambiaBianco.Left - labelpercorso.Left - 10;
            int fullWidth = TextRenderer.MeasureText(percorsoCompletoISO, labelpercorso.Font).Width;
            if (fullWidth <= maxWidth)
            {
                labelpercorso.Text = percorsoCompletoISO;
                return;
            }
            string path = percorsoCompletoISO;
            while (TextRenderer.MeasureText(path + "...", labelpercorso.Font).Width > maxWidth && path.Contains("\\"))
            {
                int lastSlash = path.LastIndexOf('\\');
                if (lastSlash <= 0) break;
                path = path.Substring(0, lastSlash);
            }

            labelpercorso.Text = path + "...";
        }

        private void btn_cambia_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    AggiornaPercorsoLabel(dialog.SelectedPath);
                }
            }
        }

        private void FormCreaISO_Resize(object sender, EventArgs e)
        {
            AggiornaPercorsoLabel(percorsoCompletoISO);
        }
    }
}