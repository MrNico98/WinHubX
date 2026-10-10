using HartUI.Controls;
using LibreHardwareMonitor.Hardware;
using System.Diagnostics;
using System.Net.NetworkInformation;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Base
{
    public partial class FormMonitoraggio : Form
    {
        private static bool IsExpectedMonitoringFailure(Exception exception) =>
            exception is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or System.ComponentModel.Win32Exception
                or System.Runtime.InteropServices.COMException
                or System.Management.ManagementException
                or InvalidOperationException
                or NotSupportedException
                or ArgumentException
                or TimeoutException;

        private sealed record HardwareSnapshot(string CpuTemperature, string GpuTemperature, double GpuUsage)
        {
            public static HardwareSnapshot Empty { get; } = new("N/A", "N/A", 0);
        }

        #region Constants and Fields
        private const string RegistryKey = @"Software\WinHubX-Monitor";
        private const string RegistryValueMonitoraggio = "IsMonitoringOn";
        private const string RegistryValueTemperature = "isTemperatureOn";
        private static readonly TimeSpan TempDirectoryRefreshInterval = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan NetworkInterfaceRefreshInterval = TimeSpan.FromMinutes(1);
        private readonly string monitoraggioPath =
    Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "WinHubX", "Impostazioni", "Monitoraggio.json");

        private NetworkInterface[] networkInterfaces = Array.Empty<NetworkInterface>();
        private string[] networkInterfaceIds = Array.Empty<string>();
        private readonly object _hardwareSync = new();
        private readonly SemaphoreSlim _monitorSettingsWriteLock = new(1, 1);
        private HardwareSnapshot _latestHardwareSnapshot = HardwareSnapshot.Empty;
        private long lastUpdateTimestamp;
        private long lastBytesSent;
        private long lastBytesReceived;
        private double networkCapacityKB;
        private long _lastNetworkInterfaceRefreshTimestamp;
        private int _networkTopologyChanged;
        private int _networkChangeEventsSubscribed;


        private readonly Form1 _mainForm;
        private Computer _computer = new();
        private System.Windows.Forms.Timer? _ramMonitorTimer;
        private PerformanceCounter? _cpuCounter;
        private PerformanceCounter? _diskUsageCounter;
        private Task? _ramCleanupTask;
        private Task? _manualRamCleanupTask;
        private Task? _manualCpuCleanupTask;
        private readonly CancellationTokenSource _monitoringCancellation = new();
        private readonly CancellationToken _monitoringToken;
        private Task[] _monitoringTasks = Array.Empty<Task>();
        private Task? _initializationTask;
        private bool _monitoringStarted;
        private bool _shutdownRequested;
        private bool _closeAfterMonitoringStops;
        private Task? _cleanupTask;
        private int _ramCleanupRunning;
        private DateTime _lastAutomaticRamCleanupUtc = DateTime.MinValue;

        private NotifyIcon? _notifyIcon;
        #endregion

        #region Constructor
        public FormMonitoraggio(Form1 mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            _monitoringToken = _monitoringCancellation.Token;

            // Riduce tearing e ridisegni parziali durante gli aggiornamenti dei monitor.
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            UpdateStyles();

            this.Shown += FormMonitoraggio_Shown;
        }

        private async void FormMonitoraggio_Shown(object? sender, EventArgs e)
        {
            if (_monitoringStarted)
                return;

            _monitoringStarted = true;
            Cursor = Cursors.WaitCursor;
            try
            {
                await Task.Delay(50, _monitoringToken);
                if (_shutdownRequested || IsDisposed || !IsHandleCreated)
                {
                    return;
                }

                LanguageManager.LoadLanguageFromSettings();

                btnPulisciCPU.Content = LanguageManager.CurrentLanguage == "it" ? "  Pulizia" : "  Clean";
                btnPulisciRam.Content = LanguageManager.CurrentLanguage == "it" ? "  Pulizia" : "  Clean";
                btnSvuotaTemp.Content = LanguageManager.CurrentLanguage == "it" ? "  Svuota" : "  Empty";

                _initializationTask = Task.Run(InitializeComputer, _monitoringToken);
                await _initializationTask;
                if (_monitoringToken.IsCancellationRequested || IsDisposed || !IsHandleCreated)
                {
                    return;
                }

                InitializePerformanceCounter();
                InitializeNotificationIcon();
                ApplyTheme();
                StartRamMonitoring();
                _monitoringTasks =
                [
                    StartCpuMonitoring(),
                    StartReteMonitoring(),
                    StartHardwareMonitoring(),
                    StartDiscoMonitoring(),
                    StartTEMPMonitoring()
                ];
                await LoadMonitoraggioSettingsAsync();
            }
            catch (OperationCanceledException) when (_monitoringToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                Debug.WriteLine($"Avvio monitoraggio non riuscito: {ex}");
                ShowErrorMessage($"Impossibile avviare il monitoraggio:\n{ex.Message}");
                Close();
            }
            finally
            {
                if (!IsDisposed)
                {
                    Cursor = Cursors.Default;
                }
            }
        }
        #endregion

        #region Initialization Methods
        private void InitializeComputer()
        {
            lock (_hardwareSync)
            {
                _computer = new Computer
                {
                    IsCpuEnabled = true,
                    IsGpuEnabled = true,
                    IsStorageEnabled = false,
                    IsMotherboardEnabled = false,
                    IsControllerEnabled = false
                };
                _computer.Open();
            }
        }

        private void InitializePerformanceCounter()
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        private void InitializeNotificationIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Visible = false,
                Icon = SystemIcons.Warning,
                BalloonTipTitle = "Cartella TEMP"
            };
        }

        private async Task LoadMonitoraggioSettingsAsync()
        {
            try
            {
                if (!File.Exists(monitoraggioPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(monitoraggioPath)
                        ?? throw new InvalidOperationException("Percorso monitoraggio non valido."));
                    await File.WriteAllTextAsync(
                        monitoraggioPath,
                        "{ \"LimiteGB\": 2, \"ShowFahrenheitcpu\": false, \"ShowFahrenheitgpu\": false }",
                        _monitoringToken);
                }

                string json = await File.ReadAllTextAsync(monitoraggioPath, _monitoringToken);
                var obj = System.Text.Json.JsonSerializer.Deserialize<MonitoraggioConfig>(json)
                    ?? new MonitoraggioConfig();
                domainUpDown1.Text = $"{obj.LimiteGB} GB";
                MonitorSettings.ShowFahrenheitcpu = obj.ShowFahrenheitcpu;
                cuiSwitch_gradicpu.Checked = obj.ShowFahrenheitcpu;

                MonitorSettings.ShowFahrenheitgpu = obj.ShowFahrenheitgpu;
                cuiSwitch_gputemperatura.Checked = obj.ShowFahrenheitgpu;
            }
            catch (OperationCanceledException) when (_monitoringToken.IsCancellationRequested)
            {
                Debug.WriteLine("Caricamento impostazioni monitoraggio annullato durante la chiusura.");
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                Debug.WriteLine($"Lettura impostazioni monitoraggio non riuscita: {ex}");
                domainUpDown1.Text = "2 GB";
                MonitorSettings.ShowFahrenheitcpu = false;
                cuiSwitch_gradicpu.Checked = false;
                MonitorSettings.ShowFahrenheitgpu = false;
                cuiSwitch_gputemperatura.Checked = false;
            }
        }

        #endregion

        #region Temperature Monitoring
        private void UpdateTemperatureDisplays()
        {
            string cpuTemperature = _latestHardwareSnapshot.CpuTemperature;
            string gpuTemperature = _latestHardwareSnapshot.GpuTemperature;
            var displayCpuTemp = ConvertTemperature(cpuTemperature, MonitorSettings.ShowFahrenheitcpu);
            var displayGpuTemp = ConvertTemperature(gpuTemperature, MonitorSettings.ShowFahrenheitgpu);

            labelCpuTemp.Text = $"{displayCpuTemp}°";
            labelGpuTemp.Text = $"{displayGpuTemp}°";

            UpdateTemperatureImage(pic_termcpu, cpuTemperature, UpdateCpuTemperatureImage);
            UpdateTemperatureImage(pic_termgpu, gpuTemperature, UpdateGpuTemperatureImage);
        }

        private string? GetGpuTemperature()
        {
            return GetTemperature(HardwareType.GpuNvidia)?.ToString("0")
                   ?? GetTemperature(HardwareType.GpuAmd)?.ToString("0")
                   ?? GetTemperature(HardwareType.GpuIntel)?.ToString("0");
        }

        private float? GetTemperature(HardwareType hardwareType)
        {
            foreach (var hardware in _computer.Hardware)
            {
                if (hardware.HardwareType == hardwareType)
                {
                    IEnumerable<float?> readings = hardware.Sensors
                        .Where(sensor => sensor.SensorType == SensorType.Temperature)
                        .OrderBy(sensor => sensor.IsDefaultHidden)
                        .Select(sensor => sensor.Value);
                    return HardwareSensorReading.SelectTemperature(readings);
                }
            }
            return null;
        }

        private void UpdateTemperatureImage(PictureBox pictureBox, string temperatureStr,
            Action<string> specificUpdateMethod)
        {
            if (temperatureStr == "N/A")
                return;

            specificUpdateMethod(temperatureStr);
        }

        private void UpdateCpuTemperatureImage(string cpuTempStr)
        {
            if (float.TryParse(cpuTempStr, out float cpuTemp))
            {
                var image = GetTemperatureImage(cpuTemp);
                SetImageSafely(pic_termcpu, image);
            }
        }

        private void UpdateGpuTemperatureImage(string gpuTempStr)
        {
            if (float.TryParse(gpuTempStr, out float gpuTemp))
            {
                var image = GetTemperatureImage(gpuTemp);
                SetImageSafely(pic_termgpu, image);
            }
        }

        private Image GetTemperatureImage(float temperature)
        {
            if (temperature >= 80)
                return Properties.Resources.term_rosso;
            else if (temperature >= 65)
                return Properties.Resources.term_giallo;
            else
                return Properties.Resources.term_verde;
        }

        private void SetImageSafely(PictureBox pictureBox, Image image)
        {
            if (image != null)
            {
                pictureBox.Image = image;
            }
            else
            {
                ShowErrorMessage("Immagine non trovata nelle risorse.");
            }
        }
        #endregion

        #region RAM Monitoring and Management
        private void StartRamMonitoring()
        {
            _ramMonitorTimer = new System.Windows.Forms.Timer { Interval = 3000 };
            _ramMonitorTimer.Tick += RamMonitorTimer_Tick;
            _ramMonitorTimer.Start();
        }

        private async void RamMonitorTimer_Tick(object? sender, EventArgs e)
        {
            (ulong totalPhysicalMemory, ulong availablePhysicalMemory) = GetMemoryStatus();
            if (totalPhysicalMemory == 0)
            {
                Debug.WriteLine("Impossibile leggere la memoria fisica totale.");
                return;
            }

            double ramUsagePercentage = ((double)(totalPhysicalMemory - availablePhysicalMemory) / totalPhysicalMemory) * 100;
            BarRAM.ProgressValue = Math.Min((int)ramUsagePercentage, 100);
            BarRAMtext.Text = $"{ramUsagePercentage:0}%";

            if (!MonitorSettings.PuliziaAutomaticaRAM ||
                ramUsagePercentage <= (double)MonitorSettings.LimiteRAM ||
                DateTime.UtcNow - _lastAutomaticRamCleanupUtc < TimeSpan.FromSeconds(30) ||
                Interlocked.Exchange(ref _ramCleanupRunning, 1) != 0)
            {
                return;
            }

            _lastAutomaticRamCleanupUtc = DateTime.UtcNow;
            try
            {
                _ramCleanupTask = Task.Run(() =>
                {
                    CollectManagedMemory();
                    CpuReduce();
                }, _monitoringToken);
                await _ramCleanupTask;
            }
            catch (OperationCanceledException) when (_monitoringCancellation.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                Debug.WriteLine($"Pulizia automatica della memoria non riuscita: {ex}");
            }
            finally
            {
                _ramCleanupTask = null;
                Interlocked.Exchange(ref _ramCleanupRunning, 0);
            }
        }

        private static void CollectManagedMemory()
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized, blocking: false, compacting: false);
        }
        #endregion

        #region CPU Monitoring and Management
        private async Task StartTEMPMonitoring()
        {
            string tempPath = Path.GetTempPath();

            try
            {
                while (!_monitoringCancellation.IsCancellationRequested)
                {
                    try
                    {
                        // La scansione ricorsiva può attraversare migliaia di file: mai eseguirla sul thread UI.
                        long totalBytes = await Task.Run(
                            () => TempDirectorySizeCalculator.Calculate(tempPath, _monitoringToken),
                            _monitoringToken);
                        if (_monitoringCancellation.IsCancellationRequested || IsDisposed || !IsHandleCreated)
                        {
                            return;
                        }

                        double usedGB = Math.Round(totalBytes / 1024.0 / 1024.0 / 1024.0, 2);
                        double limitGB = GetSelectedGB();

                        BarTEMPtext.Text = $"{usedGB}GB";

                        int percent = (int)Math.Min((usedGB / limitGB) * 100, 100);
                        BarTEMP.ProgressValue = percent;
                        BarTEMP.ProgressColor = usedGB <= limitGB ? Color.Green : Color.Red;
                    }
                    catch (OperationCanceledException) when (_monitoringCancellation.IsCancellationRequested)
                    {
                        return;
                    }
                    catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
                    {
                        Debug.WriteLine($"Lettura cartella temporanea non riuscita: {ex}");
                    }

                    // La dimensione TEMP richiede una scansione ricorsiva; aggiornarla ogni minuto evita
                    // di saturare un core mentre la scheda è aperta senza rallentare gli altri indicatori.
                    await Task.Delay(TempDirectoryRefreshInterval, _monitoringToken);
                }
            }
            catch (OperationCanceledException) when (_monitoringCancellation.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                Debug.WriteLine($"Monitoraggio cartella temporanea terminato con errore: {ex}");
            }
        }
        private int GetSelectedGB()
        {
            string text = domainUpDown1.Text.Replace(" GB", "");
            if (int.TryParse(text, out int value))
                return value;

            return 2; 
        }

        private async Task StartDiscoMonitoring()
        {
            try
            {
                while (!_monitoringCancellation.IsCancellationRequested)
                {
                    try
                    {
                        double discoUsage = await GetDiscoUsagePercentageAsync();
                        if (IsDisposed || !IsHandleCreated)
                        {
                            return;
                        }

                        UpdateDiscoUI(discoUsage);
                        await Task.Delay(3000, _monitoringToken);
                    }
                    catch (OperationCanceledException) when (_monitoringCancellation.IsCancellationRequested)
                    {
                        return;
                    }
                    catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
                    {
                        Debug.WriteLine($"Lettura utilizzo disco non riuscita: {ex}");
                        await Task.Delay(3000, _monitoringToken);
                    }
                }
            }
            catch (OperationCanceledException) when (_monitoringCancellation.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                Debug.WriteLine($"Monitoraggio disco terminato con errore: {ex}");
            }
        }
        private async Task<double> GetDiscoUsagePercentageAsync()
        {
            try
            {
                _diskUsageCounter ??= new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
                return await SampleDiskCounterAsync(_diskUsageCounter);
            }
            catch (OperationCanceledException) when (_monitoringCancellation.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                Debug.WriteLine($"Contatore PhysicalDisk non disponibile: {ex.Message}");
                _diskUsageCounter?.Dispose();
                _diskUsageCounter = null;

                try
                {
                    _diskUsageCounter = new PerformanceCounter("LogicalDisk", "% Disk Time", "_Total");
                    return await SampleDiskCounterAsync(_diskUsageCounter);
                }
                catch (OperationCanceledException) when (_monitoringCancellation.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception fallbackException) when (IsExpectedMonitoringFailure(fallbackException))
                {
                    Debug.WriteLine($"Contatore LogicalDisk non disponibile: {fallbackException.Message}");
                    _diskUsageCounter?.Dispose();
                    _diskUsageCounter = null;
                    return 0;
                }
            }
        }

        private async Task<double> SampleDiskCounterAsync(PerformanceCounter counter)
        {
            _ = counter.NextValue();
            await Task.Delay(1000, _monitoringToken);
            return Math.Clamp(counter.NextValue(), 0, 100);
        }

        private void UpdateDiscoUI(double discoUsage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => UpdateDiscoUI(discoUsage));
                return;
            }

            try
            {
                if (BarDISCO != null && !BarDISCO.IsDisposed)
                {
                    int usageValue = (int)Math.Round(Math.Max(0, Math.Min(100, discoUsage)));
                    BarDISCO.ProgressValue = usageValue;
                }

                if (BarDISCOtext != null && !BarDISCOtext.IsDisposed)
                {
                    BarDISCOtext.Text = $"{discoUsage:0}%";
                }
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                Debug.WriteLine($"Aggiornamento UI utilizzo disco non riuscito: {ex}");
            }
        }

        private async Task StartCpuMonitoring()
        {
            try
            {
                while (!_monitoringCancellation.IsCancellationRequested)
                {
                    double cpuUsagePercentage = await GetCpuUsagePercentageAsync();
                    if (_monitoringCancellation.IsCancellationRequested || IsDisposed || !IsHandleCreated)
                    {
                        return;
                    }

                    BarCPU.ProgressValue = (int)cpuUsagePercentage;
                    BarCPUtext.Text = $"{cpuUsagePercentage:0}%";
                    if (MonitorSettings.PuliziaAutomaticaCPU && cpuUsagePercentage > (double)MonitorSettings.LimiteCPU)
                    {
                        await Task.Run(CpuReduce, _monitoringToken);
                    }

                    await Task.Delay(2000, _monitoringToken);
                }
            }
            catch (OperationCanceledException) when (_monitoringCancellation.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                Debug.WriteLine($"Monitoraggio CPU terminato con errore: {ex}");
            }
        }

        private async Task StartHardwareMonitoring()
        {
            try
            {
                while (!_monitoringCancellation.IsCancellationRequested)
                {
                    try
                    {
                        HardwareSnapshot snapshot = await Task.Run(PollHardware, _monitoringToken);
                        if (IsDisposed || !IsHandleCreated)
                        {
                            return;
                        }

                        _latestHardwareSnapshot = snapshot;
                        UpdateTemperatureDisplays();
                        UpdateGpuUI(snapshot.GpuUsage);
                    }
                    catch (OperationCanceledException) when (_monitoringCancellation.IsCancellationRequested)
                    {
                        return;
                    }
                    catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
                    {
                        Debug.WriteLine($"Lettura sensori hardware non riuscita: {ex}");
                    }

                    await Task.Delay(1000, _monitoringToken);
                }
            }
            catch (OperationCanceledException) when (_monitoringCancellation.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                Debug.WriteLine($"Lettura sensori hardware non riuscita: {ex}");
            }
        }

        private HardwareSnapshot PollHardware()
        {
            lock (_hardwareSync)
            {
                foreach (IHardware hardware in _computer.Hardware.Where(static hardware =>
                    hardware.HardwareType is HardwareType.Cpu or HardwareType.GpuNvidia or HardwareType.GpuAmd or HardwareType.GpuIntel))
                {
                    hardware.Update();
                }

                string cpuTemperature = GetTemperature(HardwareType.Cpu)?.ToString("0") ?? "N/A";
                string gpuTemperature = GetGpuTemperature() ?? "N/A";
                double gpuUsage = GetGpuLoadPercentage() ?? 0;
                return new HardwareSnapshot(cpuTemperature, gpuTemperature, gpuUsage);
            }
        }

        private double? GetGpuLoadPercentage()
        {
            return _computer.Hardware
                .Where(static hardware => hardware.HardwareType is HardwareType.GpuNvidia or HardwareType.GpuAmd or HardwareType.GpuIntel)
                .Select(GetHardwareGpuLoadPercentage)
                .FirstOrDefault(static usage => usage.HasValue);
        }

        private static double? GetHardwareGpuLoadPercentage(IHardware hardware)
        {
            IEnumerable<float?> readings = hardware.Sensors
                .Where(static sensor => sensor.SensorType == SensorType.Load &&
                    (sensor.Name.Contains("Core", StringComparison.Ordinal) ||
                     sensor.Name.Contains("GPU Core", StringComparison.Ordinal) ||
                     sensor.Name.Contains("D3D 3D", StringComparison.Ordinal) ||
                     sensor.Name.Contains("Utilization", StringComparison.Ordinal)))
                .Select(static sensor => sensor.Value);

            return HardwareSensorReading.SelectPercentage(readings);
        }

        private void UpdateGpuUI(double gpuUsage)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateGpuUI(gpuUsage)));
                return;
            }

            double normalizedUsage = double.IsFinite(gpuUsage) ? Math.Clamp(gpuUsage, 0d, 100d) : 0d;
            BarGPU.ProgressValue = (int)Math.Round(normalizedUsage);
            BarGPUtext.Text = $"{normalizedUsage:0}%";
        }
        private async Task StartReteMonitoring()
        {
            try
            {
                SubscribeToNetworkChanges();
                await Task.Run(RefreshNetworkInterfaces, _monitoringToken);

                if (networkInterfaces.Length == 0)
                {
                    labelReteUtilizzo.Text = "Nessuna interfaccia attiva";
                    labelVelocitaRete.Text = "0 KB/s";
                }
                lastUpdateTimestamp = Stopwatch.GetTimestamp();
                lastBytesSent = networkInterfaces.Sum(n => n.GetIPv4Statistics().BytesSent);
                lastBytesReceived = networkInterfaces.Sum(n => n.GetIPv4Statistics().BytesReceived);
                while (!_monitoringCancellation.IsCancellationRequested)
                {
                    await Task.Delay(1000, _monitoringToken);

                    try
                    {
                        await UpdateNetworkStats();
                    }
                    catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
                    {
                        Debug.WriteLine($"Lettura statistiche di rete non riuscita: {ex}");
                    }
                }
            }
            catch (OperationCanceledException) when (_monitoringCancellation.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                Debug.WriteLine($"Monitoraggio rete terminato con errore: {ex}");
            }
        }

        private async Task UpdateNetworkStats()
        {
            string[] previousInterfaceIds = networkInterfaceIds;
            long refreshCheckTimestamp = Stopwatch.GetTimestamp();
            bool networkChangeObserved = Interlocked.Exchange(ref _networkTopologyChanged, 0) != 0;
            if (NetworkInterfaceRefreshPolicy.ShouldRefresh(
                    networkChangeObserved,
                    _lastNetworkInterfaceRefreshTimestamp,
                    refreshCheckTimestamp,
                    NetworkInterfaceRefreshInterval))
            {
                await Task.Run(RefreshNetworkInterfaces, _monitoringToken);
            }

            long currentBytesSent = 0;
            long currentBytesReceived = 0;

            foreach (var stats in networkInterfaces.Select(static netInterface => netInterface.GetIPv4Statistics()))
            {
                currentBytesSent += stats.BytesSent;
                currentBytesReceived += stats.BytesReceived;
            }

            long currentTimestamp = Stopwatch.GetTimestamp();
            bool topologyChanged = !previousInterfaceIds.SequenceEqual(networkInterfaceIds, StringComparer.Ordinal);
            bool countersReset = currentBytesSent < lastBytesSent || currentBytesReceived < lastBytesReceived;
            if (topologyChanged || countersReset)
            {
                lastBytesSent = currentBytesSent;
                lastBytesReceived = currentBytesReceived;
                lastUpdateTimestamp = currentTimestamp;
                await UpdateUI(0, 0, 0, 0);
                return;
            }

            double timeDiff = Stopwatch.GetElapsedTime(lastUpdateTimestamp, currentTimestamp).TotalSeconds;
            if (timeDiff <= 0)
            {
                return;
            }

            double sentKB = (currentBytesSent - lastBytesSent) / timeDiff / 1024d;
            double receivedKB = (currentBytesReceived - lastBytesReceived) / timeDiff / 1024d;
            double totalSpeedKB = sentKB + receivedKB;
            double networkUsage = CalculateNetworkUsage(totalSpeedKB);
            await UpdateUI(sentKB, receivedKB, totalSpeedKB, networkUsage);
            lastBytesSent = currentBytesSent;
            lastBytesReceived = currentBytesReceived;
            lastUpdateTimestamp = currentTimestamp;
        }

        private void RefreshNetworkInterfaces()
        {
            networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up &&
                            n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .ToArray();
            networkInterfaceIds = networkInterfaces
                .Select(n => n.Id)
                .OrderBy(id => id, StringComparer.Ordinal)
                .ToArray();
            // NetworkInterface.Speed è espresso in bit/s; convertiamo la capacità aggregata in KB/s.
            networkCapacityKB = NetworkUsageCalculator.CalculateCapacityKilobytesPerSecond(
                networkInterfaces.Select(n => n.Speed));
            _lastNetworkInterfaceRefreshTimestamp = Stopwatch.GetTimestamp();
        }

        private void SubscribeToNetworkChanges()
        {
            if (Interlocked.CompareExchange(ref _networkChangeEventsSubscribed, 1, 0) != 0)
            {
                return;
            }

            NetworkChange.NetworkAddressChanged += NetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged += NetworkAvailabilityChanged;
        }

        private void UnsubscribeFromNetworkChanges()
        {
            if (Interlocked.Exchange(ref _networkChangeEventsSubscribed, 0) == 0)
            {
                return;
            }

            NetworkChange.NetworkAddressChanged -= NetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged -= NetworkAvailabilityChanged;
        }

        private void NetworkAddressChanged(object? sender, EventArgs e)
        {
            Interlocked.Exchange(ref _networkTopologyChanged, 1);
        }

        private void NetworkAvailabilityChanged(object? sender, NetworkAvailabilityEventArgs e)
        {
            Interlocked.Exchange(ref _networkTopologyChanged, 1);
        }

        private double CalculateNetworkUsage(double currentSpeedKB)
        {
            return NetworkUsageCalculator.CalculateUsagePercentage(currentSpeedKB, networkCapacityKB);
        }

        private async Task UpdateUI(double sentKB, double receivedKB, double totalSpeedKB, double networkUsage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => _ = UpdateUI(sentKB, receivedKB, totalSpeedKB, networkUsage)));
                return;
            }

            try
            {
                lblUpload.Text = "        " + LanguageManager.FormatTranslation("FormMonitoraggio", "upload", sentKB.ToString("0.00"));

                lblDonwload.Text = "        " + LanguageManager.FormatTranslation("FormMonitoraggio", "download", receivedKB.ToString("0.00"));

                labelVelocitaRete.Text = "        " + LanguageManager.FormatTranslation("FormMonitoraggio", "velocita", totalSpeedKB.ToString("0.00"));

                labelReteUtilizzo.Text = networkInterfaces.Length == 0
                    ? "Nessuna interfaccia attiva"
                    : networkCapacityKB > 0 ? $"{networkUsage:0.0}%" : "—";
                progressbarRete.ProgressValue = (int)Math.Round(networkUsage);
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                Debug.WriteLine($"Aggiornamento UI statistiche di rete non riuscito: {ex}");
            }
        }


        private async Task<double> GetCpuUsagePercentageAsync()
        {
            PerformanceCounter cpuCounter = _cpuCounter
                ?? throw new InvalidOperationException("Contatore CPU non inizializzato.");
            _ = cpuCounter.NextValue();
            await Task.Delay(1000, _monitoringToken);
            return cpuCounter.NextValue();
        }

        private void CpuReduce()
        {
            var processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                using (process)
                {
                    try
                    {
                        ManageProcess(process);
                    }
                    catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
                    {
                        Debug.WriteLine($"Ottimizzazione del processo {process.Id} non riuscita: {ex.Message}");
                    }
                }
            }
        }

        private void ManageProcess(Process process)
        {
            if (process.Id != Environment.ProcessId && ShouldOptimizeProcess(process))
            {
                OptimizeProcess(process);
            }
        }

        private bool ShouldOptimizeProcess(Process process)
        {
            return process.TotalProcessorTime > TimeSpan.FromSeconds(5) &&
                   process.WorkingSet64 > 200 * 1024 * 1024;
        }

        private void OptimizeProcess(Process process)
        {
            process.PriorityClass = ProcessPriorityClass.BelowNormal;
        }
        #endregion

        #region Event Handlers

        private async void FormMonitoraggio_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_closeAfterMonitoringStops)
            {
                return;
            }

            e.Cancel = true;
            _shutdownRequested = true;
            await CleanupResourcesAsync();
            if (!IsDisposed)
            {
                _closeAfterMonitoringStops = true;
                Close();
            }
        }

        private async void btn_pulisciram_Click(object sender, EventArgs e)
        {
            btnPulisciRam.Enabled = false;
            try
            {
                _manualRamCleanupTask = Task.Run(() =>
                {
                    CollectManagedMemory();
                }, _monitoringToken);
                await _manualRamCleanupTask;
            }
            catch (OperationCanceledException) when (_monitoringToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                ShowErrorMessage($"Pulizia RAM non riuscita:\n{ex.Message}");
            }
            finally
            {
                _manualRamCleanupTask = null;
                if (!IsDisposed)
                {
                    btnPulisciRam.Enabled = true;
                }
            }
        }

        private async void btn_puliscicpu_Click(object sender, EventArgs e)
        {
            btnPulisciCPU.Enabled = false;
            try
            {
                _manualCpuCleanupTask = Task.Run(CpuReduce, _monitoringToken);
                await _manualCpuCleanupTask;
            }
            catch (OperationCanceledException) when (_monitoringToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                ShowErrorMessage($"Ottimizzazione CPU non riuscita:\n{ex.Message}");
            }
            finally
            {
                _manualCpuCleanupTask = null;
                if (!IsDisposed)
                {
                    btnPulisciCPU.Enabled = true;
                }
            }
        }

        #endregion

        #region Utility Methods
        private static (ulong TotalPhysicalMemory, ulong AvailablePhysicalMemory) GetMemoryStatus()
        {
            Microsoft.VisualBasic.Devices.ComputerInfo memoryInfo = new();
            return (memoryInfo.TotalPhysicalMemory, memoryInfo.AvailablePhysicalMemory);
        }
        private void ApplyTheme()
        {
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }

        public Task CleanupResourcesAsync()
        {
            if (_cleanupTask is not null)
            {
                return _cleanupTask;
            }

            _monitoringCancellation.Cancel();
            _cleanupTask = CleanupResourcesCoreAsync();
            return _cleanupTask;
        }

        private async Task CleanupResourcesCoreAsync()
        {
            UnsubscribeFromNetworkChanges();
            _ramMonitorTimer?.Stop();
            try
            {
                if (_initializationTask is not null)
                {
                    await _initializationTask;
                }

                await Task.WhenAll(_monitoringTasks);
                Task[] cleanupTasks = new Task?[]
                    { _ramCleanupTask, _manualRamCleanupTask, _manualCpuCleanupTask }
                    .OfType<Task>()
                    .ToArray();
                if (cleanupTasks.Length > 0)
                {
                    await Task.WhenAll(cleanupTasks);
                }
            }
            catch (OperationCanceledException)
            {
                // La cancellazione è il normale percorso di arresto dei monitor.
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                Debug.WriteLine($"Arresto dei monitor non riuscito: {ex}");
            }

            try
            {
                lock (_hardwareSync)
                {
                    _computer?.Close();
                }
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                Debug.WriteLine($"Chiusura del monitor hardware non riuscita: {ex}");
            }

            DisposeMonitoringResource(_cpuCounter, "contatore CPU");
            DisposeMonitoringResource(_diskUsageCounter, "contatore disco");
            DisposeMonitoringResource(_ramMonitorTimer, "timer RAM");
            DisposeMonitoringResource(_notifyIcon, "icona notifiche");
            DisposeMonitoringResource(_monitoringCancellation, "cancellazione monitoraggio");
        }

        private static void DisposeMonitoringResource(IDisposable? resource, string resourceName)
        {
            try
            {
                resource?.Dispose();
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                Debug.WriteLine($"Rilascio di {resourceName} non riuscito: {ex}");
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        private void puliziaautomaticoCPU_Click(object sender, EventArgs e)
        {
            puliziaautomaticoCPU.Checked = !puliziaautomaticoCPU.Checked;
            puliziaautomaticoCPU_CheckedChanged(sender, EventArgs.Empty);
        }
        private void puliziaautomaticoCPU_CheckedChanged(object sender, EventArgs e)
        {
            MonitorSettings.PuliziaAutomaticaCPU = puliziaautomaticoCPU.Checked;
        }

        private void puliziaautomaticRAM_CheckedChanged(object sender, EventArgs e)
        {
            MonitorSettings.PuliziaAutomaticaRAM = puliziaautomaticRAM.Checked;
        }

        private void limiteCPU_ValueChanged(object sender, EventArgs e)
        {
            MonitorSettings.LimiteCPU = limiteCPU.Value;
        }

        private void limiteRAM_ValueChanged(object sender, EventArgs e)
        {
            MonitorSettings.LimiteRAM = limiteRAM.Value;
        }

        private async void btnSvuotaTemp_Click(object sender, EventArgs e)
        {
            btnSvuotaTemp.Enabled = false;
            int deletedFiles = 0;
            int deletedFolders = 0;
            int failures = 0;

            try
            {
                (deletedFiles, deletedFolders, failures) = await Task.Run(() => CleanTempDirectory(_monitoringToken), _monitoringToken);

                string resultMessage =
                    $"File eliminati: {deletedFiles}\nCartelle eliminate: {deletedFolders}" +
                    (failures > 0 ? $"\nElementi non eliminati: {failures}" : string.Empty);

                MessageBox.Show(
                    resultMessage,
                    failures > 0 ? "Pulizia TEMP parziale" : "Pulizia TEMP completata",
                    MessageBoxButtons.OK,
                    failures > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information
                );
            }
            catch (OperationCanceledException) when (_monitoringCancellation.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                ShowErrorMessage($"Errore durante la pulizia della cartella TEMP:\n{ex.Message}");
            }
            finally
            {
                if (!IsDisposed)
                {
                    btnSvuotaTemp.Enabled = true;
                }
            }
        }

        private static (int DeletedFiles, int DeletedFolders, int Failures) CleanTempDirectory(CancellationToken cancellationToken)
        {
            int deletedFiles = 0;
            int deletedFolders = 0;
            int failures = 0;
            string tempPath = Path.GetTempPath();

            foreach (string filePath in Directory.EnumerateFiles(tempPath, "*", SearchOption.TopDirectoryOnly))
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    File.Delete(filePath);
                    deletedFiles++;
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    failures++;
                    Debug.WriteLine($"File TEMP non eliminato: {ex.Message}");
                }
            }

            foreach (string directoryPath in Directory.EnumerateDirectories(tempPath, "*", SearchOption.TopDirectoryOnly))
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    if ((File.GetAttributes(directoryPath) & FileAttributes.ReparsePoint) != 0)
                    {
                        failures++;
                        Debug.WriteLine($"Cartella TEMP di tipo reparse point lasciata intatta: {directoryPath}");
                        continue;
                    }

                    Directory.Delete(directoryPath, recursive: true);
                    deletedFolders++;
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    failures++;
                    Debug.WriteLine($"Cartella TEMP non eliminata: {ex.Message}");
                }
            }

            return (deletedFiles, deletedFolders, failures);
        }


        private async void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {
            await SaveMonitoraggioSettingsAsync();
        }
        private async Task SaveMonitoraggioSettingsAsync()
        {
            int gb = GetSelectedGB();

            var obj = new MonitoraggioConfig
            {
                LimiteGB = gb,
                ShowFahrenheitcpu = MonitorSettings.ShowFahrenheitcpu,
                ShowFahrenheitgpu = MonitorSettings.ShowFahrenheitgpu
            };

            string json = System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            string directory = Path.GetDirectoryName(monitoraggioPath)
                ?? throw new InvalidOperationException("Percorso monitoraggio non valido.");
            string temporaryPath = $"{monitoraggioPath}.{Guid.NewGuid():N}.tmp";

            bool lockAcquired = false;
            try
            {
                await _monitorSettingsWriteLock.WaitAsync(_monitoringToken);
                lockAcquired = true;
                Directory.CreateDirectory(directory);
                await File.WriteAllTextAsync(temporaryPath, json, _monitoringToken);
                File.Move(temporaryPath, monitoraggioPath, overwrite: true);
            }
            catch (OperationCanceledException) when (_monitoringToken.IsCancellationRequested)
            {
                Debug.WriteLine("Salvataggio impostazioni monitoraggio annullato durante la chiusura.");
            }
            catch (Exception ex) when (IsExpectedMonitoringFailure(ex))
            {
                Debug.WriteLine($"Salvataggio impostazioni monitoraggio non riuscito: {ex}");
            }
            finally
            {
                try
                {
                    if (File.Exists(temporaryPath))
                    {
                        File.Delete(temporaryPath);
                    }
                }
                catch (IOException ex)
                {
                    Debug.WriteLine($"Rimozione file temporaneo impostazioni non riuscita: {ex.Message}");
                }
                finally
                {
                    if (lockAcquired)
                    {
                        _monitorSettingsWriteLock.Release();
                    }
                }
            }
        }

        private void FormMonitoraggio_Load(object sender, EventArgs e)
        {
            puliziaautomaticoCPU.Checked = MonitorSettings.PuliziaAutomaticaCPU;
            puliziaautomaticRAM.Checked = MonitorSettings.PuliziaAutomaticaRAM;

            limiteCPU.Value = MonitorSettings.LimiteCPU;
            limiteRAM.Value = MonitorSettings.LimiteRAM;
            domainUpDown1.Items.Clear();

            for (int i = 1; i <= 100; i++)
                domainUpDown1.Items.Add($"{i} GB");

            domainUpDown1.ReadOnly = true;
        }

        private async void cuiSwitch_gradicpu_CheckedChanged(object sender, EventArgs e)
        {
            MonitorSettings.ShowFahrenheitcpu = cuiSwitch_gradicpu.Checked;
            await SaveMonitoraggioSettingsAsync();
            UpdateTemperatureDisplays();
        }

        private async void cuiSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            MonitorSettings.ShowFahrenheitgpu = cuiSwitch_gputemperatura.Checked;
            await SaveMonitoraggioSettingsAsync();
            UpdateTemperatureDisplays();
        }

        private string ConvertTemperature(string temperature, bool isFahrenheit)
        {
            if (temperature == "N/A" || !double.TryParse(temperature, out double tempC))
                return temperature;

            if (isFahrenheit)
            {
                double tempF = (tempC * 9 / 5) + 32;
                return tempF.ToString("0");
            }
            else
            {
                return tempC.ToString("0");
            }
        }
    }
}
