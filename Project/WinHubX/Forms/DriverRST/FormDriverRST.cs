using System.Diagnostics;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.DriverRST
{
    public partial class FormDriverRST : Form
    {
        private const string IntelRstDownloadPage =
            "https://www.intel.com/content/www/us/en/support/articles/000091087/technologies/intel-rapid-storage-technology-intel-rst.html";

        public FormDriverRST()
        {
            InitializeComponent();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }

        private void btnDriverRSTPrinci_Click(object? sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = IntelRstDownloadPage,
                    UseShellExecute = true
                });
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception
                or InvalidOperationException
                or System.Security.SecurityException)
            {
                MessageBox.Show($"Impossibile aprire la pagina ufficiale Intel: {ex.Message}", "Download driver RST",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
