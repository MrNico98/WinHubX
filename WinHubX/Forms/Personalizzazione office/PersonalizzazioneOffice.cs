using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace WinHubX.Forms.Personalizzazione_office
{
    public partial class PersonalizzazioneOffice : Form
    {
        private Form1 form1;
        private FormOffice formoffice;

        public PersonalizzazioneOffice(Form1 form1, FormOffice formoffice)
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            this.form1 = form1;
            this.formoffice = formoffice;
            radiobutton_office2021.Checked = false;
            radiobutton_office365.Checked = false;
            this.ActiveControl = progressBar_office;
            ExtractFolderToTemp();
        }

        private void btn_avviainstallazione_Click(object sender, EventArgs e)
        {
            if (radiobutton_office2021.Checked)
            {
                if (radioButton_x64.Checked)
                {
                    string xmlFilePath = Path.Combine(GetTempFolderPath(), "Configurazione2021x64.xml");
                    if (radioButton_EN.Checked)
                    {
                        ModifyElementFromXml(xmlFilePath, "it-it", "en-gb");
                    }
                    if (checkBox_visio.Checked)
                    {
                        AddElementToXml(xmlFilePath, CreateVisioXml());
                    }
                    if (checkBox_project.Checked)
                    {
                        AddElementToXml(xmlFilePath, CreateProjectXml());
                    }
                    if (checkBox_word.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Word");
                    }
                    if (checkBox_excel.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Excel");
                    }
                    if (checkBox_powerpoint.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "PowerPoint");
                    }
                    if (checkBox_outlook.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Outlook");
                    }
                    if (checkBox_onenote.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "OneNote");
                    }
                    if (checkBox_onedrive.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "OneDrive");
                    }
                    if (checkBox_publisher.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Publisher");
                    }
                    if (checkBox_access.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Access");
                    }
                    StartInstallation(xmlFilePath);
                }
                else if (radioButton_x32.Checked)
                {
                    string xmlFilePath = @"C:\Configurazione2021x32.xml";
                    ExtractAndSaveResource("Configurazione2021x32.xml", xmlFilePath);
                    if (radioButton_EN.Checked)
                    {
                        ModifyElementFromXml(xmlFilePath, "it-it", "en-gb");
                    }
                    if (checkBox_visio.Checked)
                    {
                        AddElementToXml(xmlFilePath, CreateVisioXml());
                    }
                    if (checkBox_project.Checked)
                    {
                        AddElementToXml(xmlFilePath, CreateProjectXml());
                    }
                    if (checkBox_word.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Word");
                    }
                    if (checkBox_excel.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Excel");
                    }
                    if (checkBox_powerpoint.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "PowerPoint");
                    }
                    if (checkBox_outlook.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Outlook");
                    }
                    if (checkBox_onenote.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "OneNote");
                    }
                    if (checkBox_onedrive.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "OneDrive");
                    }
                    if (checkBox_publisher.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Publisher");
                    }
                    if (checkBox_access.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Access");
                    }
                    StartInstallation(xmlFilePath);
                }
            }
            if (radiobutton_office365.Checked)
            {
                if (radioButton_x64.Checked)
                {
                    string xmlFilePath = @"C:\Configurazione365x64.xml";
                    ExtractAndSaveResource("Configurazione365x64.xml", xmlFilePath);
                    if (radioButton_EN.Checked)
                    {
                        ModifyElementFromXml(xmlFilePath, "it-it", "en-gb");
                    }
                    if (checkBox_visio.Checked)
                    {
                        AddElementToXml365(xmlFilePath, CreateVisioXml365());
                    }
                    if (checkBox_project.Checked)
                    {
                        AddElementToXml365(xmlFilePath, CreateProjectXml365());
                    }
                    if (checkBox_word.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Word");
                    }
                    if (checkBox_excel.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Excel");
                    }
                    if (checkBox_powerpoint.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "PowerPoint");
                    }
                    if (checkBox_outlook.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Outlook");
                    }
                    if (checkBox_onenote.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "OneNote");
                    }
                    if (checkBox_onedrive.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "OneDrive");
                    }
                    if (checkBox_publisher.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Publisher");
                    }
                    if (checkBox_access.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Access");
                    }
                    StartInstallation(xmlFilePath);
                }
                else if (radioButton_x32.Checked)
                {
                    string xmlFilePath = @"C:\Configurazione365x32.xml";
                    ExtractAndSaveResource("Configurazione365x32.xml", xmlFilePath);
                    if (radioButton_EN.Checked)
                    {
                        ModifyElementFromXml(xmlFilePath, "it-it", "en-gb");
                    }
                    if (checkBox_visio.Checked)
                    {
                        AddElementToXml365(xmlFilePath, CreateVisioXml365());
                    }
                    if (checkBox_project.Checked)
                    {
                        AddElementToXml365(xmlFilePath, CreateProjectXml365());
                    }
                    if (checkBox_word.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Word");
                    }
                    if (checkBox_excel.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Excel");
                    }
                    if (checkBox_powerpoint.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "PowerPoint");
                    }
                    if (checkBox_outlook.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Outlook");
                    }
                    if (checkBox_onenote.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "OneNote");
                    }
                    if (checkBox_onedrive.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "OneDrive");
                    }
                    if (checkBox_publisher.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Publisher");
                    }
                    if (checkBox_access.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Access");
                    }

                    StartInstallation(xmlFilePath);
                }
            }
            if (radioButton_office2024.Checked)
            {
                if (radioButton_x64.Checked)
                {
                    string xmlFilePath = @"C:\Configurazione2024x64.xml";
                    ExtractAndSaveResource("Configurazione2024x64.xml", xmlFilePath);
                    if (radioButton_EN.Checked)
                    {
                        ModifyElementFromXml(xmlFilePath, "it-it", "en-gb");
                    }
                    if (checkBox_visio.Checked)
                    {
                        AddElementToXml2024(xmlFilePath, CreateVisioXml24());
                    }
                    if (checkBox_project.Checked)
                    {
                        AddElementToXml2024(xmlFilePath, CreateProjectXml24());
                    }
                    if (checkBox_word.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Word");
                    }
                    if (checkBox_excel.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Excel");
                    }
                    if (checkBox_powerpoint.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "PowerPoint");
                    }
                    if (checkBox_outlook.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Outlook");
                    }
                    if (checkBox_onenote.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "OneNote");
                    }
                    if (checkBox_onedrive.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "OneDrive");
                    }
                    if (checkBox_publisher.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Publisher");
                    }
                    if (checkBox_access.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Access");
                    }
                    StartInstallation(xmlFilePath);
                }
                else if (radioButton_x32.Checked)
                {
                    string xmlFilePath = @"C:\Configurazione2024x32.xml";
                    ExtractAndSaveResource("Configurazione2024x32.xml", xmlFilePath);
                    if (radioButton_EN.Checked)
                    {
                        ModifyElementFromXml(xmlFilePath, "it-it", "en-gb");
                    }
                    if (checkBox_visio.Checked)
                    {
                        AddElementToXml2024(xmlFilePath, CreateVisioXml24());
                    }
                    if (checkBox_project.Checked)
                    {
                        AddElementToXml2024(xmlFilePath, CreateProjectXml24());
                    }
                    if (checkBox_word.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Word");
                    }
                    if (checkBox_excel.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Excel");
                    }
                    if (checkBox_powerpoint.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "PowerPoint");
                    }
                    if (checkBox_outlook.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Outlook");
                    }
                    if (checkBox_onenote.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "OneNote");
                    }
                    if (checkBox_onedrive.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "OneDrive");
                    }
                    if (checkBox_publisher.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Publisher");
                    }
                    if (checkBox_access.Checked)
                    {
                        RemoveElementFromXml(xmlFilePath, "ExcludeApp", "Access");
                    }
                    StartInstallation(xmlFilePath);
                }
            }
        }

        private void ExtractFolderToTemp()
        {
            try
            {
                string tempFolder = GetTempFolderPath();

                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }
                ExtractAndSaveResource("Configurazione2021x64.xml", Path.Combine(tempFolder, "Configurazione2021x64.xml"));
                ExtractAndSaveResource("Configurazione2021x32.xml", Path.Combine(tempFolder, "Configurazione2021x32.xml"));
                ExtractAndSaveResource("Configurazione365x64.xml", Path.Combine(tempFolder, "Configurazione365x64.xml"));
                ExtractAndSaveResource("Configurazione365x32.xml", Path.Combine(tempFolder, "Configurazione365x32.xml"));
                ExtractAndSaveResource("Configurazione2024x64.xml", Path.Combine(tempFolder, "Configurazione2024x64.xml"));
                ExtractAndSaveResource("Configurazione2024x32.xml", Path.Combine(tempFolder, "Configurazione2024x32.xml"));
                ExtractAndSaveResource("bin.exe", Path.Combine(tempFolder, "bin.exe"));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetTempFolderPath()
        {
            return Path.Combine(Path.GetTempPath(), "OfficePersonalizzato");
        }

        private void ModifyElementFromXml(string xmlFilePath, string oldLang, string newLang)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);

            XmlNodeList languageNodes = doc.GetElementsByTagName("Language");
            foreach (XmlNode node in languageNodes)
            {
                if (node.Attributes["ID"] != null && node.Attributes["ID"].Value == oldLang)
                {
                    node.Attributes["ID"].Value = newLang;
                }
            }

            doc.Save(xmlFilePath);
        }
        private void AddElementToXml(string xmlFilePath, string xmlToAdd)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFilePath);
                XmlNode root = xmlDoc.DocumentElement;
                XmlNode proPlusProductNode = root.SelectSingleNode("//Product[@ID='ProPlus2021Volume']");

                if (proPlusProductNode != null)
                {
                    XmlDocumentFragment fragment = xmlDoc.CreateDocumentFragment();
                    fragment.InnerXml = xmlToAdd;
                    XmlNode parentNode = proPlusProductNode.ParentNode;
                    parentNode.InsertAfter(fragment, proPlusProductNode);
                    proPlusProductNode = fragment.LastChild;
                    xmlDoc.Save(xmlFilePath);
                }
                else
                {
                    MessageBox.Show($"Non è stato trovato alcun nodo Product con ID='ProPlus2021Volume' nel file XML.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddElementToXml365(string xmlFilePath, string xmlToAdd)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFilePath);
                XmlNode root = xmlDoc.DocumentElement;
                XmlNode proPlusProductNode = root.SelectSingleNode("//Product[@ID='O365BusinessRetail']");
                if (proPlusProductNode != null)
                {
                    XmlDocumentFragment fragment = xmlDoc.CreateDocumentFragment();
                    fragment.InnerXml = xmlToAdd;
                    XmlNode parentNode = proPlusProductNode.ParentNode;
                    parentNode.InsertAfter(fragment, proPlusProductNode);
                    proPlusProductNode = fragment.LastChild;
                    xmlDoc.Save(xmlFilePath);
                }
                else
                {
                    MessageBox.Show($"Non è stato trovato alcun nodo Product con ID='O365BusinessRetail' nel file XML.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddElementToXml2024(string xmlFilePath, string xmlToAdd)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFilePath);
                XmlNode root = xmlDoc.DocumentElement;
                XmlNode proPlusProductNode = root.SelectSingleNode("//Product[@ID='ProPlus2024Volume']");

                if (proPlusProductNode != null)
                {
                    XmlDocumentFragment fragment = xmlDoc.CreateDocumentFragment();
                    fragment.InnerXml = xmlToAdd;
                    XmlNode parentNode = proPlusProductNode.ParentNode;
                    parentNode.InsertAfter(fragment, proPlusProductNode);
                    proPlusProductNode = fragment.LastChild;
                    xmlDoc.Save(xmlFilePath);
                }
                else
                {
                    MessageBox.Show($"Non è stato trovato alcun nodo Product con ID='ProPlus2024Volume' nel file XML.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string CreateVisioXml24()
        {
            string visioXml = @"
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
                </Product>
            ";

            return visioXml;
        }

        private string CreateVisioXml()
        {
            string visioXml = @"
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
                </Product>
            ";

            return visioXml;
        }

        private string CreateProjectXml()
        {
            string projectXml = @"
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
                </Product>
            ";

            return projectXml;
        }

        private string CreateVisioXml365()
        {
            string visioXml = @"
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
    </Product>
            ";

            return visioXml;
        }

        private string CreateProjectXml365()
        {
            string projectXml = @"
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
    </Product>
            ";

            return projectXml;
        }

        private string CreateProjectXml24()
        {
            string projectXml = @"
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
    </Product>
            ";

            return projectXml;
        }

        private void ExtractAndSaveResource(string resourceName, string destinationPath)
        {
            try
            {
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                string resourcePath = "WinHubX.Resources.OfficePersonalizzato." + resourceName;
                using (Stream resourceStream = currentAssembly.GetManifestResourceStream(resourcePath))
                {
                    if (resourceStream == null)
                    {
                        MessageBox.Show("Impossibile trovare la risorsa specificata.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    using (FileStream fileStream = new FileStream(destinationPath, FileMode.Create))
                    {
                        resourceStream.CopyTo(fileStream);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoveElementFromXml(string xmlFilePath, string elementName, string attributeValue)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFilePath);

                XmlNodeList elementsToRemove = xmlDoc.SelectNodes($"//{elementName}[@ID='{attributeValue}']");

                foreach (XmlNode node in elementsToRemove)
                {
                    node.ParentNode.RemoveChild(node);
                }

                xmlDoc.Save(xmlFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void StartInstallation(string xmlFilePath)
        {
            try
            {
                progressBar_office.Value = 0;
                string tempPath = Path.Combine(Path.GetTempPath(), "OfficePersonalizzato");
                string binExePath = Path.Combine(tempPath, "bin.exe");

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
                await Task.Delay(3000);

                File.Delete(xmlFilePath);

                progressBar_office.Value = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = "Office";
            form1.PnlFormLoader.Controls.Clear();
            formoffice = new FormOffice(form1)
            {
                Dock = DockStyle.Fill,
                TopLevel = false,
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None
            };
            form1.PnlFormLoader.Controls.Add(formoffice);
            formoffice.Show();
        }

        private void radioButton_office2024_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_publisher.Enabled = false;
        }

        private void radiobutton_office365_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_publisher.Enabled = true;
        }

        private void radiobutton_office2021_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_publisher.Enabled = true;
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
                    MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PersonalizzazioneOffice_Load(object sender, EventArgs e)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                radioButton_x64.Checked = true;
            }
            else
            {
                radioButton_x32.Checked = true;
            }
        }

    }
}
