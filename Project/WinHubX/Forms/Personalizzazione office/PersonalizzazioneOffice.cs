using System.Diagnostics;
using System.Reflection;
using System.Xml;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Personalizzazione_office
{
    public partial class PersonalizzazioneOffice : Form
    {
        private Form1 form1;
        private FormOffice formoffice;

        public PersonalizzazioneOffice(Form1 form1, FormOffice formoffice)
        {
            InitializeComponent();
            form1 = form1;
            formoffice = formoffice;
            ActiveControl = progressBar_office;
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            btn_CreaISOVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Installa Office",
                "en" => "  Install Office",
                _ => btn_CreaISOVerdi.Content
            };
        }

        private void btn_avviainstallazione_Click(object sender, EventArgs e)
        {
            progressBar_office.Visible = true;
            string version = comboBoxVerOffice.SelectedItem?.ToString();
            string language = comboBox_Lingua.SelectedItem?.ToString()?.ToUpperInvariant() ?? "IT";
            string arch = GetArchitecture();

            if (string.IsNullOrEmpty(version))
            {
                MessageBox.Show("Seleziona una versione di Office.");
                return;
            }
            string archLabel = (arch == "64" || arch == "ARM64") ? "x64" : "x32";
            string xmlFileName = $"Configurazione{version.Replace(" ", "")}{archLabel}.xml";
            string xmlFilePath = Path.Combine(Path.GetTempPath(), xmlFileName);
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

            foreach (var kvp in apps)
            {
                if (kvp.Key.Checked)
                    RemoveElementFromXml(xmlFilePath, "ExcludeApp", kvp.Value);
            }

            StartInstallation(xmlFilePath);
        }

        private string GetArchitecture()
        {
            try
            {
                string path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "WinHubX", "Computer", "osehardware.json");

                if (File.Exists(path))
                {
                    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText(path));
                    string arch = json?.Architettura?.ToString()?.ToUpperInvariant();
                    if (arch == "ARM64" || arch == "64")
                        return "64";
                    if (arch == "32" || arch == "X86")
                        return "32";
                }
            }
            catch { }
            return "64";
        }

        private void AddVisioElement(string version, string xmlFilePath)
        {
            string xmlToAdd = version switch
            {
                "Office 2019" => CreateVisioXml19(),
                "Office 2021" => CreateVisioXml(),
                "Office 2024" => CreateVisioXml24(),
                "Office 365" => CreateVisioXml365(),
                _ => null
            };

            if (xmlToAdd != null)
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
                _ => null
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
                if (attr?.Value == oldLang)
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
                _ => null
            };

            if (productId == null)
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

                targetNode.ParentNode.InsertAfter(fragment, targetNode);
                xmlDoc.Save(xmlFilePath);
            }
            catch (Exception ex)
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
                    node.ParentNode.RemoveChild(node);

                xmlDoc.Save(xmlFilePath);
            }
            catch (Exception ex)
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
            catch (Exception ex)
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

        private async void StartInstallation(string xmlFilePath)
        {
            try
            {
                progressBar_office.Value = 0;
                string tempPath = Path.Combine(Path.GetTempPath(), "OfficePersonalizzato");
                Directory.CreateDirectory(tempPath);
                string binExePath = Path.Combine(tempPath, "bin.exe");
                ExtractAndSaveResource("bin.exe", binExePath);

                progressBar_office.Value = 15;
                await Task.Delay(5000);

                if (!File.Exists(binExePath))
                    throw new FileNotFoundException("Executable not found.", binExePath);

                progressBar_office.Value = 30;
                await Task.Delay(3000);

                string arguments = $"/configure \"{xmlFilePath}\"";
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = binExePath,
                    Arguments = arguments,
                    WorkingDirectory = tempPath,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (Process process = Process.Start(startInfo))
                {
                    progressBar_office.Value = 50;
                    await Task.Delay(6000);

                    if (process != null)
                        await Task.Run(() => process.WaitForExit());
                }

                progressBar_office.Value = 75;
                await Task.Delay(4000);

                File.Delete(xmlFilePath);
                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                }
                progressBar_office.Value = 100;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PersonalizzazioneOffice_FormClosing(object sender, FormClosingEventArgs e)
        {
            string tempPath = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "OfficePersonalizzato");
            if (Directory.Exists(tempPath))
            {
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void comboBoxVerOffice_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel7.Visible = true;

            string selected = comboBoxVerOffice.SelectedItem?.ToString();

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

        private void PersonalizzazioneOffice_Load(object sender, EventArgs e)
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
        private void PictureBox_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pb && pb.Tag is CheckBox cb)
            {
                cb.Checked = !cb.Checked;
            }
        }
    }
}
