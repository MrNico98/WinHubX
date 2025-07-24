using System.Diagnostics;
using System.Globalization;

namespace WinHubX
{
    public partial class FormHome : Form
    {
        public FormHome()
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            this.ActiveControl = label1;
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            versioneapp.Text = "v." + AppConfig.CurrentVersion;
        }

        private void tgWinHubX_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "https://telegram.me/WinHubXbot",
                    UseShellExecute = true
                };
                _ = Process.Start(psi);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnKofi_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "https://ko-fi.com/winhubx",
                    UseShellExecute = true
                };
                _ = Process.Start(psi);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
