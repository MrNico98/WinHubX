using LibreHardwareMonitor.Hardware;
using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace WinHubX.Forms.Base
{
    public partial class FormMonitoraggio : Form
    {
        private Form1 form1;
        private const string RegistryKey = @"Software\WinHubX-Monitor";
        private const string RegistryValueMonitoraggio = "IsMonitoringOn";
        private bool isMonitoringOn = false;
        private readonly Computer _computer;
        private const string RegistryValueTemperature = "isTemperatureOn";
        private bool isTemperatureOn = false;
        private NotifyIcon notifyIcon;
        public FormMonitoraggio(Form1 form1)
        {
            InitializeComponent();
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            StopMonitoringRam();
            StartMonitoringCPU();
            this.form1 = form1;
            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
            };

            _computer.Open();
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
            LoadState();
        }
        private void LoadState()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKey))
            {
                if (key != null)
                {
                    object value = key.GetValue(RegistryValueTemperature);
                    if (value != null)
                    {
                        isTemperatureOn = Convert.ToInt32(value) == 1;
                        swapButton1.Checked = isTemperatureOn;
                    }
                }
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            var cpuTemperature = GetTemperature(HardwareType.Cpu)?.ToString("0") ?? "N/A";
            var gpuTemperature = GetTemperature(HardwareType.GpuNvidia)?.ToString("0")
                                 ?? GetTemperature(HardwareType.GpuAmd)?.ToString("0")
                                 ?? GetTemperature(HardwareType.Cpu)?.ToString("0")
                                 ?? "N/A";
            labelCpuTemp.Text = $"CPU: {cpuTemperature}°";
            labelGpuTemp.Text = $"GPU: {gpuTemperature}°";
            UpdateCpuTemperatureImage(cpuTemperature);
            UpdateGpuTemperatureImage(gpuTemperature);
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
                    if (sensor != null)
                    {
                        return sensor.Value;
                    }
                }
            }
            return null;
        }
        private void UpdateCpuTemperatureImage(string cpuTempStr)
        {
            if (float.TryParse(cpuTempStr, out float cpuTemp))
            {
                Image image = null;

                if (cpuTemp >= 80)
                {
                    image = Properties.Resources.term_rosso;
                }
                else if (cpuTemp >= 65)
                {
                    image = Properties.Resources.term_giallo;
                }
                else
                {
                    image = Properties.Resources.term_verde;
                }
                if (image != null)
                {
                    pic_termcpu.Image = image;
                }
                else
                {
                    MessageBox.Show("Immagine non trovata nelle risorse.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void UpdateGpuTemperatureImage(string gpuTempStr)
        {
            if (float.TryParse(gpuTempStr, out float gpuTemp))
            {
                Image image = null;

                if (gpuTemp >= 80 && gpuTemp <= 120)
                {
                    image = Properties.Resources.term_rosso;
                }
                else if (gpuTemp >= 65)
                {
                    image = Properties.Resources.term_giallo;
                }
                else if (gpuTemp >= 0 && gpuTemp < 65)
                {
                    image = Properties.Resources.term_verde;
                }
                if (image != null)
                {
                    pic_termgpu.Image = image;
                }
                else
                {
                    MessageBox.Show("Immagine non trovata nelle risorse.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void StartMonitoringRam()
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 3000;

            timer.Tick += (sender, e) =>
            {
                MEMORYSTATUSEX memStatus = GetMemoryStatus();
                double ramUsagePercentage = ((double)(memStatus.ullTotalPhys - memStatus.ullAvailPhys) / memStatus.ullTotalPhys) * 100;

                BarRAM.Value = (int)ramUsagePercentage;

                if (ramUsagePercentage > 40)
                {
                    CleanMemory();
                }
                Cpureduct();
                OptimizeMemory();
            };
            timer.Start();
        }
        private void StopMonitoringRam()
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 5000;

            timer.Tick += (sender, e) =>
            {
                MEMORYSTATUSEX memStatus = GetMemoryStatus();
                double ramUsagePercentage = ((double)(memStatus.ullTotalPhys - memStatus.ullAvailPhys) / memStatus.ullTotalPhys) * 100;

                BarRAM.Value = (int)ramUsagePercentage;
            };

            timer.Start();
            OptimizeMemory();
        }
        private void CleanMemory()
        {
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                try
                {
                    IntPtr processHandle = OpenProcess(PROCESS_SET_QUOTA | PROCESS_QUERY_INFORMATION, false, process.Id);

                    if (processHandle != IntPtr.Zero)
                    {
                        SetProcessWorkingSetSize(processHandle, IntPtr.Zero, IntPtr.Zero);
                        EmptyWorkingSet(processHandle);
                        CloseHandle(processHandle);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        private bool ReduceMemoryUse(int processId)
        {
            IntPtr processHandle = OpenProcess(PROCESS_SET_QUOTA | PROCESS_QUERY_INFORMATION, false, processId);

            if (processHandle == IntPtr.Zero)
                return false;

            bool result = EmptyWorkingSet(processHandle);

            CloseHandle(processHandle);

            return result;
        }
        private MEMORYSTATUSEX GetMemoryStatus()
        {
            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            memStatus.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            GlobalMemoryStatusEx(ref memStatus);
            return memStatus;
        }
        private void OptimizeMemory()
        {
            var currentProcess = Process.GetCurrentProcess();
            ReduceMemoryUse(currentProcess.Id);
        }

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
        private const uint PROCESS_SET_QUOTA = 0x0100;
        private const uint PROCESS_QUERY_INFORMATION = 0x0400;

        private async void StartMonitoringCPU()
        {
            while (true)
            {
                double cpuUsagePercentage = await Task.Run(() => GetCPUUsagePercentage());
                BarCPU.Value = (int)cpuUsagePercentage;
                await Task.Delay(2000);
            }
        }
        private double GetCPUUsagePercentage()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            double cpuUsage = cpuCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            cpuUsage = cpuCounter.NextValue();
            return cpuUsage;
        }
        private void Cpureduct()
        {
            {
                Process[] processes = Process.GetProcesses();

                foreach (Process process in processes)
                {
                    try
                    {
                        if (ProcessDaOttimizzare(process))
                        {
                            OttimizzaProcesso(process);
                        }
                        else if (ProcessDaTerminare(process))
                        {
                            process.Kill();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            static bool ProcessDaOttimizzare(Process process)
            {
                if (process.TotalProcessorTime > TimeSpan.FromSeconds(5) && process.WorkingSet64 > 200 * 1024 * 1024)
                {
                    return true;
                }

                return false;
            }
            static bool ProcessDaTerminare(Process process)
            {
                if (process.TotalProcessorTime > TimeSpan.FromSeconds(10) && process.WorkingSet64 > 500 * 1024 * 1024)
                {
                    return true;
                }

                return false;
            }

            static void OttimizzaProcesso(Process process)
            {
                process.PriorityClass = ProcessPriorityClass.BelowNormal;
            }
        }
        private void swapButton1_CheckedChanged(object sender, EventArgs e)
        {
            isMonitoringOn = swapButton1.Checked;

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryKey))
            {
                key.SetValue(RegistryValueMonitoraggio, isMonitoringOn ? 1 : 0);
            }

            if (isMonitoringOn)
            {
                MessageBox.Show(LanguageManager.GetTranslation("FormMonitoraggio", "monitoron"), "INFO");
                StartMonitoringRam();
            }
            else
            {
                MessageBox.Show(LanguageManager.GetTranslation("FormMonitoraggio", "monitoroff"), "INFO");
                StopMonitoringRam();
            }
        }

        private void FormMonitoraggio_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryKey))
            {
                key.SetValue(RegistryValueMonitoraggio, isMonitoringOn ? 1 : 0);
            }
            _computer.Close();
            e.Cancel = false;
        }
        private void FormMonitoraggio_Load(object sender, EventArgs e)
        {
            radioButton_notifica.Checked = Properties.Settings.Default.MinimizeToTray;
            radioButton_taskbar.Checked = !Properties.Settings.Default.MinimizeToTray;

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKey))
            {
                if (key != null)
                {
                    object value = key.GetValue(RegistryValueMonitoraggio);
                    if (value != null && (int)value == 1)
                    {
                        swapButton1.Checked = true;
                        isMonitoringOn = true;
                        StartMonitoringRam();
                    }
                    else
                    {
                        swapButton1.Checked = false;
                        isMonitoringOn = false;
                    }
                }
            }
            notifyIcon = new NotifyIcon();
            notifyIcon.Visible = false;
            notifyIcon.Icon = SystemIcons.Warning;
            notifyIcon.BalloonTipTitle = "Cartella TEMP";
            comboBox_gb.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_gb.SelectedItem = "2";
            comboBox_gb_SelectedIndexChanged(null, null);
            System.Windows.Forms.Timer tempMonitorTimer = new System.Windows.Forms.Timer();
            tempMonitorTimer.Interval = 5000;
            tempMonitorTimer.Tick += TempMonitorTimer_Tick;
            tempMonitorTimer.Start();
        }
        private void btn_pulisciram_Click(object sender, EventArgs e)
        {
            CleanMemory();
            OptimizeMemory();
        }
        private void btn_puliscicpu_Click(object sender, EventArgs e)
        {
            Cpureduct();
        }

        private void radioButton_notifica_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_notifica.Checked)
            {
                Properties.Settings.Default.MinimizeToTray = true;
                Properties.Settings.Default.Save();
            }
        }

        private void radioButton_taskbar_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_taskbar.Checked)
            {
                Properties.Settings.Default.MinimizeToTray = false;
                Properties.Settings.Default.Save();
            }
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
            catch { /* Ignora errori di accesso */ }
            return size;
        }
        private bool notificaGiaMostrata = false;
        private void comboBox_gb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(comboBox_gb.SelectedItem.ToString(), out int selectedLimitGB))
            {
                string tempPath = Path.GetTempPath();
                long sizeBytes = GetFolderSize(new DirectoryInfo(tempPath));
                double sizeGB = sizeBytes / (1024.0 * 1024.0 * 1024.0);

                int percent = (int)((sizeGB / selectedLimitGB) * 100);
                if (percent > 100) percent = 100;

                CartellaTemp.Value = percent;
                CartellaTemp.Text = $"{sizeGB:F2} GB";

                if (sizeGB > selectedLimitGB)
                {
                    if (!notificaGiaMostrata)
                    {
                        notifyIcon.Visible = true;
                        notifyIcon.BalloonTipText = $"La cartella TEMP supera il limite di {selectedLimitGB} GB!";
                        notifyIcon.ShowBalloonTip(3000);
                        var hideIconTimer = new System.Windows.Forms.Timer();
                        hideIconTimer.Interval = 4000;
                        hideIconTimer.Tick += (s, args) =>
                        {
                            notifyIcon.Visible = false;
                            hideIconTimer.Stop();
                            hideIconTimer.Dispose();
                        };
                        hideIconTimer.Start();

                        notificaGiaMostrata = true;
                    }
                }
                else
                {
                    notificaGiaMostrata = false;
                }
            }
        }
        private void TempMonitorTimer_Tick(object sender, EventArgs e)
        {
            comboBox_gb_SelectedIndexChanged(null, null);
        }

        private void btnPulisciTemp_Click(object sender, EventArgs e)
        {
            string tempPath = Path.GetTempPath();

            try
            {
                DirectoryInfo di = new DirectoryInfo(tempPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch { }
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    try
                    {
                        dir.Delete(true);
                    }
                    catch { }
                }
                comboBox_gb_SelectedIndexChanged(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error:\n{ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
