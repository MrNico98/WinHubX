using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO.Compression;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using WinHubX.Forms.Base;

namespace WinHubX
{
    static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();
        [STAThread]
        static void Main(string[] args)
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ?? "";
            char pathSeparator = Path.PathSeparator;

            if (!currentPath.Split(pathSeparator).Any(p => p.Equals(exePath, StringComparison.OrdinalIgnoreCase)))
            {
                var paths = currentPath.Split(pathSeparator)
                                       .Where(p => !p.Contains("WinHubX", StringComparison.OrdinalIgnoreCase))
                                       .ToList();
                paths.Add(exePath);
                string newPath = string.Join(pathSeparator.ToString(), paths);
                Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.User);
            }
            if (args.Length > 0)
            {
                _ = AllocConsole();
                Task.Run(() => ProcessCommandLineArgs(args)).Wait();
                _ = Task.Delay(8000);
                Console.WriteLine("Premi un tasto per uscire...");
                _ = Console.ReadKey();
                _ = FreeConsole();
            }
            else
            {
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                ThemeManager.SetTheme(Properties.Settings.Default.DarkTheme);
                Application.Run(new Form1());
            }
        }

        static void ProcessCommandLineArgs(string[] args)
        {
            string receivedArgs = string.Join(", ", args);
            string command = args[0].ToLower();

            switch (command)
            {
                case "/help":
                    ShowHelp();
                    break;
                case "/statoattivazione":
                    ShowActivationStatus();
                    break;
                case "/aggiornamentolite":
                    if (args.Length > 1)
                    {
                        string isoPath = args[1];
                        AggiornaLite(isoPath);
                    }
                    else
                    {
                        Console.WriteLine("Errore: devi specificare un percorso per il file ISO.\nEsempio:\n" +
                                          "winhubx /aggiornamentolite \"C:\\Users\\Download\\isolite.iso\"");
                    }
                    break;
                case "/bios":
                    VaiNelBios();
                    break;
                case "/verificaram":
                    VerificaRam();
                    break;
                case "/cronologiadefender":
                    DefenderHistory();
                    break;
                case "/batteria":
                    ReportBatteria();
                    break;
                case "/temp":
                    PuliziaCartellaTemp();
                    break;
                case "/deallocati":
                    FileDeallocati();
                    break;
                case "/driver":
                    SalvaDriver();
                    break;
                case "/defenderoff":
                    Task.Run(() => DefenderOff()).Wait();
                    break;
                case "/defenderon":
                    Task.Run(() => DefenderOn()).Wait();
                    break;
                case "/puliziaupdate":
                    PulisciUpdate();
                    break;
                case "/importasettaggi":
                    if (args.Length > 1)
                    {
                        string pathDat = args[1];
                        ImportaSettaggi(pathDat);
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("Errore: devi specificare un percorso per il file .dat", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                default:
                    Console.WriteLine("Comando non riconosciuto. Usa '/help' per visualizzare i comandi disponibili.", "Errore Comando", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
        static void ImportaSettaggi(string pathDat)
        {
            if (!File.Exists(pathDat))
            {
                Console.WriteLine("Errore: Il file specificato non esiste.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var form1 = new Form1();
            var formSettaggi = new FormSettaggi(form1);
            formSettaggi.ImportaSettaggiDaPercorso(pathDat);
        }

        static async Task DefenderOn()
        {
            Console.WriteLine("Inizio procedura Defender ON...");

            if (IsWindowsServer())
            {
                Console.WriteLine("Windows Server non supportato.");
                return;
            }

            string arch = GetArchitecture();
            Console.WriteLine($"Architettura rilevata: {arch}");

            string url = "https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json";

            string tempPath = Path.Combine(Path.GetTempPath(), "DefNot.zip");
            string extractPath = Path.Combine(Path.GetTempPath(), "DefNotExtracted");

            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(url);
                JObject data = JObject.Parse(json);

                string? downloadUrl = arch switch
                {
                    "x64" => data["Defnot"]?["DefNotx64"]?.ToString(),
                    "x86" => data["Defnot"]?["DefNotx86"]?.ToString(),
                    "arm64" => data["Defnot"]?["DefNotarm"]?.ToString(),
                    _ => null
                };

                if (string.IsNullOrEmpty(downloadUrl))
                {
                    Console.WriteLine("URL non trovato per l'architettura.");
                    return;
                }

                Console.WriteLine("Download in corso...");

                using (var response = await client.GetAsync(downloadUrl))
                {
                    _ = response.EnsureSuccessStatusCode();
                    await using var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    await response.Content.CopyToAsync(fs);
                }
                if (Directory.Exists(extractPath))
                    Directory.Delete(extractPath, true);
                Console.WriteLine("Eseguo comando...");
                ZipFile.ExtractToDirectory(tempPath, extractPath);
                string exePath = Path.Combine(extractPath, "defendnot-loader.exe");

                if (!File.Exists(exePath))
                {
                    Console.WriteLine("File defendnot-loader.exe non trovato.");
                    return;
                }
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = exePath;
                    process.StartInfo.Arguments = "--disable";
                    process.StartInfo.Verb = "runas";
                    _ = process.Start();
                    await process.WaitForExitAsync();
                }

                Console.WriteLine("Processo completato.");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore: " + ex.Message);
            }
            finally
            {
                try
                {
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);

                    if (Directory.Exists(extractPath))
                        Directory.Delete(extractPath, true);
                }
                catch (Exception cleanupEx)
                {
                    Console.WriteLine("Errore nella pulizia: " + cleanupEx.Message);
                }
            }
        }

        static bool IsWindowsServer()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                foreach (var os in searcher.Get())
                {
                    var productType = Convert.ToInt32(os["ProductType"]);
                    return productType != 1;
                }
            }
            catch
            {
            }
            return false;
        }

        static async Task DefenderOff()
        {
            Console.WriteLine("Inizio procedura Defender OFF...");

            if (IsWindowsServer())
            {
                Console.WriteLine("Windows Server non supportato.");
                return;
            }

            string arch = GetArchitecture();
            Console.WriteLine($"Architettura rilevata: {arch}");

            string url = "https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json";

            string tempPath = Path.Combine(Path.GetTempPath(), "DefNot.zip");
            string extractPath = Path.Combine(Path.GetTempPath(), "DefNotExtracted");

            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(url);
                JObject data = JObject.Parse(json);

                string? downloadUrl = arch switch
                {
                    "x64" => data["Defnot"]?["DefNotx64"]?.ToString(),
                    "x86" => data["Defnot"]?["DefNotx86"]?.ToString(),
                    "arm64" => data["Defnot"]?["DefNotarm"]?.ToString(),
                    _ => null
                };

                if (string.IsNullOrEmpty(downloadUrl))
                {
                    Console.WriteLine("URL non trovato per l'architettura.");
                    return;
                }

                Console.WriteLine("Download in corso...");

                using (var response = await client.GetAsync(downloadUrl))
                {
                    _ = response.EnsureSuccessStatusCode();
                    await using var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    await response.Content.CopyToAsync(fs);
                }
                if (Directory.Exists(extractPath))
                    Directory.Delete(extractPath, true);
                Console.WriteLine("Eseguo comando...");
                ZipFile.ExtractToDirectory(tempPath, extractPath);
                string exePath = Path.Combine(extractPath, "defendnot-loader.exe");

                if (!File.Exists(exePath))
                {
                    Console.WriteLine("File defendnot-loader.exe non trovato.");
                    return;
                }
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = exePath;
                    process.StartInfo.Arguments = "--name \"WinHubX\"";
                    process.StartInfo.Verb = "runas";
                    _ = process.Start();
                    await process.WaitForExitAsync();
                }

                Console.WriteLine("Processo completato.");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore: " + ex.Message);
            }
            finally
            {
                try
                {
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);

                    if (Directory.Exists(extractPath))
                        Directory.Delete(extractPath, true);
                }
                catch (Exception cleanupEx)
                {
                    Console.WriteLine("Errore nella pulizia: " + cleanupEx.Message);
                }
            }
        }

        static string GetArchitecture()
        {
            if (RuntimeInformation.OSArchitecture == Architecture.X64) return "x64";
            if (RuntimeInformation.OSArchitecture == Architecture.X86) return "x86";
            if (RuntimeInformation.OSArchitecture == Architecture.Arm64) return "arm64";
            return "unknown";
        }

        static void ShowHelp()
        {
            string helpMessage = "Comandi disponibili:\n" +
                                 "/help            - Mostra questo messaggio di aiuto.\n" +
                                 "/statoattivazione - Mostra lo stato di attivazione.\n" +
                                 "/bios - Vai nel bios.\n" +
                                 "/verificaram - Verifica stato ram\n" +
                                 "/cronologiadefender - Cancella cronologia minaccie defender\n" +
                                 "/puliziaupdate - Cancella file tempornaei update\n" +
                                 "/batteria - Report batteria (pc portatili)\n" +
                                 "/temp - Cancella cartelle tempornae\n" +
                                 "/deallocati - Elimina file deallocati\n" +
                                 "/driver - Salva i driver del pc\n" +
                                 "/defenderoff - Disabilita Windows Defender\n" +
                                 "/defenderon - Abilita Windows Defender\n" +
                                 "/aggiornamentolite - Upgrade in-place lite.\n";

            Console.WriteLine(helpMessage, "Aiuto", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        static void PulisciUpdate()
        {
            Console.WriteLine("Arresto del servizio Windows Update...");
            ExecutePowerShellCommand("Stop-Service", "wuauserv -Force");

            Thread.Sleep(10000);

            Console.WriteLine("Eliminazione della cartella SoftwareDistribution...");
            ExecutePowerShellCommand("Remove-Item", "C:\\Windows\\SoftwareDistribution\\Download -Recurse -Force");

            Console.WriteLine("Riavvio del servizio Windows Update...");
            ExecutePowerShellCommand("Start-Service", "wuauserv");

            Console.WriteLine("Operazione completata.");
        }

        private static void ExecutePowerShellCommand(string command, string arguments)
        {
            using (Process process = new Process())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"{command} {arguments}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Verb = "runas"
                };

                process.StartInfo = startInfo;
                _ = process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                Console.WriteLine("Output PowerShell: " + output);
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("Errore PowerShell: " + error);
                }
            }
        }
        static void SalvaDriver()
        {
            try
            {
                string driverDirectory = @"C:\DriverPC";
                if (!Directory.Exists(driverDirectory))
                {
                    _ = Directory.CreateDirectory(driverDirectory);
                    Console.WriteLine($"Cartella creata: {driverDirectory}");
                }
                var dismProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = "dism.exe",
                    Arguments = $"/online /export-driver /destination:{driverDirectory}",
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    Verb = "runus",
                    CreateNoWindow = false
                });
                string outputPath = Path.Combine(driverDirectory, "driver.txt");
                using (var commandProcess = new Process())
                {
                    commandProcess.StartInfo.FileName = "cmd.exe";
                    commandProcess.StartInfo.Arguments = $"/c driverquery > \"{outputPath}\"";
                    commandProcess.StartInfo.UseShellExecute = false;
                    commandProcess.StartInfo.CreateNoWindow = true;
                    _ = commandProcess.Start();
                    commandProcess.WaitForExit();
                }
            }
            finally
            {
                Console.WriteLine("Trovi la cartella in C:\\DriverPC");
                Console.WriteLine("Per ripristinare tutti i driver, salvati la cartella su USB e, con ISO installata, usa \"pnputil /add-driver 'percorsodriver\\*.inf' /subdirs /install /reboot\".");
            }
        }
        static void FileDeallocati()
        {
            string fileName = "cipher.exe";
            string arguments = "/w:c";
            ProcessStartInfo psi = new ProcessStartInfo(fileName, arguments);
            psi.UseShellExecute = false;
            psi.CreateNoWindow = false;
            psi.Verb = "runus";
            psi.RedirectStandardOutput = false;
            psi.RedirectStandardError = false;
            using (Process process = new Process())
            {
                process.StartInfo = psi;
                _ = process.Start();
            }
        }
        static void PuliziaCartellaTemp()
        {
            try
            {
                string tempPath = Environment.GetEnvironmentVariable("TEMP");
                string systemTempPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\Temp";
                _ = Process.Start("cmd.exe", "/c robocopy \"" + tempPath + "\" NUL /mir /njh /njs /np /r:1 /w:1 && rmdir /s /q \"" + tempPath + "\"");
                _ = Process.Start("cmd.exe", "/c robocopy \"" + systemTempPath + "\" NUL /mir /njh /njs /np /r:1 /w:1 && rmdir /s /q \"" + systemTempPath + "\"");

                Console.WriteLine("Temp folders cleared successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing temp folders: {ex.Message}");
            }
        }

        static void ReportBatteria()
        {
            GenerateBatteryReport(@"C:\battery_report.html");
        }

        private static void GenerateBatteryReport(string filePath)
        {
            string command = $"powercfg /batteryreport /output \"{filePath}\"";

            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Verb = "runas"
            };

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo = psi;
                    _ = process.Start();
                    process.StandardInput.WriteLine(command);
                    process.StandardInput.WriteLine("exit");
                    process.StandardInput.Flush();
                    process.StandardInput.Close();
                    process.WaitForExit();
                }

                if (File.Exists(filePath))
                {
                    Console.WriteLine($"Trovi il report della batteria in {filePath}.");
                }
                else
                {
                    Console.WriteLine("Impossibile generare il report della batteria.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore: {ex.Message}");
            }
        }

        static void DefenderHistory()
        {
            bool clearAV = true;
            bool clearCFA = true;
            bool removeTask = true;

            string scans = "C:\\ProgramData\\Microsoft\\Windows Defender\\Scans";
            string service = Path.Combine(scans, "History", "Service");
            string db = Path.Combine(scans, "mpenginedb.db*");
            string taskName = "DWDH";

            string command = "cmd.exe /c ";
            if (clearAV)
                command += $"rd /s /q \"{service}\" & ";
            if (clearCFA)
                command += $"del /f \"{db}\" & ";
            if (removeTask)
                command += $"schtasks /delete /f /tn {taskName}";

            _ = Process.Start(new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-Command \"{command}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            });

            Console.Write("A restart is required to clear the Protection history. Enter y to restart now: ");
            string choice = Console.ReadLine().ToLower();
            if (choice == "y")
            {
                _ = Process.Start(new ProcessStartInfo
                {
                    FileName = "shutdown",
                    Arguments = "/r /t 0",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }
        }

        static void VerificaRam()
        {
            try
            {
                string fileName = @"C:\Windows\System32\mdsched.exe";

                if (!System.IO.File.Exists(fileName))
                {
                    Console.WriteLine("Il file mdsched.exe non esiste nel percorso specificato.");
                    return;
                }

                ProcessStartInfo psi = new ProcessStartInfo(fileName)
                {
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using (Process process = new Process())
                {
                    process.StartInfo = psi;
                    _ = process.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore: " + ex.Message);
            }
        }

        static void VaiNelBios()
        {
            string fileName = "shutdown.exe";
            string arguments = "/t 0 /r /fw";
            ProcessStartInfo psi = new ProcessStartInfo(fileName, arguments);
            psi.UseShellExecute = false;
            psi.CreateNoWindow = false;
            psi.Verb = "runus";
            psi.RedirectStandardOutput = false;
            psi.RedirectStandardError = false;
            using (Process process = new Process())
            {
                process.StartInfo = psi;
                _ = process.Start();
            }
        }
        static void AggiornaLite(string? isoPath = null)
        {
            string upgradeFolder = @"C:\StartOnUpgrade";
            string startBatPath = Path.Combine(upgradeFolder, "Start.bat");
            string extractPath = Path.Combine(Path.GetTempPath(), "WinUpgrade");

            try
            {
                if (!Directory.Exists(upgradeFolder))
                {
                    _ = Directory.CreateDirectory(upgradeFolder);
                }
                if (!File.Exists(startBatPath))
                {
                    string startBatContent = @"@echo off" + Environment.NewLine +
                                             "setlocal EnableDelayedExpansion" + Environment.NewLine +
                                             "" + Environment.NewLine +
                                             "rem Ask for admin privileges" + Environment.NewLine +
                                             "set \"params=%*\"" + Environment.NewLine +
                                             "cd /d \"%~dp0\" && ( if exist \"%temp%\\getadmin.vbs\" del \"%temp%\\getadmin.vbs\" ) && fsutil dirty query %systemdrive%  1>nul 2>nul || ( echo Set UAC = CreateObject^(\"Shell.Application\"^) : UAC.ShellExecute \"cmd.exe\", \"/c cd \"\"%~sdp0\"\" && %~s0 %params%\", \"\", \"runas\", 1 >> \"%temp%\\getadmin.vbs\" && \"%temp%\\getadmin.vbs\" && exit /B )" + Environment.NewLine +
                                             "" + Environment.NewLine +
                                             "if exist C:\\Windows\\start.ps1 (" + Environment.NewLine +
                                             "    powershell -ExecutionPolicy Bypass -File C:\\Windows\\start.ps1" + Environment.NewLine +
                                             ")" + Environment.NewLine +
                                             "if exist C:\\Windows\\start10.ps1 (" + Environment.NewLine +
                                             "    powershell -ExecutionPolicy Bypass -File C:\\Windows\\start10.ps1" + Environment.NewLine +
                                             ")" + Environment.NewLine;

                    File.WriteAllText(startBatPath, startBatContent);
                }
                string regCmd = @"reg add HKLM\Software\Microsoft\Windows\CurrentVersion\RunOnce /v StartPostUpgrade /t REG_SZ /d ""cmd /c C:\StartOnUpgrade\start.bat"" /f";
                _ = RunCommand("cmd.exe", $"/c {regCmd}");
                if (!Directory.Exists(extractPath))
                {
                    _ = Directory.CreateDirectory(extractPath);
                }
                string extractorPath = null;
                string extractorArgs = null;

                string sevenZipPath = @"C:\Program Files\7-Zip\7z.exe";
                string winRarPath = @"C:\Program Files\WinRAR\WinRAR.exe";

                if (File.Exists(sevenZipPath))
                {
                    extractorPath = sevenZipPath;
                    extractorArgs = $"x \"{isoPath}\" -o\"{extractPath}\" sources\\* -y";
                }
                else if (File.Exists(winRarPath))
                {
                    extractorPath = winRarPath;
                    extractorArgs = $"x \"{isoPath}\" \"{extractPath}\\sources\" sources\\*";
                }
                else
                {
                    Console.WriteLine("Né 7-Zip né WinRAR sono installati! Installane uno per procedere.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                _ = RunCommand(extractorPath, extractorArgs);
                string sourcesPath = Path.Combine(extractPath, "sources");
                string appraiserBak = Path.Combine(sourcesPath, "appraiserres.dll.bak");
                string appraiserDll = Path.Combine(sourcesPath, "appraiserres.dll");

                if (File.Exists(appraiserBak))
                {
                    if (File.Exists(appraiserDll))
                    {
                        File.Delete(appraiserDll);
                    }
                    File.Move(appraiserBak, appraiserDll);
                }
                string setupPath = Path.Combine(sourcesPath, "setupprep.exe");

                if (File.Exists(setupPath))
                {
                    _ = Process.Start(setupPath, "/product server");
                }
                else
                {
                    Console.WriteLine("File setupprep.exe non trovato!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        static string RunCommand(string fileName, string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    return reader.ReadToEnd();
                }
            }
        }

        static void ShowActivationStatus()
        {
            string fileName = "WinHubXStatoAttivazione.cmd";
            string resourceName = "WinHubX.Resources.WinHubXStatoAttivazione.cmd"; string tempPath = Path.GetTempPath();
            string tempFilePath = Path.Combine(tempPath, fileName);
            try
            {
                using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (resourceStream != null)
                    {
                        using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                        {
                            resourceStream.CopyTo(fileStream);
                        }
                    }
                }
                _ = Process.Start(tempFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nell'avviare l'applicazione: {ex.Message}");
            }
        }
    }
}
