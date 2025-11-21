using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO.Compression;
using System.Management;
using System.Security.Policy;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.InstallaComponenti
{
    public partial class FormInstallaComponenti : Form
    {
        public FormInstallaComponenti()
        {
            InitializeComponent();
            Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            btnInstallaVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Installa",
                "en" => "  Install",
                _ => btnInstallaVerdi.Content
            };
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

            HardwareInfo hardwareInfo = null;
            if (doDefender)
                hardwareInfo = await OttieniHardwareInfoAsync();
            if (doDefender)
                await DefenderOn(hardwareInfo);

            if (doWinget)
                await WingetInstall();

            if (doStore)
                await MicrosoftStoreInstall();
        }
        private async Task WingetInstall()
        {
            string[] urls =
            {
        "https://aka.ms/getwinget",
        "https://aka.ms/Microsoft.VCLibs.x64.14.00.Desktop.appx",
        "https://github.com/microsoft/microsoft-ui-xaml/releases/download/v2.8.6/Microsoft.UI.Xaml.2.8.x64.appx"
    };

            string[] localFiles =
            {
        "Microsoft.DesktopAppInstaller_8wekyb3d8bbwe.msixbundle",
        "Microsoft.VCLibs.x64.14.00.Desktop.appx",
        "Microsoft.UI.Xaml.2.8.x64.appx"
    };

            using (HttpClient client = new HttpClient())
            {
                for (int i = 0; i < urls.Length; i++)
                {
                    try
                    {
                        using (var response = await client.GetAsync(urls[i]))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                                await File.WriteAllBytesAsync(localFiles[i], fileBytes);
                            }
                            else
                            {
                                _ = MessageBox.Show($"Error downloading {localFiles[i]}: {response.StatusCode}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show($"Error: {localFiles[i]}\n{ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            foreach (string file in localFiles)
            {
                try
                {
                    AddAppxPackage(file);
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show($"Error: {file}\n{ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void AddAppxPackage(string packagePath)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command Start-Process powershell -ArgumentList 'Add-AppxPackage -Path \"{packagePath}\"' -Verb RunAs",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            _ = process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(output))
            {
            }

            if (!string.IsNullOrEmpty(error))
            {
                _ = MessageBox.Show($"Error: {error}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task MicrosoftStoreInstall()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c WSReset -i",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                _ = process.Start();
                process.WaitForExit();
            }
            Thread.Sleep(20000);
            using (Process process = new Process { StartInfo = startInfo })
            {
                _ = process.Start();
                process.WaitForExit();
            }

            _ = MessageBox.Show(LanguageManager.GetTranslation("FormReinstallAPP", "storeinstalling"));
            Thread.Sleep(4000);
        }

        private async Task<HardwareInfo> OttieniHardwareInfoAsync()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                       "WinHubX", "Computer", "osehardware.json");

            if (!File.Exists(path))
                throw new FileNotFoundException("Il file osehardware.json non esiste.", path);

            string json = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<HardwareInfo>(json);
        }
        private async Task DefenderOn(HardwareInfo hardwareInfo)
        {
            if (IsWindowsServer())
                return;
            string arch = hardwareInfo.Architettura;

            string url = Dipendenze.GitHubConfigUrl;

            string tempPath = Path.Combine(Path.GetTempPath(), "DefNot.zip");
            string extractPath = Path.Combine(Path.GetTempPath(), "DefNotExtracted");

            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(url);
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

                using (var response = await client.GetAsync(downloadUrl))
                {
                    _ = response.EnsureSuccessStatusCode();
                    await using var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    await response.Content.CopyToAsync(fs);
                }
                if (Directory.Exists(extractPath))
                    Directory.Delete(extractPath, true);
                ZipFile.ExtractToDirectory(tempPath, extractPath);
                string exePath = Path.Combine(extractPath, "defendnot-loader.exe");

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
            catch (Exception)
            {
            }
            finally
            {
                try
                {

                    if (File.Exists(tempPath))
                        File.Delete(tempPath);

                    if (Directory.Exists(extractPath))
                        Directory.Delete(extractPath, true);
                    SetDefenderRegedit(false);
                }
                catch (Exception)
                {
                }
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
            catch (Exception)
            {

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
    }
}