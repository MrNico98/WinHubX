using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Compression;
using System.Management;
using System.Windows.Forms;
using WinHubX.Forms.DebloatAvanzato;
using WinHubX.Forms.InstallaComponenti;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Base
{
    public partial class FormDebloat : Form
    {
        private Form1 form1;
        private List<string> appxNames = new List<string>();
        public static Dictionary<string, string> appNameMappings = new Dictionary<string, string>();
        private Dictionary<string, string> imageUrls = new Dictionary<string, string>();
        private int totalSteps = 0;

        public FormDebloat(Form1 form1)
        {
            InitializeComponent();
            form1 = form1;

            this.Shown += FormDebloat_Shown;
        }

        private async void FormDebloat_Shown(object? sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            await Task.Delay(50);

            try
            {
                LanguageManager.LoadLanguageFromSettings();

                btnAvviaSelezionatiVerdi.Content = LanguageManager.CurrentLanguage == "it" ? "  Avvia" : "  Start";
                btnModificaServiziDisattivo.Content = LanguageManager.CurrentLanguage == "it" ? "  Modifica servizi" : "  Edit services";
                btnInstallaComponentiVerdi.Content = LanguageManager.CurrentLanguage == "it" ? "  Aggiungi componenti" : "  Add components";
                btnDebloatAutomaticoVerdi.Content = LanguageManager.CurrentLanguage == "it" ? "  Avvia" : "  Start";

                flowLayoutPanel1.AutoScroll = true;
                flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
                flowLayoutPanel1.WrapContents = true;
                flowLayoutPanel1.HorizontalScroll.Maximum = 0;
                flowLayoutPanel1.HorizontalScroll.Visible = false;
                flowLayoutPanel1.AutoScrollMinSize = new Size(0, 0);

                await InizializzaDati();

                ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);

                pictureBoxPowerPoint.Image = ThemeManager.IsDarkTheme
                    ? Properties.Resources.pngDebloatAutoaticoDebloat
                    : Properties.Resources.pngDebloatAutomaticoBlackDebloat;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async Task InizializzaDati()
        {
            try
            {
                var appNameTask = CaricaAppNameMappings();
                var imageTask = CaricaImmaginiApp();
                var appxTask = CaricaAppxPackagesAsync();

                await Task.WhenAll(appNameTask, imageTask, appxTask);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il caricamento: {ex.Message}");
            }
        }


        private class ImmagineData
        {
            public required string Nome { get; set; }
            public required string ID { get; set; }
            public required string ImmagineUrl { get; set; }
        }
        private async Task CaricaImmaginiApp()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync("https://raw.githubusercontent.com/MrNico98/ImageDebloat/refs/heads/main/ImmaginiDebloat.json");
                    var immaginiList = System.Text.Json.JsonSerializer.Deserialize<List<ImmagineData>>(json);

                    if (immaginiList != null)
                    {
                        foreach (var item in immaginiList)
                        {
                            string chiave = item.ID ?? item.Nome;
                            if (!string.IsNullOrEmpty(chiave) && !string.IsNullOrEmpty(item.ImmagineUrl))
                            {
                                imageUrls[chiave] = item.ImmagineUrl;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task CaricaAppNameMappings()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync("https://raw.githubusercontent.com/MrNico98/ImageDebloat/refs/heads/main/AssociazioniDebloat.json");
                    appNameMappings = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task CaricaAppxPackagesAsync()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-Command \"Get-AppxPackage | Where-Object { $_.SignatureKind -eq 'Store' } | Select-Object -ExpandProperty Name\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = psi, EnableRaisingEvents = true })
                {
                    process.Start();
                    string output = await process.StandardOutput.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    appxNames = output
                        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }
                if (!IsDisposed)
                    Invoke(new Action(() => AggiornaUI(appxNames)));
            }
            catch (Exception ex)
            {
                if (!IsDisposed)
                    MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void AggiornaUI(List<string> filteredApps)
        {
            flowLayoutPanel1.Controls.Clear();
            foreach (string appName in filteredApps)
            {
                AggiungiElemento(appName);
            }
        }

        private void AggiungiElemento(string nomeTecnico)
        {
            string nomeLeggibile = OttieniNomeLeggibile(nomeTecnico);
            string? imgUrl = imageUrls.ContainsKey(nomeLeggibile) ? imageUrls[nomeLeggibile] : null;
            if (string.IsNullOrEmpty(imgUrl) && imageUrls.ContainsKey("Generale"))
            {
                imgUrl = imageUrls["Generale"];
            }
            var itemControl = new AppItemControl(nomeTecnico, imgUrl)
            {
                Width = (flowLayoutPanel1.ClientSize.Width / 2) - 20,
                Height = 50,
                Margin = new Padding(3),
                Tag = nomeTecnico
            };
            flowLayoutPanel1.Controls.Add(itemControl);
        }

        private async Task CaricaAppxPackagesAsync(bool includeAll = false)
        {
            try
            {
                var output = await Task.Run(() =>
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = includeAll
                            ? "-Command \"Get-AppxPackage | Select-Object -ExpandProperty Name\""
                            : "-Command \"Get-AppxPackage | Where-Object { $_.SignatureKind -eq 'Store' } | Select-Object -ExpandProperty Name\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (Process process = new Process { StartInfo = psi })
                    {
                        process.Start();
                        string result = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();
                        return result;
                    }
                });

                appxNames = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                  .ToList();
                if (!IsDefenderDisabled() &&
                    !appxNames.Any(a => a.Equals("Windows Defender", StringComparison.OrdinalIgnoreCase)))
                {
                    appxNames.Add("Windows Defender");
                }
                if (IsHandleCreated)
                    Invoke(() => AggiornaUI(appxNames));
            }
            catch (Exception ex)
            {
                if (IsHandleCreated)
                    MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private string OttieniNomeLeggibile(string nomeTecnico)
        {
            if (appNameMappings.ContainsKey(nomeTecnico))
            {
                return appNameMappings[nomeTecnico];
            }
            return nomeTecnico.Replace("Microsoft.", "").Replace("_", " ");
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string filtro = textBox1.Text.Trim().ToLower();
                List<string> risultati = appxNames
                    .Where(app => OttieniNomeLeggibile(app).ToLower().Contains(filtro))
                    .ToList();

                AggiornaUI(risultati);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.ForeColor = Color.Gray;
                textBox1.SelectionStart = 0;
                AggiornaUI(appxNames);
            }
        }
        private void btnAvviaSelezionatiDebloat_Click(object sender, EventArgs e)
        {
            totalSteps = 0;
            foreach (Control control in flowLayoutPanel1.Controls)
            {
                if (control is AppItemControl appItem && appItem.IsSelected)
                {
                    totalSteps++;
                }
            }

            if (totalSteps == 0)
            {
                totalSteps = 1;
            }

            progressBar1.MaxValue = totalSteps;
            progressBar1.Value = 0;

            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void RimuoviApp(string nomeApp)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Verb = "runas",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = $"-Command \"Get-AppxPackage -allusers {nomeApp} | Remove-AppxPackage\""
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    _ = process.Start();
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RimuoviProvisioning(string nomeApp)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Verb = "runas",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = $"-Command \"Get-AppxProvisionedPackage -Online | Where-Object {{ $_.DisplayName -like '*{nomeApp}*' }} | Remove-AppxProvisionedPackage -Online\""
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    _ = process.Start();
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {nomeApp}: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDebloatAuto_Click(object sender, EventArgs e)
        {
            string messaggionbox = LanguageManager.GetTranslation("FormDebloat", "confermaEsecuzione");
            string titolo = LanguageManager.GetTranslation("FormDebloat", "titoloConferma");

            DialogResult result = MessageBox.Show(messaggionbox,
                                                  titolo,
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                string powerShellCommand = GetPowerShellCommand();
                if (powerShellCommand != null)
                {
                    ExecutePowerShellCommand(powerShellCommand);
                    string messaggio = LanguageManager.GetTranslation("Global", "modifichesuccesso");

                    _ = MessageBox.Show(
                        messaggio,
                        "WinHubX",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }

        private string? GetPowerShellCommand()
        {
            int version = Environment.OSVersion.Version.Major;
            if (version < 10)
            {
                return null;
            }

            string[] commonAppsToRemove = {
        "Microsoft.VP9VideoExtensions", "Microsoft.WebMediaExtensions",
        "Microsoft.WebpImageExtension", "Microsoft.Windows.ShellExperienceHost",
        "Microsoft.VCLibs*",
        "Microsoft.WindowsStore", "Microsoft.XboxIdentityProvider", "Microsoft.HEIFImageExtension",
        "Microsoft.UI.Xaml*"
    };

            string notepad = version == 11 ? "| Notepad" : "";

            string command = $"$ErrorActionPreference = 'SilentlyContinue'; Get-AppxPackage -AllUsers | Where-Object {{$_.name -notmatch '{string.Join("|", commonAppsToRemove)}'{notepad}}} | Remove-AppxPackage";

            return command;
        }

        private void ExecutePowerShellCommand(string command)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-command \"{command}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Verb = "runas"
            };

            using (var process = Process.Start(processStartInfo))
            {
                if (process != null)
                {
                    process.WaitForExit();
                    _ = process.StandardOutput.ReadToEnd();
                    _ = process.StandardError.ReadToEnd();
                }
            }
        }

        private void btnServizi_Click(object sender, EventArgs e)
        {
            FormServizi formServizi = new FormServizi();
            formServizi.Show();
        }

        private async void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int currentStep = 0;
            List<string> appsToRemove = new List<string>();
            bool removeDefender = false;
            var selectedApps = flowLayoutPanel1.Invoke(new Func<List<Tuple<string, string>>>(() =>
            {
                var result = new List<Tuple<string, string>>();

                foreach (Control control in flowLayoutPanel1.Controls)
                {

                    if (control is AppItemControl appItem)
                    {
                        string nomeLeggibile = appItem.lblNome.Text;
                        string nomeTecnico = appItem.NomeTecnico;
                        bool isSelected = appItem.IsSelected;    
                        if (isSelected)
                        {
                            result.Add(new Tuple<string, string>(nomeLeggibile, nomeTecnico));
                        }
                    }
                }
                return result;
            }));
            foreach (var app in selectedApps)
            {
                string nomeLeggibile = app.Item1;
                string nomeTecnico = app.Item2;
                if (nomeLeggibile.Trim().Equals("Windows Defender", StringComparison.OrdinalIgnoreCase))
                {
                    removeDefender = true;
                    continue;
                }
                if (!string.IsNullOrEmpty(nomeTecnico))
                {
                    appsToRemove.Add(nomeTecnico);
                }
            }
            foreach (string app in appsToRemove)
            {
                RimuoviApp(app);
                RimuoviProvisioning(app);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                await Task.Delay(500);
            }
            if (removeDefender)
            {
                HardwareInfo hardwareInfo = await OttieniHardwareInfoAsync();
                await DisattivaWindowsDefender(hardwareInfo);
            }
            flowLayoutPanel1.Invoke(new Action(() => CaricaAppxPackagesAsync()));
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

        private async Task DisattivaWindowsDefender(HardwareInfo hardwareInfo)
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
                    process.StartInfo.ArgumentList.Add("--name");
                    process.StartInfo.ArgumentList.Add("WinHubX");
                    process.StartInfo.ArgumentList.Add("--autorun-as-user");
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
                    SetDefenderRegedit(true);
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
        private bool IsDefenderDisabled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\WinHubX");
                if (key != null)
                {
                    object? value = key.GetValue("DefenderDisabled");
                    if (value != null && int.TryParse(value.ToString(), out int intValue))
                    {
                        return intValue == 1;
                    }
                }
            }
            catch
            {

            }
            return false;
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = Math.Min(e.ProgressPercentage, progressBar1.MaxValue);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            string messaggio = LanguageManager.GetTranslation("Global", "modifichesuccesso");

            _ = MessageBox.Show(
                messaggio,
                "WinHubX",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private async void cuiSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            string hardwarePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WinHubX", "Computer", "osehardware.json");

            if (!File.Exists(hardwarePath))
            {
                var popup = new WinHubX.DialogBlock.Form_DialogBlock(form1);
                popup.StartPosition = FormStartPosition.CenterScreen;
                popup.ShowDialog();
                return;
            }
            flowLayoutPanel1.Controls.Clear();
            Label loadingLabel = new Label
            {
                Text = "Caricamento in corso...",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.Gray,
                Margin = new Padding(10)
            };
            flowLayoutPanel1.Controls.Add(loadingLabel);
            await CaricaAppxPackagesAsync(cuiSwitch1.Checked);
        }

        private void btnInstallaComponentiVerdi_Click(object sender, EventArgs e)
        {
            FormInstallaComponenti formInstallaComponenti = new FormInstallaComponenti();
            formInstallaComponenti.Show();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
