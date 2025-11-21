using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using System.Text.Json;
using System.Windows.Forms;
using WinHubX.Impostazioni;

namespace WinHubX
{
    public partial class FormHome : Form
    {
        private readonly string jsonPath;

        public FormHome()
        {
            InitializeComponent();
            LanguageManager.LoadLanguageFromSettings();
            btnVerificaVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Riesegui la verifica PC",
                "en" => "  Rerun PC verification",
                _ => btnVerificaVerdi.Content
            };

            if (ThemeManager.IsDarkTheme)
            {
                labelcpu.Image = Properties.Resources.pngCpuHome;
                labelram.Image = Properties.Resources.pngRamHome;
                labeldisco.Image = Properties.Resources.pngHDDHome;
                labelos.Image = Properties.Resources.pngOSHome;
                labelwindows.Image = Properties.Resources.pngStatoWindowsHome;
                labeloffice.Image = Properties.Resources.pngStatoOfficeHome;
            }
            else
            {
                labelcpu.Image = Properties.Resources.pngCpuBlackFormHome;
                labelram.Image = Properties.Resources.pngRamBlackFormHome;
                labeldisco.Image = Properties.Resources.pngDiscoBlackFormHome;
                labelos.Image = Properties.Resources.pngOSBlackFormHome;
                labelwindows.Image = Properties.Resources.pngStatoWindowsBlackFormHome;
                labeloffice.Image = Properties.Resources.pngStatoOfficeBlackFormHome;
            }
            jsonPath = PrepareJsonPath();

            InitializeProgressTracker();
            InitializeUI();

            ApplicaTraduzioniUI();

            if (File.Exists(jsonPath))
            {
                ShowResultsUI();
                AggiornaRiassunto();
            }
            else
                _ = VerificaSistemaAsync();
        }
        private void ApplicaTraduzioniUI()
        {
            cuiProgressTrackerHorizontal1.Tasks = new[]
            {
        LanguageManager.GetTranslation("FormHome", "ProgressStep1"),
        LanguageManager.GetTranslation("FormHome", "ProgressStep2"),
        LanguageManager.GetTranslation("FormHome", "ProgressStep3"),
        LanguageManager.GetTranslation("FormHome", "ProgressStep4"),
        LanguageManager.GetTranslation("FormHome", "ProgressStep5")
    };
        }
        private string PrepareJsonPath()
        {
            string folderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WinHubX",
                "Computer"
            );

            Directory.CreateDirectory(folderPath);
            return Path.Combine(folderPath, "osehardware.json");
        }
        private void InitializeProgressTracker()
        {
            cuiProgressTrackerHorizontal1.Tasks = new[]
            {
        "Recupero OS",
        "Recupero HW",
        "Verifico Attivazione Windows",
        "Verifico Attivazione Office",
        "Fine"
    };
            cuiProgressTrackerHorizontal1.TasksProgress = 0;
        }

        private void InitializeUI()
        {
            cuiSpinner1.Visible = true;
            cuiProgressTrackerHorizontal1.Visible = true;
            labelverifica.Visible = true;
            label1.Visible = false;
            btnVerificaVerdi.Visible = false;
            labelcpu.Visible = false;
            labelram.Visible = false;
            labeldisco.Visible = false;
            labelos.Visible = false;
            labelwindows.Visible = false;
            labeloffice.Visible = false;
            label7.Visible = false;
        }
        private void ShowResultsUI()
        {
            cuiSpinner1.Visible = false;
            cuiProgressTrackerHorizontal1.Visible = false;
            labelverifica.Visible = false;
            label1.Visible = true;
            btnVerificaVerdi.Visible = true;
            labelcpu.Visible = true;
            labelram.Visible = true;
            labeldisco.Visible = true;
            labelos.Visible = true;
            labelwindows.Visible = true;
            labeloffice.Visible = true;
            label7.Visible = true;
        }

        public async Task VerificaSistemaAsync()
        {
            try
            {
                var systemData = await RecuperaInformazioniSistemaAsync();

                await SalvaDatiSistemaAsync(systemData);

                cuiProgressTrackerHorizontal1.TasksProgress = 5;
                await Task.Delay(1500);

                ShowResultsUI();
                AggiornaRiassunto();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la verifica: {ex.Message}",
                    "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AggiornaRiassunto()
        {
            try
            {
                if (!File.Exists(jsonPath))
                    return;

                string jsonContent = File.ReadAllText(jsonPath);
                using JsonDocument document = JsonDocument.Parse(jsonContent);
                JsonElement root = document.RootElement;
                string nd = LanguageManager.GetTranslation("FormHome", "NonDisponibile");
                string os = root.GetProperty("OperatingSystem").GetString() ?? nd;

                JsonElement hardware = root.GetProperty("Hardware");
                string cpu = hardware.GetProperty("CPU").GetString() ?? nd;
                string ram = hardware.GetProperty("RAM").GetString() ?? nd;
                string disco = hardware.GetProperty("Disk").GetString() ?? nd;

                JsonElement activation = root.GetProperty("Activation");
                string windows = activation.GetProperty("Windows").GetString() ?? nd;
                string office = activation.GetProperty("Office").GetString() ?? nd;
                labelcpu.Text = "      " + string.Format(
                    LanguageManager.GetTranslation("FormHome", "cpu"),
                    cpu
                );

                labelram.Text = "      " + string.Format(
                    LanguageManager.GetTranslation("FormHome", "ram"),
                    ram
                );

                labeldisco.Text = "      " + string.Format(
                    LanguageManager.GetTranslation("FormHome", "disco"),
                    disco
                );

                labelos.Text = "      " + string.Format(
                    LanguageManager.GetTranslation("FormHome", "os"),
                    os
                );

                labelwindows.Text = "      " + string.Format(
                    LanguageManager.GetTranslation("FormHome", "windows"),
                    windows
                );

                labeloffice.Text = "      " + string.Format(
                    LanguageManager.GetTranslation("FormHome", "office"),
                    office
                );
            }
            catch (Exception ex)
            {
                string errore = $"{LanguageManager.GetTranslation("FormHome", "ErroreCaricamento")}:\n{ex.Message}";
                labelcpu.Text = errore;
                labelram.Text = errore;
                labeldisco.Text = errore;
                labelos.Text = errore;
                labelwindows.Text = errore;
                labeloffice.Text = errore;
            }
        }

        private async Task<object> RecuperaInformazioniSistemaAsync()
        {

            await Task.Delay(3000);
            string osInfo = GetOSInfo();
            string architettura = Environment.Is64BitOperatingSystem ? "64" : "32";
            cuiProgressTrackerHorizontal1.TasksProgress = 1;


            await Task.Delay(3000);
            string cpuName = GetCPUName();
            string ramInfo = GetRAMInfo();
            string diskInfo = GetSystemDiskType();
            cuiProgressTrackerHorizontal1.TasksProgress = 2;


            await Task.Delay(2000);
            string windowsActivation = GetWindowsActivationStatus();
            cuiProgressTrackerHorizontal1.TasksProgress = 3;

 
            await Task.Delay(2000);
            string officeActivation;

            if (!IsOfficeInstalled())
            {
                officeActivation = "Non installato";
            }
            else
            {
                bool officeActivated = IsOfficeActivated();
                officeActivation = officeActivated ? "Attivato" : "Da attivare";
            }

            cuiProgressTrackerHorizontal1.TasksProgress = 4;
            await Task.Delay(2000);
            cuiProgressTrackerHorizontal1.TasksProgress = 6;
            await Task.Delay(2000);

            return new
            {
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                OperatingSystem = osInfo,
                Architettura = architettura,
                Hardware = new
                {
                    CPU = cpuName,
                    RAM = ramInfo,
                    Disk = diskInfo
                },
                Activation = new
                {
                    Windows = windowsActivation,
                    Office = officeActivation
                }
            };
        }



        private async Task SalvaDatiSistemaAsync(object data)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(jsonPath, json);
            await Task.Delay(4000); 
        }


        private string GetOSInfo()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Caption, Version, OSArchitecture, BuildNumber FROM Win32_OperatingSystem");
                foreach (var os in searcher.Get())
                {
                    string caption = os["Caption"]?.ToString() ?? "Sconosciuto";
                    string version = os["Version"]?.ToString() ?? "";
                    string build = os["BuildNumber"]?.ToString() ?? "";
                    string arch = os["OSArchitecture"]?.ToString() ?? "";

                    return $"{caption} (Versione {version}, Build {build}, {arch})";
                }
            }
            catch { }
            return "Informazioni OS non disponibili";
        }

        private string GetCPUName()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
                foreach (var item in searcher.Get())
                    return item["Name"]?.ToString() ?? "Sconosciuto";
            }
            catch { }
            return "Sconosciuto";
        }

        private string GetRAMInfo()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher(
                    "SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"
                );

                var obj = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
                if (obj == null) return "Sconosciuta";

                long totalBytes = Convert.ToInt64(obj["TotalPhysicalMemory"]);
                int totalGB = (int)Math.Round(totalBytes / (1024.0 * 1024 * 1024));

                return $"{totalGB} GB";
            }
            catch
            {
                return "Sconosciuta";
            }
        }



        private string GetSystemDiskType()
        {
            try
            {

                string systemDrive = Path.GetPathRoot(Environment.SystemDirectory)?.TrimEnd('\\');
                if (string.IsNullOrEmpty(systemDrive))
                    return "Sconosciuto";


                using var partitionSearcher = new ManagementObjectSearcher(
                    $"ASSOCIATORS OF {{Win32_LogicalDisk.DeviceID='{systemDrive}'}} WHERE AssocClass=Win32_LogicalDiskToPartition");

                foreach (var partition in partitionSearcher.Get())
                {
                    using var diskSearcher = new ManagementObjectSearcher(
                        $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partition["DeviceID"]}'}} WHERE AssocClass=Win32_DiskDriveToDiskPartition");

                    foreach (var disk in diskSearcher.Get())
                    {
                        string mediaType = disk["MediaType"]?.ToString() ?? "";
                        string model = disk["Model"]?.ToString() ?? "";
                        string interfaceType = disk["InterfaceType"]?.ToString() ?? "";

                        if (interfaceType.Equals("NVMe", StringComparison.OrdinalIgnoreCase) ||
                            model.Contains("NVMe", StringComparison.OrdinalIgnoreCase))
                            return $"NVMe ({model})";

                        if (mediaType.Contains("SSD", StringComparison.OrdinalIgnoreCase) ||
                            model.Contains("SSD", StringComparison.OrdinalIgnoreCase))
                            return $"SSD ({model})";

                        if (mediaType.Contains("HDD", StringComparison.OrdinalIgnoreCase) ||
                            mediaType.Contains("Fixed", StringComparison.OrdinalIgnoreCase) ||
                            interfaceType.Equals("IDE", StringComparison.OrdinalIgnoreCase) ||
                            interfaceType.Equals("SATA", StringComparison.OrdinalIgnoreCase))
                            return $"HDD ({model})";

                        return $"Sconosciuto ({model})";
                    }
                }
            }
            catch { }

            return "Sconosciuto";
        }

        private string GetWindowsActivationStatus()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cscript.exe",
                    Arguments = @"//nologo %windir%\system32\slmgr.vbs /xpr",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                string output = process.StandardOutput.ReadToEnd().ToLowerInvariant();
                process.WaitForExit();

                if (output.Contains("permanently activated") ||
                    output.Contains("attivato definitivamente") ||
                    output.Contains("permanentemente attivato"))
                    return "Attivato (Permanente)";

                if (output.Contains("activated") || output.Contains("attivato"))
                    return "Attivato (Temporaneo o Volume)";

                if (output.Contains("grace") || output.Contains("scade"))
                    return "Attivato (Periodo di grazia)";

                if (output.Contains("not activated") || output.Contains("non attivato"))
                    return "Non attivato";
            }
            catch { }

            try
            {
                using var searcher = new ManagementObjectSearcher(
                    "SELECT LicenseStatus, Description FROM SoftwareLicensingProduct WHERE PartialProductKey IS NOT NULL");
                foreach (ManagementObject obj in searcher.Get())
                {
                    int status = Convert.ToInt32(obj["LicenseStatus"]);
                    string desc = obj["Description"]?.ToString() ?? "";
                    if (!desc.Contains("Windows", StringComparison.OrdinalIgnoreCase)) continue;

                    return status switch
                    {
                        1 => "Attivato",
                        0 => "Non attivato",
                        5 => "Notifica - licenza scaduta o non valida",
                        _ => $"Stato {status}"
                    };
                }
            }
            catch (Exception ex)
            {
                return $"Errore durante la verifica: {ex.Message}";
            }

            return "Informazioni non disponibili";
        }

        private bool IsOfficeActivated()
        {
            try
            {
                if (!IsOfficeInstalled())
                {
                    return false;
                }

                using (var searcher = new ManagementObjectSearcher(
                    @"root\cimv2",
                    "SELECT LicenseStatus FROM SoftwareLicensingProduct WHERE (Name LIKE '%Office%') AND (PartialProductKey IS NOT NULL)"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        int status = Convert.ToInt32(obj["LicenseStatus"]);
                        if (status == 1)
                            return true; 
                    }
                }


                if (CheckOhookInstalled())
                    return true;
            }
            catch
            {

            }

            return false;
        }
        private bool IsOfficeInstalled()
        {
            try
            {
                string[] uninstallPaths = new string[]
                {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
                };

                foreach (var path in uninstallPaths)
                {
         
                    using (var baseKey64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                    {
                        using (var key = baseKey64.OpenSubKey(path))
                        {
                            if (key != null)
                            {
                                foreach (var subkeyName in key.GetSubKeyNames())
                                {
                                    using (var subkey = key.OpenSubKey(subkeyName))
                                    {
                                        var displayName = subkey?.GetValue("DisplayName") as string;
                                        if (!string.IsNullOrEmpty(displayName) && displayName.Contains("Office"))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                 
                    using (var baseKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                    {
                        using (var key = baseKey32.OpenSubKey(path))
                        {
                            if (key != null)
                            {
                                foreach (var subkeyName in key.GetSubKeyNames())
                                {
                                    using (var subkey = key.OpenSubKey(subkeyName))
                                    {
                                        var displayName = subkey?.GetValue("DisplayName") as string;
                                        if (!string.IsNullOrEmpty(displayName) && displayName.Contains("Office"))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        private bool CheckOhookInstalled()
        {
            try
            {
                string[] programPaths =
                {
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            Environment.GetEnvironmentVariable("ProgramW6432"),
            Environment.GetEnvironmentVariable("ProgramFiles(x86)")
        };


                programPaths = programPaths.Where(p => !string.IsNullOrWhiteSpace(p))
                                           .Distinct()
                                           .ToArray();


                int[] officeVersions = { 15, 16 };
                string[] systemFolders = { "System", "SystemX86" };
                string[] officeRoots = { "Office 15", "Office" };


                foreach (var path in programPaths)
                {
                    foreach (var version in officeVersions)
                    {
                        string searchPath = Path.Combine(path, $"Microsoft Office\\Office{version}");
                        if (Directory.Exists(searchPath))
                        {
                            if (Directory.EnumerateFiles(searchPath, "sppc*dll", SearchOption.AllDirectories).Any())
                                return true;
                        }
                    }

                    foreach (var sys in systemFolders)
                    {
                        foreach (var root in officeRoots)
                        {
                            string searchPath = Path.Combine(path, $"Microsoft {root}\\root\\vfs\\{sys}");
                            if (Directory.Exists(searchPath))
                            {
                                if (Directory.EnumerateFiles(searchPath, "sppc*dll", SearchOption.AllDirectories).Any())
                                    return true;
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            return false;
        }

        /*
        private void tgWinHubX_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://telegram.me/WinHubXbot",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        */

        private void btnKofi_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://ko-fi.com/winhubx",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnVerifica_Click(object sender, EventArgs e)
        {
            cuiSpinner1.Visible = true;
            cuiProgressTrackerHorizontal1.Visible = true;
            labelverifica.Visible = true;
            label1.Visible = false;
            btnVerificaVerdi.Visible = false;
            labelcpu.Visible = false;
            labelram.Visible = false;
            labeldisco.Visible = false;
            labelos.Visible = false;
            labelwindows.Visible = false;
            labeloffice.Visible = false;
            label7.Visible = false;
            await Task.Delay(2000);
            VerificaSistemaAsync();
        }
    }
}
