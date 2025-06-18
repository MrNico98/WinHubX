using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using WinHubX.Dialog;

namespace WinHubX.Forms.Windows
{
    public partial class FormWin8Dot1 : Form
    {
        private Form1 form1;
        private FormWin formWin;
        private NotifyIcon notifyIcon;
        private string linkWin8AIO32;
        private string linkWin8AIO64;
        private string linkWin8Lite32;
        private string linkWin8Lite64;
        public FormWin8Dot1(FormWin formWin, Form1 form1)
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

        private async void LoadJsonLinks()
        {
            string url = "https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync(url);
                    JObject data = JObject.Parse(json);

                    linkWin8AIO64 = data["FormWin8"]["8Stockx64"]?.ToString();
                    linkWin8AIO32 = data["FormWin8"]["8Stockx32"]?.ToString();
                    linkWin8Lite32 = data["FormWin8"]["8Litex32"]?.ToString();
                    linkWin8Lite64 = data["FormWin8"]["8Litex64"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void btnBack_Click(object sender, EventArgs e)
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

        private void btnInfoWin8dot1Lite_Click(object sender, EventArgs e)
        {
            infoWin8dot1Lite(sender, e);
        }

        public static void infoWin8dot1Lite(object sender, EventArgs e)
        {
            string description = LanguageManager.GetTranslation("FormWin8Dot1", "infoWin8dot1Lite");

            InfoDialog infoWin8dot1Lite = new InfoDialog(description)
            {
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen
            };
            infoWin8dot1Lite.Show();
        }


        private void btnWin8dot1AIO32_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                Clipboard.SetText("dfa6d6149fbe3a5247695305a466623a4a8c37cc622006c90cd414c2cfc71513");
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("Global", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("Global", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                OpenLink(linkWin8AIO32);
            }
        }

        private void btnWin8dot1AIO64_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                Clipboard.SetText("6c8739b6f9941e32604c79dea362fe8628ab8dae76f9fed36038b9cfb49abc23");
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("Global", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("Global", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                OpenLink(linkWin8AIO64);
            }
        }

        private void btnWin8dot1Lite32_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                Clipboard.SetText("19565cd2acf239a5abe7a947867b3d6a0e18390519cbefd313e8b2ebb8bf83bf");
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("Global", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("Global", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                OpenLink(linkWin8Lite32);
            }
        }

        private void btnWin8dot1Lite64_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                Clipboard.SetText("b8493b86ebd1a11f952e1545c4bd9393212eade22ad3d0e09dee582d5bd29dea");
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("Global", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("Global", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                OpenLink(linkWin8Lite64);
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

