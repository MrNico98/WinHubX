using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using WinHubX.Forms.Base;
using WinHubX.Impostazioni;


namespace WinHubX.Forms.Settaggi
{
    public partial class FormPersonalizzazione : Form, IImportedSettingsForm
    {
        private readonly Form1 form1;
        private FormSettaggi formSettaggi;
        private readonly string tempFolder = Path.Join(Path.GetTempPath(), "WinHubX");
        private static readonly HttpClient ResourceClient = CreateResourceClient();
        private int totalSteps = 0;

        private static HttpClient CreateResourceClient()
        {
            var handler = new SocketsHttpHandler
            {
                AllowAutoRedirect = false,
                MaxConnectionsPerServer = 4,
                AutomaticDecompression = DecompressionMethods.None,
                PooledConnectionLifetime = TimeSpan.FromMinutes(5)
            };
            return new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(2) };
        }

        private static Uri RequireTrustedHttpsUri(string value)
        {
            return TrustedHttpsClient.ValidateUri(value, "personalizzazione");
        }
        public FormPersonalizzazione(FormSettaggi formSettaggi, Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
            this.formSettaggi = formSettaggi;
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            cuiButton1Verdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Avvia",
                "en" => "  Start",
                _ => cuiButton1Verdi.Content
            };
            btnSettaggiExplorerVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Preferenze explorer",
                "en" => "  Explorer preferences",
                _ => btnSettaggiExplorerVerdi.Content
            };
        }

        private void btnAvviaSelezionati_Click(object sender, EventArgs e)
        {
            string[] selectedOptions = EnumerateControls(panello)
                .OfType<CheckBox>()
                .Where(checkBox => checkBox.Checked)
                .Select(checkBox => checkBox.Name)
                .ToArray();

            totalSteps = selectedOptions.Length;
            if (totalSteps == 0)
            {
                return;
            }

            progressBar1.MaxValue = totalSteps;
            progressBar1.Value = 0;
            if (!backgroundWorker1.IsBusy)
            {
                cuiButton1Verdi.Enabled = false;
                backgroundWorker1.RunWorkerAsync(selectedOptions);
            }
        }

        Task IImportedSettingsForm.ApplyImportedSettingsAsync() =>
            ImportedSettingsWorker.RunAsync(
                backgroundWorker1,
                () => btnAvviaSelezionati_Click(cuiButton1Verdi, EventArgs.Empty));

        private static IEnumerable<Control> EnumerateControls(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                yield return child;
                foreach (Control descendant in EnumerateControls(child))
                {
                    yield return descendant;
                }
            }
        }

        private void DisabilitaEndTask()
        {
            try
            {
                using (RegistryKey currentUserKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey? taskbarSettings = currentUserKey.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings"))
                    {
                        taskbarSettings?.SetValue("TaskbarEndTask", 0, RegistryValueKind.DWord);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Impostazione End Task non applicata: {ex}");
                throw;
            }
        }


        private void AbiliaEndTask()
        {
            try
            {
                using (RegistryKey currentUserKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey? taskbarSettings = currentUserKey.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings"))
                    {
                        taskbarSettings?.SetValue("TaskbarEndTask", 1, RegistryValueKind.DWord);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Impostazione End Task non ripristinata: {ex}");
                throw;
            }
        }

        private void AvviaProcessoRimuoviCopilot()
        {
            try
            {
                UpdateCopilotMachinePolicyAndPackage(disableCopilot: true);

                using (RegistryKey currentUserKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey windowsCopilotCU = currentUserKey.CreateSubKey(@"Software\Policies\Microsoft\Windows\WindowsCopilot"))
                    {
                        windowsCopilotCU?.SetValue("TurnOffWindowsCopilot", 1, RegistryValueKind.DWord);
                    }

                    using (RegistryKey explorerAdvanced = currentUserKey.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced"))
                    {
                        explorerAdvanced?.SetValue("ShowCopilotButton", 0, RegistryValueKind.DWord);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Rimozione Copilot non riuscita: {ex}");
                throw;
            }
        }

        private void AvviaProcessoAggiungiCopilot()
        {
            try
            {
                UpdateCopilotMachinePolicyAndPackage(disableCopilot: false);
                using (RegistryKey currentUserKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey windowsCopilotCU = currentUserKey.CreateSubKey(@"Software\Policies\Microsoft\Windows\WindowsCopilot"))
                    {
                        windowsCopilotCU?.SetValue("TurnOffWindowsCopilot", 0, RegistryValueKind.DWord);
                    }

                    using (RegistryKey explorerAdvanced = currentUserKey.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced"))
                    {
                        explorerAdvanced?.SetValue("ShowCopilotButton", 1, RegistryValueKind.DWord);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ripristino Copilot non riuscito: {ex}");
                throw;
            }
        }

        private void AvviaProcessoAbilitaecall()
        {
            try
            {
                StartElevatedDism("/online", "/enable-feature", "/featurename:Recall");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Attivazione Recall non riuscita: {ex}");
                throw;
            }
        }

        private void AvviaProcessoRimuovirecall()
        {
            try
            {
                StartElevatedDism("/online", "/disable-feature", "/featurename:Recall");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Disattivazione Recall non riuscita: {ex}");
                throw;
            }
        }

        private static void StartElevatedDism(params string[] arguments)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = Path.Join(Environment.SystemDirectory, "dism.exe"),
                Verb = "runas",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Normal
            };
            foreach (string argument in arguments)
                processInfo.ArgumentList.Add(argument);

            using Process process = Process.Start(processInfo)
                ?? throw new InvalidOperationException("Impossibile avviare DISM con privilegi elevati.");
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new InvalidOperationException($"DISM è terminato con codice {process.ExitCode}.");
        }

        private static void UpdateCopilotMachinePolicyAndPackage(bool disableCopilot)
        {
            var mutations = new ElevatedRegistryMutationBatch();
            mutations.SetValue(
                RegistryHive.LocalMachine,
                @"SOFTWARE\Policies\Microsoft\Windows\WindowsCopilot",
                "TurnOffWindowsCopilot",
                disableCopilot ? 1 : 0,
                RegistryValueKind.DWord,
                RegistryView.Registry32);

            string packageCommand = disableCopilot
                ? "& $dism /online /remove-package /package-name:Microsoft.Windows.Copilot"
                : "& $dism /online /add-package /package-name:Microsoft.Windows.Copilot";
            string script = string.Join(Environment.NewLine,
                mutations.BuildCommand(),
                "$dism = Join-Path $env:SystemRoot 'System32\\dism.exe'",
                packageCommand,
                "exit $LASTEXITCODE");
            var processInfo = new ProcessStartInfo
            {
                FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                UseShellExecute = true,
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Normal
            };
            processInfo.ArgumentList.Add("-NoProfile");
            processInfo.ArgumentList.Add("-NonInteractive");
            processInfo.ArgumentList.Add("-EncodedCommand");
            processInfo.ArgumentList.Add(Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(script)));

            using Process process = Process.Start(processInfo)
                ?? throw new InvalidOperationException("Impossibile avviare la modifica Copilot con privilegi elevati.");
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new InvalidOperationException($"La modifica del pacchetto Copilot è terminata con codice {process.ExitCode}.");
        }

        private void AvviaProcessoOttimizzaRicerca()
        {
            try
            {
                SetStringRegistryValue(@"SOFTWARE\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell", "FolderType", "NotSpecified", RegistryView.Registry64);
                SetStringRegistryValue(@"SOFTWARE\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell", "FolderType", "NotSpecified", RegistryView.Registry32);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ottimizzazione ricerca Explorer non riuscita: {ex}");
                throw;
            }
        }
        private void AvviaProcessoDisabilitaOttimizzaRicerca()
        {
            try
            {
                string pathShellBags = @"SOFTWARE\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell";
                SetStringRegistryValue(pathShellBags, "FolderType", "Generic", RegistryView.Registry64);
                SetStringRegistryValue(pathShellBags, "FolderType", "Generic", RegistryView.Registry32);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ripristino ottimizzazione ricerca Explorer non riuscito: {ex}");
                throw;
            }
        }
        private void AvviaProcessoDisabilitaSuggeriti()
        {
            try
            {
                string registryPath = @"SOFTWARE\Policies\Microsoft\Windows\Explorer";
                string valueName = "DisableSearchBoxSuggestions";
                DeleteRegistryValue(registryPath, valueName, RegistryView.Registry64);
                DeleteRegistryValue(registryPath, valueName, RegistryView.Registry32);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Disattivazione suggerimenti ricerca non riuscita: {ex}");
                throw;
            }
        }

        private void AvviaProcessoAbilitaSuggeriti()
        {
            try
            {
                string registryPath = @"SOFTWARE\Policies\Microsoft\Windows\Explorer";
                string valueName = "DisableSearchBoxSuggestions";
                string newValue = "1";
                SetStringRegistryValue(registryPath, valueName, newValue, RegistryView.Registry64);
                SetStringRegistryValue(registryPath, valueName, newValue, RegistryView.Registry32);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Attivazione suggerimenti ricerca non riuscita: {ex}");
                throw;
            }
        }
        private void AvviaProcessoDisabilitaRicercaInternet()
        {
            try
            {
                string registryPathBing = @"Software\Microsoft\Windows\CurrentVersion\Search";
                string valueNameBing = "BingSearchEnabled";
                string newValueBing = "0";
                SetStringRegistryValue(registryPathBing, valueNameBing, newValueBing, RegistryView.Registry64);
                SetStringRegistryValue(registryPathBing, valueNameBing, newValueBing, RegistryView.Registry32);
                string registryPathCortana = @"Software\Microsoft\Windows\CurrentVersion\Search";
                string valueNameCortana = "CortanaConsent";
                string newValueCortana = "0";
                SetStringRegistryValue(registryPathCortana, valueNameCortana, newValueCortana, RegistryView.Registry64);
                SetStringRegistryValue(registryPathCortana, valueNameCortana, newValueCortana, RegistryView.Registry32);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Disattivazione ricerca Internet non riuscita: {ex}");
                throw;
            }
        }
        private void AvviaProcessoAbilitaRicercaInternet()
        {
            try
            {
                string registryPathSearch = @"Software\Microsoft\Windows\CurrentVersion\Search";
                SetStringRegistryValue(registryPathSearch, "BingSearchEnabled", "1", RegistryView.Registry64);
                SetStringRegistryValue(registryPathSearch, "BingSearchEnabled", "1", RegistryView.Registry32);
                SetStringRegistryValue(registryPathSearch, "CortanaConsent", "1", RegistryView.Registry64);
                SetStringRegistryValue(registryPathSearch, "CortanaConsent", "1", RegistryView.Registry32);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Attivazione ricerca Internet non riuscita: {ex}");
                throw;
            }
        }

        private async Task AvviaProcessoConRegFile(string regFileName)
        {
            try
            {
                string zipFileUrl = await OttieniUrlRegFile(Dipendenze.GitHubConfigUrl);
                string zipFilePath = Path.Join(tempFolder, "resources.zip");
                await ScaricaFile(zipFileUrl, zipFilePath);
                string? regFilePath = EstraiFileReg(zipFilePath, regFileName);
                if (regFilePath is null)
                    throw new FileNotFoundException($"File .reg '{regFileName}' non trovato nel file ZIP.");
                EseguiFileReg(regFilePath);
            }
            finally
            {
                if (Directory.Exists(tempFolder))
                    Directory.Delete(tempFolder, recursive: true);
            }
        }


        private async Task<string> OttieniUrlRegFile(string jsonUrl)
        {
            Uri configUri = RequireTrustedHttpsUri(jsonUrl);
            var response = await TrustedHttpsClient.GetStringAsync(ResourceClient, configUri.AbsoluteUri);
            var json = JObject.Parse(response);
            string? resourceUrl = json["PersonaTastoDestro"]?["PersoTastoDestro"]?.Value<string>();
            return RequireTrustedHttpsUri(resourceUrl
                ?? throw new InvalidOperationException("URL risorsa non presente nella configurazione.")).ToString();
        }

        private async Task ScaricaFile(string url, string filePath)
        {
            Uri resourceUri = RequireTrustedHttpsUri(url);
            _ = Directory.CreateDirectory(tempFolder);
            await DownloadManager.DownloadFileAsync(resourceUri.ToString(), filePath,
                CancellationToken.None, autoParallel: false);
        }

        private string? EstraiFileReg(string zipFilePath, string regFileName)
        {
            string extractionRoot = Path.GetFullPath(tempFolder) + Path.DirectorySeparatorChar;
            string extractedRegFilePath = Path.GetFullPath(Path.Join(tempFolder, regFileName));
            if (!extractedRegFilePath.StartsWith(extractionRoot, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Percorso di estrazione non valido.");
            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.Equals(regFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        entry.ExtractToFile(extractedRegFilePath, true);
                        return extractedRegFilePath;
                    }
                }
            }

            return null;
        }

        private void EseguiFileReg(string filePath)
        {
            string systemRegeditPath = Path.Join(Environment.SystemDirectory, "regedit.exe");
            ImportRegistryFile(systemRegeditPath, filePath);

            if (Environment.Is64BitOperatingSystem)
            {
                string? windowsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                if (!string.IsNullOrWhiteSpace(windowsDirectory))
                {
                    string regedit32Path = Path.Join(windowsDirectory, "SysWOW64", "regedit.exe");
                    if (File.Exists(regedit32Path))
                    {
                        ImportRegistryFile(regedit32Path, filePath);
                    }
                }
            }
        }

        private static void ImportRegistryFile(string regeditPath, string filePath)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = regeditPath,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.StartInfo.ArgumentList.Add("/s");
            process.StartInfo.ArgumentList.Add(filePath);

            if (!process.Start())
                throw new InvalidOperationException($"Impossibile avviare l'importazione del file registro: {regeditPath}");

            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new InvalidOperationException($"regedit è terminato con codice {process.ExitCode}.");
        }


        private void AvviaProcessoDestroDefault()
        {
            string registryPath = @"SOFTWARE\CLASSES\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}";
            DeleteCurrentUserSubKeyIfPresent(registryPath, RegistryView.Registry32);
            DeleteCurrentUserSubKeyIfPresent(registryPath, RegistryView.Registry64);
        }

        private void AvviaProcessoDestroLegacy()
        {
            string registryPath = @"SOFTWARE\CLASSES\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32";
            SetCurrentUserDefaultString(registryPath, RegistryView.Registry32);
            SetCurrentUserDefaultString(registryPath, RegistryView.Registry64);
        }

        private static void DeleteCurrentUserSubKeyIfPresent(string subKeyPath, RegistryView registryView)
        {
            using RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, registryView);
            bool exists;
            using (RegistryKey? existingKey = baseKey.OpenSubKey(subKeyPath, writable: true))
            {
                exists = existingKey is not null;
            }

            if (exists)
                baseKey.DeleteSubKeyTree(subKeyPath, throwOnMissingSubKey: false);
        }

        private static void SetCurrentUserDefaultString(string subKeyPath, RegistryView registryView)
        {
            using RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, registryView);
            using RegistryKey key = baseKey.CreateSubKey(subKeyPath)
                ?? throw new UnauthorizedAccessException($"Impossibile creare HKCU\\{subKeyPath} nella vista {registryView}.");
            key.SetValue(string.Empty, string.Empty, RegistryValueKind.String);
        }

        private void AvviaProcessoMostraSecondi()
        {
            UpdateRegistryValue(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                "ShowSecondsInSystemClock",
                1,
                RegistryValueKind.DWord
            );
        }

        private void AvviaProcessoMostraDataSecondi()
        {
            UpdateRegistryValue(
                @"Control Panel\International",
                "sShortDate",
                "ddd dd/MM/yyyy",
                RegistryValueKind.String
            );

            UpdateRegistryValue(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                "ShowSecondsInSystemClock",
                1,
                RegistryValueKind.DWord
            );
        }

        private void AvviaProcessoOrologioStandard()
        {
            UpdateRegistryValue(
                @"Control Panel\International",
                "sShortDate",
                "dd/MM/yyyy",
                RegistryValueKind.String
            );

            UpdateRegistryValue(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                "ShowSecondsInSystemClock",
                0,
                RegistryValueKind.DWord
            );
        }

        private void AvviaProcessoNascondiOraData()
        {
            UpdateRegistryValue(
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                "ShowSystrayDateTimeValueName",
                0,
                RegistryValueKind.DWord
            );
        }

        private void AvviaProcessoMostraOraData()
        {
            UpdateRegistryValue(
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                "ShowSystrayDateTimeValueName",
                1,
                RegistryValueKind.DWord
            );
        }

        private static void UpdateRegistryValue(string registryPath, string valueName, object newValue, RegistryValueKind valueKind)
        {
            using RegistryKey key = Registry.CurrentUser.CreateSubKey(registryPath, writable: true)
                ?? throw new InvalidOperationException($"Impossibile aprire o creare HKCU\\{registryPath}.");
            key.SetValue(valueName, newValue, valueKind);
        }

        private static void RestartExplorer()
        {
            int currentSessionId = Process.GetCurrentProcess().SessionId;
            foreach (Process process in Process.GetProcessesByName("explorer"))
            {
                using (process)
                {
                    if (process.SessionId == currentSessionId)
                        process.Kill();
                }
            }
            string explorerPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");
            _ = Process.Start(new ProcessStartInfo(explorerPath) { UseShellExecute = true })
                ?? throw new InvalidOperationException("Impossibile riavviare Esplora file.");
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = "Settaggi";
            form1.PnlFormLoader.Controls.Clear();
            formSettaggi = new FormSettaggi(form1)
            {
                Dock = DockStyle.Fill,
                TopLevel = false,
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None
            };
            form1.PnlFormLoader.Controls.Add(formSettaggi);
            formSettaggi.Show();
        }

        private void btn_resetselezione_Click(object sender, EventArgs e)
        {
            radio_orologiomostrasecondi.Checked = false;
            radio_orologiomostradatasecondi.Checked = false;
            radio_orologiostandard.Checked = false;
            radio_orologionascondioradata.Checked = false;
            radio_orologiomostraoradata.Checked = false;
            radio_destrolegacy.Checked = false;
            radio_destrodefault.Checked = false;
            radio_apricmd.Checked = false;
            radio_eliminaapricmd.Checked = false;
            radio_apripowershell.Checked = false;
            radio_eliminapowershell.Checked = false;
            radio_disabilitaricercainternet.Checked = false;
            radio_abilitaRicercainternet.Checked = false;
            radio_abilitasuggeriti.Checked = false;
            radio_disabilitasuggeriti.Checked = false;
            radio_ottimizzaricerca.Checked = false;
            radio_disabilitaottimizzaricerca.Checked = false;
            radio_abilitarecall.Checked = false;
            radio_disabilitarecall.Checked = false;
            radio_attivafx.Checked = false;
            radio_disattivafx.Checked = false;
            radio_disacopilot.Checked = false;
            radio_abilicopilot.Checked = false;
            radio_abilitaendtask.Checked = false;
            radio_disabilitaendtask.Checked = false;
        }

        private bool GetCheckboxState(string itemName)
        {
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey("Software\\WinHubX\\Personalizzazione"))
            {
                if (key != null)
                {
                    object? value = key.GetValue(itemName);
                    if (value != null)
                    {
                        return (int)value == 1;
                    }
                }
            }
            return false;
        }

        private void FormPersonalizzazione_Load(object sender, EventArgs e)
        {
            radio_orologiomostrasecondi.Checked = GetCheckboxState("MostraSecondi");
            radio_orologiomostradatasecondi.Checked = GetCheckboxState("MostraDataSecondi");
            radio_orologiostandard.Checked = GetCheckboxState("OrologioStandard");
            radio_orologionascondioradata.Checked = GetCheckboxState("NascondiOraData");
            radio_orologiomostraoradata.Checked = GetCheckboxState("MostraOraData");
            radio_destrolegacy.Checked = GetCheckboxState("DestroLegacy");
            radio_destrodefault.Checked = GetCheckboxState("DestroDefault");
            radio_apricmd.Checked = GetCheckboxState("ApriCMD");
            radio_eliminaapricmd.Checked = GetCheckboxState("EliminaApriCMD");
            radio_apripowershell.Checked = GetCheckboxState("ApriPowerShell");
            radio_eliminapowershell.Checked = GetCheckboxState("EliminaPowerShell");
            radio_attivafx.Checked = GetCheckboxState("AttivaFX");
            radio_disattivafx.Checked = GetCheckboxState("DisattivaFx");
            radio_disabilitaricercainternet.Checked = GetCheckboxState("DisabilitaRicercaInternet");
            radio_abilitaRicercainternet.Checked = GetCheckboxState("AbilitaRicercaInternet");
            radio_abilitasuggeriti.Checked = GetCheckboxState("AbilitaSuggeriti");
            radio_disabilitasuggeriti.Checked = GetCheckboxState("DisabilitaSuggeriti");
            radio_ottimizzaricerca.Checked = GetCheckboxState("OttimizzaRicerca");
            radio_disabilitaottimizzaricerca.Checked = GetCheckboxState("DisabilitaOttimizzaRicerca");
            radio_abilitarecall.Checked = GetCheckboxState("AbilitaRecall");
            radio_disabilitarecall.Checked = GetCheckboxState("DisabilitaRecall");
            radio_abilicopilot.Checked = GetCheckboxState("AbilitaCopilot");
            radio_disacopilot.Checked = GetCheckboxState("DisablitaCopilot");
            radio_abilitaendtask.Checked = GetCheckboxState("AbiliEndTask");
            radio_disabilitaendtask.Checked = GetCheckboxState("DisabilEndTask");
        }

        public void SetStringRegistryValue(string path, string valueName, string value, RegistryView view)
        {
            using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view))
            {
                using RegistryKey key = baseKey.CreateSubKey(path, writable: true)
                    ?? throw new InvalidOperationException($"Impossibile aprire o creare HKCU\\{path}.");
                key.SetValue(valueName, value, RegistryValueKind.String);
            }
        }

        public void DeleteRegistryValue(string path, string valueName, RegistryView view)
        {
            using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view))
            {
                using (RegistryKey? key = baseKey.OpenSubKey(path, writable: true))
                {
                    key?.DeleteValue(valueName, throwOnMissingValue: false);
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (e.Argument is not string[] selectedOptionNames)
            {
                throw new InvalidOperationException("Selezione delle personalizzazioni non valida.");
            }

            var selectedOptions = selectedOptionNames.ToHashSet(StringComparer.Ordinal);
            int currentStep = 0;
            if (selectedOptions.Contains(nameof(radio_orologiomostrasecondi)))
            {
                AvviaProcessoMostraSecondi();
                SetCheckboxState("MostraSecondi", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_orologiomostradatasecondi)))
            {
                AvviaProcessoMostraDataSecondi();
                SetCheckboxState("MostraDataSecondi", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_orologiostandard)))
            {
                AvviaProcessoOrologioStandard();
                SetCheckboxState("OrologioStandard", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_orologionascondioradata)))
            {
                AvviaProcessoNascondiOraData();
                SetCheckboxState("NascondiOraData", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_orologiomostraoradata)))
            {
                AvviaProcessoMostraOraData();
                SetCheckboxState("MostraOraData", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_destrolegacy)))
            {
                AvviaProcessoDestroLegacy();
                SetCheckboxState("DestroLegacy", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_destrodefault)))
            {
                AvviaProcessoDestroDefault();
                SetCheckboxState("DestroDefault", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_apricmd)))
            {
                AvviaProcessoConRegFile("cmdsi.reg").GetAwaiter().GetResult();
                SetCheckboxState("ApriCMD", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_eliminaapricmd)))
            {
                AvviaProcessoConRegFile("cmdno.reg").GetAwaiter().GetResult();
                SetCheckboxState("EliminaApriCMD", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_apripowershell)))
            {
                AvviaProcessoConRegFile("powershellsi.reg").GetAwaiter().GetResult();
                SetCheckboxState("ApriPowershell", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_eliminapowershell)))
            {
                AvviaProcessoConRegFile("powershellno.reg").GetAwaiter().GetResult();
                SetCheckboxState("EliminaPowershell", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_disattivafx)))
            {
                AvviaProcessoConRegFile("disabilita_tutti_visual_fx.reg").GetAwaiter().GetResult();
                SetCheckboxState("DisattivaFx", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_attivafx)))
            {
                AvviaProcessoConRegFile("abilita_visual_fx.reg").GetAwaiter().GetResult();
                SetCheckboxState("AttivaFx", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_disabilitaricercainternet)))
            {
                AvviaProcessoDisabilitaRicercaInternet();
                SetCheckboxState("DisabilitaRicercaInternet", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_abilitaRicercainternet)))
            {
                AvviaProcessoAbilitaRicercaInternet();
                SetCheckboxState("AbilitaRicercaInternet", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_abilitasuggeriti)))
            {
                AvviaProcessoAbilitaSuggeriti();
                SetCheckboxState("AbilitaSuggeriti", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_disabilitasuggeriti)))
            {
                AvviaProcessoDisabilitaSuggeriti();
                SetCheckboxState("DisabilitaSuggeriti", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_ottimizzaricerca)))
            {
                AvviaProcessoOttimizzaRicerca();
                SetCheckboxState("OttimizzaRicerca", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_disabilitaottimizzaricerca)))
            {
                AvviaProcessoDisabilitaOttimizzaRicerca();
                SetCheckboxState("DisabilitaOttimizzaRicerca", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_abilitarecall)))
            {
                AvviaProcessoAbilitaecall();
                SetCheckboxState("AbilitaRecall", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_disabilitarecall)))
            {
                AvviaProcessoRimuovirecall();
                SetCheckboxState("DisabilitaRecall", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_abilicopilot)))
            {
                AvviaProcessoAggiungiCopilot();
                SetCheckboxState("AbilitaCopilot", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_disacopilot)))
            {
                AvviaProcessoRimuoviCopilot();
                SetCheckboxState("DisablitaCopilot", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_abilitaendtask)))
            {
                AbiliaEndTask();
                SetCheckboxState("AbiliEndTask", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (selectedOptions.Contains(nameof(radio_disabilitaendtask)))
            {
                DisabilitaEndTask();
                SetCheckboxState("DisabilEndTask", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = Math.Min(e.ProgressPercentage, progressBar1.MaxValue);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            cuiButton1Verdi.Enabled = true;
            if (e.Error is not null)
            {
                Debug.WriteLine($"Applicazione delle personalizzazioni non riuscita: {e.Error}");
                MessageBox.Show(
                    $"Applicazione interrotta; alcune modifiche precedenti potrebbero essere già state applicate.\n{e.Error.Message}",
                    "WinHubX",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            try
            {
                RestartExplorer();
                MessageBox.Show(
                    LanguageManager.GetTranslation("Global", "modifichesuccesso"),
                    "WinHubX",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception
                or IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or InvalidOperationException)
            {
                Debug.WriteLine($"Riavvio Esplora file dopo personalizzazione non riuscito: {ex}");
                MessageBox.Show(
                    $"Le impostazioni sono state applicate, ma Esplora file non è stato riavviato automaticamente.\n{ex.Message}",
                    "WinHubX",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void SetCheckboxState(string itemName, bool isChecked)
        {
            using RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\WinHubX\\Personalizzazione")
                ?? throw new InvalidOperationException("Impossibile salvare lo stato delle personalizzazioni.");
            key.SetValue(itemName, isChecked ? 1 : 0, RegistryValueKind.DWord);
        }

        private void btnSettaggiExplorer_Click(object sender, EventArgs e)
        {
            FormExplorer formExplorer = new FormExplorer();
            formExplorer.Show();
        }


        private void radio_orologio_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox current && current.Checked)
            {
                foreach (CheckBox chk in panel78.Controls.OfType<CheckBox>()
                    .Where(chk => chk.Name.StartsWith("radio_orologio", StringComparison.Ordinal) && chk != current))
                {
                    chk.Checked = false;
                }
            }
        }
        private void panel77_CheckChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox current && current.Checked)
            {
                string text = current.Text;

                bool isAbilita = text.StartsWith("Abilita");
                bool isDisabilita = text.StartsWith("Disabilita");

                if (!isAbilita && !isDisabilita)
                    return;
                string baseText = isAbilita
                    ? text.Substring("Abilita ".Length)
                    : text.Substring("Disabilita ".Length);

                foreach (CheckBox chk in panel77.Controls.OfType<CheckBox>().Where(chk =>
                    chk != current && IsOppositeSetting(chk.Text, isAbilita, baseText, "Abilita ", "Disabilita ")))
                {
                    chk.Checked = false;
                }
            }
        }

        private void panel76_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox current && current.Checked)
            {
                string text = current.Text;
                if (text == "Tasto destro default" || text == "Tasto destro legacy")
                    return;

                bool isAggiungi = text.StartsWith("Aggiungi");
                bool isRimuovi = text.StartsWith("Rimuovi");

                if (!isAggiungi && !isRimuovi)
                    return;
                string baseText = isAggiungi
                    ? text.Substring("Aggiungi ".Length)
                    : text.Substring("Rimuovi ".Length);

                foreach (CheckBox chk in panel76.Controls.OfType<CheckBox>().Where(chk =>
                    chk != current && IsOppositeSetting(chk.Text, isAggiungi, baseText, "Aggiungi ", "Rimuovi ")))
                {
                    chk.Checked = false;
                }
            }
        }

        private static bool IsOppositeSetting(string candidateText, bool enabling, string baseText, string enablePrefix, string disablePrefix)
        {
            string expectedPrefix = enabling ? disablePrefix : enablePrefix;
            return candidateText.StartsWith(expectedPrefix, StringComparison.Ordinal)
                && string.Equals(candidateText[expectedPrefix.Length..], baseText, StringComparison.Ordinal);
        }

    }
}
