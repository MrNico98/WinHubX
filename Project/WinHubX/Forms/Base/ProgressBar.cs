using System.Globalization;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Base
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            LanguageManager.LoadTranslations();
            InitializeComponent();
            SetMarquee();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }
        public void SetMarquee()
        {
            progressBar.Style = ProgressBarStyle.Marquee;
            lblStatus.Text = "Operazione in corso...";
        }
        public void SetStatus(string status, int percentComplete)
        {
            lblStatus.Text = status;
            if (progressBar.Style == ProgressBarStyle.Blocks)
            {
                progressBar.Value = percentComplete;
            }
        }
        public void CompleteOperation()
        {
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Value = 100;
            lblStatus.Text = "Operazione completata";
        }
    }
}

