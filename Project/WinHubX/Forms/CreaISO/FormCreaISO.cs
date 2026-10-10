using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;
using WinHubX.Forms.CreaISO;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Base
{
    public partial class FormCreaISO : Form
    {
        private static bool IsExpectedIsoResourceFailure(Exception exception) =>
            exception is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or System.ComponentModel.Win32Exception
                or HttpRequestException
                or System.Text.Json.JsonException
                or InvalidDataException
                or InvalidOperationException
                or NotSupportedException
                or OperationCanceledException
                or TimeoutException
                or ArgumentException;

        private static readonly HttpClient ResourceClient = new(new SocketsHttpHandler
        {
            AllowAutoRedirect = false,
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            AutomaticDecompression = System.Net.DecompressionMethods.All
        })
        {
            Timeout = TimeSpan.FromMinutes(2)
        };

        private readonly Form1 form1;
        private string selectedFile = string.Empty;
        private string percorsoCompletoISO = string.Empty;
        private string? _pendingResourceSessionPath;
        private bool _resourceSessionTransferred;
        private CancellationTokenSource? _resourcePreparationCancellation;
        public FormCreaISO(Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
            groupBox7.Hide();
            groupBox6.Hide();
            pictureBox7.Hide();
            pictureBox4.Hide();
            ActiveControl = btn_browserBianco;
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            FormClosed += OnFormClosedCleanupResourceSession;
            string downloadPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
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
        string? installwimpath;

        private async Task ScaricaFileAsync(string url, string destinazione, CancellationToken cancellationToken)
        {
            await BitsTransferDownloader.DownloadFileAsync(url, destinazione, null, cancellationToken);
        }

        private async Task<string> GetZipUrlFromGitHubConfigAsync(CancellationToken cancellationToken)
        {
            try
            {
                string json = await TrustedHttpsClient.GetStringAsync(
                    ResourceClient,
                    Dipendenze.GitHubConfigUrl,
                    cancellationToken);
                JObject obj = JObject.Parse(json);
                string? url = obj["FormWin"]?["creaISOzip"]?.ToString();

                if (string.IsNullOrWhiteSpace(url))
                    throw new InvalidDataException("URL ZIP non trovato in Dipendenze.json");

                return url;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (IsExpectedIsoResourceFailure(ex))
            {
                throw new InvalidDataException("Impossibile ottenere zipUrl.", ex);
            }
        }

        private async void btn_CreaISO_Click(object? sender, EventArgs e)
        {
            if (_resourcePreparationCancellation is not null)
                return;

            using var cancellation = new CancellationTokenSource();
            _resourcePreparationCancellation = cancellation;
            btn_CreaISOVerdi.Enabled = false;
            try
            {
                await CreateIsoAsync(cancellation.Token);
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
            {
                // Closing the form cancels resource acquisition without showing a spurious error.
            }
            finally
            {
                if (ReferenceEquals(_resourcePreparationCancellation, cancellation))
                    _resourcePreparationCancellation = null;

                if (!_resourceSessionTransferred && _pendingResourceSessionPath is { } sessionPath)
                {
                    try
                    {
                        PrivateUserWorkspace.DeleteSession(sessionPath);
                        _pendingResourceSessionPath = null;
                    }
                    catch (Exception cleanupException) when (cleanupException is IOException or UnauthorizedAccessException or InvalidDataException)
                    {
                        Debug.WriteLine($"Pulizia workspace risorse ISO non riuscita: {cleanupException}");
                    }
                }

                if (!IsDisposed)
                    btn_CreaISOVerdi.Enabled = true;
            }
        }

        private async Task CreateIsoAsync(CancellationToken cancellationToken)
        {
            string comboxstr = comboBox1.Text.Trim();
            bool selezioniSistemaValide =
                IsAnySelected(RemProcRad, NotRemProcRad) &&
                IsAnySelected(DebAppRad, StockAppRad) &&
                IsAnySelected(NotDisWinDefRad, DisWindDefRad);
            bool selezioniImmagineValide =
                IsAnySelected(NotRemEdgeRad, RemEdgeRad) &&
                IsAnySelected(IsoLite, IsoLavorWork, IsoGaming) &&
                IsAnySelected(DriverCartella, DriverQuestoPC, NoDriver);
            bool selezioniValide = selezioniSistemaValide && selezioniImmagineValide;
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

            string resourceSessionPath;
            try
            {
                resourceSessionPath = PrivateUserWorkspace.CreateSession();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Security.SecurityException)
            {
                _ = MessageBox.Show(this, $"Impossibile creare il workspace privato per le risorse ISO: {ex.Message}",
                    "WinHubX — risorse ISO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string zipFilePath = Path.Join(resourceSessionPath, "RisorseCreaISO.zip");
            string stagingPath = Path.Join(resourceSessionPath, "staging");
            string resourcePath = Path.Join(resourceSessionPath, "RisorseCreaISO");
            bool resourcePrepared = false;
            try
            {
                string zipUrl = await GetZipUrlFromGitHubConfigAsync(cancellationToken);
                await ScaricaFileAsync(zipUrl, zipFilePath, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                SafeZipExtractor.ExtractToFreshDirectory(zipFilePath, stagingPath);
                Directory.Move(stagingPath, resourcePath);
                cancellationToken.ThrowIfCancellationRequested();
                resourcePrepared = true;
                _pendingResourceSessionPath = resourceSessionPath;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex) when (IsExpectedIsoResourceFailure(ex))
            {
                _ = MessageBox.Show(this, $"Impossibile verificare o estrarre le risorse ISO: {ex.Message}",
                    "WinHubX — risorse ISO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                try
                {
                    if (File.Exists(zipFilePath))
                        File.Delete(zipFilePath);
                    if (Directory.Exists(stagingPath))
                        Directory.Delete(stagingPath, recursive: true);
                    if (!resourcePrepared)
                        PrivateUserWorkspace.DeleteSession(resourceSessionPath);
                }
                catch (Exception cleanupException) when (IsExpectedIsoResourceFailure(cleanupException))
                {
                    Debug.WriteLine($"Pulizia risorse ISO temporanee non completata: {cleanupException}");
                }
            }

            _ = await RunPowerShellAsync(
                $"$ErrorActionPreference = 'Stop'; Dismount-DiskImage -ImagePath '{EscapePowerShellLiteral(selectedFile)}'");
            cancellationToken.ThrowIfCancellationRequested();
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

            FormCreazioneISO nuovaForm = new FormCreazioneISO(form1, this, resourceSessionPath)
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
            _resourceSessionTransferred = true;
            Close();
        }

        private static bool IsAnySelected(params RadioButton[] options)
        {
            return options.Any(static option => option.Checked);
        }

        private void OnFormClosedCleanupResourceSession(object? sender, FormClosedEventArgs e)
        {
            _resourcePreparationCancellation?.Cancel();
            if (_resourcePreparationCancellation is not null)
                return;

            if (_resourceSessionTransferred || _pendingResourceSessionPath is not { } sessionPath)
                return;

            try
            {
                PrivateUserWorkspace.DeleteSession(sessionPath);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException)
            {
                Debug.WriteLine($"Pulizia workspace risorse ISO non riuscita: {ex.Message}");
            }
        }

        private async void btn_browser_Click(object? sender, EventArgs e)
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
            catch (Exception ex) when (IsExpectedIsoResourceFailure(ex))
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

        private static string? GetInstallImagePath(string driveLetter)
        {
            string basePath = $"{driveLetter}:\\sources";
            string wimPath = Path.Join(basePath, "install.wim");
            string esdPath = Path.Join(basePath, "install.esd");

            if (File.Exists(wimPath)) return wimPath;
            if (File.Exists(esdPath)) return esdPath;
            return null;
        }

        private async Task LoadWimInfoAsync(string wimPath)
        {
            string output = await RunPowerShellOutputAsync(
                $"dism /english /Get-WimInfo /WimFile:'{EscapePowerShellLiteral(wimPath)}'"
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
                StartInfo = CreatePowerShellStartInfo(command)
            };
            process.Start();
            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
            Task<string> errorTask = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            await Task.WhenAll(outputTask, errorTask);
            string standardError = await errorTask;
            if (!string.IsNullOrWhiteSpace(standardError))
                Debug.WriteLine($"PowerShell error: {standardError.Trim()}");
            return process.ExitCode;
        }

        private static async Task<string> RunPowerShellOutputAsync(string command)
        {
            using var process = new Process
            {
                StartInfo = CreatePowerShellStartInfo(command)
            };
            process.Start();
            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
            Task<string> errorTask = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            await Task.WhenAll(outputTask, errorTask);
            string standardOutput = await outputTask;
            string standardError = await errorTask;
            if (!string.IsNullOrWhiteSpace(standardError))
                Debug.WriteLine($"PowerShell error: {standardError.Trim()}");
            return standardOutput;
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

        private void FormCreaISO_Resize(object sender, EventArgs e)
        {
            AggiornaPercorsoLabel(percorsoCompletoISO);
        }
    }
}
