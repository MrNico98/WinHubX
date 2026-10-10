using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Compression;
using System.Management;
using System.Security.Cryptography;
using System.Windows.Forms;
using WinHubX.Forms.DebloatAvanzato;
using WinHubX.Forms.InstallaComponenti;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Base
{
    public partial class FormDebloat : Form
    {
        private static bool IsExpectedDebloatFailure(Exception exception) =>
            exception is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or System.ComponentModel.Win32Exception
                or System.Runtime.InteropServices.COMException
                or System.Management.ManagementException
                or InvalidOperationException
                or NotSupportedException
                or OperationCanceledException
                or TimeoutException
                or ArgumentException;

        private readonly Form1 form1;
        private static readonly HttpClient ResourceClient = CreateResourceClient();
        private List<string> appxNames = new List<string>();
        private Dictionary<string, string> appNameMappings = new(StringComparer.Ordinal);
        private readonly Dictionary<string, string> imageUrls = new Dictionary<string, string>();
        private int totalSteps = 0;

        private static HttpClient CreateResourceClient()
        {
            var handler = new SocketsHttpHandler
            {
                AllowAutoRedirect = false,
                MaxConnectionsPerServer = 4,
                AutomaticDecompression = System.Net.DecompressionMethods.None,
                PooledConnectionLifetime = TimeSpan.FromMinutes(5)
            };
            return new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(2) };
        }

        public FormDebloat(Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;

            this.Shown += FormDebloat_Shown;
        }

        private async void FormDebloat_Shown(object? sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
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
            catch (Exception ex) when (IsExpectedDebloatFailure(ex))
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
                {
                    string json = await TrustedHttpsClient.GetStringAsync(ResourceClient, "https://raw.githubusercontent.com/LightYagami28/ImageDebloat/refs/heads/main/ImmaginiDebloat.json");
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
            catch (Exception ex) when (IsExpectedDebloatFailure(ex))
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task CaricaAppNameMappings()
        {
            try
            {
                {
                    string json = await TrustedHttpsClient.GetStringAsync(ResourceClient, "https://raw.githubusercontent.com/LightYagami28/ImageDebloat/refs/heads/main/AssociazioniDebloat.json");
                    appNameMappings = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                        ?? new Dictionary<string, string>();
                }
            }
            catch (Exception ex) when (IsExpectedDebloatFailure(ex))
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
                    FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                psi.ArgumentList.Add("-NoProfile");
                psi.ArgumentList.Add("-NonInteractive");
                psi.ArgumentList.Add("-Command");
                psi.ArgumentList.Add("Get-AppxPackage | Where-Object { $_.SignatureKind -eq 'Store' } | Select-Object -ExpandProperty Name");

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
            catch (Exception ex) when (IsExpectedDebloatFailure(ex))
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
            imageUrls.TryGetValue(nomeLeggibile, out string? imgUrl);
            if (string.IsNullOrEmpty(imgUrl))
                imageUrls.TryGetValue("Generale", out imgUrl);
            var itemControl = new AppItemControl(nomeTecnico, imgUrl, appNameMappings)
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
                        FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    psi.ArgumentList.Add("-NoProfile");
                    psi.ArgumentList.Add("-NonInteractive");
                    psi.ArgumentList.Add("-Command");
                    psi.ArgumentList.Add(includeAll
                        ? "Get-AppxPackage | Select-Object -ExpandProperty Name"
                        : "Get-AppxPackage | Where-Object { $_.SignatureKind -eq 'Store' } | Select-Object -ExpandProperty Name");

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
            catch (Exception ex) when (IsExpectedDebloatFailure(ex))
            {
                if (IsHandleCreated)
                    MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private string OttieniNomeLeggibile(string nomeTecnico)
        {
            if (appNameMappings.TryGetValue(nomeTecnico, out string? nomeLeggibile))
                return nomeLeggibile;
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
                RunElevatedPowerShellCommand(
                    $"Get-AppxPackage -AllUsers -Name '{EscapePowerShellLiteral(nomeApp)}' | Remove-AppxPackage");
            }
            catch (Exception ex) when (IsExpectedDebloatFailure(ex))
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RimuoviProvisioning(string nomeApp)
        {
            try
            {
                RunElevatedPowerShellCommand(
                    $"Get-AppxProvisionedPackage -Online | Where-Object {{ $_.DisplayName -like '*{EscapePowerShellLiteral(nomeApp)}*' }} | Remove-AppxProvisionedPackage -Online");
            }
            catch (Exception ex) when (IsExpectedDebloatFailure(ex))
            {
                _ = MessageBox.Show($"Error: {nomeApp}: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void RunElevatedPowerShellCommand(string command)
        {
            ProcessStartInfo psi = new()
            {
                FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                Verb = "runas",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            psi.ArgumentList.Add("-NoProfile");
            psi.ArgumentList.Add("-NonInteractive");
            psi.ArgumentList.Add("-Command");
            psi.ArgumentList.Add(command);

            using Process process = new() { StartInfo = psi };
            _ = process.Start();
            process.WaitForExit();
        }

        private static string EscapePowerShellLiteral(string value) => value.Replace("'", "''", StringComparison.Ordinal);

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
                string? powerShellCommand = GetPowerShellCommand();
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
                FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "runas"
            };
            processStartInfo.ArgumentList.Add("-NoProfile");
            processStartInfo.ArgumentList.Add("-NonInteractive");
            processStartInfo.ArgumentList.Add("-Command");
            processStartInfo.ArgumentList.Add(command);

            using (var process = Process.Start(processStartInfo))
            {
                if (process is null)
                    throw new InvalidOperationException("Impossibile avviare il comando di rimozione AppX.");

                process.WaitForExit();
                if (process.ExitCode != 0)
                    throw new InvalidOperationException($"Rimozione AppX terminata con codice {process.ExitCode}.");
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
            flowLayoutPanel1.BeginInvoke(new Action(() => _ = CaricaAppxPackagesAsync()));
        }

        private async Task<HardwareInfo> OttieniHardwareInfoAsync()
        {
            string path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                       "WinHubX", "Computer", "osehardware.json");

            if (!File.Exists(path))
                throw new FileNotFoundException("Il file osehardware.json non esiste.", path);

            string json = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<HardwareInfo>(json)
                ?? throw new InvalidDataException("HardwareInfo non valido.");
        }
        static bool IsWindowsServer()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT ProductType FROM Win32_OperatingSystem");
                using ManagementObjectCollection operatingSystems = searcher.Get();
                ManagementObject? operatingSystem = operatingSystems.Cast<ManagementObject>().FirstOrDefault();
                if (operatingSystem is null)
                {
                    Debug.WriteLine("WMI non ha restituito Win32_OperatingSystem; DefendNot viene bloccato per sicurezza.");
                    return true;
                }

                using (operatingSystem)
                    return Convert.ToInt32(operatingSystem["ProductType"]) != 1;
            }
            catch (Exception ex) when (IsExpectedDebloatFailure(ex))
            {
                Debug.WriteLine($"Rilevamento Windows Server non riuscito; DefendNot viene bloccato: {ex}");
                return true;
            }
        }

        private async Task DisattivaWindowsDefender(HardwareInfo hardwareInfo)
        {
            if (await Task.Run(IsWindowsServer))
            {
                MessageBox.Show("La funzione DefendNot non viene eseguita su Windows Server.", "WinHubX",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(
                    "DefendNot registra un provider di sicurezza alternativo tramite Windows Security Center e può disattivare Microsoft Defender. Continuare?",
                    "Avviso sicurezza Defender", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
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
                    throw new InvalidDataException("URL DefendNot non presente nella configurazione remota.");

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
                    byte[] actualHash = await SHA256.HashDataAsync(downloadedFile);
                    if (!CryptographicOperations.FixedTimeEquals(actualHash, Convert.FromHexString(expectedHash)))
                        throw new InvalidDataException("Hash SHA-256 dell'archivio DefendNot non valido.");
                }
                ExtractZipSafely(tempPath, extractPath);
                string exePath = Path.Join(extractPath, "defendnot-loader.exe");

                if (!File.Exists(exePath))
                    throw new FileNotFoundException("Loader DefendNot non presente nell'archivio verificato.", exePath);
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = exePath;
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.ArgumentList.Add("--name");
                    process.StartInfo.ArgumentList.Add("WinHubX");
                    process.StartInfo.ArgumentList.Add("--autorun-as-user");
                    process.StartInfo.Verb = "runas";
                    _ = process.Start();
                    await process.WaitForExitAsync();
                    if (process.ExitCode != 0)
                        throw new InvalidOperationException($"Loader DefendNot terminato con codice {process.ExitCode}.");
                }
                SetDefenderRegedit(true);
            }
            catch (Exception ex) when (IsExpectedDebloatFailure(ex))
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
                        Directory.Delete(workDirectory, true);
                }
                catch (Exception ex) when (IsExpectedDebloatFailure(ex))
                {
                    Debug.WriteLine($"Pulizia temporanei DefendNot non completata: {ex}");
                }
            }
        }

        private static void ExtractZipSafely(string archivePath, string destination)
        {
            Directory.CreateDirectory(destination);
            using ZipArchive archive = ZipFile.OpenRead(archivePath);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string target;
                try
                {
                    target = SafePathResolver.ResolveContainedPath(destination, entry.FullName);
                }
                catch (InvalidDataException ex)
                {
                    throw new InvalidDataException("Archivio DefendNot non valido: percorso ZIP non sicuro.", ex);
                }

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
            catch (Exception ex) when (IsExpectedDebloatFailure(ex))
            {
                Debug.WriteLine($"Salvataggio stato Defender non riuscito: {ex}");
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
            catch (Exception ex) when (IsExpectedDebloatFailure(ex))
            {
                Debug.WriteLine($"Lettura stato Defender non riuscita: {ex}");
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
            string hardwarePath = Path.Join(
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
