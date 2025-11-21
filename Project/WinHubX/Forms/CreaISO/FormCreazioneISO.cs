using DiscUtils;
using DiscUtils.Udf;
using System.Data;
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
        public Dictionary<string, string> ParametriISO { get; set; }
        private Form1 form1;
        private CancellationTokenSource _cancellationTokenSource;
        private FormCreaISO formcreaiso;

        public FormCreazioneISO(Form1 form1, FormCreaISO formcreaiso)
        {
            LanguageManager.LoadTranslations();
            InitializeComponent();
            form1 = form1;
            formcreaiso = formcreaiso;
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }
        private async void FormCreazioneISO_Shown(object sender, EventArgs e)
        {
            await Task.Delay(2000);
            Start();
        }

        private List<Task> taskList = new();

        private async void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
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
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante l'esecuzione: {ex.Message}");
            }
            finally
            {
                SetButtonsEnabled(true);
                btnStopVerdi.Visible = false;
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
                    string successo2Template = LanguageManager.GetTranslation("FormCreazioneISO", "successo2");
                    string percorsoISO = formcreaiso.labelpercorso.Text;
                    string successo2 = string.Format(successo2Template, percorsoISO);

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

                string tempPath = Path.GetTempPath();
                string zipPath = Path.Combine(tempPath, "RisorseCreaISO.zip");
                string folderPath = Path.Combine(tempPath, "RisorseCreaISO");

                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
            }
            catch (Exception ex)
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

            string extractPath = @"C:\ISO\WinISO";
            Directory.CreateDirectory(extractPath);
            progressBar2.Value = 0;

            var progress = new Progress<int>(value =>
            {
                if (value >= 0 && value <= 100)
                {
                    progressBar2.Value = value;
                }
            });

            try
            {
                await Task.Run(() =>
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
            catch (Exception ex)
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

                string destPath = Path.Combine(targetPath, file.Name);
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
                ExtractDirectory(subDir, Path.Combine(targetPath, subDir.Name), ref extractedFiles, totalFiles, progress, token);
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
            string sourcesPath = @"C:\ISO\WinISO\sources";
            string esdPath = Path.Combine(sourcesPath, "install.esd");
            string wimPath = Path.Combine(sourcesPath, "install.wim");
            string wimProPath = Path.Combine(sourcesPath, "install_pro.wim");

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

                    string arguments = $"/export-image /SourceImageFile:\"{esdPath}\" " +
                                       $"/SourceIndex:{indexValue} " +
                                       $"/DestinationImageFile:\"{wimPath}\" " +
                                       $"/Compress:max /CheckIntegrity";

                    token.ThrowIfCancellationRequested();
                    bool success = await Task.Run(() => EseguiDISM(arguments, progress, token), token);

                    if (success && File.Exists(wimPath))
                    {
                        File.Delete(esdPath);
                        Log(LanguageManager.GetTranslation("FormCreazioneISO", "conversionesuccesso"));
                    }
                }
                else if (File.Exists(wimPath))
                {
                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "trovatoinstallwim"));

                    string arguments = $"/export-image /SourceImageFile:\"{wimPath}\" " +
                                       $"/SourceIndex:{indexValue} " +
                                       $"/DestinationImageFile:\"{wimProPath}\" " +
                                       $"/Compress:max /CheckIntegrity";

                    token.ThrowIfCancellationRequested();
                    bool success = await Task.Run(() => EseguiDISM(arguments, progress, token), token);

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
            catch (Exception ex)
            {
                Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroreoperazione")}: {ex.Message}");
                if (File.Exists(wimProPath))
                {
                    try { File.Delete(wimProPath); } catch { }
                }
            }
        }

        private async Task<bool> EseguiDISM(string arguments, IProgress<int> progress, CancellationToken token)
        {
            try
            {
                progressBar2.Value = 0;
                progressBar2.MaxValue = 100;

                using var dismProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "dism.exe",
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                dismProcess.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        int? value = ParseProgress(args.Data);
                        if (value.HasValue)
                            progress?.Report(value.Value);
                    }
                };

                dismProcess.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                        Log($"Error: {args.Data}");
                };

                dismProcess.Start();
                dismProcess.BeginOutputReadLine();
                dismProcess.BeginErrorReadLine();

                while (!dismProcess.HasExited)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(100, token);
                }

                dismProcess.WaitForExit();
                progress?.Report(100);
                return dismProcess.ExitCode == 0;
            }
            catch (OperationCanceledException)
            {
                Log(LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullatatoken"));
                return false;
            }
            catch (Exception ex)
            {
                Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroreoperazione")}: {ex.Message}");
                return false;
            }
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
            string wimPath = @"C:\ISO\WinISO\sources\install.wim";
            string mountDir = @"C:\mount\mount";

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

                string arguments = $"/mount-image /imagefile:\"{wimPath}\" /index:1 /mountdir:\"{mountDir}\"";

                bool success = await Task.Run(() => EseguiDISM(arguments, progress, token), token);

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
            catch (Exception ex)
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

                string sourceUnattend = Path.Combine(Path.GetTempPath(), @"RisorseCreaISO\Risorse\unattend.xml");
                string sourceUnattendStock = Path.Combine(Path.GetTempPath(), @"RisorseCreaISO\Risorse\unattendstock.xml");
                string destUnattend = @"C:\ISO\WinISO\sources\$OEM$\$$\Panther\unattend.xml";
                string mountDir = @"C:\mount\mount";
                string bootWimPath = @"C:\ISO\WinISO\sources\boot.wim";
                string bootMountDir = @"C:\mount\boot";
                string appraiserPath = @"C:\ISO\WinISO\sources\appraiserres.dll";
                string appraiserBakPath = appraiserPath + ".bak";
                string sourceUnattend10 = Path.Combine(Path.GetTempPath(), @"RisorseCreaISO\Risorse\unattend10.xml");
                string sourceUnattendx32 = Path.Combine(Path.GetTempPath(), @"RisorseCreaISO\Risorse\unattendx32.xml");

                _ = Directory.CreateDirectory(Path.GetDirectoryName(destUnattend));
                _ = Directory.CreateDirectory(mountDir);
                _ = Directory.CreateDirectory(bootMountDir);
                _ = Directory.CreateDirectory(Path.GetDirectoryName(appraiserPath));

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
                                progressBar2.Value = value;
                            });

                            string arguments = $"/mount-image /imagefile:\"{bootWimPath}\" /index:2 /mountdir:\"{bootMountDir}\"";
                            bool success = await Task.Run(() => EseguiDISM(arguments, progress, token), token);

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
                                string unmountArguments = $"/unmount-image /mountdir:\"{bootMountDir}\" /commit";
                                bool unmountSuccess = await Task.Run(() => EseguiDISM(arguments, progress, token), token);

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
                    if (unattendType == "Stock")
                    {
                        if (File.Exists(sourceUnattendStock))
                        {
                            File.Copy(sourceUnattendStock, destUnattend, true);
                            Log("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "copiastock"));
                        }
                    }
                }
                else if (windowsVersion == "10" && ParametriISO.TryGetValue("Architettura", out var arch))
                {
                    _ = ExecuteCommand($"reg load HKLM\\TK_SOFTWARE \"{mountDir}\\Windows\\System32\\config\\SOFTWARE\"", token);
                    var regCommands = new List<string>
                {
                    @"reg add ""HKLM\TK_SOFTWARE\Microsoft\Windows\CurrentVersion\OOBE"" /v ""BypassNRO"" /t REG_DWORD /d 1 /f"
                };
                    foreach (var cmd in regCommands) _ = ExecuteCommand(cmd, token);
                    await Task.Delay(5000);
                    _ = ExecuteCommand("reg unload HKLM\\TK_SOFTWARE", token);
                    int maxRetry = 5;
                    for (int i = 0; i < maxRetry; i++)
                    {
                        if (!RegistryKeyExists(@"HKEY_LOCAL_MACHINE\TK_SOFTWARE"))
                            break;

                        await Task.Delay(3000);
                        _ = ExecuteCommand("reg unload HKLM\\TK_SOFTWARE", token);
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
            catch (Exception ex)
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
            catch
            {
                return false;
            }
        }
        private async Task ExecuteCommand(string command, CancellationToken token)
        {
            Log($"[ESEGUITO] {command}");

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/C {command}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                _ = process.Start();

                var outputTask = Task.Run(() =>
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                });

                _ = await Task.WhenAny(outputTask, Task.Delay(Timeout.Infinite, token)).ConfigureAwait(false);

                if (token.IsCancellationRequested)
                {
                    process.Kill();
                    string operazioneannullata = LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullatatoken");
                    Log(operazioneannullata);
                }

                await outputTask.ConfigureAwait(false);
                process.WaitForExit();
            }
        }

        private async Task RimozioneDiAlcuniProcessi(CancellationToken token)
        {
            try
            {
                if (ParametriISO == null ||
                    !ParametriISO.TryGetValue("Processi", out var processo) ||
                    processo != "RimuoviProcessi")
                    return;

                string mountPath = @"C:\mount\mount";

                var pacchetti = new Dictionary<string, string>
        {
            { "InternetExplorer-Optional-Package",
              $@"$pkgs = Get-WindowsPackage -Path '{mountPath}' | Where-Object {{ $_.PackageName -like 'Microsoft-Windows-InternetExplorer-Optional-Package*' }}; foreach ($pkg in $pkgs) {{ dism /English /image:{mountPath} /Remove-Package /PackageName:$($pkg.PackageName) /NoRestart }}" },

            { "Windows-Kernel-LA57-FoD",
              $@"$pkgs = Get-WindowsPackage -Path '{mountPath}' | Where-Object {{ $_.PackageName -like 'Microsoft-Windows-Kernel-LA57-FoD*' }}; foreach ($pkg in $pkgs) {{ dism /English /image:{mountPath} /Remove-Package /PackageName:$($pkg.PackageName) /NoRestart }}" },

            { "LanguageFeatures-Handwriting",
              $@"$pkgs = Get-WindowsPackage -Path '{mountPath}' | Where-Object {{ $_.PackageName -like 'Microsoft-Windows-LanguageFeatures-Handwriting*' }}; foreach ($pkg in $pkgs) {{ dism /English /image:{mountPath} /Remove-Package /PackageName:$($pkg.PackageName) /NoRestart }}" },

            { "LanguageFeatures-OCR",
              $@"$pkgs = Get-WindowsPackage -Path '{mountPath}' | Where-Object {{ $_.PackageName -like 'Microsoft-Windows-LanguageFeatures-OCR*' }}; foreach ($pkg in $pkgs) {{ dism /English /image:{mountPath} /Remove-Package /PackageName:$($pkg.PackageName) /NoRestart }}" },

            { "LanguageFeatures-Speech",
              $@"$pkgs = Get-WindowsPackage -Path '{mountPath}' | Where-Object {{ $_.PackageName -like 'Microsoft-Windows-LanguageFeatures-Speech*' }}; foreach ($pkg in $pkgs) {{ dism /English /image:{mountPath} /Remove-Package /PackageName:$($pkg.PackageName) /NoRestart }}" },

            { "LanguageFeatures-TextToSpeech",
              $@"$pkgs = Get-WindowsPackage -Path '{mountPath}' | Where-Object {{ $_.PackageName -like 'Microsoft-Windows-LanguageFeatures-TextToSpeech*' }}; foreach ($pkg in $pkgs) {{ dism /English /image:{mountPath} /Remove-Package /PackageName:$($pkg.PackageName) /NoRestart }}" },

            { "MediaPlayer-Package",
              $@"$pkgs = Get-WindowsPackage -Path '{mountPath}' | Where-Object {{ $_.PackageName -like 'Microsoft-Windows-MediaPlayer-Package*' }}; foreach ($pkg in $pkgs) {{ dism /English /image:{mountPath} /Remove-Package /PackageName:$($pkg.PackageName) /NoRestart }}" },

            { "TabletPCMath-Package",
              $@"$pkgs = Get-WindowsPackage -Path '{mountPath}' | Where-Object {{ $_.PackageName -like 'Microsoft-Windows-TabletPCMath-Package*' }}; foreach ($pkg in $pkgs) {{ dism /English /image:{mountPath} /Remove-Package /PackageName:$($pkg.PackageName) /NoRestart }}" },

            { "Wallpaper-Content-Extended-FoD",
              $@"$pkgs = Get-WindowsPackage -Path '{mountPath}' | Where-Object {{ $_.PackageName -like 'Microsoft-Windows-Wallpaper-Content-Extended-FoD*' }}; foreach ($pkg in $pkgs) {{ dism /English /image:{mountPath} /Remove-Package /PackageName:$($pkg.PackageName) /NoRestart }}" },
        };

                progressBar2.Invoke(new Action(() =>
                {
                    progressBar2.MaxValue = pacchetti.Count;
                    progressBar2.Value = 0;
                }));

                foreach (var (nome, comando) in pacchetti)
                {
                    if (token.IsCancellationRequested)
                    {
                        string annullata = LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullatatoken");
                        Log(annullata);
                        break;
                    }

                    Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "rimozionepacchetto")}: \"{nome}\"...");

                    var psi = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{comando}; $pkgs.Count\"",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    using (var process = Process.Start(psi))
                    {
                        string output = await process.StandardOutput.ReadToEndAsync();
                        string error = await process.StandardError.ReadToEndAsync();
                        await process.WaitForExitAsync(token);

                        int.TryParse(output.Trim().Split('\n').LastOrDefault()?.Trim(), out int removedCount);

                        if (removedCount > 0)
                            Log($"{nome}: {LanguageManager.GetTranslation("FormCreazioneISO", "rimozionesuccesso")}");
                        else
                            Log($"{nome}: {LanguageManager.GetTranslation("FormCreazioneISO", "nessunpacchetto")}");

                        progressBar2.Invoke(new Action(() =>
                        {
                            if (progressBar2.Value < progressBar2.MaxValue)
                                progressBar2.Value += 1;
                        }));
                    }

                    await Task.Delay(500, token);
                }

                Log(LanguageManager.GetTranslation("FormCreazioneISO", "rimozionepacchetticompletata"));
            }
            catch (OperationCanceledException)
            {
                Log(LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullatatoken"));
            }
            catch (Exception ex)
            {
                Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroregenerico")}: {ex.Message}");
            }
        }

        private async Task VerificaParametri(CancellationToken token)
        {
            try
            {
                string targetDir = @"C:\mount\mount\Windows";

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

                        File.Create(Path.Combine(targetDir, "noedge.pref")).Dispose();

                        File.Copy(Path.Combine(Path.GetTempPath(), @"RisorseCreaISO\Risorse\OperaGXSetup.exe"), Path.Combine(targetDir, "OperaGXSetup.exe"), true);
                        File.Copy(Path.Combine(Path.GetTempPath(), @"RisorseCreaISO\Risorse\PowerRun.exe"), Path.Combine(targetDir, "PowerRun.exe"), true);

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
                        File.Create(Path.Combine(targetDir, "bypass.pref")).Dispose();
                        Log(LanguageManager.GetTranslation("FormCreazioneISO", "creazionefilebypass"));
                        IncrementProgress();
                    }

                    if (token.IsCancellationRequested) return;

                    if (ParametriISO.TryGetValue("DebloatApp", out var debloat) && debloat == "Debloat")
                    {
                        File.Create(Path.Combine(targetDir, "debloatapp.pref")).Dispose();
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
                            _ => null
                        };

                        if (fileName != null)
                        {
                            string path = Path.Combine(targetDir, fileName);
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
                        File.Create(Path.Combine(targetDir, "nodefender.pref")).Dispose();
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
                                string driverFolder = null;

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
                                    var process = Process.Start(new ProcessStartInfo
                                    {
                                        FileName = "dism.exe",
                                        Arguments = $"/Image:\"C:\\Mount\\mount\" /Add-Driver /Driver:\"{driverFolder}\" /Recurse",
                                        UseShellExecute = false,
                                        RedirectStandardOutput = true,
                                        RedirectStandardError = true,
                                        CreateNoWindow = true
                                    });

                                    string output = await process.StandardOutput.ReadToEndAsync();
                                    string error = await process.StandardError.ReadToEndAsync();
                                    await process.WaitForExitAsync(token);

                                    if (process.ExitCode == 0)
                                        Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "driverintegratocartella")}: {driverFolder}");
                                    else
                                        Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroreintegracartella")} {error}");
                                }

                                IncrementProgress();
                            }
                            else if (driverPref == "DriverQuestoPC")
                            {
                                string tempDriverDir = Path.Combine(Path.GetTempPath(), "DriverBackup_" + Guid.NewGuid().ToString("N"));
                                Directory.CreateDirectory(tempDriverDir);

                                var export = Process.Start(new ProcessStartInfo
                                {
                                    FileName = "dism.exe",
                                    Arguments = $"/Online /Export-Driver /Destination:\"{tempDriverDir}\"",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true
                                });

                                string expOut = await export.StandardOutput.ReadToEndAsync();
                                string expErr = await export.StandardError.ReadToEndAsync();
                                await export.WaitForExitAsync(token);

                                if (export.ExitCode == 0)
                                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "driversuccessoesportazione"));
                                else
                                    Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroreesportazionedriver")} {expErr}");

                                if (export.ExitCode == 0)
                                {
                                    var add = Process.Start(new ProcessStartInfo
                                    {
                                        FileName = "dism.exe",
                                        Arguments = $"/Image:\"C:\\Mount\\mount\" /Add-Driver /Driver:\"{tempDriverDir}\" /Recurse",
                                        UseShellExecute = false,
                                        RedirectStandardOutput = true,
                                        RedirectStandardError = true,
                                        CreateNoWindow = true
                                    });

                                    string addOut = await add.StandardOutput.ReadToEndAsync();
                                    string addErr = await add.StandardError.ReadToEndAsync();
                                    await add.WaitForExitAsync(token);

                                    if (add.ExitCode == 0)
                                        Log(LanguageManager.GetTranslation("FormCreazioneISO", "driverintegrazionesistema"));
                                    else
                                        Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroreintegrasistema")} {addErr}");
                                }

                                Directory.Delete(tempDriverDir, true);
                                IncrementProgress();
                            }
                        }

                        Log(LanguageManager.GetTranslation("FormCreazioneISO", "verificaparametricompletata"));
                    }
                    catch (Exception ex)
                    {
                        Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "erroregenerale")}: {ex.Message}");
                    }
                }, token);
            }
            catch (Exception ex)
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

            if (ParametriISO.TryGetValue("windowsVersion", out string windowsVersion))
            {
                if (windowsVersion == "10")
                {
                    filesToCopy = new List<string>
        {
            "lower-ram-usage.reg",
            "PowerRun.exe",
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
            "PowerRun.exe"
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

                string exportPath = Path.Combine(Path.GetTempPath(), "config.dat");
                string targetExportPath = Path.Combine(Path.GetTempPath(), @"RisorseCreaISO\Risorse\config.dat");
                string keyToExport = @"HKEY_CURRENT_USER\Software\WinHubX";

                try
                {
                    var process = new Process();
                    process.StartInfo.FileName = "reg.exe";
                    process.StartInfo.Arguments = $"export \"{keyToExport}\" \"{exportPath}\" /y";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    _ = process.Start();
                    process.WaitForExit();

                    if (File.Exists(exportPath))
                    {
                        string finalDir = Path.GetDirectoryName(targetExportPath);
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
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        Log($"\n[ERROR] {LanguageManager.GetTranslation("FormCreazioneISO", "erroreexportreg")}: {ex.Message}");
                    }));
                }
            }

            string sourceFolder = Path.Combine(Path.GetTempPath(), @"RisorseCreaISO\Risorse");
            string targetFolder = @"C:\mount\mount\Windows";

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

                        string sourceFilePath = Path.Combine(sourceFolder, file);
                        string targetFilePath = Path.Combine(targetFolder, file);

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
                catch (Exception ex)
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
            string mountDir = @"C:\mount\mount";

            try
            {
                if (!Directory.Exists(mountDir))
                {
                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "errordirectorymount"));
                    return;
                }

                string deletedFolderPath = Path.Combine(mountDir, "[DELETED]");

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

                string arguments = $"/unmount-image /mountdir:\"{mountDir}\" /commit";

                bool success = await Task.Run(() => EseguiDISM(arguments, progress, token), token);

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
            catch (Exception ex)
            {
                Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "errorecreazioneinstall")}: {ex.Message}");
            }
        }


        private async Task CreazioneISO(CancellationToken token)
        {
            string sourcePath = @"C:\ISO\WinISO";
            string isoOutputPath = formcreaiso.labelpercorso.Text;
            string oscdimgPath = Path.Combine(Path.GetTempPath(), @"RisorseCreaISO\Risorse\oscdimg");
            string destinationPath = isoOutputPath;

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

                await Task.Run(() =>
                {
                    try
                    {
                        string oscdimgArguments =
                            $"-m -o -u2 -bootdata:2#p0,e,b{sourcePath}\\boot\\etfsboot.com#pEF,e,b{sourcePath}\\efi\\microsoft\\boot\\efisys.bin {sourcePath} \"{isoOutputPath}\"";

                        ProcessStartInfo oscdimgProcess = new ProcessStartInfo
                        {
                            FileName = oscdimgPath,
                            Arguments = oscdimgArguments,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };

                        using (Process oscdimgProc = Process.Start(oscdimgProcess))
                        {
                            while (!oscdimgProc.HasExited)
                            {
                                if (token.IsCancellationRequested)
                                {
                                    oscdimgProc.Kill();
                                    Log(LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullata"));
                                    return;
                                }
                                Thread.Sleep(100);
                            }
                        }

                        AggiornaProgress(1);
                        if (File.Exists(isoOutputPath))
                            Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "filecreato")}: {isoOutputPath}");
                        else
                            Log(LanguageManager.GetTranslation("FormCreazioneISO", "errorecreazioneiso"));

                        AggiornaProgress(1);

                        if (Directory.Exists(@"C:\ISO"))
                            Directory.Delete(@"C:\ISO", true);
                        if (Directory.Exists(@"C:\mount"))
                            Directory.Delete(@"C:\mount", true);

                        AggiornaProgress(1);
                        Log(LanguageManager.GetTranslation("FormCreazioneISO", "creazioneisocompletata"));
                    }
                    catch (Exception ex)
                    {
                        Log($"{LanguageManager.GetTranslation("FormCreazioneISO", "errorecreazioneiso")}: {ex.Message}");
                    }
                }, token);
            }
            catch (OperationCanceledException)
            {
                Log(LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullata"));
            }
            catch (Exception ex)
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
            if (InvokeRequired)
                Invoke(new Action(() => AppendToTextBox(message)));
            else
                AppendToTextBox(message);
        }

        private void AppendToTextBox(string message)
        {
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;
            richTextBox1.SelectionColor = Color.White;
            richTextBox1.SelectionFont = new Font("Segoe UI", 9, FontStyle.Regular);
            Log($"{DateTime.Now:HH:mm:ss} ");
            richTextBox1.SelectionColor = Color.FromArgb(70, 130, 180);
            richTextBox1.SelectionFont = new Font("Segoe UI", 9, FontStyle.Bold);
            Log("➤ ");
            richTextBox1.SelectionColor = Color.White;
            richTextBox1.SelectionFont = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            Log($"{message}");
            richTextBox1.SelectionColor = Color.FromArgb(240, 240, 240); 
            Log("\n────────────────────────────────────────────\n");
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.ScrollToCaret();
            richTextBox1.SelectionColor = richTextBox1.ForeColor;
            richTextBox1.SelectionFont = richTextBox1.Font;
        }
    }
}
