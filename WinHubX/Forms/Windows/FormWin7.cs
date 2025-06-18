using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using WinHubX.Dialog;

namespace WinHubX
{
    public partial class FormWin7 : Form
    {
        private Form1 form1;
        private FormWin formWin;
        private NotifyIcon notifyIcon;
        private string linkWin7AIO32;
        private string linkWin7AIO64;
        private string linkWin7Lite32;
        private string linkWin7Lite64;

        public FormWin7(FormWin formWin, Form1 form1)
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

                    linkWin7AIO64 = data["FormWin7"]["7Stockx64"]?.ToString();
                    linkWin7AIO32 = data["FormWin7"]["7Stockx32"]?.ToString();
                    linkWin7Lite32 = data["FormWin7"]["7Litex32"]?.ToString();
                    linkWin7Lite64 = data["FormWin7"]["7Litex64"]?.ToString();
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

        private void infoWin7Lite_Click(object sender, EventArgs e)
        {
            infoWin7Lite(sender, e);
        }

        public static void infoWin7Lite(object sender, EventArgs e)
        {
            string description = LanguageManager.GetTranslation("FormWin7", "infoWin7Lite");

            InfoDialog infoWin7Lite = new InfoDialog(description)
            {
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen
            };
            infoWin7Lite.Show();
        }


        private void btnWin7AIO32_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                Clipboard.SetText("fb35104043af3fcb9c87a6e8fd095ca5b2a99fa085fa2bb27eff23a09f2d173a");
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("Global", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("Global", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                OpenLink(linkWin7AIO32);
            }
        }

        private void btnWin7AIO64_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                Clipboard.SetText("b78a9d0156112d93aeee7a4a0feed43bb6230b2dd173ab7d357433d0557a2a6f");
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("Global", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("Global", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                OpenLink(linkWin7AIO64);
            }
        }

        private void btnWin7Lite32_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                Clipboard.SetText("cc5b7b5fc182a2936b1dd8b5c4b07afa3eb180e4971ac768a79d688a7392bab1");
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("Global", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("Global", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                OpenLink(linkWin7Lite32);
            }
        }

        private void btnWin7Lite64_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                Clipboard.SetText("e87960fbf2913959602e510f89d23077da1068900ce4e20de683cb92f48f0185");
                notifyIcon.BalloonTipTitle = LanguageManager.GetTranslation("Global", "sha256title");
                notifyIcon.BalloonTipText = LanguageManager.GetTranslation("Global", "sha256text");
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                OpenLink(linkWin7Lite64);
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
