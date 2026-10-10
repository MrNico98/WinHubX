using System.Diagnostics;
using System.ComponentModel;
using System.Globalization;
using System.Management;
using WinHubX.Forms.Base;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Settaggi
{
    public partial class FormRipristinoSO : Form
    {
        private static bool IsExpectedRepairOperationFailure(Exception exception) =>
            exception is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or System.ComponentModel.Win32Exception
                or System.Runtime.InteropServices.COMException
                or ManagementException
                or InvalidOperationException
                or NotSupportedException
                or ArgumentException
                or OperationCanceledException
                or TimeoutException;

        private long _cpuStressResultBits;
        private readonly Form1 form1;
        private readonly FormSettaggi formSettaggi;
        private ElevatedProcessBrokerClient? _repairBroker;
        private System.Windows.Forms.Timer? countdownTimer;
        private int remainingTime;
        private CancellationTokenSource? cancellationTokenSource;

        public FormRipristinoSO(FormSettaggi formSettaggi, Form1 form1)
        {
            LanguageManager.LoadTranslations();
            InitializeComponent();
            this.form1 = form1;
            this.formSettaggi = formSettaggi;
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            btn_CreaISOVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Avvia",
                "en" => "  Start",
                _ => btn_CreaISOVerdi.Content
            };
        }

        private async void buttonStart_Click(object? sender, EventArgs e)
        {
            if (cancellationTokenSource is not null)
                return;

            bool runSoftwareRepair = checkBox_sw.Checked;
            bool runHardwareTest = checkBox_hw.Checked;
            if (!runSoftwareRepair && !runHardwareTest)
            {
                MessageBox.Show(this, "Seleziona almeno un controllo da eseguire.", "WinHubX",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            progressBar1.Value = 0;
            using CancellationTokenSource scanCancellation = new();
            cancellationTokenSource = scanCancellation;
            CancellationToken token = scanCancellation.Token;
            btn_CreaISOVerdi.Enabled = false;
            label3.Visible = false;

            try
            {
                if (runSoftwareRepair)
                    await StartScanAsyncSW(token);

                if (runHardwareTest)
                {
                    label3.Visible = true;
                    int durationMinutes = (int)dateTimePicker1.Value.TimeOfDay.TotalMinutes;
                    if (durationMinutes <= 0)
                        throw new InvalidOperationException("La durata del test hardware deve essere maggiore di zero.");
                    await StartScanAsync(durationMinutes, token);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                UpdateLabel(LanguageManager.GetTranslation("FormRipristinoSO", "operazioneAnnullata"));
            }
            catch (Exception ex) when (IsExpectedRepairOperationFailure(ex))
            {
                LogMessage($"Errore durante il ripristino: {ex.Message}");
                _ = MessageBox.Show(this, ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                label3.Visible = false;
                countdownTimer?.Stop();
                if (ReferenceEquals(cancellationTokenSource, scanCancellation))
                    cancellationTokenSource = null;
                btn_CreaISOVerdi.Enabled = true;
            }
        }

        private async Task StartScanAsyncSW(CancellationToken token)
        {
            string? workspaceRoot = null;
            bool completed = false;
            string windowsDrive = Path.GetPathRoot(Environment.SystemDirectory)?.TrimEnd(Path.DirectorySeparatorChar)
                ?? throw new IOException("Impossibile determinare il volume di Windows.");

            var steps = new (string Label, string? Executable, string[]? Arguments)[]
            {
                ("backupRegistro", null, null),
                ("controlloFileSistema", "dism.exe", ["/Online", "/Cleanup-Image", "/CheckHealth"]),
                ("scansioneErroriSistema", "dism.exe", ["/Online", "/Cleanup-Image", "/ScanHealth"]),
                ("ripristinoFileSistema", "dism.exe", ["/Online", "/Cleanup-Image", "/RestoreHealth"]),
                ("esecuzioneSfc", "sfc.exe", ["/scannow"]),
                ("puliziaWinSxS", "dism.exe", ["/Online", "/Cleanup-Image", "/StartComponentCleanup"]),
                ("pianificazioneChkdsk", "chkdsk.exe", [windowsDrive, "/scan"])
            };

            try
            {
                _repairBroker = await ElevatedProcessBrokerClient.StartAsync(token);
                workspaceRoot = _repairBroker.WorkspaceRoot;
                string repairDirectory = Path.Join(workspaceRoot, "Repair");
                Directory.CreateDirectory(repairDirectory);

                int total = steps.Length + 1;
                int current = 0;
                UpdateLabel(LanguageManager.GetTranslation("FormRipristinoSO", steps[0].Label));
                await BackupRegistryAsync(repairDirectory, token);
                UpdateProgress(++current, total);
                foreach (var step in steps.Skip(1))
                {
                    UpdateLabel(LanguageManager.GetTranslation("FormRipristinoSO", step.Label));
                    if (step.Executable is not null && step.Arguments is not null)
                        await RunCommandAsync(step.Executable, step.Arguments, token);

                    UpdateProgress(++current, total);
                }
                UpdateLabel(LanguageManager.GetTranslation("FormRipristinoSO", "registrazioneDll"));
                await RegisterSystemDLLs(token);
                UpdateProgress(total, total);
                UpdateLabel(LanguageManager.GetTranslation("FormRipristinoSO", "ripristinoCompletato"));
                MessageBox.Show(
                    LanguageManager.GetTranslation("FormRipristinoSO", "msgRipristinoCompletato"),
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                completed = true;
            }
            finally
            {
                ElevatedProcessBrokerClient? broker = _repairBroker;
                _repairBroker = null;
                if (broker is not null)
                {
                    try
                    {
                        await broker.DisposeAsync();
                    }
                    catch (Exception ex) when (IsExpectedRepairOperationFailure(ex))
                    {
                        AppendSafe($"Chiusura del broker UAC non riuscita: {ex.Message}");
                    }
                }

                if (completed && workspaceRoot is not null && Directory.Exists(workspaceRoot))
                    Directory.Delete(workspaceRoot, recursive: true);
            }
        }

        private async Task BackupRegistryAsync(string repairDirectory, CancellationToken token)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (string.IsNullOrWhiteSpace(desktop))
                throw new IOException("Impossibile determinare la cartella Desktop per i backup del Registro.");
            string pathHKLM = Path.Join(repairDirectory, "RegistryBackup_HKLM.reg");
            string pathHKCU = Path.Join(repairDirectory, "RegistryBackup_HKCU.reg");

            await RunCommandAsync("reg.exe", ["export-hklm", pathHKLM], token);
            await RunUserCommandAsync("reg.exe", ["export", "HKCU", pathHKCU, "/y"], token);

            Directory.CreateDirectory(desktop);
            File.Copy(pathHKLM, Path.Join(desktop, Path.GetFileName(pathHKLM)), overwrite: true);
            File.Copy(pathHKCU, Path.Join(desktop, Path.GetFileName(pathHKCU)), overwrite: true);

            LogMessage($"Backup registro creato sul Desktop: RegistryBackup_HKLM.reg + RegistryBackup_HKCU.reg");
        }

        private async Task RegisterSystemDLLs(CancellationToken token)
        {
            string[] dlls = { "atl.dll", "jscript.dll", "msxml3.dll", "shell32.dll", "shdocvw.dll", "urlmon.dll", "vbscript.dll", "wintrust.dll" };

            foreach (string dll in dlls)
            {
                await RunCommandAsync("regsvr32.exe", ["regsvr32", "/s", dll], token);
                LogMessage($"Registrata DLL: {dll}");
            }
        }

        private async Task RunCommandAsync(string executable, IEnumerable<string> arguments, CancellationToken token)
        {
            ElevatedProcessBrokerClient broker = _repairBroker
                ?? throw new InvalidOperationException("Broker UAC per il ripristino non inizializzato.");
            string[] processArguments = arguments.ToArray();
            string utility = Path.GetFileNameWithoutExtension(executable).ToLowerInvariant();
            IReadOnlyList<string> utilityArguments = utility switch
            {
                "sfc" => ["sfc", .. processArguments],
                "chkdsk" => ["chkdsk", .. processArguments],
                "regsvr32" => processArguments,
                "reg" when processArguments.Length == 2 && processArguments[0] == "export-hklm"
                    => processArguments,
                _ => []
            };

            int exitCode = utility switch
            {
                "dism" => await broker.RunDismAsync(processArguments,
                    (line, isError) => AppendSafe(isError ? $"[ERRORE] {line}" : line), token),
                "sfc" or "chkdsk" or "regsvr32" or "reg" when utilityArguments.Count > 0
                    => await broker.RunSystemUtilityAsync(utilityArguments,
                        (line, isError) => AppendSafe(isError ? $"[ERRORE] {line}" : line), token),
                _ => throw new InvalidOperationException($"Comando elevato non previsto nel ripristino: {executable}.")
            };

            if (exitCode != 0)
                throw new InvalidOperationException($"{executable} è terminato con codice {exitCode}.");
        }

        private async Task RunUserCommandAsync(string executable, IReadOnlyList<string> arguments, CancellationToken token)
        {
            if (!Path.GetFileName(executable).Equals("reg.exe", StringComparison.OrdinalIgnoreCase)
                || arguments.Count != 4
                || !arguments[0].Equals("export", StringComparison.OrdinalIgnoreCase)
                || !arguments[1].Equals("HKCU", StringComparison.OrdinalIgnoreCase)
                || !arguments[3].Equals("/y", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("È consentito solo esportare l'hive HKCU senza elevazione.");

            var startInfo = new ProcessStartInfo
            {
                FileName = Path.Join(Environment.SystemDirectory, executable),
                WorkingDirectory = Environment.SystemDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            foreach (string argument in arguments)
                startInfo.ArgumentList.Add(argument);

            using Process process = Process.Start(startInfo)
                ?? throw new InvalidOperationException($"Impossibile avviare {executable}.");
            Task<string> outputTask = process.StandardOutput.ReadToEndAsync(token);
            Task<string> errorTask = process.StandardError.ReadToEndAsync(token);

            try
            {
                await process.WaitForExitAsync(token);
            }
            catch (OperationCanceledException)
            {
                try
                {
                    if (!process.HasExited)
                        process.Kill(entireProcessTree: true);
                }
                catch (Exception ex) when (ex is InvalidOperationException or Win32Exception)
                {
                    Debug.WriteLine($"Process cancellation failed: {ex.Message}");
                }

                await process.WaitForExitAsync(CancellationToken.None);
                try
                {
                    _ = await Task.WhenAll(outputTask, errorTask);
                }
                catch (Exception ex) when (token.IsCancellationRequested
                    && ex is OperationCanceledException or IOException)
                {
                    Debug.WriteLine($"Drenaggio output annullato insieme al processo: {ex.Message}");
                }

                throw;
            }

            string output = await outputTask;
            string error = await errorTask;
            string errorDetails = error?.Trim() ?? string.Empty;
            AppendSafe(output.TrimEnd());
            AppendSafe(string.IsNullOrWhiteSpace(error) ? string.Empty : $"[ERRORE] {error.TrimEnd()}");
            if (process.ExitCode != 0)
                throw new InvalidOperationException($"{executable} è terminato con codice {process.ExitCode}: {errorDetails}");
        }

        private void AppendSafe(string? text)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (richTextBox2.InvokeRequired)
                richTextBox2.BeginInvoke(new Action(() => richTextBox2.AppendText(text + Environment.NewLine)));
            else
                richTextBox2.AppendText(text + Environment.NewLine);
        }

        private void UpdateProgress(int step, int total)
        {
            int percent = (int)((step / (double)total) * 100);
            progressBar1.Value = Math.Min(100, percent);
        }

        private async Task StartScanAsync(int testDurationMinutes, CancellationToken token)
        {
            remainingTime = checked(testDurationMinutes * 60);
            progressBar1.Visible = true;
            labeltempo.Visible = true;
            labeltempo.Text = LanguageManager.FormatTranslation("FormRipristinoSO", "scansioneInCorsoConTempo", testDurationMinutes);
            richTextBox1.Clear();
            cancellationTokenSource?.CancelAfter(TimeSpan.FromMinutes(testDurationMinutes));

            using System.Windows.Forms.Timer scanTimer = new() { Interval = 1000 };
            countdownTimer = scanTimer;
            scanTimer.Tick += UpdateCountdown;
            try
            {
                scanTimer.Start();
                await RunStressTestsContinuously(token);
                token.ThrowIfCancellationRequested();
                labeltempo.Text = "Completato!";
            }
            finally
            {
                scanTimer.Stop();
                scanTimer.Tick -= UpdateCountdown;
                if (ReferenceEquals(countdownTimer, scanTimer))
                    countdownTimer = null;
                progressBar1.Visible = false;
            }
        }

        private async Task RunStressTestsContinuously(CancellationToken token)
        {
            await Task.Run(() => VerifyDiskStatus(token), token);
            token.ThrowIfCancellationRequested();

            using CancellationTokenSource testTasksSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            CancellationToken testToken = testTasksSource.Token;
            Task cpuTestTask = StressTestCPUAsync(testToken);
            Task ramTestTask = TestRAMAsync(testToken);
            Task allTests = Task.WhenAll(cpuTestTask, ramTestTask);
            Task firstTest = await Task.WhenAny(cpuTestTask, ramTestTask);
            if (firstTest.IsFaulted || firstTest.IsCanceled)
                testTasksSource.Cancel();

            await allTests;
            token.ThrowIfCancellationRequested();
        }

        private void UpdateCountdown(object? sender, EventArgs e)
        {
            if (remainingTime > 0)
            {
                remainingTime--;

                TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);
                string formattedTime = timeSpan.ToString(@"hh\:mm\:ss");

                labeltempo.Text = LanguageManager.GetTranslation("FormRipristinoSO", "scansioneInCorso");
                label3.Text = LanguageManager.FormatTranslation("FormRipristinoSO", "tempoRimanente", formattedTime);
            }
            else
            {
                countdownTimer?.Stop();
                labeltempo.Text = LanguageManager.GetTranslation("FormRipristinoSO", "scansioneCompletata");
                cancellationTokenSource?.Cancel();
            }
        }

        #region Hardware

        public async Task StressTestCPUAsync(CancellationToken token)
        {
            UpdateLabel("Avvio stress test CPU...");
            LogMessage("Preparazione stress test CPU...");
            Task? monitorTask = null;
            using CancellationTokenSource workerSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            CancellationToken workerToken = workerSource.Token;

            try
            {
                int numThreads = Environment.ProcessorCount;
                LogMessage($"Utilizzando {numThreads} thread per il test.");

                monitorTask = Task.Run(() => MonitorCPUUsageAsync(workerToken), workerToken);

                List<Task> tasks = new();
                for (int i = 0; i < numThreads; i++)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                        double result = 0;
                        while (!workerToken.IsCancellationRequested)
                        {
                            for (int j = 0; j < 10_000_000; j++)
                            {
                                result += Math.Sqrt(j) * Math.Sin(j);
                                if (j % 1_000_000 == 0 && workerToken.IsCancellationRequested)
                                    return;
                            }
                            Interlocked.Exchange(ref _cpuStressResultBits, BitConverter.DoubleToInt64Bits(result));
                            _ = Thread.Yield();
                        }
                    }, workerToken));
                }

                await Task.WhenAll(tasks);
                double checksum = BitConverter.Int64BitsToDouble(Interlocked.Read(ref _cpuStressResultBits));
                LogMessage($"Checksum test CPU: {checksum:R}");
                token.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (IsExpectedRepairOperationFailure(ex))
            {
                LogError($"Errore test CPU: {ex.Message}");
                throw;
            }
            finally
            {
                workerSource.Cancel();
                try
                {
                    if (monitorTask is not null)
                        await monitorTask;
                }
                catch (OperationCanceledException) when (workerSource.IsCancellationRequested)
                {
                    Debug.WriteLine("Monitoraggio CPU annullato durante il rilascio delle risorse del test.");
                }
            }
        }

        private async Task MonitorCPUUsageAsync(CancellationToken token)
        {
            const int thermalThreshold = 90;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var cpuUsage = await GetCPUUsageAsync(token);
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
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex) when (IsExpectedRepairOperationFailure(ex))
                {
                    LogError($"Errore monitoraggio CPU: {ex.Message}");
                }

                await Task.Delay(6000, token);
            }
        }
        private static async Task<float> GetCPUUsageAsync(CancellationToken token)
        {
            using PerformanceCounter cpuCounter = new("Processor", "% Processor Time", "_Total");
            _ = cpuCounter.NextValue();
            await Task.Delay(500, token);
            return cpuCounter.NextValue();
        }

        private static float GetCPUTemperature()
        {
            try
            {
                using ManagementObjectSearcher searcher = new("root\\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
                using ManagementObjectCollection zones = searcher.Get();
                using ManagementObject? zone = zones.Cast<ManagementObject>().FirstOrDefault();
                if (zone is null)
                    return -1;

                double tempK = Convert.ToDouble(zone["CurrentTemperature"]);
                return (float)((tempK - 2732) / 10.0);
            }
            catch (Exception ex) when (IsExpectedRepairOperationFailure(ex))
            {
                Debug.WriteLine($"Lettura temperatura CPU non disponibile: {ex}");
            }
            return -1;
        }
        public async Task TestRAMAsync(CancellationToken token)
        {
            UpdateLabel("Avvio test RAM...");
            LogMessage("Preparazione test RAM...");

            try
            {
                bool skippedForSafety = false;
                await Task.Run(() =>
                {
                    const int blockSize = 50 * 1024 * 1024;
                    long totalRam = GetTotalRAM();
                    long availableRam = GetAvailableRAM();
                    long maxRam = Math.Min(totalRam * 80 / 100, Math.Min(availableRam / 4, 2L * 1024 * 1024 * 1024));
                    if (maxRam <= 0)
                    {
                        skippedForSafety = true;
                        LogMessage("Test RAM saltato: memoria disponibile non determinabile in sicurezza.");
                        return;
                    }

                    LogMessage($"Allocazione controllata fino a {maxRam / (1024 * 1024)} MB di RAM...");
                    Queue<byte[]> memoryBlocks = new();
                    long allocatedRam = 0;
                    try
                    {
                        while (allocatedRam < maxRam)
                        {
                            token.ThrowIfCancellationRequested();
                            int nextBlockSize = (int)Math.Min(blockSize, maxRam - allocatedRam);
                            byte[] block = GC.AllocateUninitializedArray<byte>(nextBlockSize);
                            for (int i = 0; i < block.Length; i += 4096)
                                block[i] = (byte)(i % 256);

                            memoryBlocks.Enqueue(block);
                            allocatedRam += nextBlockSize;
                            if (allocatedRam % (500L * 1024 * 1024) < nextBlockSize)
                                LogMessage($"RAM allocata: {allocatedRam / (1024 * 1024)} MB...");
                            Thread.Sleep(20);
                        }
                    }
                    finally
                    {
                        LogMessage("Rilascio dei riferimenti alla memoria di test...");
                        memoryBlocks.Clear();
                    }
                }, token);
                token.ThrowIfCancellationRequested();
                UpdateLabel(skippedForSafety ? "Test RAM saltato per sicurezza." : "Test RAM completato.");
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (IsExpectedRepairOperationFailure(ex))
            {
                LogError($"Errore test RAM: {ex.Message}");
                throw;
            }
        }
        private static long GetTotalRAM()
        {
            try
            {
                using ManagementObjectSearcher searcher = new("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                using ManagementObjectCollection systems = searcher.Get();
                using ManagementObject? system = systems.Cast<ManagementObject>().FirstOrDefault();
                return system is null ? 0 : Convert.ToInt64(system["TotalPhysicalMemory"]);
            }
            catch (Exception ex) when (IsExpectedRepairOperationFailure(ex))
            {
                Debug.WriteLine($"Errore lettura RAM: {ex.Message}");
            }

            return 0;
        }

        private static long GetAvailableRAM()
        {
            try
            {
                using ManagementObjectSearcher searcher = new("SELECT FreePhysicalMemory FROM Win32_OperatingSystem");
                using ManagementObjectCollection operatingSystems = searcher.Get();
                using ManagementObject? operatingSystem = operatingSystems.Cast<ManagementObject>().FirstOrDefault();
                return operatingSystem is null
                    ? 0
                    : checked(Convert.ToInt64(operatingSystem["FreePhysicalMemory"]) * 1024);
            }
            catch (Exception ex) when (IsExpectedRepairOperationFailure(ex))
            {
                Debug.WriteLine($"Lettura della RAM disponibile non riuscita: {ex}");
            }

            return 0;
        }
        private void VerifyDiskStatus(CancellationToken token)
        {
            UpdateLabel("Verifica stato del disco...");
            LogMessage("Inizio controllo avanzato del disco...");

            try
            {
                using ManagementObjectSearcher searcher = new("SELECT DeviceID, Model, Size, Signature, Status FROM Win32_DiskDrive");
                using ManagementObjectCollection disks = searcher.Get();
                foreach (ManagementObject disk in disks)
                {
                    using (disk)
                    {
                    token.ThrowIfCancellationRequested();
                    string deviceId = disk["DeviceID"]?.ToString() ?? "Sconosciuto";
                    string model = disk["Model"]?.ToString() ?? "Modello sconosciuto";
                    long diskSize = Convert.ToInt64(disk["Size"] ?? 0) / (1024 * 1024 * 1024);

                    string diskInfo = $"Disco rilevato: {model} ({deviceId}) - {diskSize} GB";
                    LogMessage(diskInfo);
                    UpdateLabel($"Analisi {model}...");

                    AppendToRichTextBox2(diskInfo + Environment.NewLine);
                    string? signature = disk["Signature"]?.ToString();
                    if (string.IsNullOrEmpty(signature))
                    {
                        AppendToRichTextBox2($"Stato WMI: {disk["Status"]?.ToString() ?? "Non disponibile"}");
                        AppendToRichTextBox2(Environment.NewLine);
                    }
                    LogMessage("-------------------------------------------------");
                    AppendToRichTextBox2("-------------------------------------------------" + Environment.NewLine);
                    }
                }

                token.ThrowIfCancellationRequested();
                string speedResults = DiskSpeedTest(token);
                AppendToRichTextBox2(speedResults + Environment.NewLine);

                UpdateLabel("Verifica disco completata.");
                LogMessage("Verifica disco completata con successo.");
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (IsExpectedRepairOperationFailure(ex))
            {
                LogError($"Errore verifica disco: {ex.Message}");
                throw;
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
        private string DiskSpeedTest(CancellationToken token)
        {
            string tempFile = Path.Join(Path.GetTempPath(), $"WinHubX-disk-test-{Guid.NewGuid():N}.tmp");
            try
            {
                const int testSizeMb = 50;
                const int bufferSize = 1024 * 1024;
                byte[] buffer = GC.AllocateUninitializedArray<byte>(bufferSize);
                Random.Shared.NextBytes(buffer);
                Stopwatch stopwatch = Stopwatch.StartNew();
                using (FileStream output = new(tempFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize, FileOptions.SequentialScan))
                {
                    for (int writtenMb = 0; writtenMb < testSizeMb; writtenMb++)
                    {
                        token.ThrowIfCancellationRequested();
                        output.Write(buffer, 0, buffer.Length);
                    }
                    output.Flush(flushToDisk: true);
                }
                stopwatch.Stop();
                double writeSpeed = testSizeMb / Math.Max(stopwatch.Elapsed.TotalSeconds, double.Epsilon);
                stopwatch.Restart();
                using (FileStream input = new(tempFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.SequentialScan))
                {
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();
                        if (input.Read(buffer, 0, buffer.Length) == 0)
                            break;
                    }
                }
                stopwatch.Stop();
                double readSpeed = testSizeMb / Math.Max(stopwatch.Elapsed.TotalSeconds, double.Epsilon);

                string volumeRoot = Path.GetPathRoot(Path.GetTempPath()) ?? "volume temporaneo";
                string speedResult = $"Velocità volume temporaneo ({volumeRoot}): scrittura {writeSpeed:F2} MB/s | lettura {readSpeed:F2} MB/s";
                LogMessage(speedResult);
                return speedResult;
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (IsExpectedRepairOperationFailure(ex))
            {
                LogError($"Errore test velocità disco: {ex.Message}");
                return "Errore test velocità disco";
            }
            finally
            {
                try
                {
                    if (File.Exists(tempFile))
                        File.Delete(tempFile);
                }
                catch (IOException ex) { Debug.WriteLine($"Impossibile eliminare il file temporaneo del benchmark disco: {ex}"); }
                catch (UnauthorizedAccessException ex) { Debug.WriteLine($"Accesso negato durante la rimozione del benchmark disco: {ex}"); }
            }
        }


        #endregion
        private void UpdateLabel(string message)
        {
            if (labeltempo.InvokeRequired)
            {
                labeltempo.Invoke(new Action(() => labeltempo.Text = message));
            }
            else
            {
                labeltempo.Text = message;
            }

            LogMessage(message);
        }
        private void checkBox_hw_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Visible = true;
            labeltempo.Visible = true;
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
    }
}

