using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinHubX.Forms.Base
{
    public partial class FormImpostazioniApp : Form
    {
        private bool isLoading = true;
        Form1 mainform1;
        FormMonitoraggio monitoraggio;
        public FormImpostazioniApp(Form1 mainform1, FormMonitoraggio monitoraggio)
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            this.mainform1 = mainform1;
            this.monitoraggio = monitoraggio;
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }

        private void bottoniSwap1_CheckedChanged(object sender, EventArgs e)
        {
            bool dark = bottoniSwap1.Checked;
            ThemeManager.SetTheme(dark);
            label1.Text = dark
                ? LanguageManager.GetTranslation("Form1", "temascuro")
                : LanguageManager.GetTranslation("Form1", "temachiaro");
            Properties.Settings.Default.DarkTheme = dark;
            Properties.Settings.Default.Save();
            ThemeManager.ApplyThemeToControl(this, dark);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            string currentLanguage = Properties.Settings.Default.Language ?? "it";
            string newLanguage = currentLanguage == "it" ? "en" : "it";
            Properties.Settings.Default.Language = newLanguage;
            Properties.Settings.Default.Save();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(newLanguage);
            Controls.Clear();
            InitializeComponent();
            bool dark = Properties.Settings.Default.DarkTheme;
            ThemeManager.SetTheme(dark);
            label1.Text = dark
                ? LanguageManager.GetTranslation("Form1", "temascuro")
                : LanguageManager.GetTranslation("Form1", "temachiaro");
            mainform1.ReloadUI();
            comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
            if (newLanguage == "it")
            {
                comboBox1.SelectedItem = "Italiano";
                pictureBox3.Image = Properties.Resources.italias;
            }
            else if (newLanguage == "en")
            {
                comboBox1.SelectedItem = "English";
                pictureBox3.Image = Properties.Resources.englisj;
            }

            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            if (!isLoading)
            {
                mainform1.CheckForUpdatesOnStartup();
            }
        }

        private void FormImpostazioniApp_Load(object sender, EventArgs e)
        {
            radioButton_notifica.Checked = Properties.Settings.Default.MinimizeToTray;
            radioButton_taskbar.Checked = !Properties.Settings.Default.MinimizeToTray;
            if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "en")
            {
                comboBox1.SelectedItem = "English";
            }
            else
            {
                comboBox1.SelectedItem = "Italiano";
            }
            isLoading = false;
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
            if (savedLanguage == "it")
            {
                comboBox1.SelectedItem = "Italiano";
                pictureBox3.Image = Properties.Resources.italias;
            }
            else if (savedLanguage == "en")
            {
                comboBox1.SelectedItem = "English";
                pictureBox3.Image = Properties.Resources.englisj;
            }
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            bool dark = Properties.Settings.Default.DarkTheme;
            bottoniSwap1.Checked = dark;
            ThemeManager.SetTheme(dark);
            label1.Text = dark
                ? LanguageManager.GetTranslation("Form1", "temascuro")
                : LanguageManager.GetTranslation("Form1", "temachiaro");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null) return;

            string languageCode = "it";
            string selectedLanguage = comboBox1.SelectedItem.ToString();
            if (selectedLanguage == "English")
            {
                languageCode = "en";
            }

            Properties.Settings.Default.Language = languageCode;
            Properties.Settings.Default.Save();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(languageCode);
            Controls.Clear();
            InitializeComponent();
            bool dark = Properties.Settings.Default.DarkTheme;
            ThemeManager.SetTheme(dark);
            label1.Text = dark
                ? LanguageManager.GetTranslation("Form1", "temascuro")
                : LanguageManager.GetTranslation("Form1", "temachiaro");
            mainform1.ReloadUI();
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            LanguageManager.SetLanguage(savedLanguage);

            comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
            if (savedLanguage == "it")
            {
                comboBox1.SelectedItem = "Italiano";
                pictureBox3.Image = Properties.Resources.italias;
            }
            else if (savedLanguage == "en")
            {
                comboBox1.SelectedItem = "English";
                pictureBox3.Image = Properties.Resources.englisj;
            }
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

            if (isLoading == false)
            {
                mainform1.CheckForUpdatesOnStartup();
            }
        }

        private void radioButton_notifica_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_notifica.Checked)
            {
                Properties.Settings.Default.MinimizeToTray = true;
                Properties.Settings.Default.Save();
            }
        }

        private void radioButton_taskbar_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_taskbar.Checked)
            {
                Properties.Settings.Default.MinimizeToTray = false;
                Properties.Settings.Default.Save();
            }
        }
    }
}
