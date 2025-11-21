using CuoreUI.Controls;
using LibreHardwareMonitor.Hardware;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Base
{
    public partial class FormMonitoraggio : Form
    {
        #region Constants and Fields
        private const string RegistryKey = @"Software\WinHubX-Monitor";
        private const string RegistryValueMonitoraggio = "IsMonitoringOn";
        private const string RegistryValueTemperature = "isTemperatureOn";
        private string monitoraggioPath =
    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "WinHubX", "Impostazioni", "Monitoraggio.json");

        private const uint PROCESS_SET_QUOTA = 0x0100;
        private const uint PROCESS_QUERY_INFORMATION = 0x0400;

        private NetworkInterface[] networkInterfaces;
        private DateTime lastUpdateTime;
        private long lastBytesSent;
        private long lastBytesReceived;


        private readonly Form1 _mainForm;
        private Computer _computer;
        private System.Windows.Forms.Timer _monitoringTimer;
        private System.Windows.Forms.Timer _tempMonitorTimer;
        private PerformanceCounter _cpuCounter;

        private bool _isMonitoringOn = false;
        private bool _isTemperatureOn = false;
        private bool _notificationAlreadyShown = false;
        private NotifyIcon _notifyIcon;
        #endregion

        #region Constructor
        public FormMonitoraggio(Form1 mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;

            this.Shown += FormMonitoraggio_Shown;
        }

        private async void FormMonitoraggio_Shown(object? sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            await Task.Delay(50);

            try
            {
                LanguageManager.LoadLanguageFromSettings();

                btnPulisciCPU.Content = LanguageManager.CurrentLanguage == "it" ? "  Pulizia" : "  Clean";
                btnPulisciRam.Content = LanguageManager.CurrentLanguage == "it" ? "  Pulizia" : "  Clean";
                btnSvuotaTemp.Content = LanguageManager.CurrentLanguage == "it" ? "  Svuota" : "  Empty";

                InitializeComputer();
                InitializeTimers();
                InitializePerformanceCounter();
                InitializeNotificationIcon();
                ApplyTheme();
                StartCpuMonitoring();
                StartRamMonitoring();
                StartReteMonitoring();
                StartGPUMonitoring();
                StartDiscoMonitoring();
                StartTEMPMonitoring();
                LoadMonitoraggioSettings();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        #endregion

        #region Initialization Methods
        private void InitializeComputer()
        {
            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsStorageEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true
            };
            _computer.Open();
        }

        private void InitializeTimers()
        {
            _monitoringTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _monitoringTimer.Tick += MonitoringTimer_Tick;
            _monitoringTimer.Start();

            _tempMonitorTimer = new System.Windows.Forms.Timer { Interval = 5000 };
            _tempMonitorTimer.Tick += TempMonitorTimer_Tick;
            _tempMonitorTimer.Start();
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

        private void LoadMonitoraggioSettings()
        {
            try
            {
                if (!File.Exists(monitoraggioPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(monitoraggioPath));
                    File.WriteAllText(monitoraggioPath, "{ \"LimiteGB\": 2, \"ShowFahrenheitcpu\": false, \"ShowFahrenheitgpu\": false }");
                }

                string json = File.ReadAllText(monitoraggioPath);
                var obj = System.Text.Json.JsonSerializer.Deserialize<MonitoraggioConfig>(json);
                domainUpDown1.Text = $"{obj.LimiteGB} GB";
                MonitorSettings.ShowFahrenheitcpu = obj.ShowFahrenheitcpu;
                cuiSwitch_gradicpu.Checked = obj.ShowFahrenheitcpu;

                MonitorSettings.ShowFahrenheitgpu = obj.ShowFahrenheitgpu;
                cuiSwitch_gputemperatura.Checked = obj.ShowFahrenheitgpu;
            }
            catch
            {
                domainUpDown1.Text = "2 GB";
                cuiSwitch_gradicpu.Checked = false;
                cuiSwitch_gputemperatura.Checked = false;
            }
        }

        #endregion

        #region Temperature Monitoring
        private void MonitoringTimer_Tick(object sender, EventArgs e)
        {
            UpdateTemperatureDisplays();
        }

        private void UpdateTemperatureDisplays()
        {
            var cpuTemperature = GetTemperature(HardwareType.Cpu)?.ToString("0") ?? "N/A";
            var gpuTemperature = GetGpuTemperature() ?? "N/A";
            var displayCpuTemp = ConvertTemperature(cpuTemperature, MonitorSettings.ShowFahrenheitcpu);
            var displayGpuTemp = ConvertTemperature(gpuTemperature, MonitorSettings.ShowFahrenheitgpu);

            labelCpuTemp.Text = $"{displayCpuTemp}°";
            labelGpuTemp.Text = $"{displayGpuTemp}°";

            UpdateTemperatureImage(pic_termcpu, cpuTemperature, UpdateCpuTemperatureImage);
            UpdateTemperatureImage(pic_termgpu, gpuTemperature, UpdateGpuTemperatureImage);
        }

        private string GetGpuTemperature()
        {
            return GetTemperature(HardwareType.GpuNvidia)?.ToString("0")
                   ?? GetTemperature(HardwareType.GpuAmd)?.ToString("0")
                   ?? GetTemperature(HardwareType.Cpu)?.ToString("0")
                   ?? "N/A";
        }

        private float? GetTemperature(HardwareType hardwareType)
        {
            foreach (var hardware in _computer.Hardware)
            {
                if (hardware.HardwareType == hardwareType)
                {
                    hardware.Update();
                    var sensor = hardware.Sensors
                        .FirstOrDefault(s => s.SensorType == SensorType.Temperature);
                    return sensor?.Value;
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
            var timer = new System.Windows.Forms.Timer { Interval = 3000 };
            timer.Tick += (sender, e) =>
            {
                MEMORYSTATUSEX memStatus = GetMemoryStatus();
                double ramUsagePercentage = ((double)(memStatus.ullTotalPhys - memStatus.ullAvailPhys) / memStatus.ullTotalPhys) * 100;

                BarRAM.ProgressValue = Math.Min((int)ramUsagePercentage, 100);
                BarRAMtext.Text = $"{ramUsagePercentage:0}%";
                if (MonitorSettings.PuliziaAutomaticaRAM && ramUsagePercentage > (double)MonitorSettings.LimiteRAM)
                {
                    CleanMemory();
                    CpuReduce();
                    OptimizeMemory();
                }
            };
            timer.Start();
        }

        private void CleanMemory()
        {
            var processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                try
                {
                    CleanProcessMemory(process);
                }
                catch (Exception)
                {
                }
            }
        }

        private void CleanProcessMemory(Process process)
        {
            IntPtr processHandle = OpenProcess(PROCESS_SET_QUOTA | PROCESS_QUERY_INFORMATION, false, process.Id);

            if (processHandle != IntPtr.Zero)
            {
                try
                {
                    _ = SetProcessWorkingSetSize(processHandle, IntPtr.Zero, IntPtr.Zero);
                    _ = EmptyWorkingSet(processHandle);
                }
                finally
                {
                    _ = CloseHandle(processHandle);
                }
            }
        }

        private bool ReduceMemoryUse(int processId)
        {
            IntPtr processHandle = OpenProcess(PROCESS_SET_QUOTA | PROCESS_QUERY_INFORMATION, false, processId);

            if (processHandle == IntPtr.Zero)
                return false;

            try
            {
                return EmptyWorkingSet(processHandle);
            }
            finally
            {
                _ = CloseHandle(processHandle);
            }
        }
        #endregion

        #region CPU Monitoring and Management
        private async void StartTEMPMonitoring()
        {
            string tempPath = Path.GetTempPath();

            while (true)
            {
                try
                {
                    long totalBytes = GetDirectorySize(tempPath);

                    double usedGB = Math.Round(totalBytes / 1024.0 / 1024.0 / 1024.0, 2);
                    double limitGB = GetSelectedGB();

                    BarTEMPtext.Text = $"{usedGB}GB";

                    int percent = (int)Math.Min((usedGB / limitGB) * 100, 100);
                    BarTEMP.ProgressValue = percent;
                    BarTEMP.ProgressColor = usedGB <= limitGB ? Color.Green : Color.Red;
                }
                catch
                {
                }

                await Task.Delay(2000);
            }
        }
        private int GetSelectedGB()
        {
            string text = domainUpDown1.Text.Replace(" GB", "");
            if (int.TryParse(text, out int value))
                return value;

            return 2; 
        }

        private long GetDirectorySize(string folderPath)
        {
            long size = 0;

            DirectoryInfo dir = new DirectoryInfo(folderPath);

            foreach (FileInfo file in dir.GetFiles("*", SearchOption.AllDirectories))
            {
                size += file.Length;
            }

            return size;
        }


        private async void StartDiscoMonitoring()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        double discoUsage = GetDiscoUsagePercentage();
                        UpdateDiscoUI(discoUsage);
                        await Task.Delay(3000);
                    }
                    catch (Exception)
                    {
                        UpdateDiscoUI(0);
                        await Task.Delay(3000);
                    }
                }
            });
        }
        private double GetDiscoUsagePercentage()
        {
            try
            {
                using (var diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total"))
                {
                    diskCounter.NextValue();
                    System.Threading.Thread.Sleep(1000);
                    float diskUsage = diskCounter.NextValue();

                    return Math.Min(diskUsage, 100);
                }
            }
            catch (Exception)
            {
                try
                {
                    using (var diskCounter = new PerformanceCounter("LogicalDisk", "% Disk Time", "_Total"))
                    {
                        diskCounter.NextValue();
                        System.Threading.Thread.Sleep(1000);
                        float diskUsage = diskCounter.NextValue();
                        return Math.Min(diskUsage, 100);
                    }
                }
                catch
                {
                    return 0;
                }
            }
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
                BarDISCO?.Refresh();
                BarDISCOtext?.Refresh();
            }
            catch (Exception)
            {
            }
        }

        private async void StartCpuMonitoring()
        {
            while (true)
            {
                double cpuUsagePercentage = await GetCpuUsagePercentageAsync();
                BarCPU.ProgressValue = (int)cpuUsagePercentage;
                BarCPUtext.Text = $"{cpuUsagePercentage:0}%";
                if (MonitorSettings.PuliziaAutomaticaCPU && cpuUsagePercentage > (double)MonitorSettings.LimiteCPU)
                {
                    CpuReduce();
                }

                await Task.Delay(2000);
            }
        }

        private async void StartGPUMonitoring()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    double gpuUsage = GetGpuLoadPercentage() ?? 0;
                    UpdateGpuUI(gpuUsage);
                    await Task.Delay(2000);
                }
            });
        }

        private double? GetGpuLoadPercentage()
        {
            foreach (var hardware in _computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.GpuNvidia ||
                    hardware.HardwareType == HardwareType.GpuAmd ||
                    hardware.HardwareType == HardwareType.GpuIntel)
                {
                    hardware.Update();
                    var loadSensor = hardware.Sensors.FirstOrDefault(s =>
                        s.SensorType == SensorType.Load &&
                        (s.Name.Contains("Core") ||
                         s.Name.Contains("GPU Core") ||
                         s.Name.Contains("D3D 3D") ||
                         s.Name.Contains("Utilization")));

                    if (loadSensor != null && loadSensor.Value.HasValue)
                        return loadSensor.Value;
                }
            }
            return null;
        }

        private void UpdateGpuUI(double gpuUsage)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateGpuUI(gpuUsage)));
                return;
            }

            BarGPU.ProgressValue = (int)Math.Round(gpuUsage);
            BarGPUtext.Text = $"{gpuUsage:0}%";
        }
        private async void StartReteMonitoring()
        {
            networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up &&
                           n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .ToArray();

            if (networkInterfaces.Length == 0)
            {
                labelReteUtilizzo.Text = "Nessuna interfaccia attiva";
                labelVelocitaRete.Text = "0 KB/s";
                return;
            }
            lastUpdateTime = DateTime.Now;
            lastBytesSent = networkInterfaces.Sum(n => n.GetIPv4Statistics().BytesSent);
            lastBytesReceived = networkInterfaces.Sum(n => n.GetIPv4Statistics().BytesReceived);
            while (true)
            {
                await Task.Delay(1000);

                try
                {
                    await UpdateNetworkStats();
                }
                catch (Exception)
                {
                }
            }
        }

        private async Task UpdateNetworkStats()
        {
            var currentTime = DateTime.Now;
            var timeDiff = (currentTime - lastUpdateTime).TotalSeconds;

            if (timeDiff <= 0) return;
            long currentBytesSent = 0;
            long currentBytesReceived = 0;
            long totalBytes = 0;

            foreach (var netInterface in networkInterfaces)
            {
                var stats = netInterface.GetIPv4Statistics();
                currentBytesSent += stats.BytesSent;
                currentBytesReceived += stats.BytesReceived;
                totalBytes += stats.BytesSent + stats.BytesReceived;
            }

            double sentKB = (currentBytesSent - lastBytesSent) / timeDiff / 1024;
            double receivedKB = (currentBytesReceived - lastBytesReceived) / timeDiff / 1024;
            double totalSpeedKB = sentKB + receivedKB;
            double networkUsage = CalculateNetworkUsage(totalSpeedKB);
            await UpdateUI(sentKB, receivedKB, totalSpeedKB, networkUsage);
            lastBytesSent = currentBytesSent;
            lastBytesReceived = currentBytesReceived;
            lastUpdateTime = currentTime;
        }

        private double CalculateNetworkUsage(double currentSpeedKB)
        {
            double maxCapacityKB = 10000;

            double usage = (currentSpeedKB / maxCapacityKB) * 100;
            return Math.Min(usage, 100);
        }

        private async Task UpdateUI(double sentKB, double receivedKB, double totalSpeedKB, double networkUsage)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateUI(sentKB, receivedKB, totalSpeedKB, networkUsage)));
                return;
            }

            try
            {
                lblUpload.Text = "        " + string.Format(
                    LanguageManager.GetTranslation("FormMonitoraggio", "upload"),
                    sentKB.ToString("0.00")
                );

                lblDonwload.Text = "        " + string.Format(
                    LanguageManager.GetTranslation("FormMonitoraggio", "download"),
                    receivedKB.ToString("0.00")
                );

                labelVelocitaRete.Text = "        " + string.Format(
                    LanguageManager.GetTranslation("FormMonitoraggio", "velocita"),
                    totalSpeedKB.ToString("0.00")
                );

                labelReteUtilizzo.Text = $"{networkUsage:0.0}%";
                progressbarRete.ProgressValue = (int)Math.Round(networkUsage);
            }
            catch (Exception)
            {

            }
        }


        private async Task<double> GetCpuUsagePercentageAsync()
        {
            return await Task.Run(() =>
            {
                _ = _cpuCounter.NextValue();
                Thread.Sleep(1000);
                return _cpuCounter.NextValue();
            });
        }

        private void CpuReduce()
        {
            var processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                try
                {
                    ManageProcess(process);
                }
                catch (Exception)
                {
                }
            }
        }

        private void ManageProcess(Process process)
        {
            if (ShouldOptimizeProcess(process))
            {
                OptimizeProcess(process);
            }
            else if (ShouldTerminateProcess(process))
            {
                TerminateProcess(process);
            }
        }

        private bool ShouldOptimizeProcess(Process process)
        {
            return process.TotalProcessorTime > TimeSpan.FromSeconds(5) &&
                   process.WorkingSet64 > 200 * 1024 * 1024;
        }

        private bool ShouldTerminateProcess(Process process)
        {
            return process.TotalProcessorTime > TimeSpan.FromSeconds(10) &&
                   process.WorkingSet64 > 500 * 1024 * 1024;
        }

        private void OptimizeProcess(Process process)
        {
            process.PriorityClass = ProcessPriorityClass.BelowNormal;
        }

        private void TerminateProcess(Process process)
        {
            process.Kill();
        }
        #endregion

        #region TEMP Folder Management
        private void TempMonitorTimer_Tick(object sender, EventArgs e)
        {
            UpdateTempFolderStatus();
        }

        private void UpdateTempFolderStatus()
        {

        }

        private long GetFolderSize(DirectoryInfo dir)
        {
            long size = 0;
            try
            {
                foreach (FileInfo file in dir.GetFiles())
                {
                    size += file.Length;
                }

                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    size += GetFolderSize(subDir);
                }
            }
            catch (Exception)
            {
            }
            return size;
        }

        private void CleanTempFolder()
        {
            string tempPath = Path.GetTempPath();

            try
            {
                var di = new DirectoryInfo(tempPath);

                CleanTempFiles(di);
                CleanTempDirectories(di);

                UpdateTempFolderStatus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error cleaning TEMP folder:\n{ex.Message}");
            }
        }

        private void CleanTempFiles(DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch (Exception)
                {
                }
            }
        }

        private void CleanTempDirectories(DirectoryInfo directory)
        {
            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                }
                catch (Exception)
                {
                }
            }
        }
        #endregion

        #region Event Handlers

        private void FormMonitoraggio_FormClosing(object sender, FormClosingEventArgs e)
        {
            CleanupResources();
            e.Cancel = false;
        }

        private void btn_pulisciram_Click(object sender, EventArgs e)
        {
            CleanMemory();
            OptimizeMemory();
        }

        private void btn_puliscicpu_Click(object sender, EventArgs e)
        {
            CpuReduce();
        }

        private void btnPulisciTemp_Click(object sender, EventArgs e)
        {
            CleanTempFolder();
        }
        #endregion

        #region Utility Methods
        private void OptimizeMemory()
        {
            var currentProcess = Process.GetCurrentProcess();
            _ = ReduceMemoryUse(currentProcess.Id);
        }

        private MEMORYSTATUSEX GetMemoryStatus()
        {
            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            memStatus.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            _ = GlobalMemoryStatusEx(ref memStatus);
            return memStatus;
        }
        private void ApplyTheme()
        {
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }

        public void CleanupResources()
        {
            _computer?.Close();
            _cpuCounter?.Dispose();
            _monitoringTimer?.Dispose();
            _tempMonitorTimer?.Dispose();
            _notifyIcon?.Dispose();
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region Native Methods
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("psapi.dll")]
        private static extern bool EmptyWorkingSet(IntPtr hProcess);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessWorkingSetSize(IntPtr hProcess, IntPtr dwMinimumWorkingSetSize, IntPtr dwMaximumWorkingSetSize);
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

        private void btnSvuotaTemp_Click(object sender, EventArgs e)
        {
            string tempPath = Path.GetTempPath();
            int deletedFiles = 0;
            int deletedFolders = 0;

            try
            {
                DirectoryInfo dir = new DirectoryInfo(tempPath);
                foreach (FileInfo file in dir.GetFiles("*", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        file.Delete();
                        deletedFiles++;
                    }
                    catch
                    {

                    }
                }

                foreach (DirectoryInfo folder in dir.GetDirectories())
                {
                    try
                    {
                        folder.Delete(true);
                        deletedFolders++;
                    }
                    catch
                    {

                    }
                }

                MessageBox.Show(
                    $"Pulizia completata!\n\n" +
                    $"File eliminati: {deletedFiles}\n" +
                    $"Cartelle eliminate: {deletedFolders}",
                    "Temp svuotata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Errore durante la pulizia della cartella TEMP:\n" + ex.Message,
                    "Errore",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {
            SaveMonitoraggioSettings();
        }
        private void SaveMonitoraggioSettings()
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

            File.WriteAllText(monitoraggioPath, json);
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

        private void cuiSwitch_gradicpu_CheckedChanged(object sender, EventArgs e)
        {
            MonitorSettings.ShowFahrenheitcpu = cuiSwitch_gradicpu.Checked;
            SaveMonitoraggioSettings();
            UpdateTemperatureDisplays();
        }

        private void cuiSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            MonitorSettings.ShowFahrenheitgpu = cuiSwitch_gputemperatura.Checked;
            SaveMonitoraggioSettings();
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