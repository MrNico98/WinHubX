using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using WinHubX.Dialog;

namespace WinHubX.Forms.Windows
{
    public partial class FormWin10 : Form
    {
        private Form1 form1;
        private FormWin formWin;
        private NotifyIcon notifyIcon;
        private string linkWin10Arm64;
        private string linkWin10AIO32;
        private string linkWin10AIO64;
        private string linkWin10Lite32;
        private string linkWin10Lite64;

        public FormWin10(FormWin formWin, Form1 form1)
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            this.form1 = form1;
            this.formWin = formWin;
            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = false
            };
            LoadJsonLinks();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = "Windows";
            form1.PnlFormLoader.Controls.Clear();
            formWin = new FormWin(form1)
            {
                Dock = DockStyle.Fill,
                TopLevel = false,
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None
            };
            form1.PnlFormLoader.Controls.Add(formWin);
            formWin.Show();
        }
        private void btnInfoWin10Lite_Click(object sender, EventArgs e)
        {
            infoWin10Lite(sender, e);
        }

        public static void infoWin10Lite(object sender, EventArgs e)
        {
            string description = LanguageManager.GetTranslation("FormWin1011", "infoWin1011Lite");

            InfoDialog infoWin10Lite = new InfoDialog(description)
            {
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen
            };
            infoWin10Lite.Show();
        }

        private async void LoadJsonLinks()
        {
            string url = "https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync(url);
                    JObject data = JObject.Parse(json);

                    linkWin10Arm64 = data["FormWin10"]["10Arm64"]?.ToString();
                    linkWin10AIO32 = data["FormWin10"]["10Stockx32"]?.ToString();
                    linkWin10AIO64 = data["FormWin10"]["10Stockx64"]?.ToString();
                    linkWin10Lite32 = data["FormWin10"]["10Litex32"]?.ToString();
                    linkWin10Lite64 = data["FormWin10"]["10Litex64"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWin10ARM64_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Clipboard.SetText("7f9c63e48578451cbc92c009f9819816a28f5605ba9b1578e9f91a49834d10ac");
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("Global", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("Global", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                OpenLink(linkWin10Arm64);
            }
        }

        private void btnWin10AIO32_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Clipboard.SetText("fde189da1265dc3eb1d3e26b560876a37c44a447afdd493ed73d53d33d766cf0");
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("Global", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("Global", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                OpenLink(linkWin10AIO32);
            }
        }

        private void btnWin10AIO64_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                Clipboard.SetText("2220ecf55ff6ee8b7c90d78ea536c7b7e4943f08593a6e1e68fc41b9b02e6f9f");
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("Global", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("Global", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                OpenLink(linkWin10AIO64);
            }
        }

        private void btnWin10Lite32_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                Clipboard.SetText("60d47df775b0f4445f69e2313844fa2516ae7efb007fd2db3bf781b93f2fac82");
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("Global", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("Global", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                MessageBox.Show(
    LanguageManager.GetTranslation("FormWin1011", "rufusWarning"),
    "Attenzione",
    MessageBoxButtons.OK,
    MessageBoxIcon.Information
);
                OpenLink(linkWin10Lite32);
            }
        }

        private void btnWin10Lite64_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                Clipboard.SetText("9f9332232520fab06c13d84a2a7ea7da02dfd31dfd9559caabdf5c19d9d3f78c");
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("Global", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("Global", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                MessageBox.Show(
    LanguageManager.GetTranslation("FormWin1011", "rufusWarning"),
    "Attenzione",
    MessageBoxButtons.OK,
    MessageBoxIcon.Information
);
                OpenLink(linkWin10Lite64);
            }
        }

        private void OpenLink(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {

            }
        }
    }
}
