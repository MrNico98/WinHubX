using System.Globalization;

namespace WinHubX.Forms.Base
{
    public partial class FormOperazioni : Form
    {
        public FormOperazioni()
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }

        private string lastLogType = "";

        public void SetStatus(string message, int? progress = null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetStatus(message, progress)));
                return;
            }

            lblStatus.Text = message;

            if (progress.HasValue)
                progressBar.Value = Math.Min(progress.Value, 100);
            if (message.StartsWith("Download"))
            {
                if (lastLogType != "Downloading")
                {
                    evLog.AppendText($"{DateTime.Now:HH:mm:ss} - Download...\n");
                    evLog.ScrollToCaret();
                    lastLogType = "Downloading";
                }
            }
            else
            {
                evLog.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\n");
                evLog.ScrollToCaret();
                lastLogType = message;
            }
        }


        public void CompleteOperation()
        {
            SetStatus("Operazione completata.", 100);
        }
    }
}

