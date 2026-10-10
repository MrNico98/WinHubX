using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO.Compression;
using System.Management;
using System.Security.Cryptography;
using System.Security.Policy;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.InstallaComponenti
{
    public partial class FormInstallaComponenti : Form
    {
        private static readonly HttpClient ResourceClient = new(new SocketsHttpHandler
        {
            AllowAutoRedirect = false,
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            AutomaticDecompression = System.Net.DecompressionMethods.All
        })
        {
            Timeout = TimeSpan.FromMinutes(2)
        };

        public FormInstallaComponenti()
        {
            InitializeComponent();
            btnInstallaVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Installa",
                "en" => "  Install",
                _ => btnInstallaVerdi.Content
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }


        private async void btnInstalla_Click(object sender, EventArgs e)
        {
            bool doDefender = checkBox_microsoftdefender.Checked;
            bool doWinget = checkBox_winget.Checked;
            bool doStore = checkBox_MicrosoftStore.Checked;
            if (!doDefender && !doWinget && !doStore)
            {
                MessageBox.Show("Seleziona almeno un’operazione da eseguire.");
                return;
            }

            HardwareInfo? hardwareInfo = null;
            if (doDefender)
                hardwareInfo = await OttieniHardwareInfoAsync();
            if (doDefender && hardwareInfo is not null)
                await DefenderOn(hardwareInfo);

            if (doWinget)
                await WingetInstall();

            if (doStore)
                await MicrosoftStoreInstall();
        }
        private async Task WingetInstall()
        {
            (string Url, string Name)[] packages =
            {
                ("https://aka.ms/getwinget", "Microsoft.DesktopAppInstaller_8wekyb3d8bbwe.msixbundle"),
                ("https://aka.ms/Microsoft.VCLibs.x64.14.00.Desktop.appx", "Microsoft.VCLibs.x64.14.00.Desktop.appx"),
                ("https://github.com/microsoft/microsoft-ui-xaml/releases/download/v2.8.6/Microsoft.UI.Xaml.2.8.x64.appx", "Microsoft.UI.Xaml.2.8.x64.appx")
            };
            string componentFolder = Path.Join(Path.GetTempPath(), "WinHubX", "Components");
            Directory.CreateDirectory(componentFolder);
            var localFiles = new List<string>(packages.Length);

            foreach ((string url, string name) in packages)
            {
                try
                {
                    string filePath = Path.Join(componentFolder, name);
                    await DownloadManager.DownloadFileAsync(url, filePath, CancellationToken.None, autoParallel: false);
                    localFiles.Add(filePath);
                }
                catch (Exception ex) when (ex is HttpRequestException
                    or IOException
                    or UnauthorizedAccessException
                    or OperationCanceledException
                    or InvalidOperationException
                    or ArgumentException)
                {
                    _ = MessageBox.Show($"Error: {name}\n{ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            foreach (string file in localFiles)
            {
                try { await AddAppxPackageAsync(file); }
                catch (Exception ex) when (ex is System.ComponentModel.Win32Exception
                    or IOException
                    or UnauthorizedAccessException
                    or OperationCanceledException
                    or InvalidOperationException)
                {
                    _ = MessageBox.Show($"Error: {Path.GetFileName(file)}\n{ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            try { Directory.Delete(componentFolder, recursive: true); }
            catch (IOException) { /* cleanup best effort; files are temporary */ }
        }

        private static async Task AddAppxPackageAsync(string packagePath)
        {
            string escapedPath = packagePath.Replace("'", "''", StringComparison.Ordinal);
            using Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                    UseShellExecute = true,
                    Verb = "runas",
                    CreateNoWindow = true
                }
            };
            process.StartInfo.ArgumentList.Add("-NoProfile");
            process.StartInfo.ArgumentList.Add("-NonInteractive");
            process.StartInfo.ArgumentList.Add("-Command");
            process.StartInfo.ArgumentList.Add($"Add-AppxPackage -LiteralPath '{escapedPath}'");
            _ = process.Start();
            await process.WaitForExitAsync();
            if (process.ExitCode != 0)
                throw new InvalidOperationException($"Add-AppxPackage terminato con codice {process.ExitCode}.");
        }

        private async Task MicrosoftStoreInstall()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Path.Join(Environment.SystemDirectory, "WSReset.exe"),
                UseShellExecute = false,
                CreateNoWindow = true
            };
            startInfo.ArgumentList.Add("-i");

            using (Process process = new Process { StartInfo = startInfo })
            {
                _ = process.Start();
                await process.WaitForExitAsync();
                if (process.ExitCode != 0)
                    throw new InvalidOperationException($"WSReset è terminato con codice {process.ExitCode}.");
            }
            await Task.Delay(TimeSpan.FromSeconds(20));
            using (Process process = new Process { StartInfo = startInfo })
            {
                _ = process.Start();
                await process.WaitForExitAsync();
                if (process.ExitCode != 0)
                    throw new InvalidOperationException($"WSReset è terminato con codice {process.ExitCode}.");
            }

            _ = MessageBox.Show(LanguageManager.GetTranslation("FormReinstallAPP", "storeinstalling"));
            await Task.Delay(TimeSpan.FromSeconds(4));
        }

        private async Task<HardwareInfo?> OttieniHardwareInfoAsync()
        {
            string path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                       "WinHubX", "Computer", "osehardware.json");

            if (!File.Exists(path))
                throw new FileNotFoundException("Il file osehardware.json non esiste.", path);

            string json = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<HardwareInfo>(json);
        }
        private async Task DefenderOn(HardwareInfo hardwareInfo)
        {
            if (await Task.Run(IsWindowsServer))
            {
                MessageBox.Show("La funzione DefendNot non viene eseguita su Windows Server.", "WinHubX",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult consent = MessageBox.Show(
                "Questa funzione registra temporaneamente DefendNot come provider di sicurezza tramite Windows Security Center e può disattivare la protezione in tempo reale di Microsoft Defender.\n\n" +
                "Usala solo se comprendi il rischio e disponi di un metodo di ripristino. Continuare?",
                "Avviso sicurezza Defender",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (consent != DialogResult.Yes)
                return;

            string arch = hardwareInfo.Architettura;

            string url = Dipendenze.GitHubConfigUrl;

            string workDirectory = Path.Join(Path.GetTempPath(), $"WinHubX-DefNot-{Guid.NewGuid():N}");
            string tempPath = Path.Join(workDirectory, "DefNot.zip");
            string extractPath = Path.Join(workDirectory, "extracted");

            try
            {
                Directory.CreateDirectory(workDirectory);
                string json = await TrustedHttpsClient.GetStringAsync(ResourceClient, url);
                JObject data = JObject.Parse(json);

                string? downloadUrl = arch switch
                {
                    "64" => data["Defnot"]?["DefNotx64"]?.ToString(),
                    "86" => data["Defnot"]?["DefNotx86"]?.ToString(),
                    "arm64" => data["Defnot"]?["DefNotarm"]?.ToString(),
                    _ => null
                };

                if (string.IsNullOrEmpty(downloadUrl))
                {
                    return;
                }

                if (!Uri.TryCreate(downloadUrl, UriKind.Absolute, out Uri? resourceUri)
                    || resourceUri.Scheme != Uri.UriSchemeHttps
                    || !resourceUri.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("La risorsa DefendNot deve provenire da GitHub tramite HTTPS.");

                await DownloadManager.DownloadFileAsync(resourceUri.ToString(), tempPath,
                    CancellationToken.None, autoParallel: false, useBits: false);
                string expectedHash = arch switch
                {
                    "64" => "A7BC789268A8933ACACACA2D0E7BBC8D6C1AAD5560FBCA2D4F8C152A4D4493",
                    "86" => "BCFD08104C863679A33FAF8E923C860FE2378FE6D6F0A031972D0DBB20930668",
                    "arm64" => "7B09DDDE16DD4D3E7C07FA4856F0956A11D6F838DCFED32D5B3FA0C777F93A44",
                    _ => throw new InvalidOperationException("Architettura non supportata.")
                };
                await using (FileStream downloadedFile = File.OpenRead(tempPath))
                {
                    string actualHash = Convert.ToHexString(await SHA256.HashDataAsync(downloadedFile));
                    if (!CryptographicOperations.FixedTimeEquals(
                            Convert.FromHexString(actualHash), Convert.FromHexString(expectedHash)))
                        throw new InvalidDataException("Hash SHA-256 dell'archivio DefendNot non valido.");
                }
                ExtractZipSafely(tempPath, extractPath);
                string exePath = Path.Join(extractPath, "defendnot-loader.exe");

                if (!File.Exists(exePath))
                {
                    return;
                }
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = exePath;
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.ArgumentList.Add("--disable-autorun");
                    process.StartInfo.ArgumentList.Add("--silent");
                    process.StartInfo.Verb = "runas";
                    _ = process.Start();
                    await process.WaitForExitAsync();
                }
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception
                or IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or OperationCanceledException
                or InvalidOperationException
                or ArgumentException)
            {
                Debug.WriteLine($"DefendNot non avviato: {ex}");
                MessageBox.Show($"DefendNot non è stato avviato.\n{ex.Message}", "WinHubX",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);

                    if (Directory.Exists(workDirectory))
                        Directory.Delete(workDirectory, recursive: true);
                    SetDefenderRegedit(false);
                }
                catch (Exception ex) when (ex is IOException
                    or UnauthorizedAccessException
                    or System.Security.SecurityException
                    or InvalidOperationException)
                {
                    Debug.WriteLine($"Pulizia temporanei DefendNot non completata: {ex}");
                }
            }
        }

        private static void ExtractZipSafely(string archivePath, string destination)
        {
            Directory.CreateDirectory(destination);
            string root = Path.GetFullPath(destination).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

            using ZipArchive archive = ZipFile.OpenRead(archivePath);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string target = SafePathResolver.ResolveContainedPath(destination, entry.FullName);
                if (!target.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Archivio DefendNot non valido: percorso ZIP non sicuro.");

                if (string.IsNullOrEmpty(entry.Name))
                {
                    Directory.CreateDirectory(target);
                    continue;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                entry.ExtractToFile(target, overwrite: true);
            }
        }
        private void SetDefenderRegedit(bool isDisabled)
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(@"Software\WinHubX");
                if (key != null)
                {
                    key.SetValue("DefenderDisabled", isDisabled ? 1 : 0, RegistryValueKind.DWord);
                }
            }
            catch (Exception ex) when (ex is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or ArgumentException)
            {
                Debug.WriteLine("Impossibile salvare lo stato della funzionalità Defender nel registro.");
            }
        }
        static bool IsWindowsServer()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT ProductType FROM Win32_OperatingSystem");
                using ManagementObjectCollection operatingSystems = searcher.Get();
                foreach (ManagementObject os in operatingSystems)
                {
                    using (os)
                    {
                        int productType = Convert.ToInt32(os["ProductType"]);
                        return productType != 1;
                    }
                }
                Debug.WriteLine("WMI non ha restituito Win32_OperatingSystem; DefendNot viene bloccato per sicurezza.");
                return true;
            }
            catch (Exception ex) when (ex is System.Management.ManagementException
                or UnauthorizedAccessException
                or System.Runtime.InteropServices.COMException
                or InvalidOperationException)
            {
                Debug.WriteLine($"Rilevamento Windows Server non riuscito; DefendNot viene bloccato: {ex}");
                return true;
            }
        }
    }
}
