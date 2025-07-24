using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace WinHubX.Forms.DebloatAvanzato
{
    public partial class FormDebloatAvanzato : Form
    {
        private List<string> packageNames = new List<string>();

        public FormDebloatAvanzato()
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            LoadAppxPackages();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }

        private void LoadAppxPackages()
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = "Get-AppxPackage | Select-Object -ExpandProperty Name";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            _ = process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            packageNames = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            checkedListBox1.Items.AddRange(packageNames.ToArray());
        }

        private async void btnAvviaSelezionati_Click(object sender, EventArgs e)
        {
            int total = checkedListBox1.CheckedItems.Count;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = total;
            progressBar1.Value = 0;

            foreach (var selectedItem in checkedListBox1.CheckedItems)
            {
                string packageName = selectedItem.ToString();
                string command = $"Get-AppxPackage -allusers {packageName} | Remove-AppxPackage";
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Verb = "runas",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    Arguments = $"-Command \"{command}\""
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    _ = process.Start();
                    await process.WaitForExitAsync();
                }

                progressBar1.Value += 1;
            }

            if (checkBox_WindowsDefender.Checked)
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
                    try
                    {
                        progressBar1.Minimum = 0;
                        int totalCommands = 25;
                        progressBar1.Maximum = totalCommands;
                        progressBar1.Value = 0;
                        progressBar1.Visible = true;

                        byte[] exeBytes = LoadEmbeddedResource(resourcePath);
                        string tempExePath = Path.Combine(Path.GetTempPath(), "PowerRun.exe");
                        File.WriteAllBytes(tempExePath, exeBytes);

                        void RunWithProgress(string command)
                        {
                            RunCommand(tempExePath, command);
                            progressBar1.Value++;
                            Application.DoEvents();
                        }

                        RunCommand(tempExePath, @"cmd.exe /c ""reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender"" /v ""DisableAntiSpyware"" /t REG_DWORD /d 1 /f""");
                        RunCommand(tempExePath, @"cmd.exe /c ""reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection"" /v ""DisableBehaviorMonitoring"" /t REG_DWORD /d 1 /f""");
                        RunCommand(tempExePath, @"cmd.exe /c ""reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection"" /v ""DisableIOAVProtection"" /t REG_DWORD /d 1 /f""");
                        RunCommand(tempExePath, @"cmd.exe /c ""reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection"" /v ""DisableOnAccessProtection"" /t REG_DWORD /d 1 /f""");
                        RunCommand(tempExePath, @"cmd.exe /c ""reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection"" /v ""DisableRealtimeMonitoring"" /t REG_DWORD /d 1 /f""");
                        RunCommand(tempExePath, @"cmd.exe /c ""reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SecurityHealthService"" /v ""Start"" /t REG_DWORD /d 4 /f""");
                        RunCommand(tempExePath, @"cmd.exe /c ""reg add ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WinDefend"" /v ""Start"" /t REG_DWORD /d 4 /f""");
                        RunCommand(tempExePath, @"cmd.exe /c ""reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection"" /v ""DisableScanOnRealtimeEnable"" /t REG_DWORD /d 1 /f""");
                        RunCommand(tempExePath, @"cmd.exe /c ""reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Reporting"" /v ""DisableEnhancedNotifications"" /t REG_DWORD /d 1 /f""");
                        RunCommand(tempExePath, @"cmd.exe /c ""reg add ""HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Spynet"" /v ""SubmitSamplesConsent"" /t REG_DWORD /d 2 /f""");
                        string[] defenderTasks = new[]
                        {
    @"schtasks /Change /TN ""Microsoft\Windows\Windows Defender\Windows Defender Scheduled Scan"" /Disable",
    @"schtasks /Change /TN ""Microsoft\Windows\Windows Defender\Windows Defender Cache Maintenance"" /Disable",
    @"schtasks /Change /TN ""Microsoft\Windows\Windows Defender\Windows Defender Cleanup"" /Disable",
    @"schtasks /Change /TN ""Microsoft\Windows\Windows Defender\Windows Defender Verification"" /Disable",
    @"schtasks /Change /TN ""Microsoft\Windows\Windows Defender\Windows Defender Antivirus Scheduled Scan"" /Disable",
    @"schtasks /Change /TN ""Microsoft\Windows\Windows Defender\Windows Defender Antivirus Verification"" /Disable",
    @"schtasks /Change /TN ""Microsoft\Windows\Windows Defender\Windows Defender Antivirus Cleanup"" /Disable",
    @"schtasks /Change /TN ""Microsoft\Windows\Windows Defender\Windows Defender Antivirus Cache Maintenance"" /Disable"
};
                        string[] defenderServices = new[]
                        {
    @"sc config WinDefend start= disabled",
    @"sc stop WinDefend",
    @"sc config SecurityHealthService start= disabled",
    @"sc stop SecurityHealthService",
    @"sc config WdNisSvc start= disabled",
    @"sc stop WdNisSvc",
    @"sc config WdFilter start= disabled",
    @"sc stop WdFilter",
    @"sc config Sense start= disabled",
    @"sc stop Sense"
};
                        foreach (var taskCmd in defenderTasks)
                        {
                            RunWithProgress($@"cmd.exe /c ""{taskCmd}""");
                        }

                        foreach (var svcCmd in defenderServices)
                        {
                            RunWithProgress($@"cmd.exe /c ""{svcCmd}""");
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {

                }
            }
            else
            {

            }
            _ = MessageBox.Show("Success", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadAppxPackages();
        }

        static byte[] LoadEmbeddedResource(string resourcePath)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                    throw new InvalidOperationException("Error: " + resourcePath);
                byte[] buffer = new byte[stream.Length];
                _ = stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        static void RunCommand(string fileName, string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            using (var process = Process.Start(startInfo))
            {
                process.WaitForExit();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            string searchText = textBox1.Text.ToLower();
            checkedListBox1.Items.Clear();
            foreach (var packageName in packageNames)
            {
                if (packageName.ToLower().Contains(searchText))
                {
                    _ = checkedListBox1.Items.Add(packageName);
                }
            }
        }

    }
}
