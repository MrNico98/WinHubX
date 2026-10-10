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
        private static bool IsExpectedSystemInformationFailure(Exception exception) =>
            exception is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or System.ComponentModel.Win32Exception
                or System.Runtime.InteropServices.COMException
                or System.Management.ManagementException
                or InvalidOperationException
                or NotSupportedException
                or TimeoutException
                or ArgumentException;

        private static readonly TimeSpan HardwareSnapshotLifetime = TimeSpan.FromHours(1);
        private readonly string jsonPath;
        private bool verificationStarted;

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

            Shown += FormHome_Shown;
        }

        private async void FormHome_Shown(object? sender, EventArgs e)
        {
            if (verificationStarted)
                return;

            verificationStarted = true;
            if (IsSnapshotFresh())
            {
                ShowResultsUI();
                AggiornaRiassunto();
            }
            else
            {
                try
                {
                    await VerificaSistemaAsync();
                }
                catch (Exception ex) when (IsExpectedSystemInformationFailure(ex))
                {
                    Debug.WriteLine($"Verifica hardware non riuscita: {ex}");
                }
            }
        }

        private bool IsSnapshotFresh()
        {
            try
            {
                return File.Exists(jsonPath)
                    && DateTime.UtcNow - File.GetLastWriteTimeUtc(jsonPath) < HardwareSnapshotLifetime;
            }
            catch (IOException)
            {
                return false;
            }
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
            string folderPath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WinHubX",
                "Computer"
            );

            Directory.CreateDirectory(folderPath);
            return Path.Join(folderPath, "osehardware.json");
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
                var progress = new Progress<int>(step => cuiProgressTrackerHorizontal1.TasksProgress = step);
                var systemData = await RecuperaInformazioniSistemaAsync(progress);

                await SalvaDatiSistemaAsync(systemData);

                cuiProgressTrackerHorizontal1.TasksProgress = 5;

                ShowResultsUI();
                AggiornaRiassunto();
            }
            catch (Exception ex) when (IsExpectedSystemInformationFailure(ex))
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
                labelcpu.Text = "      " + LanguageManager.FormatTranslation("FormHome", "cpu", cpu);

                labelram.Text = "      " + LanguageManager.FormatTranslation("FormHome", "ram", ram);

                labeldisco.Text = "      " + LanguageManager.FormatTranslation("FormHome", "disco", disco);

                labelos.Text = "      " + LanguageManager.FormatTranslation("FormHome", "os", os);

                labelwindows.Text = "      " + LanguageManager.FormatTranslation("FormHome", "windows", windows);

                labeloffice.Text = "      " + LanguageManager.FormatTranslation("FormHome", "office", office);
            }
            catch (Exception ex) when (IsExpectedSystemInformationFailure(ex))
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

        private async Task<object> RecuperaInformazioniSistemaAsync(IProgress<int> progress)
        {
            return await Task.Run<object>(() =>
            {
                string osInfo = GetOSInfo();
                string architettura = Environment.Is64BitOperatingSystem ? "64" : "32";
                progress.Report(1);

                string cpuName = GetCPUName();
                string ramInfo = GetRAMInfo();
                string diskInfo = GetSystemDiskType();
                progress.Report(2);

                string windowsActivation = GetWindowsActivationStatus();
                progress.Report(3);

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

                progress.Report(4);
                progress.Report(5);

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
            });
        }



        private async Task SalvaDatiSistemaAsync(object data)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(jsonPath, json);
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
            catch (Exception ex) when (IsExpectedSystemInformationFailure(ex))
            {
                Debug.WriteLine($"Lettura informazioni OS tramite WMI non riuscita: {ex}");
            }
            return "Informazioni OS non disponibili";
        }

        private string GetCPUName()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher(
                    "SELECT Name, NumberOfLogicalProcessors FROM Win32_Processor");
                using ManagementObjectCollection processors = searcher.Get();
                var processorInfo = new List<CpuProcessorInfo>();
                foreach (ManagementObject processor in processors)
                {
                    using (processor)
                    {
                        string name = processor["Name"]?.ToString()?.Trim() ?? string.Empty;
                        int.TryParse(processor["NumberOfLogicalProcessors"]?.ToString(), out int logicalProcessorCount);
                        processorInfo.Add(new CpuProcessorInfo(name, logicalProcessorCount));
                    }
                }

                return CpuInfoFormatter.Format(processorInfo);
            }
            catch (Exception ex) when (IsExpectedSystemInformationFailure(ex))
            {
                Debug.WriteLine($"Lettura nome CPU tramite WMI non riuscita: {ex}");
            }
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
                double totalGiB = totalBytes / (1024d * 1024 * 1024);
                return $"{totalGiB:0.0} GB";
            }
            catch (Exception ex) when (IsExpectedSystemInformationFailure(ex))
            {
                Debug.WriteLine($"Lettura memoria RAM tramite WMI non riuscita: {ex}");
                return "Sconosciuta";
            }
        }



        private string GetSystemDiskType()
        {
            try
            {

                string? systemDrive = Path.GetPathRoot(Environment.SystemDirectory)?.TrimEnd('\\');
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
                            interfaceType.Equals("IDE", StringComparison.OrdinalIgnoreCase))
                            return $"HDD ({model})";

                        if (interfaceType.Equals("SATA", StringComparison.OrdinalIgnoreCase))
                            return $"Unità SATA ({model})";

                        return $"Sconosciuto ({model})";
                    }
                }
            }
            catch (Exception ex) when (IsExpectedSystemInformationFailure(ex))
            {
                Debug.WriteLine($"Lettura tipo disco tramite WMI non riuscita: {ex}");
            }

            return "Sconosciuto";
        }

        private string GetWindowsActivationStatus()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = Path.Join(Environment.SystemDirectory, "cscript.exe"),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                psi.ArgumentList.Add("//nologo");
                psi.ArgumentList.Add(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "slmgr.vbs"));
                psi.ArgumentList.Add("/xpr");

                using var process = Process.Start(psi)
                    ?? throw new InvalidOperationException("Impossibile avviare cscript.");
                Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                Task<string> errorTask = process.StandardError.ReadToEndAsync();
                process.WaitForExit();
                string output = outputTask.GetAwaiter().GetResult().ToLowerInvariant();
                string error = errorTask.GetAwaiter().GetResult();
                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException($"Verifica licenza terminata con codice {process.ExitCode}: {error}");
                }

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
            catch (Exception ex) when (IsExpectedSystemInformationFailure(ex))
            {
                Debug.WriteLine($"Verifica licenza Windows con cscript non riuscita; provo WMI: {ex}");
            }

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
            catch (Exception ex) when (IsExpectedSystemInformationFailure(ex))
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


            }
            catch (Exception ex) when (IsExpectedSystemInformationFailure(ex))
            {
                Debug.WriteLine($"Verifica attivazione Office tramite WMI non riuscita: {ex}");
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
            catch (Exception ex) when (IsExpectedSystemInformationFailure(ex))
            {
                Debug.WriteLine($"Ricerca installazione Office nel registro non riuscita: {ex}");
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
            catch (Exception ex) when (IsExpectedSystemInformationFailure(ex))
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
            catch (Exception ex) when (IsExpectedSystemInformationFailure(ex))
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
            await VerificaSistemaAsync();
        }
    }
}
