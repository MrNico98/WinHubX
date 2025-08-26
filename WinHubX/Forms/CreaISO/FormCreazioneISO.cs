using DiscUtils;
using DiscUtils.Udf;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using WinHubX.Forms.Base;

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
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            this.form1 = form1;
            this.formcreaiso = formcreaiso;
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
            form1.pictureBox4.Enabled = false;
            form1.btnHome.Enabled = false;
            form1.btnTools.Enabled = false;
            form1.btnWin.Enabled = false;
            form1.btnOffice.Enabled = false;
            form1.btnSettaggi.Enabled = false;
            form1.btnDebloat.Enabled = false;
            form1.btnCreaISO.Enabled = false;
            form1.btnCreaISO.Enabled = false;
            form1.btnmonitoraggio.Enabled = false;
            form1.btnReinstallaApp.Enabled = false;
            try
            {
                btnStop.Visible = true;
                btnStop.Enabled = true;

                await Task.Delay(3000, token);
                await AddAndAwait(Settaggi(token));
                progressBar1.Value = 10;

                await Task.Delay(2000, token);
                await AddAndAwait(CreazioneCartella(token));
                progressBar1.Value = 20;

                await Task.Delay(2000, token);
                await AddAndAwait(VerificaWIMoESD(token));
                progressBar1.Value = 30;

                await Task.Delay(2000, token);
                await AddAndAwait(MontaggioInstall(token));
                progressBar1.Value = 40;

                await Task.Delay(2000, token);
                await AddAndAwait(Unattend(token));
                progressBar1.Value = 50;

                await Task.Delay(2000, token);
                await AddAndAwait(RimozioneDiAlcuniProcessi(token));
                progressBar1.Value = 60;

                await Task.Delay(2000, token);
                await AddAndAwait(VerificaParametri(token));
                progressBar1.Value = 70;

                await Task.Delay(2000, token);
                await AddAndAwait(CopiaFileNecessari(token));
                progressBar1.Value = 80;

                var stillRunning = taskList.Where(t => !t.IsCompleted).ToList();
                if (stillRunning.Any())
                {
                    string info = string.Join("\n", stillRunning.Select((t, i) => $"Task {i + 1} ancora attivo"));
                    _ = MessageBox.Show("Warning! There are still active tasks:\n" + info);
                    return;
                }

                await AddAndAwait(CreazioneInstall(token));
                progressBar1.Value = 90;

                await Task.Delay(2000, token);
                await AddAndAwait(CreazioneISO(token));
                progressBar1.Value = 90;

                await Task.Delay(2000, token);
                await AddAndAwait(Finito(token));
                progressBar1.Value = 100;
            }
            catch (OperationCanceledException)
            {
                _ = MessageBox.Show("Aborted");
            }
        }

        private async Task AddAndAwait(Task task)
        {
            taskList.Add(task);
            await task;
        }

        private Task Finito(CancellationToken token)
        {
            progressBar2.Value = 0;
            try
            {
                _ = this.Invoke((MethodInvoker)delegate
                {
                    string successo1 = LanguageManager.GetTranslation("FormCreazioneISO", "successo1");
                    string successo2 = LanguageManager.GetTranslation("FormCreazioneISO", "successo2");
                    Color originalColor = richTextBox1.SelectionColor;
                    richTextBox1.SelectionColor = Color.Orange;
                    richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                    richTextBox1.AppendText("\n\n" + successo1);
                    richTextBox1.AppendText("\n" + successo2);
                    richTextBox1.SelectionColor = originalColor;
                    richTextBox1.ScrollToCaret();
                });

                btnStop.Visible = false;
                btnBack.Enabled = true;
                form1.pictureBox4.Enabled = true;
                form1.btnHome.Enabled = true;
                form1.btnTools.Enabled = true;
                form1.btnWin.Enabled = true;
                form1.btnOffice.Enabled = true;
                form1.btnSettaggi.Enabled = true;
                form1.btnDebloat.Enabled = true;
                form1.btnCreaISO.Enabled = true;
                form1.btnmonitoraggio.Enabled = true;
                form1.btnReinstallaApp.Enabled = true;

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
                richTextBox1.AppendText($"\nError: {ex.Message}");
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
            if (ParametriISO != null && ParametriISO.TryGetValue("SelectedFile", out var selectedFile))
            {
                try
                {
                    if (!File.Exists(selectedFile))
                    {
                        string erroreisononcista = LanguageManager.GetTranslation("FormCreazioneISO", "erroreisonontrovata");
                        richTextBox1.AppendText($"\n{erroreisononcista} {selectedFile}");
                        return;
                    }

                    string extractPath = @"C:\ISO\WinISO";
                    _ = Directory.CreateDirectory(extractPath);
                    progressBar2.Value = 0;

                    await Task.Run(() =>
                    {
                        token.ThrowIfCancellationRequested();

                        using (FileStream fs = File.Open(selectedFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        using (UdfReader reader = new UdfReader(fs))
                        {
                            DiscDirectoryInfo root = reader.GetDirectoryInfo("");
                            int totalFiles = CountFiles(root);
                            int extractedFiles = 0;
                            ExtractDirectory(root, extractPath, ref extractedFiles, totalFiles, token);
                        }
                    }, token);
                    string estrazioneiso = LanguageManager.GetTranslation("FormCreazioneISO", "estrazioneisook");
                    richTextBox1.AppendText("\n" + estrazioneiso);
                }
                catch (OperationCanceledException)
                {
                    richTextBox1.AppendText("\nAborted");
                }
                catch (Exception ex)
                {
                    richTextBox1.AppendText($"\nError: {ex.Message}");
                }
            }
        }

        private void ExtractDirectory(DiscDirectoryInfo directory, string targetPath, ref int extractedFiles, int totalFiles, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            _ = Directory.CreateDirectory(targetPath);
            foreach (DiscFileInfo file in directory.GetFiles())
            {
                token.ThrowIfCancellationRequested();

                string destPath = Path.Combine(targetPath, file.Name);
                using (Stream source = file.OpenRead())
                using (FileStream dest = File.Create(destPath))
                {
                    source.CopyTo(dest);
                }
                extractedFiles++;
                int progress = (int)((double)extractedFiles / totalFiles * 100);
                Invoke(new Action(() => progressBar2.Value = progress));
            }
            foreach (DiscDirectoryInfo subDir in directory.GetDirectories())
            {
                token.ThrowIfCancellationRequested();

                string destPath = Path.Combine(targetPath, subDir.Name);
                ExtractDirectory(subDir, destPath, ref extractedFiles, totalFiles, token);
            }
        }


        private int CountFiles(DiscDirectoryInfo directory)
        {
            int count = 0;

            foreach (DiscFileInfo file in directory.GetFiles())
            {
                count++;
            }

            foreach (DiscDirectoryInfo subDir in directory.GetDirectories())
            {
                count += CountFiles(subDir);
            }

            return count;
        }
        private async Task VerificaWIMoESD(CancellationToken token)
        {
            string sourcesPath = @"C:\ISO\WinISO\sources";
            string esdPath = Path.Combine(sourcesPath, "install.esd");
            string wimPath = Path.Combine(sourcesPath, "install.wim");
            string wimProPath = Path.Combine(sourcesPath, "install_pro.wim");
            string erroreIndiceNonSelezionato = LanguageManager.GetTranslation("FormCreazioneISO", "erroreindicenonselezionato");
            string conversioneEsdWim = LanguageManager.GetTranslation("FormCreazioneISO", "conversioneesdwim");
            string conversioneSuccesso = LanguageManager.GetTranslation("FormCreazioneISO", "conversionesuccesso");
            string trovatoWim = LanguageManager.GetTranslation("FormCreazioneISO", "trovatoinstallwim");
            string ottimizzazioneSuccesso = LanguageManager.GetTranslation("FormCreazioneISO", "ottimizzazionesuccesso");
            string nessunFileTrovato = LanguageManager.GetTranslation("FormCreazioneISO", "nessunfilewimesd");
            string erroreOperazione = LanguageManager.GetTranslation("FormCreazioneISO", "erroreoperazione");
            string operazioneannullata = LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullatatoken");

            try
            {
                if (ParametriISO == null || !ParametriISO.TryGetValue("ComboSelected", out var indexValue))
                {
                    richTextBox1.AppendText("\n" + erroreIndiceNonSelezionato);
                    return;
                }
                if (File.Exists(esdPath))
                {
                    richTextBox1.AppendText("\n" + conversioneEsdWim);

                    string arguments = $"/export-image /SourceImageFile:\"{esdPath}\" " +
                                      $"/SourceIndex:{indexValue} " +
                                      $"/DestinationImageFile:\"{wimPath}\" " +
                                      $"/Compress:max /CheckIntegrity";
                    token.ThrowIfCancellationRequested();

                    bool success = await Task.Run(() => EseguiDISM(arguments, token), token);

                    if (success && File.Exists(wimPath))
                    {
                        File.Delete(esdPath);
                        richTextBox1.AppendText("\n" + conversioneSuccesso);
                    }
                }
                else if (File.Exists(wimPath))
                {
                    richTextBox1.AppendText("\n" + trovatoWim);

                    string arguments = $"/export-image /SourceImageFile:\"{wimPath}\" " +
                                      $"/SourceIndex:{indexValue} " +
                                      $"/DestinationImageFile:\"{wimProPath}\" " +
                                      $"/Compress:max /CheckIntegrity";
                    token.ThrowIfCancellationRequested();

                    bool success = await Task.Run(() => EseguiDISM(arguments, token), token);

                    if (success && File.Exists(wimProPath))
                    {
                        File.Delete(wimPath);
                        File.Move(wimProPath, wimPath);
                        richTextBox1.AppendText("\n" + ottimizzazioneSuccesso);
                    }
                }
                else
                {
                    richTextBox1.AppendText("\n" + nessunFileTrovato);
                }
            }
            catch (OperationCanceledException)
            {
                richTextBox1.AppendText("\n" + operazioneannullata);
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText($"\n{erroreOperazione}: {ex.Message}");
                if (File.Exists(wimProPath))
                {
                    try { File.Delete(wimProPath); } catch { }
                }
            }
        }

        private async Task<bool> EseguiDISM(string arguments, CancellationToken token)
        {
            try
            {
                _ = this.Invoke((MethodInvoker)delegate
                {
                    progressBar2.Maximum = 100;
                    progressBar2.Value = 0;
                });

                using (Process dismProcess = new Process())
                {
                    dismProcess.StartInfo.FileName = "dism.exe";
                    dismProcess.StartInfo.Arguments = arguments;
                    dismProcess.StartInfo.UseShellExecute = false;
                    dismProcess.StartInfo.RedirectStandardOutput = true;
                    dismProcess.StartInfo.RedirectStandardError = true;
                    dismProcess.StartInfo.CreateNoWindow = true;

                    dismProcess.OutputDataReceived += (sender, args) =>
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                        {
                            UpdateProgressFromOutput(args.Data);
                        }
                    };

                    dismProcess.ErrorDataReceived += (sender, args) =>
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                        {
                            _ = richTextBox1.Invoke((MethodInvoker)delegate
                            {
                                richTextBox1.AppendText($"\nError: {args.Data}");
                            });
                        }
                    };

                    _ = dismProcess.Start();
                    dismProcess.BeginOutputReadLine();
                    dismProcess.BeginErrorReadLine();
                    while (!dismProcess.HasExited)
                    {
                        token.ThrowIfCancellationRequested();
                        await Task.Delay(100);
                    }

                    dismProcess.WaitForExit();
                    return dismProcess.ExitCode == 0;
                }
            }
            catch (OperationCanceledException)
            {
                _ = richTextBox1.Invoke((MethodInvoker)delegate
                {
                    string operazioneannullata = LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullatatoken");
                    richTextBox1.AppendText("\n" + operazioneannullata);
                });
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                _ = this.Invoke((MethodInvoker)delegate
                {
                    progressBar2.Value = 100;
                });
            }
        }

        private void UpdateProgressFromOutput(string output)
        {
            if (output.Contains("%"))
            {
                Match match = Regex.Match(output, @"(\d+(\.\d+)?)%");
                if (match.Success)
                {
                    double percent = double.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
                    int progressValue = (int)Math.Round(percent);

                    _ = this.Invoke((MethodInvoker)delegate
                    {
                        progressBar2.Value = Math.Min(Math.Max(progressValue, 0), 100);
                    });
                }
            }
        }

        private async Task MontaggioInstall(CancellationToken token)
        {
            string wimPath = @"C:\ISO\WinISO\sources\install.wim";
            string mountDir = @"C:\mount\mount";
            string erroreFileWimNonTrovato = LanguageManager.GetTranslation("FormCreazioneISO", "errorefilewimnontrovato");
            string montaggioInCorso = LanguageManager.GetTranslation("FormCreazioneISO", "montaggioincorso");
            string montaggioSuccesso = LanguageManager.GetTranslation("FormCreazioneISO", "montaggiosuccesso");
            string erroreMontaggio = LanguageManager.GetTranslation("FormCreazioneISO", "erroremontaggio");
            string erroreGenericoMontaggio = LanguageManager.GetTranslation("FormCreazioneISO", "erroregenericomontaggio");

            try
            {
                if (!File.Exists(wimPath))
                {
                    richTextBox1.AppendText("\n" + erroreFileWimNonTrovato);
                    return;
                }

                _ = Directory.CreateDirectory(mountDir);
                richTextBox1.AppendText("\n" + montaggioInCorso);

                string arguments = $"/mount-image /imagefile:\"{wimPath}\" /index:1 /mountdir:\"{mountDir}\"";
                bool success = await Task.Run(() => EseguiDISM(arguments, token), token);

                if (success)
                {
                    richTextBox1.AppendText("\n" + montaggioSuccesso);
                }
                else
                {
                    richTextBox1.AppendText("\n" + erroreMontaggio);
                }
            }
            catch (OperationCanceledException)
            {
                string operazioneannullata = LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullatatoken");
                richTextBox1.AppendText("\n" + operazioneannullata);
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText($"\n{erroreGenericoMontaggio}: {ex.Message}");
            }
        }


        private async Task Unattend(CancellationToken token)
        {
            try
            {
                if (ParametriISO == null || !ParametriISO.TryGetValue("windowsVersion", out var windowsVersion))
                {
                    richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "erroreversionewindows"));
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
                            richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "copiabypass"));
                        }
                        richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "configbypass"));

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
                            richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "montaggioboot"));

                            string arguments = $"/mount-image /imagefile:\"{bootWimPath}\" /index:2 /mountdir:\"{bootMountDir}\"";
                            bool success = await Task.Run(() => EseguiDISM(arguments, token), token);

                            if (success)
                            {
                                richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "montaggiobootsuccesso"));
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
                                    richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "rinominatoappraiser"));
                                }
                                else
                                {
                                    richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "appraisernontrovato"));
                                }
                                await ExecuteCommand("reg unload HKLM\\TK_BOOT_SYSTEM", token);
                                int retry = 0;
                                while (RegistryKeyExists(@"HKEY_LOCAL_MACHINE\TK_BOOT_SYSTEM") && retry < 5)
                                {
                                    await Task.Delay(3000, token);
                                    await ExecuteCommand("reg unload HKLM\\TK_BOOT_SYSTEM", token);
                                    retry++;
                                }
                                richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "smontaggioboot"));
                                string unmountArguments = $"/unmount-image /mountdir:\"{bootMountDir}\" /commit";
                                bool unmountSuccess = await Task.Run(() => EseguiDISM(unmountArguments, token), token);

                                if (unmountSuccess)
                                    richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "smontaggiobootsuccesso"));
                                else
                                    richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "erroresmontaggioboot"));
                            }
                            else
                            {
                                richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "erroremontaggioboot"));
                            }

                            richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "bypasscompletato"));
                        }
                    }
                    if (unattendType == "Stock")
                    {
                        if (File.Exists(sourceUnattendStock))
                        {
                            File.Copy(sourceUnattendStock, destUnattend, true);
                            richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "copiastock"));
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
                        richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "copiaunattend10x64"));
                    }
                    else if (arch == "x32" && File.Exists(sourceUnattendx32))
                    {
                        File.Copy(sourceUnattendx32, destUnattend, true);
                        richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "copiaunattend10x32"));
                    }
                    else
                    {
                        richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "erroreunattendarch"));
                    }
                }
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "erroregenericaunattend") + $": {ex.Message}");
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
            _ = richTextBox1.Invoke((MethodInvoker)(() =>
            {
                richTextBox1.AppendText($"\n[ESEGUITO] {command}");
            }));

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
                    _ = richTextBox1.Invoke((MethodInvoker)(() =>
                    {
                        richTextBox1.AppendText("\n" + operazioneannullata);
                    }));
                }

                await outputTask.ConfigureAwait(false);
                process.WaitForExit();
            }
        }

        private async Task RimozioneDiAlcuniProcessi(CancellationToken token)
        {
            if (ParametriISO != null &&
                ParametriISO.TryGetValue("Processi", out var processo) &&
                processo == "RimuoviProcessi")
            {
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

                Invoke(new Action(() =>
                {
                    progressBar2.Minimum = 0;
                    progressBar2.Maximum = pacchetti.Count;
                    progressBar2.Value = 0;
                }));

                foreach (var kvp in pacchetti)
                {
                    string nome = kvp.Key;
                    string comando = kvp.Value;

                    if (token.IsCancellationRequested)
                    {
                        Invoke(new Action(() =>
                        {
                            string operazioneannullata = LanguageManager.GetTranslation("FormCreazioneISO", "operazioneannullatatoken");
                            richTextBox1.AppendText("\n" + operazioneannullata);
                        }));
                        break;
                    }

                    Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"\nRimozione di \"{nome}\"...");
                    }));

                    var processInfo = new ProcessStartInfo()
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{comando}; $pkgs.Count\"",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    using (var process = Process.Start(processInfo))
                    {
                        string output = await process.StandardOutput.ReadToEndAsync();
                        string error = await process.StandardError.ReadToEndAsync();
                        await process.WaitForExitAsync();
                        int removedCount = 0;
                        int.TryParse(output.Trim().Split('\n').LastOrDefault()?.Trim(), out removedCount);

                        Invoke(new Action(() =>
                        {
                            if (removedCount > 0)
                                richTextBox1.AppendText(" OK");
                            else
                                richTextBox1.AppendText(" ⚠ Nessun pacchetto rimosso");

                            if (progressBar2.Value < progressBar2.Maximum)
                                progressBar2.Value += 1;
                        }));
                    }

                    await Task.Delay(500, token);
                }

                Invoke(new Action(() =>
                {
                    string rimozionepacchetti = LanguageManager.GetTranslation("FormCreazioneISO", "rimozionepacchetticompletata");
                    richTextBox1.AppendText("\n" + rimozionepacchetti);
                }));
            }
        }
        private async Task VerificaParametri(CancellationToken token)
        {
            string targetDir = @"C:\mount\mount\Windows";

            Invoke(new Action(() =>
            {
                progressBar2.Minimum = 0;
                progressBar2.Maximum = 3;
                progressBar2.Value = 0;
            }));

            await Task.Run(() =>
            {
                try
                {
                    if (token.IsCancellationRequested) return;

                    if (!Directory.Exists(targetDir))
                        _ = Directory.CreateDirectory(targetDir);

                    if (ParametriISO != null)
                    {
                        if (token.IsCancellationRequested) return;

                        if (ParametriISO.TryGetValue("edgeRemovalPreference", out var edgePref) && edgePref == "RemoveEdge")
                        {
                            File.Create(Path.Combine(targetDir, "noedge.pref")).Dispose();

                            File.Copy(Path.Combine(Path.GetTempPath(), @"RisorseCreaISO\Risorse\OperaGXSetup.exe"), Path.Combine(targetDir, "OperaGXSetup.exe"), true);
                            File.Copy(Path.Combine(Path.GetTempPath(), @"RisorseCreaISO\Risorse\PowerRun.exe"), Path.Combine(targetDir, "PowerRun.exe"), true);

                            Invoke(new Action(() => progressBar2.Value += 1));
                        }

                        if (token.IsCancellationRequested) return;
                        if (ParametriISO == null || !ParametriISO.TryGetValue("windowsVersion", out var windowsVersion))
                        {
                            richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "erroreversionewindows"));
                            return;
                        }

                        if (windowsVersion == "11" && ParametriISO.TryGetValue("Unattend", out var unattendType))
                        {
                            if (unattendType == "Bypass")
                                File.Create(Path.Combine(targetDir, "bypass.pref")).Dispose();

                            Invoke(new Action(() => progressBar2.Value += 1));
                        }

                        if (token.IsCancellationRequested) return;

                        if (ParametriISO.TryGetValue("DebloatApp", out var debloat) && debloat == "Debloat")
                        {
                            File.Create(Path.Combine(targetDir, "debloatapp.pref")).Dispose();
                            Invoke(new Action(() => progressBar2.Value += 1));
                        }

                        if (token.IsCancellationRequested) return;

                        if (ParametriISO.TryGetValue("TipoOttimizzazione", out var TipoOttimizzazione))
                        {
                            string fileName = TipoOttimizzazione switch
                            {
                                "LavorWork" => "workstation.pref",
                                "IsoGaming" => "gaming.pref",
                                _ => null
                            };

                            if (fileName != null)
                            {
                                string filePath = Path.Combine(targetDir, fileName);

                                if (!File.Exists(filePath))
                                {
                                    File.Create(filePath).Dispose();
                                    Invoke(new Action(() => progressBar2.Value += 1));
                                }
                            }
                        }

                        if (token.IsCancellationRequested) return;

                        if (ParametriISO.TryGetValue("defenderPreference", out var defender) && defender == "DisableWindowsDefender")
                        {
                            File.Create(Path.Combine(targetDir, "nodefender.pref")).Dispose();
                            Invoke(new Action(() => progressBar2.Value += 1));
                        }

                        if (token.IsCancellationRequested) return;

                        try
                        {
                            if (ParametriISO.TryGetValue("DriverWin", out var DriverWin) && DriverWin == "DriverCartella")
                            {
                                string driverFolder = null;

                                Invoke(new Action(() =>
                                {
                                    using (var dialog = new FolderBrowserDialog())
                                    {
                                        dialog.Description = LanguageManager.GetTranslation("FormCreazioneISO", "selezionacartelladriver");
                                        if (dialog.ShowDialog() == DialogResult.OK)
                                        {
                                            driverFolder = dialog.SelectedPath;
                                        }
                                    }
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

                                    string output = process.StandardOutput.ReadToEnd();
                                    string error = process.StandardError.ReadToEnd();
                                    process.WaitForExit();

                                    Invoke(new Action(() =>
                                    {
                                        if (process.ExitCode == 0)
                                            richTextBox1.AppendText($"\n{LanguageManager.GetTranslation("FormCreazioneISO", "driverintegratocartella")}: {driverFolder}");
                                        else
                                            richTextBox1.AppendText($"\n{LanguageManager.GetTranslation("FormCreazioneISO", "erroreintegracartella")}\n{error}");
                                    }));
                                }

                                Invoke(new Action(() => progressBar2.Value += 1));
                            }

                            if (ParametriISO.TryGetValue("DriverWin", out DriverWin) && DriverWin == "DriverQuestoPC")
                            {
                                string tempDriverDir = Path.Combine(Path.GetTempPath(), "DriverBackup_" + Guid.NewGuid().ToString("N"));
                                _ = Directory.CreateDirectory(tempDriverDir);

                                var exportProcess = Process.Start(new ProcessStartInfo
                                {
                                    FileName = "dism.exe",
                                    Arguments = $"/Online /Export-Driver /Destination:\"{tempDriverDir}\"",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true
                                });

                                string exportOutput = exportProcess.StandardOutput.ReadToEnd();
                                string exportError = exportProcess.StandardError.ReadToEnd();
                                exportProcess.WaitForExit();

                                Invoke(new Action(() =>
                                {
                                    if (exportProcess.ExitCode == 0)
                                        richTextBox1.AppendText($"\n{LanguageManager.GetTranslation("FormCreazioneISO", "driversuccessoesportazione")}.");
                                    else
                                        richTextBox1.AppendText($"\n{LanguageManager.GetTranslation("FormCreazioneISO", "erroreesportazionedriver")}\n{exportError}");
                                }));

                                if (exportProcess.ExitCode == 0)
                                {
                                    var addDriverProcess = Process.Start(new ProcessStartInfo
                                    {
                                        FileName = "dism.exe",
                                        Arguments = $"/Image:\"C:\\Mount\\mount\" /Add-Driver /Driver:\"{tempDriverDir}\" /Recurse",
                                        UseShellExecute = false,
                                        RedirectStandardOutput = true,
                                        RedirectStandardError = true,
                                        CreateNoWindow = true
                                    });

                                    string addOutput = addDriverProcess.StandardOutput.ReadToEnd();
                                    string addError = addDriverProcess.StandardError.ReadToEnd();
                                    addDriverProcess.WaitForExit();

                                    Invoke(new Action(() =>
                                    {
                                        if (addDriverProcess.ExitCode == 0)
                                            richTextBox1.AppendText($"\n{LanguageManager.GetTranslation("FormCreazioneISO", "driverintegrazionesistema")}");
                                        else
                                            richTextBox1.AppendText($"\n{LanguageManager.GetTranslation("FormCreazioneISO", "erroreintegrasistema")}\n{addError}");
                                    }));
                                }

                                Directory.Delete(tempDriverDir, true);
                                Invoke(new Action(() => progressBar2.Value += 1));
                            }

                            Invoke(new Action(() =>
                            {
                                string verificaparametri = LanguageManager.GetTranslation("FormCreazioneISO", "verificaparametricompletata");
                                richTextBox1.AppendText("\n" + verificaparametri);
                            }));
                        }
                        catch (Exception ex)
                        {
                            Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText($"\n{LanguageManager.GetTranslation("FormCreazioneISO", "erroregenerale")}: {ex.Message}");
                            }));
                        }
                    }
                }
                catch
                {
                    richTextBox1.AppendText(LanguageManager.GetTranslation("FormCreazioneISO", "erroregenerale"));
                }
            }, token);
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
                richTextBox1.AppendText($"\n[INFO] {LanguageManager.GetTranslation("FormCreazioneISO", "iniziocopianeccessari")}");
            }));

            if (ParametriISO.TryGetValue("ImportaSettaggiWinhubx", out var ImportaSettaggiWinhubx) && ImportaSettaggiWinhubx == "SiImporta")
            {
                Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"\n[INFO] {LanguageManager.GetTranslation("FormCreazioneISO", "importazionesettaggiabilitata")}");
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
                            richTextBox1.AppendText($"\n[INFO] {LanguageManager.GetTranslation("FormCreazioneISO", "fileconfigcopiato")}");
                        }));
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText($"\n[WARN] {LanguageManager.GetTranslation("FormCreazioneISO", "fileconfignontrovato")}");
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"\n[ERROR] {LanguageManager.GetTranslation("FormCreazioneISO", "erroreexportreg")}: {ex.Message}");
                    }));
                }
            }

            string sourceFolder = Path.Combine(Path.GetTempPath(), @"RisorseCreaISO\Risorse");
            string targetFolder = @"C:\mount\mount\Windows";

            Invoke(new Action(() =>
            {
                progressBar2.Minimum = 0;
                progressBar2.Maximum = filesToCopy.Count;
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
                                richTextBox1.AppendText($"\n[OK] {LanguageManager.GetTranslation("FormCreazioneISO", "copiatofile")}: {file}");
                            }));
                        }
                        else
                        {
                            Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText($"\n{LanguageManager.GetTranslation("FormCreazioneISO", "filenontrovato")}: {sourceFilePath}");
                            }));
                        }

                        Invoke(new Action(() =>
                        {
                            progressBar2.Value += 1;
                        }));
                    }

                    Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"\n{LanguageManager.GetTranslation("FormCreazioneISO", "copiacompletata")}");
                    }));
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"\n{LanguageManager.GetTranslation("FormCreazioneISO", "erroregenerico")}: {ex.Message}");
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
                    richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "errordirectorymount"));
                    return;
                }
                string deletedFolderPath = Path.Combine(mountDir, "[DELETED]");
                if (Directory.Exists(deletedFolderPath))
                {
                    Directory.Delete(deletedFolderPath, true);
                    richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "cartelladeletedrimossa"));
                }
                richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "smontaggiosalvataggio"));
                await Task.Delay(6000);
                string arguments = $"/unmount-image /mountdir:\"{mountDir}\" /commit";
                bool success = await Task.Run(() => EseguiDISM(arguments, token), token);

                if (success)
                {
                    richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "immaginesmontata"));
                }
                else
                {
                    richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "erroresmontaggio"));
                }
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText($"\n{LanguageManager.GetTranslation("FormCreazioneISO", "errorecreazioneinstall")}: {ex.Message}");
            }
        }

        private async Task CreazioneISO(CancellationToken token)
        {
            string sourcePath = @"C:\ISO\WinISO";
            string isoOutputPath = @"C:\ISO\WindowsISO_edited.iso";
            string oscdimgPath = Path.Combine(Path.GetTempPath(), @"RisorseCreaISO\Risorse\oscdimg");

            Invoke(new Action(() =>
            {
                progressBar2.Minimum = 0;
                progressBar2.Maximum = 3;
                progressBar2.Value = 0;
            }));

            await Task.Run(() =>
            {
                try
                {
                    string oscdimgArguments = $"-m -o -u2 -bootdata:2#p0,e,b{sourcePath}\\boot\\etfsboot.com#pEF,e,b{sourcePath}\\efi\\microsoft\\boot\\efisys.bin {sourcePath} {isoOutputPath}";
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
                                return;
                            }

                            Thread.Sleep(100);
                        }
                    }

                    Invoke(new Action(() =>
                    {
                        progressBar2.Value += 1;
                    }));

                    string destinationPath = @"C:\WindowsISO_edited.iso";
                    File.Copy(isoOutputPath, destinationPath, true);

                    Invoke(new Action(() =>
                    {
                        progressBar2.Value += 1;
                    }));

                    Directory.Delete(@"C:\ISO", true);
                    Directory.Delete(@"C:\mount", true);

                    Invoke(new Action(() =>
                    {
                        progressBar2.Value += 1;
                        richTextBox1.AppendText("\n" + LanguageManager.GetTranslation("FormCreazioneISO", "creazioneisocompletata"));
                    }));
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"\n{LanguageManager.GetTranslation("FormCreazioneISO", "errorecreazioneiso")}: {ex.Message}");
                    }));
                }
            }, token);
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            _ = LanguageManager.GetTranslation("Form1", "titoloCreaISO");
            form1.lblPanelTitle.Text = "creazioneiso";
            form1.PnlFormLoader.Controls.Clear();
            formcreaiso = new FormCreaISO(form1)
            {
                Dock = DockStyle.Fill,
                TopLevel = false,
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None
            };
            form1.PnlFormLoader.Controls.Add(formcreaiso);
            formcreaiso.Show();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            btnBack.Enabled = true;
            btnBack.Cursor = Cursors.Hand;
            form1.pictureBox4.Enabled = true;
            form1.btnHome.Enabled = true;
            form1.btnWin.Enabled = true;
            form1.btnOffice.Enabled = true;
            form1.btnSettaggi.Enabled = true;
            form1.btnDebloat.Enabled = true;
            form1.btnCreaISO.Enabled = true;
            form1.btnCreaISO.Enabled = true;
            form1.btnmonitoraggio.Enabled = true;
            form1.btnReinstallaApp.Enabled = true;
            form1.btnTools.Enabled = true;
        }
    }
}
