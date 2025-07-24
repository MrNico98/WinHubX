using System.Diagnostics;

namespace WinHubX.Dialog.Tools
{
    public partial class DialogKasperskyLive : Form
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int borderWidth = 2;

            Color borderColor = Color.Coral;

            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                Rectangle borderRectangle = new Rectangle(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);

                e.Graphics.DrawRectangle(pen, borderRectangle);
            }
        }

        private NotifyIcon notifyIcon;
        public DialogKasperskyLive()
        {
            InitializeComponent();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            LanguageManager.LoadTranslations();
            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = false
            };
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            notifyIcon.BalloonTipTitle = "Download di KasperskyLive!";
            notifyIcon.BalloonTipText = "Verrà aperta la pagina di download, attendi.";
            notifyIcon.ShowBalloonTip(1000);

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "https://devuploads.com/miujf39ew58r",
                    UseShellExecute = true
                };
                _ = Process.Start(psi);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"URL Error: {ex.Message}");
            }
        }
    }
}
