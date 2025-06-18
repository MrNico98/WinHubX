using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using WinHubX.Forms.Base;


namespace WinHubX.Forms.Settaggi
{
    public partial class FormPersonalizzazione : Form
    {
        private Form1 form1;
        private FormSettaggi formSettaggi;
        private string tempFolder = Path.Combine(Path.GetTempPath(), "WinHubX");
        private int totalSteps = 0;
        private bool isSSD;
        public FormPersonalizzazione(FormSettaggi formSettaggi, Form1 form1)
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            this.form1 = form1;
            this.formSettaggi = formSettaggi;
        }
        public void ImpostaSpecifichePC(string driveType, string ramSize, bool isSSD)
        {
            this.isSSD = isSSD;
            label3.Text = driveType;
            label5.Text = ramSize;
        }

        private async void btnAvviaSelezionati_Click(object sender, EventArgs e)
        {
            totalSteps = 0;
            List<Panel> pannelli = new List<Panel>
    {
        panel1, panel3, panel4, panel5, panel6,
        panel8, panel9, panel10, panel11, panel12, panel13
    };
            foreach (var pannello in pannelli)
            {
                foreach (Control control in pannello.Controls)
                {
                    if (control is RadioButton radioButton && radioButton.Checked)
                    {
                        totalSteps++;
                    }
                }
            }

            if (totalSteps == 0)
            {
                totalSteps = 1;
            }

            progressBar1.Maximum = totalSteps;
            progressBar1.Value = 0;

            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }
        private void DisabilitaEndTask()
        {
            try
            {
                using (RegistryKey currentUserKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey taskbarSettings = currentUserKey.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings"))
                    {
                        taskbarSettings?.SetValue("TaskbarEndTask", 0, RegistryValueKind.DWord);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void AbiliaEndTask()
        {
            try
            {
                using (RegistryKey currentUserKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey taskbarSettings = currentUserKey.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings"))
                    {
                        taskbarSettings?.SetValue("TaskbarEndTask", 1, RegistryValueKind.DWord);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Si è verificato un errore durante l'abilitazione del pulsante 'Termina attività': " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AvviaProcessoRimuoviCopilot()
        {
            try
            {
                using (RegistryKey localMachineKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                using (RegistryKey windowsCopilotLM = localMachineKey.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsCopilot"))
                {
                    windowsCopilotLM?.SetValue("TurnOffWindowsCopilot", 1, RegistryValueKind.DWord);
                }

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
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c dism /online /remove-package /package-name:Microsoft.Windows.Copilot",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal
                };

                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AvviaProcessoAggiungiCopilot()
        {
            try
            {
                using (RegistryKey localMachineKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                using (RegistryKey windowsCopilotLM = localMachineKey.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsCopilot"))
                {
                    windowsCopilotLM?.SetValue("TurnOffWindowsCopilot", 0, RegistryValueKind.DWord);
                }
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
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c dism /online /add-package /package-name:Microsoft.Windows.Copilot",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal
                };

                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AvviaProcessoAbilitaecall()
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c dism /online /enable-feature /featurename:Recall",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal
                };
                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AvviaProcessoRimuovirecall()
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c dism /online /disable-feature /featurename:Recall",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal
                };
                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void AvviaProcessoDisabilitaRicercaInternet()
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
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task AvviaProcessoConRegFile(string regFileName)
        {
            string jsonUrl = "https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json";
            string zipFileUrl = await OttieniUrlRegFile(jsonUrl);

            if (!string.IsNullOrEmpty(zipFileUrl))
            {
                string zipFilePath = Path.Combine(tempFolder, "resources.zip");
                await ScaricaFile(zipFileUrl, zipFilePath);
                await Task.Delay(1000);
                string regFilePath = EstraiFileReg(zipFilePath, regFileName);
                if (regFilePath != null)
                {
                    EseguiFileReg(regFilePath);
                    await Task.Delay(3000);
                }
                else
                {
                    throw new FileNotFoundException($"File .reg '{regFileName}' non trovato nel file ZIP.");
                }
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                    await Task.Delay(3000);
                }
                if (Directory.Exists(tempFolder))
                {
                    try
                    {
                        Directory.Delete(tempFolder, true);
                        await Task.Delay(3000);
                    }
                    catch (IOException)
                    {
                        throw new IOException("Errore nell'eliminazione della cartella temporanea.");
                    }
                }
            }
            else
            {
                throw new Exception("URL del file ZIP non trovato.");
            }
        }

        private async Task<string> OttieniUrlRegFile(string jsonUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(jsonUrl);
                var json = JObject.Parse(response);
                return json["PersonaTastoDestro"]["PersoTastoDestro"].ToString();
            }
        }

        private async Task ScaricaFile(string url, string filePath)
        {
            using (HttpClient client = new HttpClient())
            {
                Directory.CreateDirectory(tempFolder);
                using (var response = await client.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    using (var fs = new FileStream(filePath, FileMode.CreateNew))
                    {
                        await response.Content.CopyToAsync(fs);
                    }
                }
            }
        }

        private string EstraiFileReg(string zipFilePath, string regFileName)
        {
            string extractedRegFilePath = Path.Combine(tempFolder, regFileName);
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
            string regedit64Path = @"C:\Windows\System32\regedit.exe";
            string regedit32Path = @"C:\Windows\SysWOW64\regedit.exe";

            try
            {
                System.Diagnostics.Process.Start(regedit64Path, $"/s \"{filePath}\"");
            }
            catch (Exception)
            {

            }

            try
            {
                System.Diagnostics.Process.Start(regedit32Path, $"/s \"{filePath}\"");
            }
            catch (Exception)
            {

            }
        }


        private void AvviaProcessoDestroDefault()
        {
            string registryPath = @"SOFTWARE\CLASSES\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}";
            try
            {
                using (RegistryKey key32 = Registry.CurrentUser.OpenSubKey(registryPath, true))
                {
                    if (key32 != null)
                    {
                        Registry.CurrentUser.DeleteSubKeyTree(registryPath, false);
                    }
                    else
                    {

                    }
                }
            }
            catch (UnauthorizedAccessException)
            {

            }
            catch (Exception)
            {

            }
            try
            {
                using (RegistryKey key64 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                {
                    using (RegistryKey subKey64 = key64.OpenSubKey(registryPath, true))
                    {
                        if (subKey64 != null)
                        {
                            key64.DeleteSubKeyTree(registryPath, false);

                        }
                        else
                        {

                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {

            }
            catch (Exception)
            {

            }
        }

        private void AvviaProcessoDestroLegacy()
        {
            string registryPath = @"SOFTWARE\CLASSES\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32";
            try
            {
                using (RegistryKey key32 = Registry.CurrentUser.CreateSubKey(registryPath))
                {
                    if (key32 != null)
                    {
                        key32.SetValue("", "", RegistryValueKind.String);

                    }
                    else
                    {

                    }
                }
            }
            catch (UnauthorizedAccessException)
            {

            }
            catch (Exception)
            {

            }
            try
            {
                using (RegistryKey key64 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).CreateSubKey(registryPath))
                {
                    if (key64 != null)
                    {
                        key64.SetValue("", "", RegistryValueKind.String);
                    }
                    else
                    {

                    }
                }
            }
            catch (UnauthorizedAccessException)
            {

            }
            catch (Exception)
            {

            }
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
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath, writable: true))
                {
                    if (key != null)
                    {
                        key.SetValue(valueName, newValue, valueKind);
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private static void RestartExplorer()
        {
            foreach (var process in Process.GetProcessesByName("explorer"))
            {
                process.Kill();
            }
            System.Diagnostics.Process.Start("explorer.exe");
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
            radio_mostrasecondi.Checked = false;
            radio_mostradatasecondi.Checked = false;
            radio_orologiostandard.Checked = false;
            radio_nascondioradata.Checked = false;
            radio_mostraoradata.Checked = false;
            radio_destrolegacy.Checked = false;
            radio_destrodefault.Checked = false;
            radio_apricmd.Checked = false;
            radio_eliminaapricmd.Checked = false;
            radio_apripowershell.Checked = false;
            radio_eliminapowershell.Checked = false;
            radio_disabilitaricercainternet.Checked = false;
            radio_abilitasuggeriti.Checked = false;
            radio_disabilitasuggeriti.Checked = false;
            radio_ottimizzaricerca.Checked = false;
            radio_abilitarecall.Checked = false;
            radio_disabilitarecall.Checked = false;
            radio_ottimizzawindows.Checked = false;
            radio_attivafx.Checked = false;
            radio_disattivafx.Checked = false;
            radio_ripristinaottimizzazionewin.Checked = false;
            radio_disacopilot.Checked = false;
            radio_abilicopilot.Checked = false;
            radio_abilitaendtask.Checked = false;
            radio_disabilitaendtask.Checked = false;
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\WinHubX\Personalizzazione", true);
            }
            catch (Exception)
            {

            }
        }

        private bool GetCheckboxState(string itemName)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\WinHubX\\Personalizzazione"))
            {
                if (key != null)
                {
                    object value = key.GetValue(itemName);
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
            radio_mostrasecondi.Checked = GetCheckboxState("MostraSecondi");
            radio_mostradatasecondi.Checked = GetCheckboxState("MostraDataSecondi");
            radio_orologiostandard.Checked = GetCheckboxState("OrologioStandard");
            radio_nascondioradata.Checked = GetCheckboxState("NascondiOraData");
            radio_mostraoradata.Checked = GetCheckboxState("MostraOraData");
            radio_destrolegacy.Checked = GetCheckboxState("DestroLegacy");
            radio_destrodefault.Checked = GetCheckboxState("DestroDefault");
            radio_apricmd.Checked = GetCheckboxState("ApriCMD");
            radio_eliminaapricmd.Checked = GetCheckboxState("EliminaApriCMD");
            radio_apripowershell.Checked = GetCheckboxState("ApriPowerShell");
            radio_eliminapowershell.Checked = GetCheckboxState("EliminaPowerShell");
            radio_ottimizzawindows.Checked = GetCheckboxState("OttimizzaWindows");
            radio_ripristinaottimizzazionewin.Checked = GetCheckboxState("RipristinaOttimizzazioneWin");
            radio_attivafx.Checked = GetCheckboxState("AttivaFX");
            radio_disattivafx.Checked = GetCheckboxState("DisattivaFx");
            radio_disabilitaricercainternet.Checked = GetCheckboxState("DisabilitaRicercaInternet");
            radio_abilitasuggeriti.Checked = GetCheckboxState("AbilitaSuggeriti");
            radio_disabilitasuggeriti.Checked = GetCheckboxState("DisabilitaSuggeriti");
            radio_ottimizzaricerca.Checked = GetCheckboxState("OttimizzaRicerca");
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
                using (RegistryKey key = baseKey.OpenSubKey(path, writable: true))
                {
                    if (key != null)
                    {
                        key.SetValue(valueName, value, RegistryValueKind.String);
                    }
                    else
                    {

                    }
                }
            }
        }

        public void DeleteRegistryValue(string path, string valueName, RegistryView view)
        {
            using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
            {
                using (RegistryKey key = baseKey.OpenSubKey(path, writable: true))
                {
                    if (key != null)
                    {
                        try
                        {
                            key.DeleteValue(valueName, throwOnMissingValue: false);
                        }
                        catch (ArgumentException)
                        {

                        }
                    }
                    else
                    {

                    }
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int currentStep = 0;
            if (radio_mostrasecondi.Checked)
            {
                AvviaProcessoMostraSecondi();
                SetCheckboxState("MostraSecondi", radio_mostrasecondi.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_mostradatasecondi.Checked)
            {
                AvviaProcessoMostraDataSecondi();
                SetCheckboxState("MostraDataSecondi", radio_mostradatasecondi.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_orologiostandard.Checked)
            {
                AvviaProcessoOrologioStandard();
                SetCheckboxState("OrologioStandard", radio_orologiostandard.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_nascondioradata.Checked)
            {
                AvviaProcessoNascondiOraData();
                SetCheckboxState("NascondiOraData", radio_nascondioradata.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_mostraoradata.Checked)
            {
                AvviaProcessoMostraOraData();
                SetCheckboxState("MostraOraData", radio_mostraoradata.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_destrolegacy.Checked)
            {
                AvviaProcessoDestroLegacy();
                SetCheckboxState("DestroLegacy", radio_destrolegacy.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_destrodefault.Checked)
            {
                AvviaProcessoDestroDefault();
                SetCheckboxState("DestroDefault", radio_destrodefault.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_apricmd.Checked)
            {
                AvviaProcessoConRegFile("cmdsi.reg");
                SetCheckboxState("ApriCMD", radio_apricmd.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_eliminaapricmd.Checked)
            {
                AvviaProcessoConRegFile("cmdno.reg");
                SetCheckboxState("EliminaApriCMD", radio_eliminaapricmd.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_apripowershell.Checked)
            {
                AvviaProcessoConRegFile("powershellsi.reg");
                SetCheckboxState("ApriPowershell", radio_apripowershell.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_eliminapowershell.Checked)
            {
                AvviaProcessoConRegFile("powershellno.reg");
                SetCheckboxState("EliminaPowershell", radio_eliminapowershell.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_ottimizzawindows.Checked)
            {
                if (isSSD)
                {
                    AvviaProcessoConRegFile("ottimizzazioni_ssd.reg");
                }
                else
                {
                    AvviaProcessoConRegFile("ottimizzazioni_hdd.reg");
                }
                SetCheckboxState("OttimazzazioneWindows", radio_ottimizzawindows.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_disattivafx.Checked)
            {
                AvviaProcessoConRegFile("disabilita_tutti_visual_fx.reg");
                SetCheckboxState("DisattivaFx", radio_disattivafx.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_attivafx.Checked)
            {
                AvviaProcessoConRegFile("abilita_visual_fx.reg");
                SetCheckboxState("AttivaFx", radio_attivafx.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_ripristinaottimizzazionewin.Checked)
            {
                AvviaProcessoConRegFile("ripristina_impostazioni_windows.reg");
                SetCheckboxState("RipristinaOttimizzazioneWin", radio_ripristinaottimizzazionewin.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_disabilitaricercainternet.Checked)
            {
                AvviaProcessoDisabilitaRicercaInternet();
                SetCheckboxState("DisabilitaRicercaInternet", radio_disabilitaricercainternet.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_abilitasuggeriti.Checked)
            {
                AvviaProcessoAbilitaSuggeriti();
                SetCheckboxState("AbilitaSuggeriti", radio_abilitasuggeriti.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_disabilitasuggeriti.Checked)
            {
                AvviaProcessoDisabilitaSuggeriti();
                SetCheckboxState("DisabilitaSuggeriti", radio_disabilitasuggeriti.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_ottimizzaricerca.Checked)
            {
                AvviaProcessoOttimizzaRicerca();
                SetCheckboxState("OttimizzaRicerca", radio_ottimizzaricerca.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_abilitarecall.Checked)
            {
                AvviaProcessoAbilitaecall();
                SetCheckboxState("AbilitaRecall", radio_abilitarecall.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_disabilitarecall.Checked)
            {
                AvviaProcessoRimuovirecall();
                SetCheckboxState("DisabilitaRecall", radio_disabilitarecall.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_abilicopilot.Checked)
            {
                AvviaProcessoAggiungiCopilot();
                SetCheckboxState("AbilitaCopilot", radio_abilicopilot.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_disacopilot.Checked)
            {
                AvviaProcessoRimuoviCopilot();
                SetCheckboxState("DisablitaCopilot", radio_disacopilot.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_abilitaendtask.Checked)
            {
                AbiliaEndTask();
                SetCheckboxState("AbiliEndTask", radio_disacopilot.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            if (radio_disabilitaendtask.Checked)
            {
                DisabilitaEndTask();
                SetCheckboxState("DisabilEndTask", radio_disacopilot.Checked);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = Math.Min(e.ProgressPercentage, progressBar1.Maximum);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            RestartExplorer();
            string messaggio = LanguageManager.GetTranslation("Global", "modifichesuccesso");

            MessageBox.Show(
                messaggio,
                "WinHubX",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void SetCheckboxState(string itemName, bool isChecked)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\WinHubX\\Personalizzazione"))
            {
                key.SetValue(itemName, isChecked ? 1 : 0, RegistryValueKind.DWord);
            }
        }
    }
}
