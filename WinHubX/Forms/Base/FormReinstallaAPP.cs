using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WinHubX.Forms.ReinstallaAPP
{
    public partial class FormReinstallaAPP : Form
    {
        public FormReinstallaAPP()
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(224, 224, 224);
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }
        static void RegisterAppxPackage(string packageName)
        {
            string getManifestPathCommand = $"Get-AppxPackage -AllUsers '{packageName}' | Select -ExpandProperty InstallLocation";

            string manifestPath = ExecutePowerShellCommand(getManifestPathCommand);

            if (!string.IsNullOrEmpty(manifestPath))
            {
                string registerCommand = $"Add-AppxPackage -DisableDevelopmentMode -Register '{manifestPath}\\AppXManifest.xml'";
                _ = ExecutePowerShellCommand(registerCommand);
            }
        }

        static void Eseguiinstallstore()
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

        static string ExecutePowerShellCommand(string command)
        {
            string output = "";

            using (Process powerShellProcess = new Process())
            {
                powerShellProcess.StartInfo.FileName = "powershell.exe";
                powerShellProcess.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy unrestricted -Command \"{command}\"";
                powerShellProcess.StartInfo.UseShellExecute = false;
                powerShellProcess.StartInfo.CreateNoWindow = true;
                powerShellProcess.StartInfo.RedirectStandardOutput = true;
                powerShellProcess.StartInfo.RedirectStandardError = true;

                _ = powerShellProcess.Start();

                output = powerShellProcess.StandardOutput.ReadToEnd();
                string error = powerShellProcess.StandardError.ReadToEnd();

                if (!string.IsNullOrEmpty(error))
                {
                    _ = MessageBox.Show($"Error: {error}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                powerShellProcess.WaitForExit();
            }

            return output.Trim();
        }

        private void btnAvviaSelezionatiApp_Click(object sender, EventArgs e)
        {
            if (App1.CheckedItems.Contains("Microsoft Store"))
            {
                RegisterAppxPackage("Microsoft.DesktopAppInstaller");
                Eseguiinstallstore();
            }

            if (App1.CheckedItems.Contains("Microsoft Edge"))
            {
                RunPowerShellCommand1("winget install Microsoft.Edge");

                _ = MessageBox.Show(LanguageManager.GetTranslation("FormReinstallAPP", "presskeyexit"));
                _ = Console.ReadKey();
            }
            if (App1.CheckedItems.Contains("Windows Defender"))
            {
                DialogResult result = MessageBox.Show(
                    LanguageManager.GetTranslation("Global", "restartrequired"),
                    "WinHubX",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning
                );
                if (result == DialogResult.OK)
                {
                    string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                    string resourcePath = $"{assemblyName}.Resources.PowerRun.exe";
                    string tempPath = Path.GetTempPath();
                    string powerRunPath = Path.Combine(tempPath, "PowerRun.exe");
                    if (!File.Exists(powerRunPath))
                    {
                        using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
                        {
                            using (FileStream fileStream = new FileStream(powerRunPath, FileMode.Create, FileAccess.Write))
                            {
                                resourceStream.CopyTo(fileStream);
                            }
                        }
                    }
                    string[] commands = new string[]
                    {
            @"cmd.exe /c reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender"" /v ""DisableAntiSpyware"" /t REG_DWORD /d 0 /f",
            @"cmd.exe /c reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection"" /v ""DisableBehaviorMonitoring"" /t REG_DWORD /d 0 /f",
            @"cmd.exe /c reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection"" /v ""DisableIOAVProtection"" /t REG_DWORD /d 0 /f",
            @"cmd.exe /c reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection"" /v ""DisableOnAccessProtection"" /t REG_DWORD /d 0 /f",
            @"cmd.exe /c reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection"" /v ""DisableRealtimeMonitoring"" /t REG_DWORD /d 0 /f",
            @"cmd.exe /c reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SecurityHealthService"" /v ""Start"" /t REG_DWORD /d 2 /f",
            @"cmd.exe /c reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WinDefend"" /v ""Start"" /t REG_DWORD /d 2 /f"
                    };
                    foreach (var command in commands)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = powerRunPath,
                            Arguments = command,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        };

                        using (Process process = new Process())
                        {
                            process.StartInfo = startInfo;
                            _ = process.Start();
                            string output = process.StandardOutput.ReadToEnd();
                            string error = process.StandardError.ReadToEnd();
                            if (process.ExitCode != 0)
                            {

                            }
                        }
                    }
                }
            }

            static void RunPowerShellCommand1(string command)
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    using (var process = Process.Start(startInfo))
                    {
                        var output = process.StandardOutput.ReadToEnd();
                        var error = process.StandardError.ReadToEnd();
                        if (!string.IsNullOrEmpty(error))
                        {
                            _ = MessageBox.Show($"Error: {error}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        if (!string.IsNullOrEmpty(output))
                        {
                            _ = MessageBox.Show($"Error: {output}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            string messaggio = LanguageManager.GetTranslation("Global", "modifichesuccesso");
            _ = MessageBox.Show(
                messaggio,
                "WinHubX",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void ParseSearchResults(string output)
        {
            dataGridViewResults.Rows.Clear();
            dataGridViewResults.Columns.Clear();
            if (dataGridViewResults.Columns.Count == 0)
            {
                _ = dataGridViewResults.Columns.Add("Name", "Name");
                _ = dataGridViewResults.Columns.Add("Id", "Id");
                _ = dataGridViewResults.Columns.Add("Version", "Version");
                _ = dataGridViewResults.Columns.Add("Source", "Source");
                _ = dataGridViewResults.Columns.Add("Origine", "Origine");
            }
            string[] lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                var match = Regex.Match(line, @"^(.+?)\s{2,}(.+?)\s{2,}(.+?)\s{2,}(.+?)\s{2,}(.*)$");

                if (match.Success)
                {
                    string name = match.Groups[1].Value.Trim();
                    string id = match.Groups[2].Value.Trim();
                    string version = match.Groups[3].Value.Trim();
                    string source = match.Groups[4].Value.Trim();
                    string origine = match.Groups[5].Value.Trim();
                    _ = dataGridViewResults.Rows.Add(name, id, version, source, origine);
                }
            }

            if (dataGridViewResults.Rows.Count == 0)
            {
                _ = MessageBox.Show(
                    LanguageManager.GetTranslation("FormReinstallAPP", "noresults"),
                    "Info",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void InstallPackage(string packageId)
        {
            try
            {
                txtOutput.Text = $"Installazione di: {packageId}...\r\n";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "winget",
                    Arguments = $"install {packageId} --silent --accept-package-agreements",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string output = reader.ReadToEnd();
                        txtOutput.AppendText(output);
                    }
                }
                string messaggio = LanguageManager.GetTranslation("Global", "modifichesuccesso");

                _ = MessageBox.Show(
                    messaggio,
                    "WinHubX",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string packageId = dataGridViewResults.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                InstallPackage(packageId);
            }
        }

        private void btnInstalla_Click(object sender, EventArgs e)
        {
            if (dataGridViewResults.SelectedRows.Count > 0)
            {
                string packageId = dataGridViewResults.SelectedRows[0].Cells["Id"].Value.ToString();
                if (!string.IsNullOrEmpty(packageId))
                {
                    InstallPackage(packageId);
                }
            }
            else
            {
                _ = MessageBox.Show(
                    LanguageManager.GetTranslation("FormReinstallAPP", "selectpackage"),
                    "WARNING",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void UpgradePackage(string packageId)
        {
            try
            {
                txtOutput.Text = $"Aggiornamento di: {packageId}...\r\n";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "winget",
                    Arguments = $"upgrade {packageId} --silent --accept-package-agreements",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string output = reader.ReadToEnd();
                        txtOutput.AppendText(output);
                    }
                }

                _ = MessageBox.Show(
                    LanguageManager.GetTranslation("FormReinstallAPP", "updatecompleted"),
                    "Successo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Aggiorna_Click(object sender, EventArgs e)
        {
            if (dataGridViewResults.SelectedRows.Count > 0)
            {
                string packageId = dataGridViewResults.SelectedRows[0].Cells["Id"].Value.ToString();
                if (!string.IsNullOrEmpty(packageId))
                {
                    UpgradePackage(packageId);
                }
            }
            else
            {
                _ = MessageBox.Show(
                    LanguageManager.GetTranslation("FormReinstallAPP", "selectpackage"),
                    "WARNING",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private async void btnSearch_Click_1(object sender, EventArgs e)
        {
            string query = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(query))
            {
                return;
            }

            try
            {
                string result = await Task.Run(() => RunWingetSearch(query));
                ParseSearchResults(result);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string RunWingetSearch(string query)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "winget",
                    Arguments = $"search \"{query}\"",
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
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        private void FormReinstallaAPP_Load(object sender, EventArgs e)
        {
            string wingetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\WindowsApps\winget.exe");
            if (File.Exists(wingetPath))
            {
                label1.Text = LanguageManager.GetTranslation("FormReinstallAPP", "wingetinstalled");
                panel1.BackColor = Color.Green;
            }
            else
            {
                label1.Text = LanguageManager.GetTranslation("FormReinstallAPP", "wingetnotfound");
                panel1.BackColor = Color.Red;
                _ = Task.Delay(3000);
                _ = InstallWinGetAsync();
            }
            string windowsAppsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\WindowsApps");
            string envPath = Environment.GetEnvironmentVariable("PATH");

            if (!envPath.Contains(windowsAppsPath, StringComparison.OrdinalIgnoreCase))
            {
                Environment.SetEnvironmentVariable("PATH", envPath + ";" + windowsAppsPath, EnvironmentVariableTarget.User);
                label1.Text = "Percorso di Winget aggiunto al PATH.";
            }
        }

        private async Task InstallWinGetAsync()
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
                        label1.Text = $"Scaricando {localFiles[i]}...";
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
                label1.Text = output;
            }

            if (!string.IsNullOrEmpty(error))
            {
                _ = MessageBox.Show($"Error: {error}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
