using Microsoft.Win32;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Windows.Forms;
using WinHubX.Forms.Base;
using WinHubX.Forms.CreaISO;
using WinHubX.Forms.ImpostazioniApp;
using WinHubX.Impostazioni;
using static System.Net.WebRequestMethods;

namespace WinHubX
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        private NotifyIcon notifyIcon;
        private ContextMenuStrip trayIconContextMenu;
        private static readonly HttpClient client = new HttpClient();
        private readonly List<Button> bottoni = new();

        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;
        private const int WM_NCHITTEST = 0x84;
        private Form previousForm = null;

        protected override void WndProc(ref Message m)
        {
            const int gripSize = 10;
            if (m.Msg == WM_NCHITTEST)
            {
                Point pos = PointToClient(new Point(m.LParam.ToInt32()));

                if (pos.X <= gripSize)
                {
                    if (pos.Y <= gripSize)
                        m.Result = HTTOPLEFT;
                    else if (pos.Y >= ClientSize.Height - gripSize)
                        m.Result = HTBOTTOMLEFT;
                    else
                        m.Result = HTLEFT;
                }
                else if (pos.X >= ClientSize.Width - gripSize)
                {
                    if (pos.Y <= gripSize)
                        m.Result = HTTOPRIGHT;
                    else if (pos.Y >= ClientSize.Height - gripSize)
                        m.Result = HTBOTTOMRIGHT;
                    else
                        m.Result = HTRIGHT;
                }
                else if (pos.Y <= gripSize)
                {
                    m.Result = HTTOP;
                }
                else if (pos.Y >= ClientSize.Height - gripSize)
                {
                    m.Result = HTBOTTOM;
                }
                else
                {
                    base.WndProc(ref m);
                }
                return;
            }

            base.WndProc(ref m);
        }

        public Form1()
        {
            InitializeComponent();
            pictureBox3.Visible = false;
            FormBorderStyle = FormBorderStyle.None;
            Padding = new Padding(2);
            bottoni.AddRange(new[] { btnHome, btnWin, btnOffice, btnSettaggi, btnDebloat, btnmonitoraggio });
            LoadForm(new FormHome(), btnHome, "Home");
            ActiveControl = pictureBoxLogoForm1;
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);

            if (ThemeManager.IsDarkTheme)
            {
                picCloseApp.Image = Properties.Resources.pngChiudiForm1;
                picEspandiApp.Image = Properties.Resources.pngEspandiForm1;
                picMinimizzaApp.Image = Properties.Resources.pngMinimizzaForm1;
                pictureBox3.Image = Properties.Resources.pngFrecciaHome;
                picImpostazioniApp.Image = Properties.Resources.pngImpostazioniForm1;
            }
            else
            {
                picCloseApp.Image = Properties.Resources.pngChiudiBlackForm1;
                picEspandiApp.Image = Properties.Resources.pngEspandiBlackForm1;
                picMinimizzaApp.Image = Properties.Resources.pngMinimizzaBlackForm1;
                pictureBox3.Image = Properties.Resources.pngFrecciaIndietroBlackForm1;
                picImpostazioniApp.Image = Properties.Resources.pngImpostazioniBlackForm1;
            }
            Shown += (s, e) =>
            {
                Show();
                BringToFront();
                Activate();
            };
            ApplicaTraduzioniUI();
        }

        private void ApplicaTraduzioniUI()
        {
            Text = LanguageManager.GetTranslation("FormMain", "TitoloApp");

            btnHome.Text = LanguageManager.GetTranslation("FormMain", "Home");
            btnWin.Text = LanguageManager.GetTranslation("FormMain", "Windows");
            btnOffice.Text = LanguageManager.GetTranslation("FormMain", "Office");
            btnSettaggi.Text = LanguageManager.GetTranslation("FormMain", "Tweaks");
            btnDebloat.Text = LanguageManager.GetTranslation("FormMain", "Debloat");
            btnmonitoraggio.Text = LanguageManager.GetTranslation("FormMain", "Monitoraggio");
        }

        private void EnableDragging(Control control)
        {
            if ((control is PictureBox && control.Name != "pictureBoxLogoForm1") ||
                control is Button ||
                control is TextBox ||
                control is CheckBox ||
                control is ComboBox ||
                control is ListBox ||
                control is CuoreUI.Controls.cuiSwitch ||
                control is CuoreUI.Controls.cuiFileDropper ||
                control is CuoreUI.Controls.cuiPictureBox ||
                control is RadioButton)
                return;
            control.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    _ = ReleaseCapture();
                    _ = SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };
            foreach (Control child in control.Controls.Cast<Control>().ToList())
            {
                EnableDragging(child);
            }
        }

        private void swap_pnlNav(Button activeButton, bool darkTheme)
        {
            Color baseColor = darkTheme ? Color.FromArgb(64, 60, 59) : Color.FromArgb(245, 245, 245);
            Color activeColor = darkTheme ? Color.FromArgb(80, 80, 80) : Color.FromArgb(220, 220, 220);

            foreach (var button in bottoni)
            {
                button.BackColor = baseColor;
                button.ForeColor = darkTheme ? Color.White : Color.Black;
            }
            pnlNav.SetBounds(activeButton.Left, activeButton.Top, pnlNav.Width, activeButton.Height);
        }

        public void LoadForm(Form form, Button button, string title, bool showBackArrow = false)
        {
            var config = ThemeConfig.Load();
            bool dark = config.DarkTheme;
            swap_pnlNav(button, dark);
            lblPanelTitle.Text = title;
            pictureBox3.Visible = showBackArrow;
            pictureBoxlblalto.Image = button?.Image;
            if (showBackArrow && PnlFormLoader.Controls.Count > 0)
            {
                previousForm = PnlFormLoader.Controls[0] as Form;
            }
            else
            {
                previousForm = null;
            }
            LoadFormIntoPanel(form);
            EnableDragging(this);
            ThemeManager.ApplyThemeToControl(form, dark);
        }


        private void LoadFormIntoPanel(Form form)
        {
            PnlFormLoader.Controls.Clear();

            form.Dock = DockStyle.Fill;
            form.TopLevel = false;
            form.TopMost = true;
            form.FormBorderStyle = FormBorderStyle.None;

            PnlFormLoader.Controls.Add(form);
            form.Show();
        }


        private void btnHome_Click(object sender, EventArgs e) =>
            LoadForm(new FormHome(), btnHome, LanguageManager.GetTranslation("FormMain", "Home"));

        private void btnWin_Click(object sender, EventArgs e) =>
            LoadForm(new FormWin(this), btnWin, LanguageManager.GetTranslation("FormMain", "Windows"));

        private void btnOffice_Click(object sender, EventArgs e) =>
            LoadForm(new FormOffice(this), btnOffice, LanguageManager.GetTranslation("FormMain", "Office"));

        private void btnDebloat_Click(object sender, EventArgs e) =>
            LoadForm(new FormDebloat(this), btnDebloat, LanguageManager.GetTranslation("FormMain", "Debloat"));

        private void btnmonitoraggio_Click(object sender, EventArgs e) =>
            LoadForm(new FormMonitoraggio(this), btnmonitoraggio, LanguageManager.GetTranslation("FormMain", "Monitoraggio"));

        private void btnSettaggi_Click(object sender, EventArgs e)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\WinHubX"))
            {
                if (key != null)
                {
                    object value = key.GetValue("SettaggiRiavviati");
                    if (value != null && value.ToString() == "1")
                    {
                        puntodiripristino();
                        return;
                    }
                }
            }
            string titolo = LanguageManager.GetTranslation("Form1", "reboot_title");
            string messaggio = LanguageManager.GetTranslation("Form1", "reboot_message");

            var result = MessageBox.Show(
                messaggio,
                titolo,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                _ = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c \"sc config UCPD start= disabled && schtasks /change /Enable /TN \"\\Microsoft\\Windows\\AppxDeploymentClient\\UCPD velocity\" && shutdown /r /t 0\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                });

                using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey("Software\\WinHubX"))
                {
                    regKey.SetValue("SettaggiRiavviati", 1);
                }
                Application.Exit();
            }
            else
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\WinHubX"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("SettaggiRiavviati");
                        if (value != null && value.ToString() == "0")
                        {
                            _ = MessageBox.Show("I need registry access to access this menu");
                        }
                    }
                }
            }
        }

        private void puntodiripristino()
        {
            string titolo = LanguageManager.GetTranslation("FormMain", "restorepoint_title");
            string messaggio = LanguageManager.GetTranslation("FormMain", "restorepoint_message");

            var result = MessageBox.Show(
                messaggio,
                titolo,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    string script = @"
try {
    Enable-ComputerRestore -Drive ""$env:SystemDrive""
} catch {
    Write-Host ""Errore nell'abilitazione del Ripristino configurazione di sistema: $_""
}

$exists = Get-ItemProperty -path ""HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore"" -Name ""SystemRestorePointCreationFrequency"" -ErrorAction SilentlyContinue
if($null -eq $exists) {
    Set-ItemProperty -Path ""HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore"" -Name ""SystemRestorePointCreationFrequency"" -Value 0 -Type DWord -Force
}

try {
    Import-Module Microsoft.PowerShell.Management -ErrorAction Stop
} catch {
    return
}

try {
    $existingRestorePoints = Get-ComputerRestorePoint | Where-Object { $_.CreationTime.Date -eq (Get-Date).Date }
} catch {
    return
}
if ($existingRestorePoints.Count -eq 0) {
    Checkpoint-Computer -Description ""Punto di ripristino creato da WinHubX"" -RestorePointType MODIFY_SETTINGS
}
";

                    string tempScriptPath = Path.Combine(Path.GetTempPath(), "CreateRestorePoint.ps1");
                    System.IO.File.WriteAllText(tempScriptPath, script);

                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-ExecutionPolicy Bypass -NoProfile -File \"{tempScriptPath}\"",
                        UseShellExecute = false,
                        Verb = "runas"
                    };

                    Process.Start(psi)?.WaitForExit();
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string regBackupPath = Path.Combine(desktopPath, $"BackupRegistroWinHubX_{DateTime.Now:yyyyMMdd_HHmmss}.reg");

                    ProcessStartInfo regExport = new ProcessStartInfo()
                    {
                        FileName = "reg.exe",
                        Arguments = $"export HKLM \"{regBackupPath}\" /y",
                        UseShellExecute = true,
                        Verb = "runas"
                    };

                    Process.Start(regExport)?.WaitForExit();
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            LoadForm(new FormSettaggi(this), btnSettaggi, LanguageManager.GetTranslation("FormMain", "Tweaks"));
        }


        private async void btnClose_Click(object sender, EventArgs e)
        {
            if (WinHubX.Impostazioni.DownloadManager.IsDownloading)
            {
                var result = MessageBox.Show(
                    "Un download è attualmente in corso.\nChiudendo l'app il download verrà ANNULLATO.\n\nVuoi davvero uscire?",
                    "Download in corso",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.No)
                    return;
                WinHubX.Impostazioni.DownloadManager.ForceStopDownload();
            }

            var formMonitoraggio = Application.OpenForms
    .OfType<FormMonitoraggio>()
    .FirstOrDefault();

            if (formMonitoraggio != null)
            {
                try
                {
                    formMonitoraggio.CleanupResources();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Errore nella pulizia di FormMonitoraggio: {ex.Message}");
                }
            }


            if (AppState.IsoMontata && !string.IsNullOrWhiteSpace(AppState.IsoPath))
            {
                try
                {
                    await RunPowerShellAsync($"Dismount-DiskImage -ImagePath '{AppState.IsoPath}'");
                    AppState.IsoMontata = false;
                    AppState.IsoDriveLetter = null;
                    AppState.IsoPath = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Errore nello smontare ISO: {ex.Message}");
                }
            }
            foreach (Form openForm in Application.OpenForms.Cast<Form>().ToList())
            {
                openForm.Close();
            }
            Application.Exit();
        }
        private static async Task<int> RunPowerShellAsync(string command)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"{command}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            process.Start();
            await process.WaitForExitAsync();
            return process.ExitCode;
        }
        private void btnMnmz_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            EnableDragging(tableLayoutPanel1);
            EnableDragging(panel1);
            EnableDragging(panel2);
            EnableDragging(panel3);
            EnableDragging(PnlFormLoader);
            AppConfig.LoadSettings();

            if (AppConfig.CheckUpdatesOnStartup)
            {
                var impostazioni = new FormImpostazioniApp();
                bool updateAvailable = await impostazioni.VerificaAggiornamentiAutomaticiAsync();
                if (impostazioni.UpdateDetectedAtStartup)
                {
                    MostraNotificaAggiornamento();
                }
            }
        }


        private bool isFullScreen = false;
        private FormWindowState previousWindowState;
        private FormBorderStyle previousBorderStyle;
        private Rectangle previousBounds;
        private void btnFullScreen_Click(object sender, EventArgs e)
        {
            if (!isFullScreen)
            {
                previousWindowState = WindowState;
                previousBorderStyle = FormBorderStyle;
                previousBounds = Bounds;

                cuiFormRounder1.TargetForm = null;
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Normal;
                Bounds = Screen.FromControl(this).WorkingArea;

                isFullScreen = true;
            }
            else
            {
                FormBorderStyle = previousBorderStyle;
                WindowState = previousWindowState;
                Bounds = previousBounds;

                cuiFormRounder1.TargetForm = this;

                isFullScreen = false;
            }
        }


        public void pictureBox3_Click(object sender, EventArgs e)
        {
            NavigateBack();
        }

        public void NavigateBack()
        {
            if (previousForm != null)
            {
                LoadFormIntoPanel(previousForm);
                lblPanelTitle.Text = previousForm.Text;
                pictureBox3.Visible = false;
                previousForm = null;
            }
        }

        private void cuiPictureBox1_Click(object sender, EventArgs e)
        {
            FormImpostazioniApp formImpostazioniApp = new FormImpostazioniApp();
            formImpostazioniApp.Show();
        }


        private void MostraNotificaAggiornamento()
        {
            MessageBox.Show(
                LanguageManager.GetTranslation("Form1", "aggiornamento_disponibile_msg"),
                LanguageManager.GetTranslation("Form1", "aggiornamento_disponibile_title"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}
