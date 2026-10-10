using DiscUtils;
using DiscUtils.Udf;
using System.Data;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using WinHubX.Forms.Base;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.CreaISO
{
    public partial class FormCreazioneISO : Form
    {
        private static bool IsExpectedIsoOperationFailure(Exception exception) =>
            exception is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or System.ComponentModel.Win32Exception
                or System.Runtime.InteropServices.COMException
                or System.Management.ManagementException
                or System.Xml.XmlException
                or InvalidOperationException
                or NotSupportedException
                or ArgumentException
                or FormatException
                or OperationCanceledException
                or TimeoutException;

        private const string ResourceFilesFolderName = "Risorse";
        private const string PowerRunExecutableName = "PowerRun.exe";

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, string> ParametriISO { get; set; } = new();
        private readonly Form1 form1;
        private CancellationTokenSource? _cancellationTokenSource;
        private ElevatedProcessBrokerClient? _elevatedBroker;
        private string WorkspaceRoot => _elevatedBroker?.WorkspaceRoot
            ?? throw new InvalidOperationException("Workspace ISO non inizializzato.");
        private string IsoWorkingRoot => Path.Join(WorkspaceRoot, "ISO", "WinISO");
        private string InstallMountRoot => Path.Join(WorkspaceRoot, "Mount", "mount");
        private string BootMountRoot => Path.Join(WorkspaceRoot, "Mount", "boot");
        private readonly FormCreaISO formcreaiso;
        private readonly string resourceSessionPath;
        private string ResourceRoot => Path.Join(resourceSessionPath, "RisorseCreaISO");

        public FormCreazioneISO(Form1 form1, FormCreaISO formcreaiso, string resourceSessionPath)
        {
            LanguageManager.LoadTranslations();
            InitializeComponent();
            this.form1 = form1;
            this.formcreaiso = formcreaiso;
            this.resourceSessionPath = PrivateUserWorkspace.ValidateSessionPath(resourceSessionPath);
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            FormClosed += FormCreazioneISO_FormClosed;
        }
        private async void FormCreazioneISO_Shown(object? sender, EventArgs e)
        {
            using CancellationTokenSource cancellationTokenSource = new();
            _cancellationTokenSource = cancellationTokenSource;
            try
            {
                await Task.Delay(2000, cancellationTokenSource.Token);
                await StartAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
            {
                Debug.WriteLine("Avvio creazione ISO annullato durante la chiusura della finestra.");
            }
            finally
            {
                if (ReferenceEquals(_cancellationTokenSource, cancellationTokenSource))
                    _cancellationTokenSource = null;
            }
        }

        private readonly List<Task> taskList = new();

        private void FormCreazioneISO_FormClosed(object? sender, FormClosedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task StartAsync(CancellationToken token)
        {
            SetButtonsEnabled(false);
            btnStopVerdi.Visible = btnStopVerdi.Enabled = true;

            var steps = new (Func<CancellationToken, Task> action, int progress, int delay)[]
            {
        (Settaggi, 10, 3000),
        (CreazioneCartella, 20, 2000),
        (VerificaWIMoESD, 30, 2000),
        (MontaggioInstall, 40, 2000),
        (Unattend, 50, 2000),
        (RimozioneDiAlcuniProcessi, 60, 2000),
        (VerificaParametri, 70, 2000),
        (CopiaFileNecessari, 80, 2000),
        (CreazioneInstall, 90, 2000),
        (CreazioneISO, 95, 2000),
        (Finito, 100, 2000),
            };

            try
            {
                _elevatedBroker = await ElevatedProcessBrokerClient.StartAsync(token);
                foreach (var (action, progress, delay) in steps)
                {
                    await Task.Delay(delay, token);
                    await AddAndAwait(action(token));
                    progressBar1.Value = progress;
                    var stillRunning = taskList.Where(t => !t.IsCompleted).ToList();
                    if (stillRunning.Any())
                    {
                        string info = string.Join("\n", stillRunning.Select((t, i) => $"Task {i + 1} ancora attivo"));
                        MessageBox.Show("Warning! There are still active tasks:\n" + info);
                        return;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Operazione annullata.");
            }
            catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
            {
                MessageBox.Show($"Errore durante l'esecuzione: {ex.Message}");
            }
            finally
            {
                try
                {
                    if (_elevatedBroker is not null)
                        await _elevatedBroker.DisposeAsync();
                }
                catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
                {
                    Log($"Chiusura del broker UAC non riuscita: {ex.Message}");
                }
                finally
                {
                    _elevatedBroker = null;
                    if (!IsDisposed && !Disposing)
                    {
                        SetButtonsEnabled(true);
                        btnStopVerdi.Visible = false;
                    }
                    try
                    {
                        PrivateUserWorkspace.DeleteSession(resourceSessionPath);
                    }
                    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException)
                    {
                        Log($"Pulizia workspace risorse ISO non riuscita: {ex.Message}");
                    }
                }
            }
        }

        private async Task AddAndAwait(Task task)
        {
            taskList.Add(task);
            await task;
        }

        private void SetButtonsEnabled(bool enabled)
        {
            form1.btnHome.Enabled = enabled;
            form1.pictureBox3.Enabled = enabled;
            form1.btnWin.Enabled = enabled;
            form1.btnOffice.Enabled = enabled;
            form1.btnSettaggi.Enabled = enabled;
            form1.btnDebloat.Enabled = enabled;
            form1.btnmonitoraggio.Enabled = enabled;
        }

        private Task Finito(CancellationToken token)
        {
            progressBar2.Value = 0;
            try
            {
                _ = Invoke((MethodInvoker)delegate
                {
                    string successo1 = LanguageManager.GetTranslation("FormCreazioneISO", "successo1");
                    string percorsoISO = formcreaiso.labelpercorso.Text;
                    string successo2 = LanguageManager.FormatTranslation("FormCreazioneISO", "successo2", percorsoISO);

                    Color originalColor = richTextBox1.SelectionColor;
                    richTextBox1.SelectionColor = Color.Orange;
                    richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);

                    Log("\n\n" + successo1);
                    Log("\n" + successo2);

                    richTextBox1.SelectionColor = originalColor;
                    richTextBox1.ScrollToCaret();
                });

                btnStopVerdi.Visible = false;
                form1.btnHome.Enabled = true;
                form1.btnWin.Enabled = true;
                form1.btnOffice.Enabled = true;
                form1.btnSettaggi.Enabled = true;
                form1.btnDebloat.Enabled = true;
                form1.btnmonitoraggio.Enabled = true;
                form1.pictureBox3.Enabled = true;
            }
            catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
            {
                Log($"\nError: {ex.Message}");
            }

            return Task.CompletedTask;
        }


        private async Task Settaggi(CancellationToken token)
        {
            if (ParametriISO != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var kvp in ParametriISO)
                {
                    token.ThrowIfCancellationRequested();

                    _ = sb.AppendLine($"{kvp.Key} = {kvp.Value}");
                }
                richTextBox1.Text = sb.ToString();
            }

            await Task.CompletedTask;
        }

        private async Task CreazioneCartella(CancellationToken token)
        {
            if (ParametriISO == null || !ParametriISO.TryGetValue("SelectedFile", out var selectedFile))
                return;

            if (!File.Exists(selectedFile))
            {
                string erroreIsoNonTrovata = LanguageManager.GetTranslation("FormCreazioneISO", "erroreisonontrovata");
                Log($"{erroreIsoNonTrovata} {selectedFile}");
                return;
            }

            string extractPath = IsoWorkingRoot;
            Directory.CreateDirectory(extractPath);
            progressBar2.Value = 0;

            IProgress<int> progress = new Progress<int>(value =>
            {
                if (value >= 0 && value <= 100)
                {
                    progressBar2.Value = value;
                }
            });

            try
            {
                await Task.Run(async () =>
                {
                    token.ThrowIfCancellationRequested();

                    using var fs = File.OpenRead(selectedFile);
                    using var reader = new UdfReader(fs);

                    DiscDirectoryInfo root = reader.GetDirectoryInfo("");
                    int totalFiles = CountFiles(root);

                    if (totalFiles == 0)
                    {
                        string nessunFile = LanguageManager.GetTranslation("FormCreazioneISO", "nessunfile");
                        Log(nessunFile);
                        return;
                    }

                    int extractedFiles = 0;
                    ExtractDirectory(root, extractPath, ref extractedFiles, totalFiles, progress, token);

                }, token);

                string estrazioneOk = LanguageManager.GetTranslation("FormCreazioneISO", "estrazioneisook");
                Log(estrazioneOk);
            }
            catch (OperationCanceledException)
            {
                string aborted = LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullata");
                Log(aborted);
            }
            catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
            {
                string errore = LanguageManager.GetTranslation("FormCreazioneISO", "erroregenerico");
                Log($"{errore}: {ex.Message}");
            }
        }

        private void ExtractDirectory(DiscDirectoryInfo directory, string targetPath, ref int extractedFiles, int totalFiles, IProgress<int> progress, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            Directory.CreateDirectory(targetPath);

            foreach (var file in directory.GetFiles())
            {
                token.ThrowIfCancellationRequested();

                string destPath = SafePathResolver.ResolveContainedPath(targetPath, file.Name);
                using (var source = file.OpenRead())
                using (var dest = File.Create(destPath))
                {
                    source.CopyTo(dest, 81920);
                }

                extractedFiles++;
                int percent = (int)((double)extractedFiles / totalFiles * 100);
                progress?.Report(percent);
            }

            foreach (var subDir in directory.GetDirectories())
            {
                token.ThrowIfCancellationRequested();
                ExtractDirectory(subDir, SafePathResolver.ResolveContainedPath(targetPath, subDir.Name), ref extractedFiles, totalFiles,
                    progress ?? throw new InvalidOperationException("Progress reporter non inizializzato."), token);
            }
        }

        private int CountFiles(DiscDirectoryInfo directory)
        {
            int count = directory.GetFiles().Count();
            foreach (var subDir in directory.GetDirectories())
                count += CountFiles(subDir);
            return count;
        }

        private async Task VerificaWIMoESD(CancellationToken token)
        {
            string sourcesPath = Path.Join(IsoWorkingRoot, "sources");
            string esdPath = Path.Join(sourcesPath, "install.esd");
            string wimPath = Path.Join(sourcesPath, "install.wim");
            string wimProPath = Path.Join(sourcesPath, "install_pro.wim");

            try
            {
                if (ParametriISO == null || !ParametriISO.TryGetValue("ComboSelected", out var indexValue))
                {
                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "erroreindicenonselezionato"));
                    return;
                }
                var progress = new Progress<int>(value =>
                {
                    if (value >= 0 && value <= 100)
                    {
                        progressBar2.Value = value;
                    }
                });

                if (File.Exists(esdPath))
                {
                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "conversioneesdwim"));

                    string[] arguments =
                    [
                        "/export-image",
                        $"/SourceImageFile:{esdPath}",
                        $"/SourceIndex:{indexValue}",
                        $"/DestinationImageFile:{wimPath}",
                        "/Compress:max",
                        "/CheckIntegrity"
                    ];

                    token.ThrowIfCancellationRequested();
                    bool success = await EseguiDISM(arguments, progress, token);

                    if (success && File.Exists(wimPath))
                    {
                        File.Delete(esdPath);
                        Log(LanguageManager.GetTranslation("FormCreazioneISO", "conversionesuccesso"));
                    }
                }
                else if (File.Exists(wimPath))
                {
                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "trovatoinstallwim"));

                    string[] arguments =
                    [
                        "/export-image",
                        $"/SourceImageFile:{wimPath}",
                        $"/SourceIndex:{indexValue}",
                        $"/DestinationImageFile:{wimProPath}",
                        "/Compress:max",
                        "/CheckIntegrity"
                    ];

                    token.ThrowIfCancellationRequested();
                    bool success = await EseguiDISM(arguments, progress, token);

                    if (success && File.Exists(wimProPath))
                    {
                        File.Delete(wimPath);
                        File.Move(wimProPath, wimPath);
                        Log(LanguageManager.GetTranslation("FormCreazioneISO", "ottimizzazionesuccesso"));
                    }
                }
                else
                {
                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "nessunfilewimesd"));
                }
            }
            catch (OperationCanceledException)
            {
                Log(LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullatatoken"));
            }
            catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
            {
                Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroreoperazione")}: {ex.Message}");
                if (File.Exists(wimProPath))
                {
                    try { File.Delete(wimProPath); }
                    catch (Exception cleanupException) when (IsExpectedIsoOperationFailure(cleanupException))
                    {
                        Debug.WriteLine($"Impossibile rimuovere il WIM temporaneo '{wimProPath}': {cleanupException}");
                    }
                }
            }
        }

        private async Task<bool> EseguiDISM(IReadOnlyList<string> arguments, IProgress<int> progress, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                UpdateProgressBar(0, 100);

                ElevatedProcessBrokerClient broker = _elevatedBroker
                    ?? throw new InvalidOperationException("Broker UAC per DISM non inizializzato.");
                int exitCode = await broker.RunDismAsync(arguments, (line, isError) =>
                {
                    if (isError)
                        Log($"DISM: {line}");
                    int? value = ParseProgress(line);
                    if (value.HasValue)
                        progress?.Report(value.Value);
                }, token);

                if (exitCode == 0)
                    progress?.Report(100);
                else
                    Log($"DISM è terminato con codice {exitCode}.");
                return exitCode == 0;
            }
            catch (OperationCanceledException)
            {
                Log(LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullatatoken"));
                return false;
            }
            catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
            {
                Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroreoperazione")}: {ex.Message}");
                return false;
            }
        }

        private void UpdateProgressBar(int value, int? maximum = null)
        {
            if (progressBar2.InvokeRequired)
            {
                progressBar2.Invoke(new Action(() => UpdateProgressBar(value, maximum)));
                return;
            }

            if (maximum.HasValue)
                progressBar2.MaxValue = maximum.Value;
            progressBar2.Value = Math.Clamp(value, 0, progressBar2.MaxValue);
        }

        private async Task<(int ExitCode, string StandardOutput, string StandardError)> RunDismCapturingOutputAsync(
            IReadOnlyList<string> arguments,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            ElevatedProcessBrokerClient broker = _elevatedBroker
                ?? throw new InvalidOperationException("Broker UAC per DISM non inizializzato.");
            StringBuilder standardOutput = new();
            StringBuilder standardError = new();
            int exitCode = await broker.RunDismAsync(arguments, (line, isError) =>
            {
                StringBuilder destination = isError ? standardError : standardOutput;
                _ = destination.AppendLine(line);
            }, token);
            return (exitCode, standardOutput.ToString(), standardError.ToString());
        }

        private int? ParseProgress(string output)
        {
            if (output.Contains("%"))
            {
                Match match = Regex.Match(output, @"(\d+(?:\.\d+)?)%");
                if (match.Success && double.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double percent))
                    return Math.Min(Math.Max((int)Math.Round(percent), 0), 100);
            }
            return null;
        }


        private async Task MontaggioInstall(CancellationToken token)
        {
                string wimPath = Path.Join(IsoWorkingRoot, "sources", "install.wim");
                string mountDir = InstallMountRoot;

            try
            {
                if (!File.Exists(wimPath))
                {
                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "errorefilewimnontrovato"));
                    return;
                }

                Directory.CreateDirectory(mountDir);
                Log(LanguageManager.GetTranslation("FormCreazioneISO", "montaggioincorso"));

                var progress = new Progress<int>(value =>
                {
                    if (value >= 0 && value <= 100)
                    {
                        progressBar2.Value = value;
                    }
                });

                string[] arguments = ["/mount-image", $"/imagefile:{wimPath}", "/index:1", $"/mountdir:{mountDir}"];

                bool success = await EseguiDISM(arguments, progress, token);

                if (success)
                {
                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "montaggiosuccesso"));
                }
                else
                {
                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "erroremontaggio"));
                }
            }
            catch (OperationCanceledException)
            {
                Log(LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullatatoken"));
            }
            catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
            {
                Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroregenericomontaggio")}: {ex.Message}");
            }
        }

        private async Task Unattend(CancellationToken token)
        {
            try
            {
                if (ParametriISO == null || !ParametriISO.TryGetValue("windowsVersion", out var windowsVersion))
                {
                    Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "erroreversionewindows"));
                    return;
                }

                string sourceUnattend = Path.Join(ResourceRoot, ResourceFilesFolderName, "unattend.xml");
                string sourceUnattendStock = Path.Join(ResourceRoot, ResourceFilesFolderName, "unattendstock.xml");
                string destUnattend = Path.Join(IsoWorkingRoot, "sources", "$OEM$", "$$", "Panther", "unattend.xml");
                string mountDir = InstallMountRoot;
                string bootWimPath = Path.Join(IsoWorkingRoot, "sources", "boot.wim");
                string bootMountDir = BootMountRoot;
                string appraiserPath = Path.Join(IsoWorkingRoot, "sources", "appraiserres.dll");
                string appraiserBakPath = appraiserPath + ".bak";
                string sourceUnattend10 = Path.Join(ResourceRoot, ResourceFilesFolderName, "unattend10.xml");
                string sourceUnattendx32 = Path.Join(ResourceRoot, ResourceFilesFolderName, "unattendx32.xml");

                _ = Directory.CreateDirectory(Path.GetDirectoryName(destUnattend)
                    ?? throw new InvalidOperationException("Percorso unattend non valido."));
                _ = Directory.CreateDirectory(mountDir);
                _ = Directory.CreateDirectory(bootMountDir);
                _ = Directory.CreateDirectory(Path.GetDirectoryName(appraiserPath)
                    ?? throw new InvalidOperationException("Percorso appraiser non valido."));

                if (windowsVersion == "11" && ParametriISO.TryGetValue("Unattend", out var unattendType))
                {
                    if (unattendType == "Bypass")
                    {
                        if (File.Exists(sourceUnattend))
                        {
                            File.Copy(sourceUnattend, destUnattend, true);
                            Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "copiabypass"));
                        }
                        Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "configbypass"));

                        await Task.Run(async () =>
                        {
                            await ExecuteCommand($"reg load HKLM\\TK_COMPONENTS \"{mountDir}\\Windows\\System32\\config\\COMPONENTS\"", token);
                            await ExecuteCommand($"reg load HKLM\\TK_DEFAULT \"{mountDir}\\Windows\\System32\\config\\default\"", token);
                            await ExecuteCommand($"reg load HKLM\\TK_NTUSER \"{mountDir}\\Users\\Default\\ntuser.dat\"", token);
                            await ExecuteCommand($"reg load HKLM\\TK_SOFTWARE \"{mountDir}\\Windows\\System32\\config\\SOFTWARE\"", token);
                            await ExecuteCommand($"reg load HKLM\\TK_SYSTEM \"{mountDir}\\Windows\\System32\\config\\SYSTEM\"", token);
                            var regCommands = new List<string>
            {
                @"reg add ""HKLM\TK_SOFTWARE\Microsoft\Windows\CurrentVersion\Communications"" /v ""ConfigureChatAutoInstall"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_NTUSER\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"" /v ""OemPreInstalledAppsEnabled"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_NTUSER\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"" /v ""PreInstalledAppsEnabled"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_NTUSER\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"" /v ""SilentInstalledAppsEnabled"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_SOFTWARE\Policies\Microsoft\Windows\CloudContent"" /v ""DisableWindowsConsumerFeature"" /t REG_DWORD /d 1 /f",
                @"reg add ""HKLM\TK_NTUSER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"" /v ""ContentDeliveryAllowed"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_SOFTWARE\Microsoft\PolicyManager\current\device\Start"" /v ""ConfigureStartPins"" /t REG_SZ /d ""{\""pinnedList\"": [{}]}"" /f",
                @"reg add ""HKLM\TK_NTUSER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"" /v ""FeatureManagementEnabled"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_NTUSER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"" /v ""PreInstalledAppsEverEnabled"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_NTUSER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"" /v ""SoftLandingEnabled"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_NTUSER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"" /v ""SubscribedContentEnabled"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_NTUSER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"" /v ""SubscribedContent-310093Enabled"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_NTUSER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"" /v ""SubscribedContent-338388Enabled"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_NTUSER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"" /v ""SubscribedContent-338389Enabled"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_NTUSER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"" /v ""SubscribedContent-338393Enabled"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_NTUSER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"" /v ""SubscribedContent-353694Enabled"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_NTUSER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"" /v ""SubscribedContent-353696Enabled"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_NTUSER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"" /v ""SystemPaneSuggestionsEnabled"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_SOFTWARE\Policies\Microsoft\PushToInstall"" /v ""DisablePushToInstall"" /t REG_DWORD /d 1 /f",
                @"reg add ""HKLM\TK_SOFTWARE\Policies\Microsoft\MRT"" /v ""DontOfferThroughWUAU"" /t REG_DWORD /d 1 /f",
                @"reg delete ""HKLM\TK_NTUSER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager\Subscriptions"" /f",
                @"reg delete ""HKLM\TK_NTUSER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager\SuggestedApps"" /f",
                @"reg add ""HKLM\TK_SOFTWARE\Policies\Microsoft\Windows\CloudContent"" /v ""DisableConsumerAccountStateContent"" /t REG_DWORD /d 1 /f",
                @"reg add ""HKLM\TK_SOFTWARE\Policies\Microsoft\Windows\CloudContent"" /v ""DisableCloudOptimizedContent"" /t REG_DWORD /d 1 /f",
                @"reg add ""HKLM\TK_SOFTWARE\Microsoft\Windows\CurrentVersion\ReserveManager"" /v ""ShippedWithReserves"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_SOFTWARE\Policies\Microsoft\Windows\Windows Chat"" /v ""ChatIcon"" /t REG_DWORD /d 3 /f",
                @"reg add ""HKLM\TK_NTUSER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced"" /v ""TaskbarMn"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_DEFAULT\Control Panel\UnsupportedHardwareNotificationCache"" /v ""SV1"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_DEFAULT\Control Panel\UnsupportedHardwareNotificationCache"" /v ""SV2"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_NTUSER\Control Panel\UnsupportedHardwareNotificationCache"" /v ""SV1"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_NTUSER\Control Panel\UnsupportedHardwareNotificationCache"" /v ""SV2"" /t REG_DWORD /d 0 /f",
                @"reg add ""HKLM\TK_SYSTEM\Setup\LabConfig"" /v ""BypassCPUCheck"" /t REG_DWORD /d 1 /f",
                @"reg add ""HKLM\TK_SYSTEM\Setup\LabConfig"" /v ""BypassRAMCheck"" /t REG_DWORD /d 1 /f",
                @"reg add ""HKLM\TK_SYSTEM\Setup\LabConfig"" /v ""BypassSecureBootCheck"" /t REG_DWORD /d 1 /f",
                @"reg add ""HKLM\TK_SYSTEM\Setup\LabConfig"" /v ""BypassStorageCheck"" /t REG_DWORD /d 1 /f",
                @"reg add ""HKLM\TK_SYSTEM\Setup\LabConfig"" /v ""BypassTPMCheck"" /t REG_DWORD /d 1 /f",
                @"reg add ""HKLM\TK_SYSTEM\Setup\MoSetup"" /v ""AllowUpgradesWithUnsupportedTPMOrCPU"" /t REG_DWORD /d 1 /f",
                @"reg add ""HKLM\TK_SOFTWARE\Microsoft\Windows\CurrentVersion\OOBE"" /v ""BypassNRO"" /t REG_DWORD /d 1 /f"
            };

                            foreach (var cmd in regCommands)
                            {
                                await ExecuteCommand(cmd, token);
                            }
                            string[] unloadMounts = { "TK_COMPONENTS", "TK_DEFAULT", "TK_NTUSER", "TK_SOFTWARE", "TK_SYSTEM" };

                            foreach (var mount in unloadMounts)
                            {
                                await ExecuteCommand($"reg unload HKLM\\{mount}", token);
                                await Task.Delay(3000, token);
                            }
                            int maxRetry = 5;
                            for (int i = 0; i < maxRetry; i++)
                            {
                                bool allUnloaded = true;

                                foreach (var subKey in unloadMounts)
                                {
                                    string fullKeyPath = $@"HKEY_LOCAL_MACHINE\{subKey}";
                                    if (RegistryKeyExists(fullKeyPath))
                                    {
                                        allUnloaded = false;
                                        await ExecuteCommand($"reg unload HKLM\\{subKey}", token);
                                    }
                                }

                                if (allUnloaded)
                                    break;

                                await Task.Delay(5000, token);
                            }
                        }, token);

                        if (File.Exists(bootWimPath))
                        {
                            Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "montaggioboot"));
                            var progress = new Progress<int>(value =>
                            {
                                UpdateProgressBar(value);
                            });

                            string[] arguments = ["/mount-image", $"/imagefile:{bootWimPath}", "/index:2", $"/mountdir:{bootMountDir}"];
                            bool success = await EseguiDISM(arguments, progress, token);

                            if (success)
                            {
                                Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "montaggiobootsuccesso"));
                                await ExecuteCommand($"reg load HKLM\\TK_BOOT_SYSTEM \"{bootMountDir}\\Windows\\System32\\Config\\SYSTEM\"", token);
                                var regCommands = new List<string>
        {
            @"reg add ""HKLM\TK_BOOT_SYSTEM\Setup\LabConfig"" /v ""BypassCPUCheck"" /t REG_DWORD /d 1 /f",
            @"reg add ""HKLM\TK_BOOT_SYSTEM\Setup\LabConfig"" /v ""BypassRAMCheck"" /t REG_DWORD /d 1 /f",
            @"reg add ""HKLM\TK_BOOT_SYSTEM\Setup\LabConfig"" /v ""BypassSecureBootCheck"" /t REG_DWORD /d 1 /f",
            @"reg add ""HKLM\TK_BOOT_SYSTEM\Setup\LabConfig"" /v ""BypassStorageCheck"" /t REG_DWORD /d 1 /f",
            @"reg add ""HKLM\TK_BOOT_SYSTEM\Setup\LabConfig"" /v ""BypassTPMCheck"" /t REG_DWORD /d 1 /f"
        };

                                foreach (var cmd in regCommands)
                                {
                                    await ExecuteCommand(cmd, token);
                                }
                                if (File.Exists(appraiserPath))
                                {
                                    File.Move(appraiserPath, appraiserBakPath, true);
                                    Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "rinominatoappraiser"));
                                }
                                else
                                {
                                    Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "appraisernontrovato"));
                                }
                                await ExecuteCommand("reg unload HKLM\\TK_BOOT_SYSTEM", token);
                                int retry = 0;
                                while (RegistryKeyExists(@"HKEY_LOCAL_MACHINE\TK_BOOT_SYSTEM") && retry < 5)
                                {
                                    await Task.Delay(3000, token);
                                    await ExecuteCommand("reg unload HKLM\\TK_BOOT_SYSTEM", token);
                                    retry++;
                                }
                                Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "smontaggioboot"));
                                string[] unmountArguments = ["/unmount-image", $"/mountdir:{bootMountDir}", "/commit"];
                                bool unmountSuccess = await EseguiDISM(unmountArguments, progress, token);

                                if (unmountSuccess)
                                    Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "smontaggiobootsuccesso"));
                                else
                                    Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "erroresmontaggioboot"));
                            }
                            else
                            {
                                Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "erroremontaggioboot"));
                            }

                            Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "bypasscompletato"));
                        }
                    }
                    if (unattendType == "Stock" && File.Exists(sourceUnattendStock))
                    {
                        File.Copy(sourceUnattendStock, destUnattend, true);
                        Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "copiastock"));
                    }
                }
                else if (windowsVersion == "10" && ParametriISO.TryGetValue("Architettura", out var arch))
                {
                    await ExecuteCommand($"reg load HKLM\\TK_SOFTWARE \"{mountDir}\\Windows\\System32\\config\\SOFTWARE\"", token);
                    var regCommands = new List<string>
                {
                    @"reg add ""HKLM\TK_SOFTWARE\Microsoft\Windows\CurrentVersion\OOBE"" /v ""BypassNRO"" /t REG_DWORD /d 1 /f"
                };
                    foreach (var cmd in regCommands)
                        await ExecuteCommand(cmd, token);

                    await Task.Delay(5000, token);
                    await ExecuteCommand("reg unload HKLM\\TK_SOFTWARE", token);
                    int maxRetry = 5;
                    for (int i = 0; i < maxRetry; i++)
                    {
                        if (!RegistryKeyExists(@"HKEY_LOCAL_MACHINE\TK_SOFTWARE"))
                            break;

                        await Task.Delay(3000, token);
                        await ExecuteCommand("reg unload HKLM\\TK_SOFTWARE", token);
                    }
                    if (arch == "x64" && File.Exists(sourceUnattend10))
                    {
                        File.Copy(sourceUnattend10, destUnattend, true);
                        Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "copiaunattend10x64"));
                    }
                    else if (arch == "x32" && File.Exists(sourceUnattendx32))
                    {
                        File.Copy(sourceUnattendx32, destUnattend, true);
                        Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "copiaunattend10x32"));
                    }
                    else
                    {
                        Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "erroreunattendarch"));
                    }
                }
            }
            catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
            {
                Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "erroregenericaunattend") + $": {ex.Message}");
            }
        }


        private bool RegistryKeyExists(string keyPath)
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(keyPath.Replace("HKEY_LOCAL_MACHINE\\", "")))
                {
                    return key != null;
                }
            }
            catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
            {
                Debug.WriteLine($"Verifica chiave del Registro non riuscita: {ex}");
                return false;
            }
        }
        private async Task ExecuteCommand(string command, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            IReadOnlyList<string> parsedArguments = ParseRegistryCommand(command);
            Log($"[ESEGUITO] reg.exe {string.Join(' ', parsedArguments.Skip(1))}");
            ElevatedProcessBrokerClient broker = _elevatedBroker
                ?? throw new InvalidOperationException("Broker UAC del Registro non inizializzato.");
            int exitCode = await broker.RunRegistryAsync(parsedArguments.Skip(1).ToArray(),
                (line, isError) => Log(isError ? $"reg.exe: {line}" : line), token);

            if (exitCode != 0)
                throw new InvalidOperationException(
                    $"reg.exe è terminato con codice {exitCode}.");
        }

        private static IReadOnlyList<string> ParseRegistryCommand(string command)
        {
            var arguments = new List<string>();
            var argument = new StringBuilder();
            bool insideQuotes = false;

            for (int index = 0; index < command.Length;)
            {
                while (index < command.Length && char.IsWhiteSpace(command[index]) && !insideQuotes)
                    index++;
                if (index >= command.Length)
                    break;

                argument.Clear();
                while (index < command.Length && (!char.IsWhiteSpace(command[index]) || insideQuotes))
                {
                    int backslashCount = 0;
                    while (index < command.Length && command[index] == '\\')
                    {
                        backslashCount++;
                        index++;
                    }

                    if (index < command.Length && command[index] == '"')
                    {
                        argument.Append('\\', backslashCount / 2);
                        if (backslashCount % 2 == 0)
                            insideQuotes = !insideQuotes;
                        else
                            argument.Append('"');
                        index++;
                        continue;
                    }

                    argument.Append('\\', backslashCount);
                    if (index < command.Length && (!char.IsWhiteSpace(command[index]) || insideQuotes))
                        argument.Append(command[index++]);
                }

                if (insideQuotes)
                    throw new InvalidOperationException("Il comando di registro contiene virgolette non bilanciate.");
                arguments.Add(argument.ToString());
            }

            if (arguments.Count < 2 || !string.Equals(arguments[0], "reg", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Sono consentiti solo comandi reg nel flusso di personalizzazione ISO.");

            return arguments;
        }

        private async Task RimozioneDiAlcuniProcessi(CancellationToken token)
        {
            try
            {
                if (ParametriISO == null ||
                    !ParametriISO.TryGetValue("Processi", out var processo) ||
                    processo != "RimuoviProcessi")
                    return;

                string mountPath = InstallMountRoot;
                string[] packagePrefixes =
                [
                    "Microsoft-Windows-InternetExplorer-Optional-Package",
                    "Microsoft-Windows-Kernel-LA57-FoD",
                    "Microsoft-Windows-LanguageFeatures-Handwriting",
                    "Microsoft-Windows-LanguageFeatures-OCR",
                    "Microsoft-Windows-LanguageFeatures-Speech",
                    "Microsoft-Windows-LanguageFeatures-TextToSpeech",
                    "Microsoft-Windows-MediaPlayer-Package",
                    "Microsoft-Windows-TabletPCMath-Package",
                    "Microsoft-Windows-Wallpaper-Content-Extended-FoD"
                ];

                var packageQuery = await RunDismCapturingOutputAsync(
                    [$"/Image:{mountPath}", "/English", "/Get-Packages", "/Format:List"], token);
                if (packageQuery.ExitCode != 0)
                {
                    Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroregenerico")}: DISM /Get-Packages è terminato con codice {packageQuery.ExitCode}. {packageQuery.StandardError}");
                    return;
                }

                IReadOnlyList<string> installedPackages = ElevatedProcessCommandValidator
                    .ParseInstalledPackageIdentities(packageQuery.StandardOutput);

                progressBar2.Invoke(new Action(() =>
                {
                    progressBar2.MaxValue = packagePrefixes.Length;
                    progressBar2.Value = 0;
                }));

                foreach (string packagePrefix in packagePrefixes)
                {
                    token.ThrowIfCancellationRequested();
                    string[] matchingPackages = installedPackages
                        .Where(identity => identity.StartsWith(packagePrefix, StringComparison.OrdinalIgnoreCase))
                        .ToArray();
                    Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "rimozionepacchetto")}: \"{packagePrefix}\"...");

                    if (matchingPackages.Length == 0)
                    {
                        Log($"{packagePrefix}: {LanguageManager.GetTranslation("FormCreazioneISO", "nessunpacchetto")}");
                    }
                    else
                    {
                        foreach (string packageIdentity in matchingPackages)
                        {
                            token.ThrowIfCancellationRequested();
                            var removal = await RunDismCapturingOutputAsync(
                                [$"/Image:{mountPath}", "/English", "/Remove-Package", $"/PackageName:{packageIdentity}", "/NoRestart"], token);
                            if (removal.ExitCode == 0)
                                Log($"{packageIdentity}: {LanguageManager.GetTranslation("FormCreazioneISO", "rimozionesuccesso")}");
                            else
                                Log($"{packageIdentity}: DISM è terminato con codice {removal.ExitCode}. {removal.StandardError}");
                        }
                    }

                    progressBar2.Invoke(new Action(() =>
                    {
                        if (progressBar2.Value < progressBar2.MaxValue)
                            progressBar2.Value += 1;
                    }));
                }

                Log(LanguageManager.GetTranslation("FormCreazioneISO", "rimozionepacchetticompletata"));
            }
            catch (OperationCanceledException)
            {
                Log(LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullatatoken"));
            }
            catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
            {
                Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroregenerico")}: {ex.Message}");
            }
        }

        private async Task VerificaParametri(CancellationToken token)
        {
            try
            {
                string targetDir = Path.Join(InstallMountRoot, "Windows");

                progressBar2.Invoke(new Action(() =>
                {
                    progressBar2.MaxValue = 6;
                    progressBar2.Value = 0;
                }));

                await Task.Run(async () =>
                {
                    if (token.IsCancellationRequested) return;

                    if (!Directory.Exists(targetDir))
                        Directory.CreateDirectory(targetDir);

                    if (ParametriISO == null) return;
                    if (ParametriISO.TryGetValue("edgeRemovalPreference", out var edgePref) && edgePref == "RemoveEdge")
                    {
                        Log(LanguageManager.GetTranslation("FormCreazioneISO", "creazionefileedge"));

                        File.Create(Path.Join(targetDir, "noedge.pref")).Dispose();

                        File.Copy(Path.Join(ResourceRoot, ResourceFilesFolderName, "OperaGXSetup.exe"), Path.Join(targetDir, "OperaGXSetup.exe"), true);
                        File.Copy(Path.Join(ResourceRoot, ResourceFilesFolderName, PowerRunExecutableName), Path.Join(targetDir, PowerRunExecutableName), true);

                        IncrementProgress();
                    }

                    if (token.IsCancellationRequested) return;
                    if (!ParametriISO.TryGetValue("windowsVersion", out var windowsVersion))
                    {
                        Log(LanguageManager.GetTranslation("FormCreazioneISO", "erroreversionewindows"));
                        return;
                    }
                    if (windowsVersion == "11" && ParametriISO.TryGetValue("Unattend", out var unattendType) && unattendType == "Bypass")
                    {
                        File.Create(Path.Join(targetDir, "bypass.pref")).Dispose();
                        Log(LanguageManager.GetTranslation("FormCreazioneISO", "creazionefilebypass"));
                        IncrementProgress();
                    }

                    if (token.IsCancellationRequested) return;

                    if (ParametriISO.TryGetValue("DebloatApp", out var debloat) && debloat == "Debloat")
                    {
                        File.Create(Path.Join(targetDir, "debloatapp.pref")).Dispose();
                        Log(LanguageManager.GetTranslation("FormCreazioneISO", "creazionefiledebloat"));
                        IncrementProgress();
                    }

                    if (token.IsCancellationRequested) return;

                    if (ParametriISO.TryGetValue("TipoOttimizzazione", out var tipo))
                    {
                        string fileName = tipo switch
                        {
                            "LavorWork" => "workstation.pref",
                            "IsoGaming" => "gaming.pref",
                            _ => string.Empty
                        };

                        if (fileName != null)
                        {
                            string path = Path.Join(targetDir, fileName);
                            if (!File.Exists(path))
                            {
                                File.Create(path).Dispose();
                                Log(LanguageManager.GetTranslation("FormCreazioneISO", "creazionefileottimizzazione") + $" ({fileName})");
                                IncrementProgress();
                            }
                        }
                    }

                    if (token.IsCancellationRequested) return;
                    if (ParametriISO.TryGetValue("defenderPreference", out var defender) && defender == "DisableWindowsDefender")
                    {
                        File.Create(Path.Join(targetDir, "nodefender.pref")).Dispose();
                        Log(LanguageManager.GetTranslation("FormCreazioneISO", "creazionefiledefender"));
                        IncrementProgress();
                    }

                    if (token.IsCancellationRequested) return;
                    try
                    {
                        if (ParametriISO.TryGetValue("DriverWin", out var driverPref))
                        {
                            if (driverPref == "DriverCartella")
                            {
                                string? driverFolder = null;

                                Invoke(new Action(() =>
                                {
                                    using var dialog = new FolderBrowserDialog
                                    {
                                        Description = LanguageManager.GetTranslation("FormCreazioneISO", "selezionacartelladriver")
                                    };
                                    if (dialog.ShowDialog() == DialogResult.OK)
                                        driverFolder = dialog.SelectedPath;
                                }));

                                if (!string.IsNullOrEmpty(driverFolder))
                                {
                                    var driverResult = await RunDismCapturingOutputAsync(
                                        [$"/Image:{InstallMountRoot}", "/Add-Driver", $"/Driver:{driverFolder}", "/Recurse"], token);

                                    if (driverResult.ExitCode == 0)
                                        Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "driverintegratocartella")}: {driverFolder}");
                                    else
                                        Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroreintegracartella")} {driverResult.StandardError}");
                                }

                                IncrementProgress();
                            }
                            else if (driverPref == "DriverQuestoPC")
                            {
                                    string tempDriverDir = Path.Join(WorkspaceRoot, "DriverExport", Guid.NewGuid().ToString("N"));
                                    Directory.CreateDirectory(tempDriverDir);

                                try
                                {
                                    var exportResult = await RunDismCapturingOutputAsync(
                                        ["/Online", "/Export-Driver", $"/Destination:{tempDriverDir}"], token);

                                    if (exportResult.ExitCode == 0)
                                        Log(LanguageManager.GetTranslation("FormCreazioneISO", "driversuccessoesportazione"));
                                    else
                                        Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroreesportazionedriver")} {exportResult.StandardError}");

                                    if (exportResult.ExitCode == 0)
                                    {
                                        var addResult = await RunDismCapturingOutputAsync(
                                            [$"/Image:{InstallMountRoot}", "/Add-Driver", $"/Driver:{tempDriverDir}", "/Recurse"], token);

                                        if (addResult.ExitCode == 0)
                                            Log(LanguageManager.GetTranslation("FormCreazioneISO", "driverintegrazionesistema"));
                                        else
                                            Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroreintegrasistema")} {addResult.StandardError}");
                                    }
                                }
                                finally
                                {
                                    if (Directory.Exists(tempDriverDir))
                                        Directory.Delete(tempDriverDir, recursive: true);
                                }

                                IncrementProgress();
                            }
                        }

                        Log(LanguageManager.GetTranslation("FormCreazioneISO", "verificaparametricompletata"));
                    }
                    catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
                    {
                        Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroregenerale")}: {ex.Message}");
                    }
                }, token);
            }
            catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
            {
                Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroregenerale")}: {ex.Message}");
            }
        }

        private void IncrementProgress()
        {
            progressBar2.Invoke(new Action(() =>
            {
                if (progressBar2.Value < progressBar2.MaxValue)
                    progressBar2.Value += 1;
            }));
        }


        private async Task CopiaFileNecessari(CancellationToken token)
        {
            List<string> filesToCopy = new List<string>();

            if (ParametriISO.TryGetValue("windowsVersion", out string? windowsVersion)
                && !string.IsNullOrWhiteSpace(windowsVersion))
            {
                if (windowsVersion == "10")
                {
                    filesToCopy = new List<string>
        {
            "lower-ram-usage.reg",
            PowerRunExecutableName,
            "tweaks10.bat",
            "start10.ps1",
            "unpin_start_tiles.ps1"
        };
                }
                else if (windowsVersion == "11")
                {
                    filesToCopy = new List<string>
        {
            "tweaks.bat",
            "lower-ram-usage.reg",
            "start.ps1",
            PowerRunExecutableName
        };
                }
            }

            Invoke(new Action(() =>
            {
                Log($"\n[INFO] {LanguageManager.GetTranslation("FormCreazioneISO", "iniziocopianeccessari")}");
            }));

            if (ParametriISO.TryGetValue("ImportaSettaggiWinhubx", out var ImportaSettaggiWinhubx) && ImportaSettaggiWinhubx == "SiImporta")
            {
                Invoke(new Action(() =>
                {
                    Log($"\n[INFO] {LanguageManager.GetTranslation("FormCreazioneISO", "importazionesettaggiabilitata")}");
                }));

                string exportPath = Path.Join(resourceSessionPath, "config.dat");
                string targetExportPath = Path.Join(ResourceRoot, ResourceFilesFolderName, "config.dat");
                string keyToExport = @"HKEY_CURRENT_USER\Software\WinHubX";

                try
                {
                    using Process process = new();
                    process.StartInfo.FileName = Path.Join(Environment.SystemDirectory, "reg.exe");
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.ArgumentList.Add("export");
                    process.StartInfo.ArgumentList.Add(keyToExport);
                    process.StartInfo.ArgumentList.Add(exportPath);
                    process.StartInfo.ArgumentList.Add("/y");
                    if (!process.Start())
                        throw new InvalidOperationException("Impossibile avviare reg.exe per esportare la configurazione.");

                    // Una volta avviato, attendere la fine del processo prima di toccare i file della sessione.
                    await process.WaitForExitAsync(CancellationToken.None);
                    if (process.ExitCode != 0)
                        throw new InvalidOperationException($"reg.exe export è terminato con codice {process.ExitCode}.");

                    if (File.Exists(exportPath))
                    {
                        string finalDir = Path.GetDirectoryName(targetExportPath)
                            ?? throw new InvalidOperationException("Percorso di esportazione non valido.");
                        if (!Directory.Exists(finalDir))
                            _ = Directory.CreateDirectory(finalDir);

                        File.Move(exportPath, targetExportPath);
                        filesToCopy.Add("config.dat");

                        Invoke(new Action(() =>
                        {
                            Log($"\n[INFO] {LanguageManager.GetTranslation("FormCreazioneISO", "fileconfigcopiato")}");
                        }));
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            Log($"\n[WARN] {LanguageManager.GetTranslation("FormCreazioneISO", "fileconfignontrovato")}");
                        }));
                    }
                }
                catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
                {
                    Invoke(new Action(() =>
                    {
                        Log($"\n[ERROR] {LanguageManager.GetTranslation("FormCreazioneISO", "erroreexportreg")}: {ex.Message}");
                    }));
                }
            }

            string sourceFolder = Path.Join(ResourceRoot, ResourceFilesFolderName);
            string targetFolder = Path.Join(InstallMountRoot, "Windows");

            Invoke(new Action(() =>
            {
                progressBar2.MaxValue = filesToCopy.Count;
                progressBar2.Value = 0;
            }));

            await Task.Run(() =>
            {
                try
                {
                    foreach (var file in filesToCopy)
                    {
                        if (token.IsCancellationRequested) return;

                        string sourceFilePath = SafePathResolver.ResolveContainedPath(sourceFolder, file);
                        string targetFilePath = SafePathResolver.ResolveContainedPath(targetFolder, file);

                        if (File.Exists(sourceFilePath))
                        {
                            File.Copy(sourceFilePath, targetFilePath, true);

                            Invoke(new Action(() =>
                            {
                                Log($"\n[OK] {LanguageManager.GetTranslation("FormCreazioneISO", "copiatofile")}: {file}");
                            }));
                        }
                        else
                        {
                            Invoke(new Action(() =>
                            {
                                Log($"\n{LanguageManager.GetTranslation("FormCreazioneISO", "filenontrovato")}: {sourceFilePath}");
                            }));
                        }

                        Invoke(new Action(() =>
                        {
                            progressBar2.Value += 1;
                        }));
                    }

                    Invoke(new Action(() =>
                    {
                        Log($"\n{LanguageManager.GetTranslation("FormCreazioneISO", "copiacompletata")}");
                    }));
                }
                catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
                {
                    Invoke(new Action(() =>
                    {
                        Log($"\n{LanguageManager.GetTranslation("FormCreazioneISO", "erroregenerico")}: {ex.Message}");
                    }));
                }
            }, token);
        }

        private async Task CreazioneInstall(CancellationToken token)
        {
            string mountDir = InstallMountRoot;

            try
            {
                if (!Directory.Exists(mountDir))
                {
                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "errordirectorymount"));
                    return;
                }

                string deletedFolderPath = Path.Join(mountDir, "[DELETED]");

                if (Directory.Exists(deletedFolderPath))
                {
                    Directory.Delete(deletedFolderPath, true);
                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "cartelladeletedrimossa"));
                }

                Log(LanguageManager.GetTranslation("FormCreazioneISO", "smontaggiosalvataggio"));

                await Task.Delay(6000, token);

                var progress = new Progress<int>(value =>
                {
                    if (progressBar2.InvokeRequired)
                        progressBar2.Invoke(new Action(() => progressBar2.Value = value));
                    else
                        progressBar2.Value = value;
                });

                string[] arguments = ["/unmount-image", $"/mountdir:{mountDir}", "/commit"];

                bool success = await EseguiDISM(arguments, progress, token);

                if (token.IsCancellationRequested)
                {
                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullata"));
                    return;
                }

                if (success)
                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "immaginesmontata"));
                else
                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "erroresmontaggio"));
            }
            catch (OperationCanceledException)
            {
                Log(LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullata"));
            }
            catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
            {
                Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "errorecreazioneinstall")}: {ex.Message}");
            }
        }


        private async Task CreazioneISO(CancellationToken token)
        {
            string sourcePath = IsoWorkingRoot;
            string isoOutputPath = formcreaiso.labelpercorso.Text;
            string oscdimgPath = Path.Join(ResourceRoot, ResourceFilesFolderName, "oscdimg");

            try
            {
                if (!Directory.Exists(sourcePath))
                {
                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "erroresorgenteisomancante"));
                    return;
                }
                if (progressBar2.InvokeRequired)
                    progressBar2.Invoke(new Action(() =>
                    {
                        progressBar2.MaxValue = 3;
                        progressBar2.Value = 0;
                    }));
                else
                {
                    progressBar2.MaxValue = 3;
                    progressBar2.Value = 0;
                }

                Log(LanguageManager.GetTranslation("FormCreazioneISO", "inizioCreazioneISO"));

                await Task.Run(async () =>
                {
                    try
                    {
                        string[] oscdimgArguments =
                        [
                            "-m",
                            "-o",
                            "-u2",
                            $"-bootdata:2#p0,e,b{Path.Join(sourcePath, "boot", "etfsboot.com")}#pEF,e,b{Path.Join(sourcePath, "efi", "microsoft", "boot", "efisys.bin")}",
                            sourcePath,
                            isoOutputPath
                        ];

                        ProcessStartInfo oscdimgProcess = new ProcessStartInfo
                        {
                            FileName = oscdimgPath,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        foreach (string argument in oscdimgArguments)
                            oscdimgProcess.ArgumentList.Add(argument);

                        using (Process oscdimgProc = Process.Start(oscdimgProcess)
                            ?? throw new InvalidOperationException("Impossibile avviare oscdimg."))
                        {
                            try
                            {
                                await oscdimgProc.WaitForExitAsync(token);
                            }
                            catch (OperationCanceledException)
                            {
                                try
                                {
                                    if (!oscdimgProc.HasExited)
                                        oscdimgProc.Kill(entireProcessTree: true);
                                }
                                catch (Exception ex) when (ex is InvalidOperationException or Win32Exception)
                                {
                                    Debug.WriteLine($"Impossibile terminare oscdimg dopo l’annullamento: {ex.Message}");
                                }

                                await oscdimgProc.WaitForExitAsync(CancellationToken.None);
                                Log(LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullata"));
                                return;
                            }

                            if (oscdimgProc.ExitCode != 0)
                                throw new InvalidOperationException($"oscdimg è terminato con codice {oscdimgProc.ExitCode}.");
                        }

                        AggiornaProgress(1);
                        if (File.Exists(isoOutputPath))
                            Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "filecreato")}: {isoOutputPath}");
                        else
                            Log(LanguageManager.GetTranslation("FormCreazioneISO", "errorecreazioneiso"));

                        AggiornaProgress(1);

                        if (Directory.Exists(WorkspaceRoot))
                            Directory.Delete(WorkspaceRoot, recursive: true);

                        AggiornaProgress(1);
                        Log(LanguageManager.GetTranslation("FormCreazioneISO", "creazioneisocompletata"));
                    }
                    catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
                    {
                        Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "errorecreazioneiso")}: {ex.Message}");
                    }
                }, token);
            }
            catch (OperationCanceledException)
            {
                Log(LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullata"));
            }
            catch (Exception ex) when (IsExpectedIsoOperationFailure(ex))
            {
                Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "errorecreazioneiso")}: {ex.Message}");
            }
        }

        private void AggiornaProgress(int step)
        {
            if (progressBar2.InvokeRequired)
                progressBar2.Invoke(new Action(() => progressBar2.Value += step));
            else
                progressBar2.Value += step;
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            form1.btnHome.Enabled = true;
            form1.btnWin.Enabled = true;
            form1.btnOffice.Enabled = true;
            form1.btnSettaggi.Enabled = true;
            form1.btnDebloat.Enabled = true;
            form1.btnmonitoraggio.Enabled = true;
        }
        private void Log(string message)
        {
            if (richTextBox1.IsDisposed || richTextBox1.Disposing || !richTextBox1.IsHandleCreated)
                return;

            if (richTextBox1.InvokeRequired)
            {
                try
                {
                    _ = richTextBox1.BeginInvoke(new Action(() => AppendToTextBox(message)));
                }
                catch (InvalidOperationException ex)
                {
                    Debug.WriteLine($"WinHubX log dispatch failed: {ex.Message}");
                }

                return;
            }

            AppendToTextBox(message);
        }

        private void AppendToTextBox(string message)
        {
            if (richTextBox1.IsDisposed || richTextBox1.Disposing || !richTextBox1.IsHandleCreated)
                return;

            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;
            richTextBox1.SelectionColor = Color.White;
            richTextBox1.SelectionFont = richTextBox1.Font;
            richTextBox1.AppendText($"{DateTime.Now:HH:mm:ss} ");

            richTextBox1.SelectionColor = Color.FromArgb(70, 130, 180);
            richTextBox1.AppendText("➤ ");

            richTextBox1.SelectionColor = Color.White;
            richTextBox1.AppendText(message);

            richTextBox1.SelectionColor = Color.FromArgb(240, 240, 240);
            richTextBox1.AppendText("\n────────────────────────────────────────────\n");

            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.ScrollToCaret();
            richTextBox1.SelectionColor = richTextBox1.ForeColor;
            richTextBox1.SelectionFont = richTextBox1.Font;
        }
    }
}
