using Microsoft.Win32;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace WinHubX.Forms
{
    public partial class FormWinHubXLiteOS : Form
    {
        private int ramGB = 0;
        private bool isHDD = false;
        private bool is24H2 = false;
        private bool windo11 = false;
        Form1 form1;
        FormTools formtools;
        public FormWinHubXLiteOS(Form1 form1, FormTools formtools)
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            this.form1 = form1;
            this.formtools = formtools;
        }
        #region Prepare

        private void puntodiripristino()
        {
            string titolo = LanguageManager.GetTranslation("Form1", "restorepoint_title");
            string messaggio = LanguageManager.GetTranslation("Form1", "restorepoint_message");

            var result = MessageBox.Show(
                messaggio,
                titolo,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    string script = @"
# Abilita System Restore sull'unità di sistema
try {
    Enable-ComputerRestore -Drive ""$env:SystemDrive""
} catch {
    Write-Host ""Errore nell'abilitazione del Ripristino configurazione di sistema: $_""
}

# Imposta la frequenza di creazione dei punti di ripristino
$exists = Get-ItemProperty -path ""HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore"" -Name ""SystemRestorePointCreationFrequency"" -ErrorAction SilentlyContinue
if($null -eq $exists) {
    Set-ItemProperty -Path ""HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore"" -Name ""SystemRestorePointCreationFrequency"" -Value 0 -Type DWord -Force
}

# Importa il modulo necessario
try {
    Import-Module Microsoft.PowerShell.Management -ErrorAction Stop
} catch {
    Write-Host ""Errore nel caricamento del modulo: $_""
    return
}

# Controlla se oggi esistono già punti di ripristino
try {
    $existingRestorePoints = Get-ComputerRestorePoint | Where-Object { $_.CreationTime.Date -eq (Get-Date).Date }
} catch {
    Write-Host ""Errore nel recupero dei punti di ripristino: $_""
    return
}

# Crea un punto di ripristino se non ce ne sono già oggi
if ($existingRestorePoints.Count -eq 0) {
    Checkpoint-Computer -Description ""Punto di ripristino creato da WinHubX"" -RestorePointType MODIFY_SETTINGS
    Write-Host ""Punto di ripristino creato correttamente""
}
";

                    string tempScriptPath = Path.Combine(Path.GetTempPath(), "CreateRestorePoint.ps1");
                    File.WriteAllText(tempScriptPath, script);

                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-ExecutionPolicy Bypass -NoProfile -File \"{tempScriptPath}\"",
                        UseShellExecute = true,
                        Verb = "runas"
                    };

                    Process.Start(psi)?.WaitForExit();
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string regBackupPath = Path.Combine(desktopPath, $"BackupRegistroWinHubX_{DateTime.Now:yyyyMMdd_HHmmss}.reg");

                    ProcessStartInfo regExport = new ProcessStartInfo()
                    {
                        FileName = "reg.exe",
                        Arguments = $"export HKLM \"{regBackupPath}\" /y",
                        UseShellExecute = true,
                        Verb = "runas"
                    };

                    Process.Start(regExport)?.WaitForExit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task startall()
        {
            await Task.Delay(8000);
            await VERIFICOOS();
            await Task.Delay(3000);
            progressBar1.Value = 10;
            await EnableAllPrivileges();
            progressBar1.Value = 15;
            await TerminateAllNonCriticalProcesses();
            progressBar1.Value = 20;
            await StopNonEssentialServices();
            progressBar1.Value = 25;
            await TerminateNonEssentialProcesses();
            progressBar1.Value = 30;
            await PrepareSystemForModification();
            progressBar1.Value = 35;
            progressBar1.Value = 40;
            await DisableDesktop();
            progressBar1.Value = 45;
            await HideDesktopIcons();
            progressBar1.Value = 50;
            await DisableExplorerAutostart();
            progressBar1.Value = 55;
            await KillExplorer();
            progressBar1.Value = 60;
            if (windo11)
            {
                await StartProcess11();
            }
            else
            {
                await StartProcess10();
            }
            progressBar1.Value = 65;
            await RestoreDesktopIcons();
            progressBar1.Value = 70;
            await RestoreDesktop();
            progressBar1.Value = 80;
            await RestoreExplorerAutostart();
            progressBar1.Value = 90;
            await StartExplorer();
            progressBar1.Value = 100;
            MessageBox.Show("Succsso");
        }

        private async Task StartProcess10()
        {
            await Task.Delay(5000);
            await P000W10();
            await Task.Delay(5000);
            await P001W10();
            await Task.Delay(5000);
            await P002W10();
            await Task.Delay(5000);
            await P003W10();
            await Task.Delay(5000);
            await DownloadTools();
            await Task.Delay(5000);
            await P004W10();
            await Task.Delay(5000);
            await P005W10();
            await Task.Delay(5000);
            await LastTweakW10();
            await DeleteTools();
        }


        private async Task StartProcess11()
        {
            await Task.Delay(5000);
            await P000();
            await Task.Delay(5000);
            await P001();
            await Task.Delay(5000);
            await P002();
            await Task.Delay(5000);
            await P003();
            await Task.Delay(5000);
            await DownloadTools();
            await Task.Delay(5000);
            await P004();
            await Task.Delay(5000);
            await P005();
            await Task.Delay(5000);
            await P006();
            await Task.Delay(5000);
            await LastTweak();
            await DeleteTools();
        }


        #endregion

        #region Funzioni di supporto

        public struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public uint Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public LUID_AND_ATTRIBUTES[] Privileges;
        }

        private const int SE_PRIVILEGE_ENABLED = 0x00000002;
        private const string SE_DEBUG_NAME = "SeDebugPrivilege";
        private const string SE_TAKE_OWNERSHIP_NAME = "SeTakeOwnershipPrivilege";
        private const string SE_TCB_NAME = "SeTcbPrivilege";
        private const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        private const uint TOKEN_QUERY = 0x0008;

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out LUID lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle,
                                                       [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges,
                                                       ref TOKEN_PRIVILEGES NewState,
                                                       uint BufferLength,
                                                       IntPtr PreviousState,
                                                       IntPtr ReturnLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        public async Task HideDesktopIcons()
        {
            string registryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
            Registry.SetValue(registryKey, "NoDesktop", 1, RegistryValueKind.DWord);
        }
        public async Task DisableDesktop()
        {
            string registryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
            Registry.SetValue(registryKey, "NoDesktop", 1, RegistryValueKind.DWord);
        }
        public async Task DisableExplorerAutostart()
        {
            string registryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows NT\CurrentVersion\Winlogon";
            string valueName = "Shell";

            try
            {
                Registry.SetValue(registryKey, valueName, "cmd.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore nel modificare il registro: " + ex.Message);
            }
        }

        public async Task KillExplorer()
        {
            Process[] processes = Process.GetProcessesByName("explorer");

            foreach (var process in processes)
            {
                process.Kill();
            }
        }

        private async Task EnableAllPrivileges()
        {
            IntPtr hToken = IntPtr.Zero;

            try
            {
                if (!OpenProcessToken(Process.GetCurrentProcess().Handle,
                                    TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out hToken))
                {
                    throw new Exception("OpenProcessToken failed. Error: " + Marshal.GetLastWin32Error());
                }
                EnablePrivilege(hToken, SE_DEBUG_NAME);
                EnablePrivilege(hToken, SE_TAKE_OWNERSHIP_NAME);
                EnablePrivilege(hToken, SE_TCB_NAME);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore nell'abilitazione dei privilegi: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (hToken != IntPtr.Zero)
                    CloseHandle(hToken);
            }
        }

        private async Task TerminateAllNonCriticalProcesses()
        {
            string[] criticalProcesses = {
        "csrss", "wininit", "winlogon", "System", "smss",
        Process.GetCurrentProcess().ProcessName.ToLower()
    };

            foreach (var process in Process.GetProcesses()
                     .Where(p => !criticalProcesses.Contains(p.ProcessName.ToLower())))
            {
                try
                {
                    if (process.SessionId != 0 || process.ProcessName.Equals("explorer", StringComparison.OrdinalIgnoreCase))
                    {
                        process.Kill();
                    }
                }
                catch { }
            }

            try { Process.GetProcessesByName("explorer").ToList().ForEach(p => p.Kill()); } catch { }
        }

        private void EnablePrivilege(IntPtr hToken, string privilegeName)
        {
            LUID luid;
            if (!LookupPrivilegeValue(null, privilegeName, out luid))
            {
                throw new Exception($"LookupPrivilegeValue failed for {privilegeName}. Error: " + Marshal.GetLastWin32Error());
            }

            TOKEN_PRIVILEGES tp = new TOKEN_PRIVILEGES();
            tp.PrivilegeCount = 1;
            tp.Privileges = new LUID_AND_ATTRIBUTES[1];
            tp.Privileges[0].Luid = luid;
            tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

            if (!AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero))
            {
                throw new Exception($"AdjustTokenPrivileges failed for {privilegeName}. Error: " + Marshal.GetLastWin32Error());
            }
        }

        private async Task StopNonEssentialServices()
        {
            string[] nonEssentialServices = {
        "wuauserv", "BITS", "WinDefend", "Wsearch", "SysMain",
        "DiagTrack", "DPS", "dmwappushservice", "lfsvc", "MapsBroker"
    };

            foreach (var service in nonEssentialServices)
            {
                try
                {
                    Process.Start("net.exe", $"stop {service}").WaitForExit();
                }
                catch { /* Ignora gli errori */ }
            }
        }

        private async Task TerminateNonEssentialProcesses()
        {
            string[] essentialProcesses = { "explorer", "csrss", "wininit", "winlogon", "System", "smss" };
            string[] processesToKill = { "dllhost", "RuntimeBroker", "ShellExperienceHost", "SearchUI", "MicrosoftEdge" };

            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    string name = process.ProcessName;
                    if (Array.IndexOf(essentialProcesses, name) >= 0)
                        continue;
                    if (name.Equals("explorer", StringComparison.OrdinalIgnoreCase) || name.Equals("desktop", StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (Array.IndexOf(processesToKill, name) >= 0 ||
                        name.EndsWith("Host") || name.EndsWith("Helper"))
                    {
                        process.Kill();
                        Thread.Sleep(100);
                    }
                }
                catch { }
            }
        }

        private async Task PrepareSystemForModification()
        {
            try
            {
                Process.Start("reg.exe", "ADD HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System /v EnableLUA /t REG_DWORD /d 0 /f").WaitForExit();
                Process.Start("vssadmin.exe", "Delete Shadows /All /Quiet").WaitForExit();
                Process.Start("wmic.exe", "pagefileset delete").WaitForExit();
                Process.Start("powercfg.exe", "/hibernate off").WaitForExit();
                Process.Start("ipconfig.exe", "/flushdns").WaitForExit();
                Process.Start("wmic.exe", "shadowcopy delete").WaitForExit();
                Process.Start("vssadmin.exe", "Delete Shadows /All /Quiet").WaitForExit();
            }
            catch { }
        }

        public async Task RestoreDesktopIcons()
        {
            string registryKey32 = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
            string registryKey64 = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
            Registry.SetValue(registryKey32, "NoDesktop", 0, RegistryValueKind.DWord);
            Registry.SetValue(registryKey64, "NoDesktop", 0, RegistryValueKind.DWord);
        }

        public async Task RestoreDesktop()
        {
            string registryKey32 = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
            string registryKey64 = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
            Registry.SetValue(registryKey32, "NoDesktop", 0, RegistryValueKind.DWord);
            Registry.SetValue(registryKey64, "NoDesktop", 0, RegistryValueKind.DWord);
        }

        public async Task RestoreExplorerAutostart()
        {
            string registryKey32 = @"HKEY_CURRENT_USER\Software\Microsoft\Windows NT\CurrentVersion\Winlogon";
            string registryKey64 = @"HKEY_CURRENT_USER\Software\Microsoft\Windows NT\CurrentVersion\Winlogon";
            string valueName = "Shell";

            try
            {
                Registry.SetValue(registryKey32, valueName, "explorer.exe");
                Registry.SetValue(registryKey64, valueName, "explorer.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore nel modificare il registro: " + ex.Message);
            }
        }

        public async Task StartExplorer()
        {
            foreach (var process in Process.GetProcessesByName("explorer"))
            {
                process.Kill();
            }
            System.Diagnostics.Process.Start("explorer.exe");
        }

        #endregion

        #region Settaggio immagine
        #endregion
        #region Introduzione
        #endregion
        #region Verifico OS
        private async Task VERIFICOOS()
        {
            progressBar2.Invoke(() => progressBar2.Value = 0);

            await Task.Run(() =>
            {
                int step = 20;

                string osName = GetOSName();
                string osVersion = RuntimeInformation.OSDescription;
                string build = "";
                string edition = "";
                try
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT Version, BuildNumber FROM Win32_OperatingSystem"))
                    {
                        foreach (var os in searcher.Get())
                        {
                            string version = os["Version"]?.ToString() ?? "";
                            build = os["BuildNumber"]?.ToString() ?? "";
                            edition = GetWindowsReleaseName(version, build);
                            break;
                        }
                    }
                }
                catch { }
                if (edition.Contains("24H2"))
                    is24H2 = true;
                else if (edition.Contains("23H2"))
                    is24H2 = false;
                if (edition.Contains("Windows 11"))
                    windo11 = true;
                else
                    windo11 = false;
                progressBar2.Invoke(() => progressBar2.Value += step);

                string ramInfo = GetRAMInfo();
                Match match = Regex.Match(ramInfo, @"\d+");
                if (match.Success)
                {
                    ramGB = int.Parse(match.Value);
                }
                progressBar2.Invoke(() => progressBar2.Value += step);
                string cpuInfo = GetCPUInfo();

                progressBar2.Invoke(() => progressBar2.Value += step);

                string driveType = GetDriveType();
                isHDD = driveType.Equals("HDD", StringComparison.OrdinalIgnoreCase);

                progressBar2.Invoke(() => progressBar2.Value += step);

                string userName = Environment.UserName;

                progressBar2.Invoke(() => progressBar2.Value += step);

                string result = "";
                result += $"🖥️ Sistema Operativo: {osName} - {edition}\n";
                result += $"👤 Utente: {userName}\n";
                result += $"💾 RAM: {ramInfo}\n";
                result += $"⚙️ CPU: {cpuInfo}\n";
                result += $"📀 Disco: {driveType}\n";

                Invoke(() =>
                {
                    richTextBox1.Clear();
                    richTextBox1.AppendText(result);
                    progressBar2.Value = 100;
                });
            });
        }

        private string GetOSName()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT Caption, Version, BuildNumber FROM Win32_OperatingSystem"))
                {
                    foreach (var os in searcher.Get())
                    {
                        string caption = os["Caption"]?.ToString() ?? "Sconosciuto";
                        string version = os["Version"]?.ToString() ?? "?.?.?";
                        string build = os["BuildNumber"]?.ToString() ?? "?";

                        string edition = GetWindowsReleaseName(version, build);
                        return $"{caption} ({edition}) - Versione: {version} - Build: {build}";
                    }
                }
            }
            catch { }

            return "Sconosciuto";
        }

        private string GetWindowsReleaseName(string version, string build)
        {
            if (int.TryParse(build, out int buildNumber))
            {
                if (buildNumber >= 26100)
                    return "Windows 11 24H2";
                else if (buildNumber >= 22631)
                    return "Windows 11 23H2";
                else if (buildNumber >= 22621)
                    return "Windows 11 22H2";
                else
                    return "Versione Windows 11 precedente";
            }

            return "Versione non riconosciuta";
        }


        private string GetRAMInfo()
        {
            try
            {
                ulong totalMemory = 0;
                using (var searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory"))
                {
                    foreach (var ram in searcher.Get())
                    {
                        totalMemory += (ulong)(ram["Capacity"] ?? 0);
                    }
                }

                return $"{totalMemory / (1024 * 1024 * 1024)} GB";
            }
            catch
            {
                return "Sconosciuta";
            }
        }

        private string GetCPUInfo()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor"))
                {
                    foreach (var cpu in searcher.Get())
                    {
                        return cpu["Name"]?.ToString() ?? "Sconosciuto";
                    }
                }
            }
            catch { }

            return "Sconosciuto";
        }

        private string GetDriveType()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT MediaType, Model FROM Win32_DiskDrive"))
                {
                    foreach (var drive in searcher.Get())
                    {
                        string mediaType = drive["MediaType"]?.ToString() ?? "";
                        string model = drive["Model"]?.ToString()?.ToUpper() ?? "";

                        if (mediaType.Contains("SSD") || model.Contains("NVME"))
                            return "SSD / NVMe";
                        else if (mediaType.Contains("HDD") || mediaType.Contains("Fixed hard disk media"))
                            return "HDD";
                    }
                }
            }
            catch { }

            return "Sconosciuto";
        }
        #endregion
        #region Inzio
        private async Task P000()
        {
            AppendToLog("Inizio P000");
            string url = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/WinHubXLiteOS/Win11/P000.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string jsonContent = await client.GetStringAsync(url);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var config = JsonSerializer.Deserialize<WinHubXConfig>(jsonContent, options);
                    label1.Invoke(() => label1.Text = config.Description);
                    if (config?.CMD != null)
                    {
                        int totalCommands = config.CMD.Count;
                        progressBar2.Invoke(() =>
                        {
                            progressBar2.Maximum = totalCommands;
                            progressBar2.Value = 0;
                        });

                        foreach (var cmd in config.CMD)
                        {
                            try
                            {
                                AppendToLog($"Eseguo \"{cmd.Nome}\"");

                                ProcessStartInfo psi = new ProcessStartInfo
                                {
                                    FileName = "cmd.exe",
                                    Arguments = $"/C {cmd.Command}",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true,
                                    Verb = "runas"
                                };

                                using (Process process = Process.Start(psi))
                                {
                                    string error = await process.StandardError.ReadToEndAsync();
                                    await process.WaitForExitAsync();

                                    if (string.IsNullOrWhiteSpace(error))
                                        AppendToLog($"✅ \"{cmd.Nome}\" completato con successo");
                                    else
                                        AppendToLog($"❌ Errore durante l'esecuzione di \"{cmd.Nome}\"");
                                }
                            }
                            catch
                            {
                                AppendToLog($"❌ Errore durante l'esecuzione di \"{cmd.Nome}\"");
                            }

                            progressBar2.Invoke(() => progressBar2.Value++);
                        }
                    }
                    else
                    {
                        AppendToLog("Nessun comando CMD trovato nel JSON.");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToLog($"❌ Errore P000: {ex.Message}");
            }

            AppendToLog("Esecuzione P000 completata.");
        }


        private async Task P001()
        {
            AppendToLog("Inizio P001");
            string url = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/WinHubXLiteOS/Win11/P001.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string jsonContent = await client.GetStringAsync(url);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var config = JsonSerializer.Deserialize<WinHubXConfig>(jsonContent, options);
                    label1.Invoke(() => label1.Text = config.Description);
                    if (config?.PS != null)
                    {
                        var eseguibili = config.PS.Where(ps =>
                            (string.IsNullOrEmpty(ps._4ram) || !(ps._4ram.Equals("true", StringComparison.OrdinalIgnoreCase) && ramGB != 4)) &&
                            (string.IsNullOrEmpty(ps._hdd) ||
                                (ps._hdd.Equals("true", StringComparison.OrdinalIgnoreCase) && isHDD) ||
                                (ps._hdd.Equals("false", StringComparison.OrdinalIgnoreCase) && !isHDD))
                        ).ToList();

                        progressBar2.Invoke(() =>
                        {
                            progressBar2.Maximum = eseguibili.Count;
                            progressBar2.Value = 0;
                        });

                        foreach (var ps in config.PS)
                        {
                            if (!string.IsNullOrEmpty(ps._4ram) && ps._4ram.Equals("true", StringComparison.OrdinalIgnoreCase) && ramGB != 4)
                            {
                                AppendToLog($"⏭️ Salto \"{ps.Nome}\" (Richiede 4 GB RAM)");
                                continue;
                            }

                            if (!string.IsNullOrEmpty(ps._hdd) && ps._hdd.Equals("true", StringComparison.OrdinalIgnoreCase) && !isHDD)
                            {
                                AppendToLog($"⏭️ Salto \"{ps.Nome}\" (Non hai un HDD)");
                                continue;
                            }

                            if (!string.IsNullOrEmpty(ps._hdd) && ps._hdd.Equals("false", StringComparison.OrdinalIgnoreCase) && isHDD)
                            {
                                AppendToLog($"⏭️ Salto \"{ps.Nome}\" (Hai un HDD)");
                                continue;
                            }

                            AppendToLog($"Eseguo \"{ps.Nome}\"");

                            try
                            {
                                ProcessStartInfo psi = new ProcessStartInfo
                                {
                                    FileName = "powershell.exe",
                                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{ps.Command}\"",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true,
                                    Verb = "runas"
                                };

                                using (Process process = Process.Start(psi))
                                {
                                    string error = await process.StandardError.ReadToEndAsync();
                                    await process.WaitForExitAsync();

                                    if (string.IsNullOrWhiteSpace(error))
                                        AppendToLog($"✅ Comando completato con successo");
                                    else
                                        AppendToLog($"❌ Errore: {error}");
                                }
                            }
                            catch
                            {
                                AppendToLog($"❌ Errore nell'esecuzione di \"{ps.Nome}\"");
                            }

                            progressBar2.Invoke(() => progressBar2.Value++);
                        }
                    }

                    if (config?.CMD == null && config?.PS == null)
                    {
                        AppendToLog("Nessun comando CMD o PowerShell trovato nel JSON.");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToLog($"❌ Errore P001: {ex.Message}");
            }

            AppendToLog("Esecuzione P001 completata.");
        }


        private async Task P002()
        {
            AppendToLog("Inizio P002");
            string url = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/WinHubXLiteOS/Win11/P002.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string jsonContent = await client.GetStringAsync(url);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var config = JsonSerializer.Deserialize<WinHubXConfig>(jsonContent, options);
                    label1.Invoke(() => label1.Text = config.Description);
                    if (config?.PSArray != null)
                    {
                        var eseguibili = config.PSArray.Where(ps =>
                            (
                                (is24H2 && ps.Command24h2 != null && ps.Command24h2.Count > 0) ||
                                (!is24H2 && ps.Command23h2 != null && ps.Command23h2.Count > 0) ||
                                (ps.Command != null && ps.Command.Count > 0)
                            )
                        ).ToList();

                        progressBar2.Invoke(() =>
                        {
                            progressBar2.Maximum = eseguibili.Count;
                            progressBar2.Value = 0;
                        });

                        foreach (var ps in config.PSArray)
                        {
                            AppendToLog($"Eseguo \"{ps.Nome}\"");

                            try
                            {
                                List<string> comandoDaEseguire = new();

                                if (is24H2 && ps.Command24h2 != null)
                                    comandoDaEseguire = ps.Command24h2;
                                else if (!is24H2 && ps.Command23h2 != null)
                                    comandoDaEseguire = ps.Command23h2;
                                else if (ps.Command != null)
                                    comandoDaEseguire = ps.Command;

                                if (comandoDaEseguire.Count == 0)
                                {
                                    AppendToLog("⚠️ Nessun comando disponibile per questa versione.");
                                    continue;
                                }

                                string comandoUnito = string.Join(";", comandoDaEseguire);

                                ProcessStartInfo psi = new ProcessStartInfo
                                {
                                    FileName = "powershell.exe",
                                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{comandoUnito}\"",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true,
                                    Verb = "runas"
                                };

                                using (Process process = Process.Start(psi))
                                {
                                    string error = await process.StandardError.ReadToEndAsync();
                                    await process.WaitForExitAsync();

                                    if (string.IsNullOrWhiteSpace(error))
                                        AppendToLog($"✅ Comando completato con successo");
                                    else
                                        AppendToLog($"❌ Errore: {error}");
                                }
                            }
                            catch
                            {
                                AppendToLog($"❌ Errore nell'esecuzione di \"{ps.Nome}\"");
                            }

                            progressBar2.Invoke(() => progressBar2.Value++);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToLog($"❌ Errore P002: {ex.Message}");
            }

            AppendToLog("Esecuzione P002 completata.");
        }


        private async Task P003()
        {
            AppendToLog("Inizio P003");
            string url = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/WinHubXLiteOS/Win11/P003.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string jsonContent = await client.GetStringAsync(url);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var config = JsonSerializer.Deserialize<WinHubXConfig>(jsonContent, options);
                    label1.Invoke(() => label1.Text = config.Description);
                    if (config?.CMD != null)
                    {
                        var eseguibili = config.CMD.Where(cmd =>
                             (string.IsNullOrEmpty(cmd._hdd) ||
                             (cmd._hdd.Equals("true", StringComparison.OrdinalIgnoreCase) && isHDD) ||
                             (cmd._hdd.Equals("false", StringComparison.OrdinalIgnoreCase) && !isHDD))
                        ).ToList();

                        progressBar2.Invoke(() =>
                        {
                            progressBar2.Maximum = eseguibili.Count;
                            progressBar2.Value = 0;
                        });

                        foreach (var cmd in config.CMD)
                        {
                            if (!string.IsNullOrEmpty(cmd._hdd) && cmd._hdd.Equals("true", StringComparison.OrdinalIgnoreCase) && !isHDD)
                            {
                                AppendToLog($"⏭️ Salto \"{cmd.Nome}\" (Non hai un HDD)");
                                continue;
                            }

                            if (!string.IsNullOrEmpty(cmd._hdd) && cmd._hdd.Equals("false", StringComparison.OrdinalIgnoreCase) && isHDD)
                            {
                                AppendToLog($"⏭️ Salto \"{cmd.Nome}\" (Hai un HDD)");
                                continue;
                            }

                            try
                            {
                                AppendToLog($"Eseguo \"{cmd.Nome}\"");

                                ProcessStartInfo psi = new ProcessStartInfo
                                {
                                    FileName = "cmd.exe",
                                    Arguments = $"/C {cmd.Command}",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true,
                                    Verb = "runas"
                                };

                                using (Process process = Process.Start(psi))
                                {
                                    string error = await process.StandardError.ReadToEndAsync();
                                    await process.WaitForExitAsync();

                                    if (string.IsNullOrWhiteSpace(error))
                                        AppendToLog($"✅ \"{cmd.Nome}\" completato con successo");
                                    else
                                        AppendToLog($"❌ Errore durante l'esecuzione di \"{cmd.Nome}\"");
                                }
                            }
                            catch
                            {
                                AppendToLog($"❌ Errore durante l'esecuzione di \"{cmd.Nome}\"");
                            }

                            progressBar2.Invoke(() => progressBar2.Value++);
                        }
                    }
                    else
                    {
                        AppendToLog("Nessun comando CMD trovato nel JSON.");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToLog($"❌ Errore P003: {ex.Message}");
            }

            AppendToLog("Esecuzione P003 completata.");
        }


        private async Task P004()
        {
            AppendToLog("Inizio P004");

            string toolAIMODSPath = @"C:\ToolAIMODS";
            string powerRunFile = Path.Combine(toolAIMODSPath, "PowerRun.exe");
            string regFile = Path.Combine(toolAIMODSPath, "lower-ram-usage.reg");

            if (File.Exists(powerRunFile) && File.Exists(regFile))
            {
                AppendToLog("File necessari trovati in ToolAIMODS, procedo con l'esecuzione.");

                string url = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/WinHubXLiteOS/Win11/P004.json";

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string jsonContent = await client.GetStringAsync(url);

                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        var config = JsonSerializer.Deserialize<WinHubXConfig>(jsonContent, options);
                        label1.Invoke(() => label1.Text = config.Description);
                        if (config?.CMDArray != null)
                        {
                            progressBar2.Invoke(() =>
                            {
                                progressBar2.Maximum = config.CMDArray.Count;
                                progressBar2.Value = 0;
                            });

                            foreach (var cmd in config.CMDArray)
                            {
                                AppendToLog($"Eseguo: \"{cmd.Nome}\"");

                                bool success = true;
                                foreach (var line in cmd.Command)
                                {
                                    ProcessStartInfo psi = new ProcessStartInfo
                                    {
                                        FileName = "cmd.exe",
                                        Arguments = $"/C {line}",
                                        UseShellExecute = false,
                                        RedirectStandardOutput = true,
                                        RedirectStandardError = true,
                                        CreateNoWindow = true,
                                        Verb = "runas"
                                    };

                                    try
                                    {
                                        using (Process process = Process.Start(psi))
                                        {
                                            await process.WaitForExitAsync();
                                            string error = await process.StandardError.ReadToEndAsync();
                                            if (!string.IsNullOrWhiteSpace(error))
                                            {
                                                success = false;
                                                break;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        success = false;
                                        break;
                                    }
                                }

                                if (success)
                                {
                                    AppendToLog($"✅ \"{cmd.Nome}\" completato");
                                }
                                else
                                {
                                    AppendToLog($"❌ Errore durante l'esecuzione di \"{cmd.Nome}\"");
                                }

                                progressBar2.Invoke(() => progressBar2.Value++);
                            }
                        }
                        else
                        {
                            AppendToLog("⚠️ Nessun comando CMD trovato nel JSON.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppendToLog($"❌ Errore P004: {ex.Message}");
                }

                AppendToLog("✅ Esecuzione P004 completata.");
            }
            else
            {
                AppendToLog("❌ File mancanti in ToolAIMODS. Assicurati che PowerRun.exe e lower-ram-usage.reg siano presenti.");
            }
        }

        private async Task P005()
        {
            AppendToLog("Inizio P005");
            string url = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/WinHubXLiteOS/Win11/P005.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string jsonContent = await client.GetStringAsync(url);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var config = JsonSerializer.Deserialize<WinHubXConfig>(jsonContent, options);
                    label1.Invoke(() => label1.Text = config.Description);
                    if (config?.CMD != null)
                    {
                        var eseguibili = config.CMD.Where(cmd =>
                             (string.IsNullOrEmpty(cmd._hdd) ||
                             (cmd._hdd.Equals("true", StringComparison.OrdinalIgnoreCase) && isHDD) ||
                             (cmd._hdd.Equals("false", StringComparison.OrdinalIgnoreCase) && !isHDD))
                        ).ToList();

                        progressBar2.Invoke(() =>
                        {
                            progressBar2.Maximum = eseguibili.Count;
                            progressBar2.Value = 0;
                        });

                        foreach (var cmd in config.CMD)
                        {
                            if (!string.IsNullOrEmpty(cmd._hdd) && cmd._hdd.Equals("true", StringComparison.OrdinalIgnoreCase) && !isHDD)
                            {
                                AppendToLog($"⏭️ Salto \"{cmd.Nome}\" (Non hai un HDD)");
                                continue;
                            }

                            if (!string.IsNullOrEmpty(cmd._hdd) && cmd._hdd.Equals("false", StringComparison.OrdinalIgnoreCase) && isHDD)
                            {
                                AppendToLog($"⏭️ Salto \"{cmd.Nome}\" (Hai un HDD)");
                                continue;
                            }

                            try
                            {
                                AppendToLog($"Eseguo \"{cmd.Nome}\"");

                                ProcessStartInfo psi = new ProcessStartInfo
                                {
                                    FileName = "cmd.exe",
                                    Arguments = $"/C {cmd.Command}",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true,
                                    Verb = "runas"
                                };

                                using (Process process = Process.Start(psi))
                                {
                                    string error = await process.StandardError.ReadToEndAsync();
                                    await process.WaitForExitAsync();

                                    if (string.IsNullOrWhiteSpace(error))
                                        AppendToLog($"✅ \"{cmd.Nome}\" completato con successo");
                                    else
                                        AppendToLog($"❌ Errore durante l'esecuzione di \"{cmd.Nome}\"");
                                }
                            }
                            catch
                            {
                                AppendToLog($"❌ Errore durante l'esecuzione di \"{cmd.Nome}\"");
                            }

                            progressBar2.Invoke(() => progressBar2.Value++);
                        }
                    }
                    else
                    {
                        AppendToLog("Nessun comando CMD trovato nel JSON.");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToLog($"❌ Errore P005: {ex.Message}");
            }

            AppendToLog("Esecuzione P005 completata.");
        }

        private async Task P006()
        {
            AppendToLog("Inizio P006");
            string url = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/WinHubXLiteOS/Win11/P006.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string jsonContent = await client.GetStringAsync(url);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var config = JsonSerializer.Deserialize<WinHubXConfig>(jsonContent, options);
                    label1.Invoke(() => label1.Text = config.Description);
                    if (config?.PSArray != null)
                    {
                        progressBar2.Invoke(() =>
                        {
                            progressBar2.Maximum = config.PSArray.Count;
                            progressBar2.Value = 0;
                        });

                        foreach (var ps in config.PSArray)
                        {
                            AppendToLog($"Eseguo \"{ps.Nome}\"");

                            try
                            {
                                string comandoUnito = string.Join(";", ps.Command);

                                ProcessStartInfo psi = new ProcessStartInfo
                                {
                                    FileName = "powershell.exe",
                                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{comandoUnito}\"",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true,
                                    Verb = "runas"
                                };

                                using (Process process = Process.Start(psi))
                                {
                                    string error = await process.StandardError.ReadToEndAsync();
                                    await process.WaitForExitAsync();

                                    if (string.IsNullOrWhiteSpace(error))
                                        AppendToLog($"✅ Comando completato con successo");
                                    else
                                        AppendToLog($"❌ Errore: {error}");
                                }
                            }
                            catch (Exception ex)
                            {
                                AppendToLog($"❌ Errore nell'esecuzione di \"{ps.Nome}\": {ex.Message}");
                            }

                            progressBar2.Invoke(() => progressBar2.Value++);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToLog($"❌ Errore P006: {ex.Message}");
            }

            AppendToLog("Esecuzione P006 completata.");
        }


        private async Task LastTweak()
        {
            string url = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/WinHubXLiteOS/Win11/LastTweak.bat";
            string filePath = Path.Combine(Path.GetTempPath(), "LastTweak.bat");
            label1.Text = "Configurazione finale";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string scriptContent = await client.GetStringAsync(url);
                    await File.WriteAllTextAsync(filePath, scriptContent, Encoding.UTF8);
                }

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true,
                    Verb = "runas"
                };

                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il download o l'avvio dello script:\n{ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task P000W10()
        {
            AppendToLog("Inizio P000");
            string url = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/WinHubXLiteOS/Win10/P000.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string jsonContent = await client.GetStringAsync(url);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var config = JsonSerializer.Deserialize<WinHubXConfig>(jsonContent, options);
                    label1.Invoke(() => label1.Text = config.Description);
                    if (config?.CMD != null)
                    {
                        int totalCommands = config.CMD.Count;
                        progressBar2.Invoke(() =>
                        {
                            progressBar2.Maximum = totalCommands;
                            progressBar2.Value = 0;
                        });

                        foreach (var cmd in config.CMD)
                        {
                            try
                            {
                                AppendToLog($"Eseguo \"{cmd.Nome}\"");

                                ProcessStartInfo psi = new ProcessStartInfo
                                {
                                    FileName = "cmd.exe",
                                    Arguments = $"/C {cmd.Command}",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true,
                                    Verb = "runas"
                                };

                                using (Process process = Process.Start(psi))
                                {
                                    string error = await process.StandardError.ReadToEndAsync();
                                    await process.WaitForExitAsync();

                                    if (string.IsNullOrWhiteSpace(error))
                                        AppendToLog($"✅ \"{cmd.Nome}\" completato con successo");
                                    else
                                        AppendToLog($"❌ Errore durante l'esecuzione di \"{cmd.Nome}\"");
                                }
                            }
                            catch
                            {
                                AppendToLog($"❌ Errore durante l'esecuzione di \"{cmd.Nome}\"");
                            }

                            progressBar2.Invoke(() => progressBar2.Value++);
                        }
                    }
                    else
                    {
                        AppendToLog("Nessun comando CMD trovato nel JSON.");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToLog($"❌ Errore P000: {ex.Message}");
            }

            AppendToLog("Esecuzione P000 completata.");
        }


        private async Task P001W10()
        {
            AppendToLog("Inizio P001");
            string url = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/WinHubXLiteOS/Win10/P001.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string jsonContent = await client.GetStringAsync(url);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var config = JsonSerializer.Deserialize<WinHubXConfig>(jsonContent, options);
                    label1.Invoke(() => label1.Text = config.Description);
                    if (config?.PS != null)
                    {
                        var eseguibili = config.PS.Where(ps =>
                            (string.IsNullOrEmpty(ps._4ram) || !(ps._4ram.Equals("true", StringComparison.OrdinalIgnoreCase) && ramGB != 4)) &&
                            (string.IsNullOrEmpty(ps._hdd) ||
                                (ps._hdd.Equals("true", StringComparison.OrdinalIgnoreCase) && isHDD) ||
                                (ps._hdd.Equals("false", StringComparison.OrdinalIgnoreCase) && !isHDD))
                        ).ToList();

                        progressBar2.Invoke(() =>
                        {
                            progressBar2.Maximum = eseguibili.Count;
                            progressBar2.Value = 0;
                        });

                        foreach (var ps in config.PS)
                        {
                            if (!string.IsNullOrEmpty(ps._4ram) && ps._4ram.Equals("true", StringComparison.OrdinalIgnoreCase) && ramGB != 4)
                            {
                                AppendToLog($"⏭️ Salto \"{ps.Nome}\" (Richiede 4 GB RAM)");
                                continue;
                            }

                            if (!string.IsNullOrEmpty(ps._hdd) && ps._hdd.Equals("true", StringComparison.OrdinalIgnoreCase) && !isHDD)
                            {
                                AppendToLog($"⏭️ Salto \"{ps.Nome}\" (Non hai un HDD)");
                                continue;
                            }

                            if (!string.IsNullOrEmpty(ps._hdd) && ps._hdd.Equals("false", StringComparison.OrdinalIgnoreCase) && isHDD)
                            {
                                AppendToLog($"⏭️ Salto \"{ps.Nome}\" (Hai un HDD)");
                                continue;
                            }

                            AppendToLog($"Eseguo \"{ps.Nome}\"");

                            try
                            {
                                ProcessStartInfo psi = new ProcessStartInfo
                                {
                                    FileName = "powershell.exe",
                                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{ps.Command}\"",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true,
                                    Verb = "runas"
                                };

                                using (Process process = Process.Start(psi))
                                {
                                    string error = await process.StandardError.ReadToEndAsync();
                                    await process.WaitForExitAsync();

                                    if (string.IsNullOrWhiteSpace(error))
                                        AppendToLog($"✅ Comando completato con successo");
                                    else
                                        AppendToLog($"❌ Errore: {error}");
                                }
                            }
                            catch
                            {
                                AppendToLog($"❌ Errore nell'esecuzione di \"{ps.Nome}\"");
                            }

                            progressBar2.Invoke(() => progressBar2.Value++);
                        }
                    }

                    if (config?.CMD == null && config?.PS == null)
                    {
                        AppendToLog("Nessun comando CMD o PowerShell trovato nel JSON.");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToLog($"❌ Errore P001: {ex.Message}");
            }

            AppendToLog("Esecuzione P001 completata.");
        }


        private async Task P002W10()
        {
            AppendToLog("Inizio P002");
            string url = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/WinHubXLiteOS/Win10/P002.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string jsonContent = await client.GetStringAsync(url);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var config = JsonSerializer.Deserialize<WinHubXConfig>(jsonContent, options);
                    label1.Invoke(() => label1.Text = config.Description);
                    if (config?.PSArray != null)
                    {
                        var eseguibili = config.PSArray.Where(ps =>
                            (
                                (is24H2 && ps.Command24h2 != null && ps.Command24h2.Count > 0) ||
                                (!is24H2 && ps.Command23h2 != null && ps.Command23h2.Count > 0) ||
                                (ps.Command != null && ps.Command.Count > 0)
                            )
                        ).ToList();

                        progressBar2.Invoke(() =>
                        {
                            progressBar2.Maximum = eseguibili.Count;
                            progressBar2.Value = 0;
                        });

                        foreach (var ps in config.PSArray)
                        {
                            AppendToLog($"Eseguo \"{ps.Nome}\"");

                            try
                            {
                                List<string> comandoDaEseguire = new();

                                if (is24H2 && ps.Command24h2 != null)
                                    comandoDaEseguire = ps.Command24h2;
                                else if (!is24H2 && ps.Command23h2 != null)
                                    comandoDaEseguire = ps.Command23h2;
                                else if (ps.Command != null)
                                    comandoDaEseguire = ps.Command;

                                if (comandoDaEseguire.Count == 0)
                                {
                                    AppendToLog("⚠️ Nessun comando disponibile per questa versione.");
                                    continue;
                                }

                                string comandoUnito = string.Join(";", comandoDaEseguire);

                                ProcessStartInfo psi = new ProcessStartInfo
                                {
                                    FileName = "powershell.exe",
                                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{comandoUnito}\"",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true,
                                    Verb = "runas"
                                };

                                using (Process process = Process.Start(psi))
                                {
                                    string error = await process.StandardError.ReadToEndAsync();
                                    await process.WaitForExitAsync();

                                    if (string.IsNullOrWhiteSpace(error))
                                        AppendToLog($"✅ Comando completato con successo");
                                    else
                                        AppendToLog($"❌ Errore: {error}");
                                }
                            }
                            catch
                            {
                                AppendToLog($"❌ Errore nell'esecuzione di \"{ps.Nome}\"");
                            }

                            progressBar2.Invoke(() => progressBar2.Value++);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToLog($"❌ Errore P002: {ex.Message}");
            }

            AppendToLog("Esecuzione P002 completata.");
        }


        private async Task P003W10()
        {
            AppendToLog("Inizio P003");
            string url = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/WinHubXLiteOS/Win10/P003.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string jsonContent = await client.GetStringAsync(url);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var config = JsonSerializer.Deserialize<WinHubXConfig>(jsonContent, options);
                    label1.Invoke(() => label1.Text = config.Description);
                    if (config?.CMD != null)
                    {
                        var eseguibili = config.CMD.Where(cmd =>
                             (string.IsNullOrEmpty(cmd._hdd) ||
                             (cmd._hdd.Equals("true", StringComparison.OrdinalIgnoreCase) && isHDD) ||
                             (cmd._hdd.Equals("false", StringComparison.OrdinalIgnoreCase) && !isHDD))
                        ).ToList();

                        progressBar2.Invoke(() =>
                        {
                            progressBar2.Maximum = eseguibili.Count;
                            progressBar2.Value = 0;
                        });

                        foreach (var cmd in config.CMD)
                        {
                            if (!string.IsNullOrEmpty(cmd._hdd) && cmd._hdd.Equals("true", StringComparison.OrdinalIgnoreCase) && !isHDD)
                            {
                                AppendToLog($"⏭️ Salto \"{cmd.Nome}\" (Non hai un HDD)");
                                continue;
                            }

                            if (!string.IsNullOrEmpty(cmd._hdd) && cmd._hdd.Equals("false", StringComparison.OrdinalIgnoreCase) && isHDD)
                            {
                                AppendToLog($"⏭️ Salto \"{cmd.Nome}\" (Hai un HDD)");
                                continue;
                            }

                            try
                            {
                                AppendToLog($"Eseguo \"{cmd.Nome}\"");

                                ProcessStartInfo psi = new ProcessStartInfo
                                {
                                    FileName = "cmd.exe",
                                    Arguments = $"/C {cmd.Command}",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true,
                                    Verb = "runas"
                                };

                                using (Process process = Process.Start(psi))
                                {
                                    string error = await process.StandardError.ReadToEndAsync();
                                    await process.WaitForExitAsync();

                                    if (string.IsNullOrWhiteSpace(error))
                                        AppendToLog($"✅ \"{cmd.Nome}\" completato con successo");
                                    else
                                        AppendToLog($"❌ Errore durante l'esecuzione di \"{cmd.Nome}\"");
                                }
                            }
                            catch
                            {
                                AppendToLog($"❌ Errore durante l'esecuzione di \"{cmd.Nome}\"");
                            }

                            progressBar2.Invoke(() => progressBar2.Value++);
                        }
                    }
                    else
                    {
                        AppendToLog("Nessun comando CMD trovato nel JSON.");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToLog($"❌ Errore P003: {ex.Message}");
            }

            AppendToLog("Esecuzione P003 completata.");
        }


        private async Task P004W10()
        {
            AppendToLog("Inizio P004");

            string toolAIMODSPath = @"C:\ToolAIMODS";
            string powerRunFile = Path.Combine(toolAIMODSPath, "PowerRun.exe");
            string regFile = Path.Combine(toolAIMODSPath, "lower-ram-usage.reg");

            if (File.Exists(powerRunFile) && File.Exists(regFile))
            {
                AppendToLog("File necessari trovati in ToolAIMODS, procedo con l'esecuzione.");

                string url = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/WinHubXLiteOS/Win10/P004.json";

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string jsonContent = await client.GetStringAsync(url);

                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        var config = JsonSerializer.Deserialize<WinHubXConfig>(jsonContent, options);
                        label1.Invoke(() => label1.Text = config.Description);
                        if (config?.CMDArray != null)
                        {
                            progressBar2.Invoke(() =>
                            {
                                progressBar2.Maximum = config.CMDArray.Count;
                                progressBar2.Value = 0;
                            });

                            foreach (var cmd in config.CMDArray)
                            {
                                AppendToLog($"Eseguo: \"{cmd.Nome}\"");

                                bool success = true;
                                foreach (var line in cmd.Command)
                                {
                                    ProcessStartInfo psi = new ProcessStartInfo
                                    {
                                        FileName = "cmd.exe",
                                        Arguments = $"/C {line}",
                                        UseShellExecute = false,
                                        RedirectStandardOutput = true,
                                        RedirectStandardError = true,
                                        CreateNoWindow = true,
                                        Verb = "runas"
                                    };

                                    try
                                    {
                                        using (Process process = Process.Start(psi))
                                        {
                                            await process.WaitForExitAsync();
                                            string error = await process.StandardError.ReadToEndAsync();
                                            if (!string.IsNullOrWhiteSpace(error))
                                            {
                                                success = false;
                                                break;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        success = false;
                                        break;
                                    }
                                }

                                if (success)
                                {
                                    AppendToLog($"✅ \"{cmd.Nome}\" completato");
                                }
                                else
                                {
                                    AppendToLog($"❌ Errore durante l'esecuzione di \"{cmd.Nome}\"");
                                }

                                progressBar2.Invoke(() => progressBar2.Value++);
                            }
                        }
                        else
                        {
                            AppendToLog("⚠️ Nessun comando CMD trovato nel JSON.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppendToLog($"❌ Errore P004: {ex.Message}");
                }

                AppendToLog("✅ Esecuzione P004 completata.");
            }
            else
            {
                AppendToLog("❌ File mancanti in ToolAIMODS. Assicurati che PowerRun.exe e lower-ram-usage.reg siano presenti.");
            }
        }

        private async Task P005W10()
        {
            AppendToLog("Inizio P005");
            string url = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/WinHubXLiteOS/Win10/P005.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string jsonContent = await client.GetStringAsync(url);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var config = JsonSerializer.Deserialize<WinHubXConfig>(jsonContent, options);
                    label1.Invoke(() => label1.Text = config.Description);
                    if (config?.CMD != null)
                    {
                        var eseguibili = config.CMD.Where(cmd =>
                             (string.IsNullOrEmpty(cmd._hdd) ||
                             (cmd._hdd.Equals("true", StringComparison.OrdinalIgnoreCase) && isHDD) ||
                             (cmd._hdd.Equals("false", StringComparison.OrdinalIgnoreCase) && !isHDD))
                        ).ToList();

                        progressBar2.Invoke(() =>
                        {
                            progressBar2.Maximum = eseguibili.Count;
                            progressBar2.Value = 0;
                        });

                        foreach (var cmd in config.CMD)
                        {
                            if (!string.IsNullOrEmpty(cmd._hdd) && cmd._hdd.Equals("true", StringComparison.OrdinalIgnoreCase) && !isHDD)
                            {
                                AppendToLog($"⏭️ Salto \"{cmd.Nome}\" (Non hai un HDD)");
                                continue;
                            }

                            if (!string.IsNullOrEmpty(cmd._hdd) && cmd._hdd.Equals("false", StringComparison.OrdinalIgnoreCase) && isHDD)
                            {
                                AppendToLog($"⏭️ Salto \"{cmd.Nome}\" (Hai un HDD)");
                                continue;
                            }

                            try
                            {
                                AppendToLog($"Eseguo \"{cmd.Nome}\"");

                                ProcessStartInfo psi = new ProcessStartInfo
                                {
                                    FileName = "cmd.exe",
                                    Arguments = $"/C {cmd.Command}",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true,
                                    Verb = "runas"
                                };

                                using (Process process = Process.Start(psi))
                                {
                                    string error = await process.StandardError.ReadToEndAsync();
                                    await process.WaitForExitAsync();

                                    if (string.IsNullOrWhiteSpace(error))
                                        AppendToLog($"✅ \"{cmd.Nome}\" completato con successo");
                                    else
                                        AppendToLog($"❌ Errore durante l'esecuzione di \"{cmd.Nome}\"");
                                }
                            }
                            catch
                            {
                                AppendToLog($"❌ Errore durante l'esecuzione di \"{cmd.Nome}\"");
                            }

                            progressBar2.Invoke(() => progressBar2.Value++);
                        }
                    }
                    else
                    {
                        AppendToLog("Nessun comando CMD trovato nel JSON.");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToLog($"❌ Errore P005: {ex.Message}");
            }

            AppendToLog("Esecuzione P005 completata.");
        }

        private async Task LastTweakW10()
        {
            string url = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/WinHubXLiteOS/Win10/LastTweak.bat";
            string filePath = Path.Combine(Path.GetTempPath(), "LastTweak.bat");
            label1.Text = "Configurazione finale";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string scriptContent = await client.GetStringAsync(url);
                    await File.WriteAllTextAsync(filePath, scriptContent, Encoding.UTF8);
                }

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true,
                    Verb = "runas"
                };

                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il download o l'avvio dello script:\n{ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AppendToLog(string message)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(message + Environment.NewLine);
                }));
            }
            else
            {
                richTextBox1.AppendText(message + Environment.NewLine);
            }
        }

        private async Task DownloadTools()
        {
            string downloadPath = @"C:\ToolAIMODS";
            string url1 = "https://github.com/MrNico98/WinHubX-Resource/releases/download/WinHubX-Risorse/PowerRun.exe";
            string url2 = "https://github.com/MrNico98/WinHubX-Resource/releases/download/WinHubX-Risorse/lower-ram-usage.reg";
            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }
            string filePath1 = Path.Combine(downloadPath, "PowerRun.exe");
            using (HttpClient client = new HttpClient())
            {
                byte[] fileBytes = await client.GetByteArrayAsync(url1);
                await File.WriteAllBytesAsync(filePath1, fileBytes);
            }
            string filePath2 = Path.Combine(downloadPath, "lower-ram-usage.reg");
            using (HttpClient client = new HttpClient())
            {
                byte[] fileBytes = await client.GetByteArrayAsync(url2);
                await File.WriteAllBytesAsync(filePath2, fileBytes);
            }
        }

        private async Task DeleteTools()
        {
            string pathTools = @"C:\ToolAIMODS";
            if (Directory.Exists(pathTools))
            {
                await Task.Run(() => Directory.Delete(pathTools, true));
            }

        }

        #endregion

        private void btnBack_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = "Tools";
            form1.PnlFormLoader.Controls.Clear();
            formtools = new FormTools(form1)
            {
                Dock = DockStyle.Fill,
                TopLevel = false,
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None
            };
            form1.PnlFormLoader.Controls.Add(formtools);
            formtools.Show();
        }

        private async void btnAvvia_Click(object sender, EventArgs e)
        {
            await Task.Delay(4000);
            puntodiripristino();
            await Task.Delay(4000);
            await startall();
        }
    }
    public class WinHubXConfig
    {
        public string Content { get; set; }
        public string Description { get; set; }
        public string Order { get; set; }
        public string Registry { get; set; }
        public List<CMDCommand> CMD { get; set; }
        public List<CMDCommandArray> CMDArray { get; set; }
        public List<PowerShellCommand> PS { get; set; }
        public List<PowerShellCommandArray> PSArray { get; set; }
    }
    public class CMDCommandArray
    {
        public string Nome { get; set; }
        public List<string> Command { get; set; }
    }

    public class PowerShellCommandArray
    {
        public string Nome { get; set; }
        public List<string> Command { get; set; }
        public List<string> Command23h2 { get; set; }
        public List<string> Command24h2 { get; set; }
    }

    public class CMDCommand
    {
        public string Nome { get; set; }
        public string Command { get; set; }
        public string Command1 { get; set; }

        [JsonPropertyName("hdd")]
        public string _hdd { get; set; }
    }

    public class PowerShellCommand
    {
        public string Nome { get; set; }
        public string Command { get; set; }
        [JsonPropertyName("4ram")]
        public string _4ram { get; set; }
        [JsonPropertyName("hdd")]
        public string _hdd { get; set; }
    }
}
