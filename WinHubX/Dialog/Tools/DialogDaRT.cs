using System.Diagnostics;

namespace WinHubX.Dialog.Tools
{
    public partial class DialogDaRT : Form
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

        public DialogDaRT()
        {
            InitializeComponent();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            LanguageManager.LoadTranslations();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "https://devuploads.com/5dh0kron451m",
                    UseShellExecute = true
                };
                _ = Process.Start(psi);
                this.Close();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"URL Error: {ex.Message}");
            }
        }
    }
}
