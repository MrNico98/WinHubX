using System.Diagnostics;
using System.Globalization;
using System.Management;
using WinHubX.Forms.Base;

namespace WinHubX.Forms.Settaggi
{
    public partial class FormRipristinoSO : Form
    {
        private Form1 form1;
        private FormSettaggi formSettaggi;
        private CancellationTokenSource cts = new();
        private System.Windows.Forms.Timer countdownTimer;
        private int remainingTime;
        private CancellationTokenSource cancellationTokenSource;

        public FormRipristinoSO(FormSettaggi formSettaggi, Form1 form1)
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            this.form1 = form1;
            this.formSettaggi = formSettaggi;
            dateTimePicker1.Format = DateTimePickerFormat.Time;
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker1.Value = DateTime.Today.AddMinutes(30);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = "Settaggi";
            form1.PnlFormLoader.Controls.Clear();
            formSettaggi = new FormSettaggi(form1)
            {
                Dock = DockStyle.Fill,
                TopLevel = false,
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None
            };
            form1.PnlFormLoader.Controls.Add(formSettaggi);
            formSettaggi.Show();
        }

        private async void buttonStart_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            btnStop.Visible = true;
            progressBar1.Value = 0;

            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                if (checkBox_sw.Checked)
                {
                    await StartScanAsyncSW(cancellationTokenSource.Token);
                }
                if (checkBox_hw.Checked)
                {
                    label3.Visible = true;
                    await StartScanAsync(cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                UpdateLabel(LanguageManager.GetTranslation("FormRipristinoSO", "operazioneAnnullata"));
            }
            finally
            {
                button1.Visible = true;
                btnStop.Visible = false;
                label3.Visible = false;
            }
        }

        private async Task StartScanAsyncSW(CancellationToken cancellationToken)
        {
            int totalSteps = 7;
            int currentStep = 0;

            UpdateLabel(LanguageManager.GetTranslation("FormRipristinoSO", "backupRegistro"));
            await BackupRegistryAsync(cancellationToken);
            UpdateProgress(++currentStep, totalSteps, cancellationToken);

            UpdateLabel(LanguageManager.GetTranslation("FormRipristinoSO", "controlloFileSistema"));
            await RunCommandAsync("DISM /Online /Cleanup-Image /CheckHealth", cancellationToken);
            UpdateProgress(++currentStep, totalSteps, cancellationToken);

            UpdateLabel(LanguageManager.GetTranslation("FormRipristinoSO", "scansioneErroriSistema"));
            await RunCommandAsync("DISM /Online /Cleanup-Image /ScanHealth", cancellationToken);
            UpdateProgress(++currentStep, totalSteps, cancellationToken);

            UpdateLabel(LanguageManager.GetTranslation("FormRipristinoSO", "ripristinoFileSistema"));
            await RunCommandAsync("DISM /Online /Cleanup-Image /RestoreHealth", cancellationToken);
            UpdateProgress(++currentStep, totalSteps, cancellationToken);

            UpdateLabel(LanguageManager.GetTranslation("FormRipristinoSO", "esecuzioneSfc"));
            await RunCommandAsync("sfc /scannow", cancellationToken);
            UpdateProgress(++currentStep, totalSteps, cancellationToken);

            UpdateLabel(LanguageManager.GetTranslation("FormRipristinoSO", "puliziaWinSxS"));
            await RunCommandAsync("Dism.exe /online /Cleanup-Image /StartComponentCleanup", cancellationToken);
            UpdateProgress(++currentStep, totalSteps, cancellationToken);

            UpdateLabel(LanguageManager.GetTranslation("FormRipristinoSO", "pianificazioneChkdsk"));
            await RunCommandAsync("fsutil dirty set C:", cancellationToken);
            UpdateProgress(++currentStep, totalSteps, cancellationToken);

            UpdateLabel(LanguageManager.GetTranslation("FormRipristinoSO", "registrazioneDll"));
            await RegisterSystemDLLs(cancellationToken);
            UpdateProgress(++currentStep, totalSteps, cancellationToken);

            UpdateLabel(LanguageManager.GetTranslation("FormRipristinoSO", "ripristinoCompletato"));
            MessageBox.Show(
                LanguageManager.GetTranslation("FormRipristinoSO", "msgRipristinoCompletato"),
                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task BackupRegistryAsync(CancellationToken cancellationToken)
        {
            string backupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "RegistryBackup.reg");
            await RunCommandAsync($"reg export HKLM\\SOFTWARE {backupPath} /y", cancellationToken);
            await RunCommandAsync($"reg export HKCU {backupPath} /y", cancellationToken);
            LogMessage(string.Format(LanguageManager.GetTranslation("FormRipristinoSO", "logBackupRegistro"), backupPath));
        }

        private async Task RegisterSystemDLLs(CancellationToken cancellationToken)
        {
            string[] dlls = { "atl.dll", "jscript.dll", "msxml3.dll", "shell32.dll", "shdocvw.dll", "urlmon.dll", "vbscript.dll", "wintrust.dll" };

            foreach (var dll in dlls)
            {
                await RunCommandAsync($"regsvr32 /s {dll}", cancellationToken);
                LogMessage(string.Format(LanguageManager.GetTranslation("FormRipristinoSO", "logDllRegistrata"), dll));
            }
        }

        private async Task RunCommandAsync(string command, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();

                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c {command}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (Process process = new Process { StartInfo = psi })
                    {
                        process.OutputDataReceived += (sender, e) => AppendToRichTextBox2(e.Data);
                        process.ErrorDataReceived += (sender, e) => AppendToRichTextBox2($"[ERRORE] {e.Data}");

                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        process.WaitForExit();

                        if (cancellationToken.IsCancellationRequested)
                            throw new OperationCanceledException();
                    }
                }
                catch (OperationCanceledException)
                {
                    AppendToRichTextBox2("Operazione annullata.");
                    throw;
                }
                catch (Exception ex)
                {
                    AppendToRichTextBox2($"Errore: {ex.Message}");
                }
            }, cancellationToken);
        }

        private async Task StartScanAsync(CancellationToken cancellationToken)
        {
            try
            {
                int testDurationMinutes = (int)dateTimePicker1.Value.TimeOfDay.TotalMinutes;
                remainingTime = testDurationMinutes;

                progressBar1.Visible = true;
                label2.Visible = true;
                label2.Text = string.Format(LanguageManager.GetTranslation("FormRipristinoSO", "scansioneInCorsoConTempo"), remainingTime);
                richTextBox1.Clear();
                cancellationTokenSource = new CancellationTokenSource();
                CancellationToken token = cancellationTokenSource.Token;

                if (checkBox_hw.Checked)
                {
                    if (countdownTimer == null)
                    {
                        countdownTimer = new System.Windows.Forms.Timer();
                        countdownTimer.Interval = 1000;
                        countdownTimer.Tick += UpdateCountdown;
                    }
                    remainingTime = testDurationMinutes * 60;
                    countdownTimer.Start();

                    await RunStressTestsContinuously(testDurationMinutes, token);
                }
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText($"Error: {ex.Message}\n");
            }
            finally
            {
                label2.Text = "Completato!";
                progressBar1.Visible = false;
                countdownTimer?.Dispose();
            }
        }

        private async Task RunStressTestsContinuously(int testDurationMinutes, CancellationToken token)
        {
            await VerifyDiskStatusAsync();

            Task cpuTestTask = StressTestCPUAsync(token);
            Task ramTestTask = TestRAMAsync(token);

            await Task.WhenAll(cpuTestTask, ramTestTask);
        }

        private void UpdateCountdown(object sender, EventArgs e)
        {
            if (remainingTime > 0)
            {
                remainingTime--;

                TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);
                string formattedTime = timeSpan.ToString(@"hh\:mm\:ss");

                label2.Text = LanguageManager.GetTranslation("FormRipristinoSO", "scansioneInCorso");
                label3.Text = string.Format(LanguageManager.GetTranslation("FormRipristinoSO", "tempoRimanente"), formattedTime);
            }
            else
            {
                countdownTimer?.Stop();
                label2.Text = LanguageManager.GetTranslation("FormRipristinoSO", "scansioneCompletata");
            }
        }

        #region Hardware

        public async Task StressTestCPUAsync(CancellationToken token)
        {
            UpdateLabel("Avvio stress test CPU...");
            LogMessage("Preparazione stress test CPU...");

            try
            {
                int numThreads = Environment.ProcessorCount;
                LogMessage($"Utilizzando {numThreads} thread per il test.");

                Task monitorTask = MonitorCPUUsageAsync(token);

                List<Task> tasks = new();
                for (int i = 0; i < numThreads; i++)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                        double result = 0;
                        while (!token.IsCancellationRequested)
                        {
                            for (int j = 0; j < 10_000_000; j++)
                            {
                                result += Math.Sqrt(j) * Math.Sin(j);
                                if (j % 1_000_000 == 0 && token.IsCancellationRequested)
                                    return; 
                            }
                            Thread.Yield();
                        }
                    }, token));
                }

                await Task.WhenAll(tasks);
                monitorTask.Dispose();
            }
            catch (Exception ex)
            {
                LogError($"Errore test CPU: {ex.Message}");
            }
        }

        private async Task MonitorCPUUsageAsync(CancellationToken token)
        {
            const int thermalThreshold = 90; 

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var cpuUsage = GetCPUUsage();
                    var cpuTemp = GetCPUTemperature();
                    if (cpuTemp > thermalThreshold)
                    {
                        LogMessage($"ATTENZIONE: CPU in thermal throttling! Temp: {cpuTemp}°C");
                    }
                    else
                    {
                        LogMessage($"CPU Usage: {cpuUsage}%");
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Errore monitoraggio CPU: {ex.Message}");
                }

                await Task.Delay(6000, token);
            }
        }
        private static float GetCPUUsage()
        {
            using PerformanceCounter cpuCounter = new("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            Task.Delay(500).Wait();
            return cpuCounter.NextValue();
        }

        private static float GetCPUTemperature()
        {
            try
            {
                using ManagementObjectSearcher searcher = new("root\\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
                foreach (ManagementObject obj in searcher.Get())
                {
                    double tempK = Convert.ToDouble(obj["CurrentTemperature"]);
                    return (float)((tempK - 2732) / 10.0);
                }
            }
            catch { }
            return -1; 
        }
        public async Task TestRAMAsync(CancellationToken token)
        {
            UpdateLabel("Avvio test RAM...");
            LogMessage("Preparazione test RAM...");

            try
            {
                int blockSize = 1024 * 1024 * 50; 
                long maxRam = GetTotalRAM() * 80 / 100; 
                long allocatedRam = 0;

                LogMessage($"Allocazione fino a {maxRam / (1024 * 1024)} MB di RAM...");
                Queue<byte[]> memoryBlocks = new();

                while (!token.IsCancellationRequested && allocatedRam < maxRam)
                {
                    byte[] block = new byte[blockSize];
                    for (int i = 0; i < block.Length; i += 4096)
                    {
                        block[i] = (byte)(i % 256);
                    }

                    memoryBlocks.Enqueue(block);
                    allocatedRam += blockSize;
                    if (allocatedRam % (1024 * 1024 * 500) == 0)
                    {
                        LogMessage($"RAM allocata: {allocatedRam / (1024 * 1024)} MB...");
                    }

                    await Task.Delay(20, token);
                }

                LogMessage("Liberazione memoria...");
                memoryBlocks.Clear();
                GC.Collect();

                UpdateLabel("Test RAM completato.");
            }
            catch (Exception ex)
            {
                LogError($"Errore test RAM: {ex.Message}");
            }
        }
        private static long GetTotalRAM()
        {
            try
            {
                using ManagementObjectSearcher searcher = new("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    return Convert.ToInt64(obj["TotalPhysicalMemory"]);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore lettura RAM: {ex.Message}");
            }

            return 8L * 1024 * 1024 * 1024;
        }
        public async Task VerifyDiskStatusAsync()
        {
            UpdateLabel("Verifica stato del disco...");
            LogMessage("Inizio controllo avanzato del disco...");

            try
            {
                using ManagementObjectSearcher searcher = new("SELECT * FROM Win32_DiskDrive");
                foreach (ManagementObject disk in searcher.Get())
                {
                    string deviceId = disk["DeviceID"]?.ToString() ?? "Sconosciuto";
                    string model = disk["Model"]?.ToString() ?? "Modello sconosciuto";
                    long diskSize = Convert.ToInt64(disk["Size"] ?? 0) / (1024 * 1024 * 1024);

                    string diskInfo = $"Disco rilevato: {model} ({deviceId}) - {diskSize} GB";
                    LogMessage(diskInfo);
                    UpdateLabel($"Analisi {model}...");

                    AppendToRichTextBox2(diskInfo + Environment.NewLine);
                    string NamespacePath = @"\\.\root\cimv2";
                    string ClassName = "Win32_DiskDrive";
                    ManagementClass oClass = new ManagementClass(NamespacePath + ":" + ClassName);

                    foreach (ManagementObject oObject in oClass.GetInstances())
                    {
                        var sign = Convert.ToString(oObject["Signature"]);
                        var smartModel = Convert.ToString(oObject["Model"]);
                        var status = Convert.ToString(oObject["Status"]);

                        if (Equals(sign, ""))
                        {
                            AppendToRichTextBox2("DISK model: " + smartModel);
                            AppendToRichTextBox2("Status: " + status);
                            AppendToRichTextBox2(Environment.NewLine);
                        }
                    }
                    string speedResults = DiskSpeedTest(deviceId);
                    AppendToRichTextBox2(speedResults + Environment.NewLine);

                    LogMessage("-------------------------------------------------");
                    AppendToRichTextBox2("-------------------------------------------------" + Environment.NewLine);
                }

                UpdateLabel("Verifica disco completata.");
                LogMessage("Verifica disco completata con successo.");
            }
            catch (Exception ex)
            {
                LogError($"Errore verifica disco: {ex.Message}");
            }
        }
        private void AppendToRichTextBox2(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            if (richTextBox2.InvokeRequired)
            {
                richTextBox2.Invoke(new Action(() => AppendToRichTextBox2(message)));
            }
            else
            {
                richTextBox2.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\n");
                richTextBox2.ScrollToCaret();
            }
        }
        private string DiskSpeedTest(string deviceId)
        {
            try
            {
                string tempFile = Path.Combine(Path.GetTempPath(), "disk_speed_test.tmp");
                byte[] data = new byte[1024 * 1024 * 50];
                new Random().NextBytes(data);
                Stopwatch stopwatch = Stopwatch.StartNew();
                File.WriteAllBytes(tempFile, data);
                stopwatch.Stop();
                double writeSpeed = (50.0 / (stopwatch.ElapsedMilliseconds / 1000.0));
                stopwatch.Restart();
                _ = File.ReadAllBytes(tempFile);
                stopwatch.Stop();
                double readSpeed = (50.0 / (stopwatch.ElapsedMilliseconds / 1000.0));

                File.Delete(tempFile);

                string speedResult = $"Velocità scrittura: {writeSpeed:F2} MB/s | Velocità lettura: {readSpeed:F2} MB/s";
                LogMessage(speedResult);
                return speedResult;
            }
            catch (Exception ex)
            {
                LogError($"Errore test velocità disco: {ex.Message}");
                return "Errore test velocità disco";
            }
        }


        #endregion
        private void UpdateLabel(string message)
        {
            if (label2.InvokeRequired)
            {
                label2.Invoke(new Action(() => label2.Text = message));
            }
            else
            {
                label2.Text = message;
            }

            LogMessage(message);
        }
        private void checkBox_hw_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Visible = true;
            label1.Visible = true;
        }

        private void UpdateProgress(int step, int totalSteps, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();

            int progress = (step * 100) / totalSteps;
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action(() => progressBar1.Value = progress));
            }
            else
            {
                progressBar1.Value = progress;
            }
        }

        private void LogMessage(string message)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action(() => LogMessage(message)));
            }
            else
            {
                richTextBox1.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\n");
                richTextBox1.ScrollToCaret();
            }
        }

        private void LogError(string message)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action(() => LogError(message)));
            }
            else
            {
                richTextBox1.AppendText($"{DateTime.Now}: [ERROR] {message}\n");
                richTextBox1.ScrollToCaret();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            button1.Visible = true;
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                LogMessage(LanguageManager.GetTranslation("FormRipristinoSO", "logScansioneInterrotta"));
                label2.Text = LanguageManager.GetTranslation("FormRipristinoSO", "scansioneInterrotta");
                label3.Text = LanguageManager.GetTranslation("FormRipristinoSO", "testAnnullato");
                countdownTimer?.Stop();
                progressBar1.Visible = false;
            }
            btnStop.Visible = false;
        }
    }
}

