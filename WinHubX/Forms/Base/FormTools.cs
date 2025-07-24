using System.Globalization;
using WinHubX.Dialog.Tools;
using WinHubX.Forms;

namespace WinHubX
{
    public partial class FormTools : Form
    {
        Form1 form1;
        public FormTools(Form1 form1)
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            this.form1 = form1;
            SetTooltips();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }
        private void SetTooltips()
        {
            toolTip1.SetToolTip(btnKasperky, GetTooltipText("kasperky"));
            toolTip1.SetToolTip(btnDaRT, GetTooltipText("dart"));
            toolTip1.SetToolTip(btnMPM, GetTooltipText("mpm"));
            toolTip1.SetToolTip(btnWimTK, GetTooltipText("wimtk"));
            toolTip1.SetToolTip(btnWinHubXLiteOS, GetTooltipText("winhubx"));
            toolTip1.SetToolTip(btnRSTDriver, GetTooltipText("rst"));
        }

        private string GetTooltipText(string key)
        {
            return LanguageManager.GetTranslation("FormTools", $"tooltip_{key}");
        }
        private void btnKasperky_Click(object sender, EventArgs e)
        {
            DialogKasperskyLive dialogWinC = new DialogKasperskyLive()
            {
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen
            };
            _ = dialogWinC.ShowDialog();
        }

        private void btnWimTK_Click(object sender, EventArgs e)
        {
            DialogWIMToolKit dialogWIMTK = new DialogWIMToolKit()
            {
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen
            };
            _ = dialogWIMTK.ShowDialog();
        }

        private void btnDaRT_Click(object sender, EventArgs e)
        {
            DialogDaRT dialogDaRT = new DialogDaRT()
            {
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen
            };
            _ = dialogDaRT.ShowDialog();
        }

        private void btnWinHubXLiteOS_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = LanguageManager.GetTranslation("FormTools", "paneltitle");
            form1.PnlFormLoader.Controls.Clear();
            FormWinHubXLiteOS formwinhubxliteos = new FormWinHubXLiteOS(form1, this) { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            formwinhubxliteos.FormBorderStyle = FormBorderStyle.None;
            form1.PnlFormLoader.Controls.Add(formwinhubxliteos);
            formwinhubxliteos.Show();
        }

        private void btnMPM_Click(object sender, EventArgs e)
        {
            DialogMsPCManager dialogMsPCManager = new DialogMsPCManager()
            {
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen
            };
            _ = dialogMsPCManager.ShowDialog();
        }

        private void btnRSTDriver_Click(object sender, EventArgs e)
        {
            DialogRSTDriver dialogRSTDriver = new DialogRSTDriver()
            {
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen
            };
            _ = dialogRSTDriver.ShowDialog();
        }
    }
}
