using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using WinHubX.Forms.Base;
using WinHubX.Forms.DriverRST;
using WinHubX.Forms.ImpostazioniApp;
using WinHubX.Impostazioni;

namespace WinHubX
{
    public partial class FormWin : Form
    {
        public FormWin(Form1 form1)
        {
            LanguageManager.LoadLanguageFromSettings();
            InitializeComponent();
            btnAttivaWindowsPrinci.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Attiva Windows",
                "en" => "  Activate Windows",
                _ => btnAttivaWindowsPrinci.Content
            };
            btnCambiaEdizionePrinci.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Cambia edizione",
                "en" => "  Change edition",
                _ => btnCambiaEdizionePrinci.Content
            };
            btnCreaIsoPrinci.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Crea ISO",
                "en" => "  Create ISO",
                _ => btnCreaIsoPrinci.Content
            };
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }

        private void btnAttivaWin_Click(object? sender, EventArgs e)
        {
            try
            {
                string windowsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                var startInfo = new ProcessStartInfo
                {
                    FileName = Path.Join(windowsDirectory, "explorer.exe"),
                    WorkingDirectory = windowsDirectory,
                    UseShellExecute = false
                };
                startInfo.ArgumentList.Add("ms-settings:activation");
                _ = Process.Start(startInfo)
                    ?? throw new InvalidOperationException("Impossibile avviare Esplora file.");
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception
                or InvalidOperationException
                or System.Security.SecurityException
                or ArgumentException)
            {
                _ = MessageBox.Show($"Impossibile aprire le impostazioni di attivazione: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCambioEdizione_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    "WinHubX aprirà il sito ufficiale massgrave.dev nel browser predefinito. L'app non scaricherà né eseguirà script di attivazione. Continuare?",
                    "Apri Massgrave", MessageBoxButtons.YesNo, MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            try
            {
                _ = Process.Start(new ProcessStartInfo("https://massgrave.dev/") { UseShellExecute = true })
                    ?? throw new InvalidOperationException("Il browser predefinito non è stato avviato.");
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception
                or InvalidOperationException
                or System.Security.SecurityException
                or ArgumentException)
            {
                _ = MessageBox.Show($"Impossibile aprire massgrave.dev: {ex.Message}",
                    "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreaIso_Click(object sender, EventArgs e)
        {
            panel50.Controls.Clear();
            Form1? mainForm = Application.OpenForms["Form1"] as Form1;
            if (mainForm == null) return;
            mainForm.pictureBox3.Visible = true;
            mainForm.pictureBox3.Click += (s, ev) =>
            {
                mainForm.pictureBox3.Visible = false;
                mainForm.LoadForm(new FormWin(mainForm), mainForm.btnWin, LanguageManager.GetTranslation("FormMain", "Windows"));
            };
            mainForm.pictureBoxlblalto.Image = btnCreaIsoPrinci.Image;
            mainForm.lblPanelTitle.Text = LanguageManager.GetTranslation("FormWin", "CreaISO");
            FormCreaISO formCreaIso = new FormCreaISO(mainForm);
            formCreaIso.TopLevel = false;
            formCreaIso.FormBorderStyle = FormBorderStyle.None;
            formCreaIso.Dock = DockStyle.Fill;
            panel50.Controls.Add(formCreaIso);
            formCreaIso.Show();
        }

        private void btnDownloadIsoPrinci_Click(object sender, EventArgs e)
        {
            string url = "https://mrnico98.github.io/ISODownloader/";

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception
                or InvalidOperationException
                or System.Security.SecurityException
                or ArgumentException)
            {
                MessageBox.Show($"Impossibile aprire il browser: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDriverRSTPrinci_Click(object sender, EventArgs e)
        {
            FormDriverRST formDriverRST = new FormDriverRST();
            formDriverRST.Show();
        }
    }
}
