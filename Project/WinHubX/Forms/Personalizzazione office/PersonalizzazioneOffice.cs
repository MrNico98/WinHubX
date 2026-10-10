using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Runtime.InteropServices;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Personalizzazione_office
{
    public partial class PersonalizzazioneOffice : Form
    {
        private static bool IsExpectedOfficeInstallationFailure(Exception exception) =>
            exception is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or System.ComponentModel.Win32Exception
                or System.Runtime.InteropServices.COMException
                or InvalidOperationException
                or NotSupportedException
                or ArgumentException
                or OperationCanceledException
                or TimeoutException
                or XmlException;

        private readonly Form1 form1;
        private readonly FormOffice formoffice;
        private CancellationTokenSource? _installationCancellation;
        private Task? _installationTask;
        private bool _closeAfterInstallation;
        private bool _allowClose;

        public PersonalizzazioneOffice(Form1 form1, FormOffice formoffice)
        {
            InitializeComponent();
            this.form1 = form1;
            this.formoffice = formoffice;
            ActiveControl = progressBar_office;
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            btn_CreaISOVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Installa Office",
                "en" => "  Install Office",
                _ => btn_CreaISOVerdi.Content
            };
        }

        private async void btn_avviainstallazione_Click(object? sender, EventArgs e)
        {
            if (_installationTask is { IsCompleted: false })
            {
                MessageBox.Show("È già in corso una personalizzazione di Office.");
                return;
            }

            progressBar_office.Visible = true;
            string version = comboBoxVerOffice.SelectedItem?.ToString() ?? string.Empty;
            string language = comboBox_Lingua.SelectedItem?.ToString()?.ToUpperInvariant() ?? "IT";
            string arch = GetArchitecture();

            if (string.IsNullOrEmpty(version))
            {
                MessageBox.Show("Seleziona una versione di Office.");
                return;
            }
            string archLabel = (arch == "64" || arch == "ARM64") ? "x64" : "x32";
            string xmlFileName = $"Configurazione{version.Replace(" ", "")}{archLabel}.xml";
            string sessionDirectory = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WinHubX",
                "OfficePersonalizzato",
                Guid.NewGuid().ToString("N"));
            string xmlFilePath = Path.Join(sessionDirectory, xmlFileName);
            try
            {
                Directory.CreateDirectory(sessionDirectory);
                ExtractAndSaveResource(xmlFileName, xmlFilePath);
                if (language == "EN")
                    ModifyElementFromXml(xmlFilePath, "it-it", "en-gb");
                if (checkBox_visio.Checked)
                    AddVisioElement(version, xmlFilePath);
                if (checkBox_project.Checked)
                    AddProjectElement(version, xmlFilePath);
                Dictionary<CheckBox, string> apps = new()
                {
                    { checkBox_word, "Word" },
                    { checkBox_excel, "Excel" },
                    { checkBox_powerpoint, "PowerPoint" },
                    { checkBox_outlook, "Outlook" },
                    { checkBox_onenote, "OneNote" },
                    { checkBox_onedrive, "OneDrive" },
                    { checkBox_publisher, "Publisher" },
                    { checkBox_access, "Access" }
                };

                foreach (var kvp in apps.Where(static pair => pair.Key.Checked))
                {
                    RemoveElementFromXml(xmlFilePath, "ExcludeApp", kvp.Value);
                }
            }
            catch (Exception ex) when (IsExpectedOfficeInstallationFailure(ex))
            {
                try
                {
                    if (Directory.Exists(sessionDirectory))
                        Directory.Delete(sessionDirectory, recursive: true);
                }
                catch (Exception cleanupException) when (cleanupException is IOException or UnauthorizedAccessException)
                {
                    Debug.WriteLine($"Cleanup sessione Office fallito: {cleanupException}");
                }

                MessageBox.Show($"Preparazione installazione non riuscita: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using CancellationTokenSource cancellation = new();
            _installationCancellation = cancellation;
            _installationTask = StartInstallationAsync(xmlFilePath, sessionDirectory, cancellation.Token);
            try
            {
                await _installationTask;
            }
            finally
            {
                _installationTask = null;
                _installationCancellation = null;
                if (_closeAfterInstallation && !IsDisposed && IsHandleCreated)
                {
                    _allowClose = true;
                    BeginInvoke(new Action(Close));
                }
            }
        }

        private string GetArchitecture()
        {
            try
            {
                string path = Path.Join(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "WinHubX", "Computer", "osehardware.json");

                if (File.Exists(path))
                {
                    var json = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(File.ReadAllText(path));
                    string? arch = json?["Architettura"]?.ToObject<string>()?.ToUpperInvariant();
                    if (arch == "ARM64" || arch == "64")
                        return "64";
                    if (arch == "32" || arch == "X86")
                        return "32";
                }
            }
            catch (Exception ex) when (IsExpectedOfficeInstallationFailure(ex))
            {
                Debug.WriteLine($"Lettura architettura hardware non riuscita; uso l'architettura OS: {ex}");
            }

            return RuntimeInformation.OSArchitecture switch
            {
                Architecture.X86 => "32",
                Architecture.X64 or Architecture.Arm64 => "64",
                _ => throw new PlatformNotSupportedException($"Architettura non supportata: {RuntimeInformation.OSArchitecture}.")
            };
        }

        private void AddVisioElement(string version, string xmlFilePath)
        {
            string xmlToAdd = version switch
            {
                "Office 2019" => CreateVisioXml19(),
                "Office 2021" => CreateVisioXml(),
                "Office 2024" => CreateVisioXml24(),
                "Office 365" => CreateVisioXml365(),
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(xmlToAdd))
                AddElementByVersion(version, xmlFilePath, xmlToAdd);
        }

        private void AddProjectElement(string version, string xmlFilePath)
        {
            string xmlToAdd = version switch
            {
                "Office 2019" => CreateProjectXml19(),
                "Office 2021" => CreateProjectXml(),
                "Office 2024" => CreateProjectXml24(),
                "Office 365" => CreateProjectXml365(),
                _ => string.Empty
            };

            if (xmlToAdd != null)
                AddElementByVersion(version, xmlFilePath, xmlToAdd);
        }

        private static void ModifyElementFromXml(string xmlFilePath, string oldLang, string newLang)
        {
            var doc = new XmlDocument();
            doc.Load(xmlFilePath);

            foreach (XmlNode node in doc.GetElementsByTagName("Language"))
            {
                var attr = node.Attributes?["ID"];
                if (attr is not null && attr.Value == oldLang)
                    attr.Value = newLang;
            }

            doc.Save(xmlFilePath);
        }

        private void AddElementByVersion(string version, string xmlFilePath, string xmlToAdd)
        {
            string productId = version switch
            {
                "Office 2019" => "ProPlus2019Volume",
                "Office 2021" => "ProPlus2021Volume",
                "Office 2024" => "ProPlus2024Volume",
                "Office 365" => "O365BusinessRetail",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(productId))
            {
                MessageBox.Show($"Versione '{version}' non riconosciuta.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFilePath);

                var targetNode = xmlDoc.SelectSingleNode($"//Product[@ID='{productId}']");
                if (targetNode == null)
                {
                    MessageBox.Show($"Nessun nodo Product con ID='{productId}' trovato nel file XML.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var fragment = xmlDoc.CreateDocumentFragment();
                fragment.InnerXml = xmlToAdd;

                targetNode.ParentNode?.InsertAfter(fragment, targetNode);
                xmlDoc.Save(xmlFilePath);
            }
            catch (Exception ex) when (IsExpectedOfficeInstallationFailure(ex))
            {
                MessageBox.Show($"Errore durante la modifica di '{xmlFilePath}': {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoveElementFromXml(string xmlFilePath, string elementName, string attributeValue)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFilePath);

                var nodes = xmlDoc.SelectNodes($"//{elementName}[@ID='{attributeValue}']");
                if (nodes == null || nodes.Count == 0)
                {
                    MessageBox.Show($"Nessun nodo {elementName} con ID='{attributeValue}' trovato.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                foreach (XmlNode node in nodes)
                    node.ParentNode?.RemoveChild(node);

                xmlDoc.Save(xmlFilePath);
            }
            catch (Exception ex) when (IsExpectedOfficeInstallationFailure(ex))
            {
                MessageBox.Show($"Errore durante la rimozione: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExtractAndSaveResource(string resourceName, string destinationPath)
        {
            try
            {
                string resourcePath = $"WinHubX.Resources.OfficePersonalizzato.{resourceName}";
                using Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath)
                    ?? throw new FileNotFoundException($"Risorsa non trovata: {resourcePath}");

                using FileStream fileStream = new(destinationPath, FileMode.Create, FileAccess.Write);
                resourceStream.CopyTo(fileStream);

            }
            catch (Exception ex) when (IsExpectedOfficeInstallationFailure(ex))
            {
                MessageBox.Show($"Errore durante l'estrazione di '{resourceName}': {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string CreateVisioXml24() => @"
<Product ID=""VisioPro2024Volume"" PIDKEY=""B7TN8-FJ8V3-7QYCP-HQPMV-YY89G"">
  <Language ID=""it-it"" />
  <ExcludeApp ID=""Access"" />
  <ExcludeApp ID=""Excel"" />
  <ExcludeApp ID=""Lync"" />
  <ExcludeApp ID=""OneDrive"" />
  <ExcludeApp ID=""OneNote"" />
  <ExcludeApp ID=""Outlook"" />
  <ExcludeApp ID=""PowerPoint"" />
  <ExcludeApp ID=""Publisher"" />
  <ExcludeApp ID=""Word"" />
</Product>";

        private string CreateVisioXml19() => @"
<Product ID=""VisioPro2019Volume"" PIDKEY=""9BGNQ-K37YR-RQHF2-38RQ3-7VCBB"">
  <Language ID=""it-it"" />
  <ExcludeApp ID=""Access"" />
  <ExcludeApp ID=""Groove"" />
  <ExcludeApp ID=""Excel"" />
  <ExcludeApp ID=""Lync"" />
  <ExcludeApp ID=""OneDrive"" />
  <ExcludeApp ID=""OneNote"" />
  <ExcludeApp ID=""Outlook"" />
  <ExcludeApp ID=""PowerPoint"" />
  <ExcludeApp ID=""Publisher"" />
  <ExcludeApp ID=""Word"" />
</Product>";

        private string CreateVisioXml() => @"
<Product ID=""VisioPro2021Volume"" PIDKEY=""KNH8D-FGHT4-T8RK3-CTDYJ-K2HT4"">
  <Language ID=""it-it"" />
  <ExcludeApp ID=""Access"" />
  <ExcludeApp ID=""Excel"" />
  <ExcludeApp ID=""Lync"" />
  <ExcludeApp ID=""OneDrive"" />
  <ExcludeApp ID=""OneNote"" />
  <ExcludeApp ID=""Outlook"" />
  <ExcludeApp ID=""PowerPoint"" />
  <ExcludeApp ID=""Publisher"" />
  <ExcludeApp ID=""Word"" />
</Product>";

        private string CreateVisioXml365() => @"
<Product ID=""VisioPro2021Volume"" PIDKEY=""KNH8D-FGHT4-T8RK3-CTDYJ-K2HT4"">
  <Language ID=""it-it"" />
  <ExcludeApp ID=""Access"" />
  <ExcludeApp ID=""Excel"" />
  <ExcludeApp ID=""Groove"" />
  <ExcludeApp ID=""Lync"" />
  <ExcludeApp ID=""OneDrive"" />
  <ExcludeApp ID=""OneNote"" />
  <ExcludeApp ID=""Outlook"" />
  <ExcludeApp ID=""PowerPoint"" />
  <ExcludeApp ID=""Publisher"" />
  <ExcludeApp ID=""Teams"" />
  <ExcludeApp ID=""Word"" />
</Product>";

        private string CreateProjectXml19() => @"
<Product ID=""ProjectPro2019Volume"" PIDKEY=""B4NPR-3FKK7-T2MBV-FRQ4W-PKD2B"">
  <Language ID=""it-it"" />
  <ExcludeApp ID=""Access"" />
  <ExcludeApp ID=""Excel"" />
  <ExcludeApp ID=""Lync"" />
  <ExcludeApp ID=""OneDrive"" />
  <ExcludeApp ID=""OneNote"" />
  <ExcludeApp ID=""Outlook"" />
  <ExcludeApp ID=""PowerPoint"" />
  <ExcludeApp ID=""Publisher"" />
  <ExcludeApp ID=""Word"" />
</Product>";

        private string CreateProjectXml() => @"
<Product ID=""ProjectPro2021Volume"" PIDKEY=""FTNWT-C6WBT-8HMGF-K9PRX-QV9H8"">
  <Language ID=""it-it"" />
  <ExcludeApp ID=""Access"" />
  <ExcludeApp ID=""Excel"" />
  <ExcludeApp ID=""Lync"" />
  <ExcludeApp ID=""OneDrive"" />
  <ExcludeApp ID=""OneNote"" />
  <ExcludeApp ID=""Outlook"" />
  <ExcludeApp ID=""PowerPoint"" />
  <ExcludeApp ID=""Publisher"" />
  <ExcludeApp ID=""Word"" />
</Product>";

        private string CreateProjectXml365() => @"
<Product ID=""ProjectPro2021Volume"" PIDKEY=""FTNWT-C6WBT-8HMGF-K9PRX-QV9H8"">
  <Language ID=""it-it"" />
  <ExcludeApp ID=""Access"" />
  <ExcludeApp ID=""Excel"" />
  <ExcludeApp ID=""Groove"" />
  <ExcludeApp ID=""Lync"" />
  <ExcludeApp ID=""OneDrive"" />
  <ExcludeApp ID=""OneNote"" />
  <ExcludeApp ID=""Outlook"" />
  <ExcludeApp ID=""PowerPoint"" />
  <ExcludeApp ID=""Publisher"" />
  <ExcludeApp ID=""Teams"" />
  <ExcludeApp ID=""Word"" />
</Product>";

        private string CreateProjectXml24() => @"
<Product ID=""ProjectPro2024Volume"" PIDKEY=""FQQ23-N4YCY-73HQ3-FM9WC-76HF4"">
  <Language ID=""it-it"" />
  <ExcludeApp ID=""Access"" />
  <ExcludeApp ID=""Excel"" />
  <ExcludeApp ID=""Lync"" />
  <ExcludeApp ID=""OneDrive"" />
  <ExcludeApp ID=""OneNote"" />
  <ExcludeApp ID=""Outlook"" />
  <ExcludeApp ID=""PowerPoint"" />
  <ExcludeApp ID=""Publisher"" />
  <ExcludeApp ID=""Word"" />
</Product>";

        private async Task StartInstallationAsync(
            string xmlFilePath,
            string sessionDirectory,
            CancellationToken cancellationToken)
        {
            try
            {
                progressBar_office.Value = 0;
                string binExePath = Path.Join(sessionDirectory, "bin.exe");
                ExtractAndSaveResource("bin.exe", binExePath);

                progressBar_office.Value = 15;

                if (!File.Exists(binExePath))
                    throw new FileNotFoundException("Executable not found.", binExePath);

                progressBar_office.Value = 30;
                cancellationToken.ThrowIfCancellationRequested();

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = binExePath,
                    WorkingDirectory = sessionDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                startInfo.ArgumentList.Add("/configure");
                startInfo.ArgumentList.Add(xmlFilePath);
                using (Process process = Process.Start(startInfo)
                    ?? throw new InvalidOperationException("Impossibile avviare l'installazione di Office."))
                {
                    progressBar_office.Value = 50;
                    // Il setup usa i file della sessione: attendere la sua uscita prima del cleanup.
                    await process.WaitForExitAsync(CancellationToken.None);
                    if (process.ExitCode != 0)
                    {
                        throw new InvalidOperationException(
                            $"Il programma di installazione di Office è terminato con codice {process.ExitCode}.");
                    }
                }

                progressBar_office.Value = 75;
                progressBar_office.Value = 100;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                Debug.WriteLine("Personalizzazione Office annullata prima dell'avvio del setup.");
            }
            catch (Exception ex) when (IsExpectedOfficeInstallationFailure(ex))
            {
                _ = MessageBox.Show($"Installazione Office non riuscita: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    if (Directory.Exists(sessionDirectory))
                        Directory.Delete(sessionDirectory, recursive: true);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    Debug.WriteLine($"Cleanup sessione personalizzazione Office non riuscito: {ex}");
                }
            }
        }

        private void PersonalizzazioneOffice_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_allowClose || _installationTask is not { IsCompleted: false })
                return;

            e.Cancel = true;
            _closeAfterInstallation = true;
            _installationCancellation?.Cancel();
            if (!IsDisposed)
            {
                Debug.WriteLine("Chiusura rinviata finché il processo Office non ha terminato e la sessione non è pulita.");
            }
        }

        private void comboBoxVerOffice_SelectedIndexChanged(object? sender, EventArgs e)
        {
            panel7.Visible = true;

            string? selected = comboBoxVerOffice.SelectedItem?.ToString();

            if (selected == null) return;

            if (selected.Contains("2019") || selected.Contains("2021") || selected.Contains("2024"))
            {
                pictureBoxWord.Image = Properties.Resources.Microsoft_Office_Word_20192025;

                pictureBoxExcel.Image = Properties.Resources.Microsoft_Office_Excel_20192025;
                pictureBoxPowerPoint.Image = Properties.Resources.Microsoft_Office_PowerPoint_20192025;
                pictureBoxAccess.Image = Properties.Resources.Microsoft_Office_Access_20192025;
                pictureBoxOneDrive.Image = Properties.Resources.Microsoft_Office_OneDrive_20192025;
                pictureBoxOneNote.Image = Properties.Resources.Microsoft_Office_OneNote_20192025;
                pictureBoxOutlok.Image = Properties.Resources.Microsoft_Office_Outlook_20182024;
                pictureBoxWord.Image = Properties.Resources.Microsoft_Office_Word_20192025;
                pictureBoxPublisher.Image = Properties.Resources.Microsoft_Office_Publisher_2019present;
                pictureBoxVisio.Image = Properties.Resources.Microsoft_Office_Visio_2019;
                pictureBoxProject.Image = Properties.Resources.Microsoft_Project_2019present;
            }
            else if (selected.Contains("365"))
            {
                pictureBoxWord.Image = Properties.Resources.Microsoft_Office_Word_2025present;
                pictureBoxExcel.Image = Properties.Resources.Microsoft_Office_Excel_2025present;
                pictureBoxPowerPoint.Image = Properties.Resources.Microsoft_Office_PowerPoint_2025present;
                pictureBoxAccess.Image = Properties.Resources.Microsoft_Office_Access_20192025;
                pictureBoxOneDrive.Image = Properties.Resources.Microsoft_OneDrive_Icon_2025present;
                pictureBoxOneNote.Image = Properties.Resources.Microsoft_OneNote_Icon_2025present;
                pictureBoxOutlok.Image = Properties.Resources.Microsoft_Outlook_Icon_2025present;
                pictureBoxPublisher.Image = Properties.Resources.Microsoft_Office_Publisher_2019present;
                pictureBoxVisio.Image = Properties.Resources.Microsoft_Office_Visio_2019;
                pictureBoxProject.Image = Properties.Resources.Microsoft_Project_2019present;
            }
        }

        private void PersonalizzazioneOffice_Load(object? sender, EventArgs e)
        {
            pictureBoxWord.Tag = checkBox_word;
            pictureBoxExcel.Tag = checkBox_excel;
            pictureBoxPowerPoint.Tag = checkBox_powerpoint;
            pictureBoxOutlok.Tag = checkBox_outlook;
            pictureBoxAccess.Tag = checkBox_access;
            pictureBoxOneDrive.Tag = checkBox_onedrive;
            pictureBoxOneNote.Tag = checkBox_onenote;
            pictureBoxVisio.Tag = checkBox_visio;
            pictureBoxProject.Tag = checkBox_project;
            pictureBoxPublisher.Tag = checkBox_publisher;
            pictureBoxWord.Click += PictureBox_Click;
            pictureBoxExcel.Click += PictureBox_Click;
            pictureBoxPowerPoint.Click += PictureBox_Click;
            pictureBoxOutlok.Click += PictureBox_Click;
            pictureBoxAccess.Click += PictureBox_Click;
            pictureBoxOneDrive.Click += PictureBox_Click;
            pictureBoxOneNote.Click += PictureBox_Click;
            pictureBoxVisio.Click += PictureBox_Click;
            pictureBoxProject.Click += PictureBox_Click;
            pictureBoxPublisher.Click += PictureBox_Click;

        }
        private void PictureBox_Click(object? sender, EventArgs e)
        {
            if (sender is PictureBox pb && pb.Tag is CheckBox cb)
            {
                cb.Checked = !cb.Checked;
            }
        }
    }
}
