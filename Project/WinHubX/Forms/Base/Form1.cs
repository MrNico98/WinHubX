using Microsoft.Win32;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Security.Policy;
using System.Windows.Forms;
using WinHubX.Forms.Base;
using WinHubX.Forms.CreaISO;
using WinHubX.Forms.ImpostazioniApp;
using WinHubX.Impostazioni;

namespace WinHubX
{
    public partial class Form1 : Form
    {
        private readonly List<Button> bottoni = new();
        private bool _isDraggingWindow;
        private Point _dragCursorOrigin;
        private Point _dragWindowOrigin;

        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;
        private const int WM_NCHITTEST = 0x84;
        private Form? previousForm;
        private Form? activeForm;
        private Button? updateAvailableButton;

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
                control is HartUI.Controls.cuiSwitch ||
                control is HartUI.Controls.cuiFileDropper ||
                control is HartUI.Controls.cuiPictureBox ||
                control is RadioButton)
                return;
            control.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    _isDraggingWindow = true;
                    _dragCursorOrigin = Cursor.Position;
                    _dragWindowOrigin = Location;
                }
            };
            control.MouseMove += (s, e) =>
            {
                if (_isDraggingWindow && e.Button.HasFlag(MouseButtons.Left))
                {
                    Point cursorPosition = Cursor.Position;
                    Location = new Point(
                        _dragWindowOrigin.X + cursorPosition.X - _dragCursorOrigin.X,
                        _dragWindowOrigin.Y + cursorPosition.Y - _dragCursorOrigin.Y);
                }
            };
            control.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    _isDraggingWindow = false;
            };
            foreach (Control child in control.Controls.Cast<Control>().ToList())
            {
                EnableDragging(child);
            }
        }

        private void swap_pnlNav(Button activeButton, bool darkTheme)
        {
            Color baseColor = darkTheme ? Color.FromArgb(64, 60, 59) : Color.FromArgb(245, 245, 245);

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
            previousForm = showBackArrow && PnlFormLoader.Controls.Count > 0
                ? PnlFormLoader.Controls[0] as Form
                : null;
            LoadFormIntoPanel(form, showBackArrow ? previousForm : null);
            EnableDragging(this);
            ThemeManager.ApplyThemeToControl(form, dark);
        }


        private void LoadFormIntoPanel(Form form, Form? preservedForm = null)
        {
            if (activeForm is not null && activeForm != preservedForm && !activeForm.IsDisposed)
            {
                PnlFormLoader.Controls.Remove(activeForm);
                activeForm.Close();
                activeForm.Dispose();
            }

            PnlFormLoader.Controls.Clear();

            form.Dock = DockStyle.Fill;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;

            PnlFormLoader.Controls.Add(form);
            activeForm = form;
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

        private void btnSettaggi_Click(object sender, EventArgs e) =>
            LoadForm(new FormSettaggi(this), btnSettaggi, LanguageManager.GetTranslation("FormMain", "Tweaks"));

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
$ErrorActionPreference = 'Stop'

try {
    Enable-ComputerRestore -Drive ""$env:SystemDrive""
} catch {
    Write-Error ""Errore nell'abilitazione del Ripristino configurazione di sistema: $_""
    exit 1
}

$exists = Get-ItemProperty -path ""HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore"" -Name ""SystemRestorePointCreationFrequency"" -ErrorAction SilentlyContinue
if($null -eq $exists) {
    Set-ItemProperty -Path ""HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore"" -Name ""SystemRestorePointCreationFrequency"" -Value 0 -Type DWord -Force
}

try {
    Import-Module Microsoft.PowerShell.Management -ErrorAction Stop
} catch {
    Write-Error ""Errore nel caricamento del modulo di ripristino: $_""
    exit 1
}

try {
    $existingRestorePoints = Get-ComputerRestorePoint | Where-Object { $_.CreationTime.Date -eq (Get-Date).Date }
} catch {
    Write-Error ""Errore nella verifica dei punti di ripristino: $_""
    exit 1
}
if ($existingRestorePoints.Count -eq 0) {
    Checkpoint-Computer -Description ""Punto di ripristino creato da WinHubX"" -RestorePointType MODIFY_SETTINGS
}
";

                    string tempScriptPath = Path.Join(Path.GetTempPath(), $"WinHubX-RestorePoint-{Guid.NewGuid():N}.ps1");
                    try
                    {
                        System.IO.File.WriteAllText(tempScriptPath, script);

                        ProcessStartInfo psi = new ProcessStartInfo
                        {
                            FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                            UseShellExecute = true,
                            Verb = "runas",
                            WindowStyle = ProcessWindowStyle.Hidden
                        };
                        psi.ArgumentList.Add("-NoProfile");
                        psi.ArgumentList.Add("-NoLogo");
                        psi.ArgumentList.Add("-NonInteractive");
                        psi.ArgumentList.Add("-File");
                        psi.ArgumentList.Add(tempScriptPath);

                        using Process restorePointProcess = Process.Start(psi)
                            ?? throw new InvalidOperationException("Impossibile avviare la creazione del punto di ripristino.");
                        restorePointProcess.WaitForExit();
                        if (restorePointProcess.ExitCode != 0)
                            throw new InvalidOperationException($"Creazione punto di ripristino terminata con codice {restorePointProcess.ExitCode}.");
                    }
                    finally
                    {
                        try { File.Delete(tempScriptPath); }
                        catch (IOException ex) { Debug.WriteLine($"Impossibile eliminare lo script temporaneo del ripristino: {ex}"); }
                        catch (UnauthorizedAccessException ex) { Debug.WriteLine($"Accesso negato durante la rimozione dello script temporaneo: {ex}"); }
                    }
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string regBackupPath = Path.Join(desktopPath, $"BackupRegistroWinHubX_{DateTime.Now:yyyyMMdd_HHmmss}.reg");

                    ProcessStartInfo regExport = new ProcessStartInfo()
                    {
                        FileName = Path.Join(Environment.SystemDirectory, "reg.exe"),
                        UseShellExecute = true,
                        Verb = "runas"
                    };
                    regExport.ArgumentList.Add("export");
                    regExport.ArgumentList.Add("HKLM");
                    regExport.ArgumentList.Add(regBackupPath);
                    regExport.ArgumentList.Add("/y");

                    using Process exportProcess = Process.Start(regExport)
                        ?? throw new InvalidOperationException("Impossibile avviare l'esportazione del registro.");
                    exportProcess.WaitForExit();
                    if (exportProcess.ExitCode != 0)
                        throw new InvalidOperationException($"Esportazione del registro terminata con codice {exportProcess.ExitCode}.");
                }
                catch (Exception ex) when (ex is IOException
                    or UnauthorizedAccessException
                    or System.Security.SecurityException
                    or System.ComponentModel.Win32Exception
                    or InvalidOperationException
                    or OperationCanceledException)
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
                    await formMonitoraggio.CleanupResourcesAsync();
                }
                catch (Exception ex) when (ex is IOException
                    or UnauthorizedAccessException
                    or System.Security.SecurityException
                    or InvalidOperationException
                    or OperationCanceledException)
                {
                    MessageBox.Show($"Errore nella pulizia di FormMonitoraggio: {ex.Message}");
                }
            }


            if (AppState.IsoMontata && !string.IsNullOrWhiteSpace(AppState.IsoPath))
            {
                try
                {
                    await RunPowerShellAsync(
                        $"Dismount-DiskImage -ImagePath '{EscapePowerShellLiteral(AppState.IsoPath)}'");
                    AppState.IsoMontata = false;
                    AppState.IsoDriveLetter = null;
                    AppState.IsoPath = null;
                }
                catch (Exception ex) when (ex is IOException
                    or UnauthorizedAccessException
                    or System.Security.SecurityException
                    or System.ComponentModel.Win32Exception
                    or InvalidOperationException
                    or OperationCanceledException)
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
                    FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            process.StartInfo.ArgumentList.Add("-NoProfile");
            process.StartInfo.ArgumentList.Add("-NonInteractive");
            process.StartInfo.ArgumentList.Add("-Command");
            process.StartInfo.ArgumentList.Add(command);
            process.Start();
            await process.WaitForExitAsync();
            return process.ExitCode;
        }

        private static string EscapePowerShellLiteral(string value) => value.Replace("'", "''");
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
                using var impostazioni = new FormImpostazioniApp();
                await impostazioni.VerificaAggiornamentiAutomaticiAsync();
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
            ShowSettingsWindow();
        }

        private void ShowSettingsWindow()
        {
            var settings = new FormImpostazioniApp();
            settings.FormClosed += (_, _) => settings.Dispose();
            settings.Show(this);
        }


        private void MostraNotificaAggiornamento()
        {
            if (updateAvailableButton is not null || panel3.IsDisposed)
                return;

            updateAvailableButton = new Button
            {
                Name = "updateAvailableButton",
                Text = LanguageManager.GetTranslation("Form1", "update_available_button"),
                AccessibleName = LanguageManager.GetTranslation("Form1", "update_available_button"),
                AccessibleDescription = LanguageManager.GetTranslation("Form1", "update_available_description"),
                AutoSize = false,
                Size = new Size(184, 32),
                Location = new Point(Math.Max(8, picMinimizzaApp.Left - 196), Math.Max(4, (panel3.ClientSize.Height - 32) / 2)),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.FromArgb(255, 190, 70),
                ForeColor = Color.FromArgb(35, 35, 35),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            updateAvailableButton.FlatAppearance.BorderSize = 0;
            updateAvailableButton.Click += (_, _) => ShowSettingsWindow();
            panel3.Controls.Add(updateAvailableButton);
            updateAvailableButton.BringToFront();
        }
    }
}
