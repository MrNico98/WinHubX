using Microsoft.Win32;
using System.Globalization;

namespace WinHubX.Forms.Settaggi
{
    public partial class FormExplorer : Form
    {
        public FormExplorer()
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            LoadExplorerSettings();
            this.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }

        private void LoadExplorerSettings()
        {
            try
            {
                using (RegistryKey explorerKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced"))
                {
                    if (explorerKey != null)
                    {
                        chkHideFileExtensions.Checked = GetRegistryValue(explorerKey, "HideFileExt", 0) == 1;
                        chkShowHiddenFiles.Checked = GetRegistryValue(explorerKey, "Hidden", 0) == 1;
                        chkShowSuperHidden.Checked = GetRegistryValue(explorerKey, "ShowSuperHidden", 0) == 1;
                        chkShowStatusBar.Checked = GetRegistryValue(explorerKey, "ShowStatusBar", 1) == 1;
                        chkShowPreviewPane.Checked = GetRegistryValue(explorerKey, "ShowPreviewHandlers", 1) == 1;
                        chkShowDetailsPane.Checked = GetRegistryValue(explorerKey, "ShowInfoTip", 1) == 1;
                        chkShowFullPath.Checked = GetRegistryValue(explorerKey, "FullPath", 0) == 1;
                        chkShowEncryptedCompressed.Checked = GetRegistryValue(explorerKey, "ShowEncryptCompressed", 1) == 1;
                        chkStartWithThisPC.Checked = GetRegistryValue(explorerKey, "LaunchTo", 1) == 1;
                        chkShowRibbon.Checked = GetRegistryValue(explorerKey, "RibbonExpanded", 1) == 1;
                    }
                }

                lblStatus.Text = LanguageManager.GetTranslation("FormExplorer", "settings_loaded");
            }
            catch (Exception ex)
            {
                lblStatus.Text = LanguageManager.GetTranslation("FormExplorer", "apply_error") + ": " + ex.Message;
            }
        }

        private int GetRegistryValue(RegistryKey key, string valueName, int defaultValue)
        {
            object value = key.GetValue(valueName, defaultValue);
            return value != null ? Convert.ToInt32(value) : defaultValue;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            try
            {
                using (RegistryKey explorerKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true))
                {
                    if (explorerKey != null)
                    {
                        explorerKey.SetValue("HideFileExt", chkHideFileExtensions.Checked ? 1 : 0, RegistryValueKind.DWord);
                        explorerKey.SetValue("Hidden", chkShowHiddenFiles.Checked ? 1 : 0, RegistryValueKind.DWord);
                        explorerKey.SetValue("ShowSuperHidden", chkShowSuperHidden.Checked ? 1 : 0, RegistryValueKind.DWord);
                        explorerKey.SetValue("ShowStatusBar", chkShowStatusBar.Checked ? 1 : 0, RegistryValueKind.DWord);
                        explorerKey.SetValue("ShowPreviewHandlers", chkShowPreviewPane.Checked ? 1 : 0, RegistryValueKind.DWord);
                        explorerKey.SetValue("ShowInfoTip", chkShowDetailsPane.Checked ? 1 : 0, RegistryValueKind.DWord);
                        explorerKey.SetValue("FullPath", chkShowFullPath.Checked ? 1 : 0, RegistryValueKind.DWord);
                        explorerKey.SetValue("ShowEncryptCompressed", chkShowEncryptedCompressed.Checked ? 1 : 0, RegistryValueKind.DWord);
                        explorerKey.SetValue("LaunchTo", chkStartWithThisPC.Checked ? 1 : 0, RegistryValueKind.DWord);
                        explorerKey.SetValue("RibbonExpanded", chkShowRibbon.Checked ? 1 : 0, RegistryValueKind.DWord);
                    }
                }

                lblStatus.Text = LanguageManager.GetTranslation("FormExplorer", "settings_applied");
            }
            catch (Exception ex)
            {
                lblStatus.Text = LanguageManager.GetTranslation("FormExplorer", "apply_error") + ": " + ex.Message;
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                LanguageManager.GetTranslation("FormExplorer", "confirm_reset"),
                LanguageManager.GetTranslation("FormExplorer", "confirm_title"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    chkHideFileExtensions.Checked = false;
                    chkShowHiddenFiles.Checked = false;
                    chkShowSuperHidden.Checked = false;
                    chkShowStatusBar.Checked = true;
                    chkShowPreviewPane.Checked = true;
                    chkShowDetailsPane.Checked = true;
                    chkShowFullPath.Checked = false;
                    chkShowEncryptedCompressed.Checked = true;
                    chkStartWithThisPC.Checked = true;
                    chkShowRibbon.Checked = true;

                    lblStatus.Text = LanguageManager.GetTranslation("FormExplorer", "settings_reset");
                }
                catch (Exception ex)
                {
                    lblStatus.Text = LanguageManager.GetTranslation("FormExplorer", "reset_error") + ": " + ex.Message;
                }
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}