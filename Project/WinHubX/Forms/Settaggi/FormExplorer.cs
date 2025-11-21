using Microsoft.Win32;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Settaggi
{
    public partial class FormExplorer : Form
    {
        public FormExplorer()
        {
            InitializeComponent();
            LoadExplorerSettings();
            Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            btnApplyVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Avvia",
                "en" => "  Start",
                _ => btnApplyVerdi.Content
            };
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
            }
            catch (Exception)
            {
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
            }
            catch (Exception)
            {
            }
        }
    }
}