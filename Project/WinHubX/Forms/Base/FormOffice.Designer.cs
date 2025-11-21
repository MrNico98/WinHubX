using CuoreUI.Controls;

namespace WinHubX
{
    partial class FormOffice
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOffice));
            panel50 = new Panel();
            tableLayoutPanel50 = new TableLayoutPanel();
            panel60 = new Panel();
            btnPersonalizzaOfficePrinci = new cuiButton();
            btnAggRimAppOfficePrinci = new cuiButton();
            btnAttivaOfficePrinci = new cuiButton();
            btnScrubberPrinci = new cuiButton();
            panel70 = new Panel();
            label2 = new Label();
            Checkbox_Salva = new CheckBox();
            Checkbox_Installa = new CheckBox();
            btn_cambiaBianco = new cuiButton();
            comboBox_Lingua = new ComboBox();
            label1 = new Label();
            comboBoxVerOffice = new ComboBox();
            labelversione = new Label();
            labelTipoInstallazione = new Label();
            labelavviso = new Label();
            comboBoxInstallazione = new ComboBox();
            labelpercorso = new Label();
            lblSelezionLingua = new Label();
            progressBar1 = new cuiProgressBarHorizontal();
            btnDownloadVerdi = new cuiButton();
            toolTip1 = new ToolTip(components);
            panel50.SuspendLayout();
            tableLayoutPanel50.SuspendLayout();
            panel60.SuspendLayout();
            panel70.SuspendLayout();
            SuspendLayout();
            // 
            // panel50
            // 
            resources.ApplyResources(panel50, "panel50");
            panel50.Controls.Add(tableLayoutPanel50);
            panel50.Name = "panel50";
            toolTip1.SetToolTip(panel50, resources.GetString("panel50.ToolTip"));
            panel50.Resize += panel50_Resize;
            // 
            // tableLayoutPanel50
            // 
            resources.ApplyResources(tableLayoutPanel50, "tableLayoutPanel50");
            tableLayoutPanel50.Controls.Add(panel60, 1, 0);
            tableLayoutPanel50.Controls.Add(panel70, 0, 0);
            tableLayoutPanel50.Name = "tableLayoutPanel50";
            toolTip1.SetToolTip(tableLayoutPanel50, resources.GetString("tableLayoutPanel50.ToolTip"));
            tableLayoutPanel50.Resize += tableLayoutPanel50_Resize;
            // 
            // panel60
            // 
            resources.ApplyResources(panel60, "panel60");
            panel60.Controls.Add(btnPersonalizzaOfficePrinci);
            panel60.Controls.Add(btnAggRimAppOfficePrinci);
            panel60.Controls.Add(btnAttivaOfficePrinci);
            panel60.Controls.Add(btnScrubberPrinci);
            panel60.Name = "panel60";
            toolTip1.SetToolTip(panel60, resources.GetString("panel60.ToolTip"));
            // 
            // btnPersonalizzaOfficePrinci
            // 
            resources.ApplyResources(btnPersonalizzaOfficePrinci, "btnPersonalizzaOfficePrinci");
            btnPersonalizzaOfficePrinci.CheckButton = false;
            btnPersonalizzaOfficePrinci.Checked = false;
            btnPersonalizzaOfficePrinci.CheckedBackground = Color.White;
            btnPersonalizzaOfficePrinci.CheckedForeColor = Color.Black;
            btnPersonalizzaOfficePrinci.CheckedImageTint = Color.White;
            btnPersonalizzaOfficePrinci.CheckedOutline = Color.White;
            btnPersonalizzaOfficePrinci.Content = " Crea versione personalizzata";
            btnPersonalizzaOfficePrinci.DialogResult = DialogResult.None;
            btnPersonalizzaOfficePrinci.ForeColor = Color.White;
            btnPersonalizzaOfficePrinci.HoverBackground = Color.White;
            btnPersonalizzaOfficePrinci.HoverForeColor = Color.Black;
            btnPersonalizzaOfficePrinci.HoverImageTint = Color.White;
            btnPersonalizzaOfficePrinci.HoverOutline = Color.White;
            btnPersonalizzaOfficePrinci.Image = Properties.Resources.pngPersonalizzaOffice;
            btnPersonalizzaOfficePrinci.ImageAutoCenter = true;
            btnPersonalizzaOfficePrinci.ImageExpand = new Point(0, 0);
            btnPersonalizzaOfficePrinci.ImageOffset = new Point(0, 0);
            btnPersonalizzaOfficePrinci.Name = "btnPersonalizzaOfficePrinci";
            btnPersonalizzaOfficePrinci.NormalBackground = Color.FromArgb(37, 38, 39);
            btnPersonalizzaOfficePrinci.NormalForeColor = Color.White;
            btnPersonalizzaOfficePrinci.NormalImageTint = Color.White;
            btnPersonalizzaOfficePrinci.NormalOutline = Color.White;
            btnPersonalizzaOfficePrinci.OutlineThickness = 1F;
            btnPersonalizzaOfficePrinci.PressedBackground = Color.White;
            btnPersonalizzaOfficePrinci.PressedForeColor = Color.Black;
            btnPersonalizzaOfficePrinci.PressedImageTint = Color.White;
            btnPersonalizzaOfficePrinci.PressedOutline = Color.White;
            btnPersonalizzaOfficePrinci.Rounding = new Padding(8);
            btnPersonalizzaOfficePrinci.TextAlignment = StringAlignment.Center;
            btnPersonalizzaOfficePrinci.TextOffset = new Point(0, 0);
            toolTip1.SetToolTip(btnPersonalizzaOfficePrinci, resources.GetString("btnPersonalizzaOfficePrinci.ToolTip"));
            btnPersonalizzaOfficePrinci.Click += btnPersonalizzaOffice_Click;
            // 
            // btnAggRimAppOfficePrinci
            // 
            resources.ApplyResources(btnAggRimAppOfficePrinci, "btnAggRimAppOfficePrinci");
            btnAggRimAppOfficePrinci.CheckButton = false;
            btnAggRimAppOfficePrinci.Checked = false;
            btnAggRimAppOfficePrinci.CheckedBackground = Color.White;
            btnAggRimAppOfficePrinci.CheckedForeColor = Color.Black;
            btnAggRimAppOfficePrinci.CheckedImageTint = Color.White;
            btnAggRimAppOfficePrinci.CheckedOutline = Color.White;
            btnAggRimAppOfficePrinci.Content = " Aggiungi/Rimuovi app";
            btnAggRimAppOfficePrinci.DialogResult = DialogResult.None;
            btnAggRimAppOfficePrinci.ForeColor = Color.White;
            btnAggRimAppOfficePrinci.HoverBackground = Color.White;
            btnAggRimAppOfficePrinci.HoverForeColor = Color.Black;
            btnAggRimAppOfficePrinci.HoverImageTint = Color.White;
            btnAggRimAppOfficePrinci.HoverOutline = Color.White;
            btnAggRimAppOfficePrinci.Image = Properties.Resources.pngAggiungiRimuoviOffice;
            btnAggRimAppOfficePrinci.ImageAutoCenter = true;
            btnAggRimAppOfficePrinci.ImageExpand = new Point(0, 0);
            btnAggRimAppOfficePrinci.ImageOffset = new Point(0, 0);
            btnAggRimAppOfficePrinci.Name = "btnAggRimAppOfficePrinci";
            btnAggRimAppOfficePrinci.NormalBackground = Color.FromArgb(37, 38, 39);
            btnAggRimAppOfficePrinci.NormalForeColor = Color.White;
            btnAggRimAppOfficePrinci.NormalImageTint = Color.White;
            btnAggRimAppOfficePrinci.NormalOutline = Color.White;
            btnAggRimAppOfficePrinci.OutlineThickness = 1F;
            btnAggRimAppOfficePrinci.PressedBackground = Color.White;
            btnAggRimAppOfficePrinci.PressedForeColor = Color.Black;
            btnAggRimAppOfficePrinci.PressedImageTint = Color.White;
            btnAggRimAppOfficePrinci.PressedOutline = Color.White;
            btnAggRimAppOfficePrinci.Rounding = new Padding(8);
            btnAggRimAppOfficePrinci.TextAlignment = StringAlignment.Center;
            btnAggRimAppOfficePrinci.TextOffset = new Point(0, 0);
            toolTip1.SetToolTip(btnAggRimAppOfficePrinci, resources.GetString("btnAggRimAppOfficePrinci.ToolTip"));
            btnAggRimAppOfficePrinci.Click += btnAggRimAppOffice_Click;
            // 
            // btnAttivaOfficePrinci
            // 
            resources.ApplyResources(btnAttivaOfficePrinci, "btnAttivaOfficePrinci");
            btnAttivaOfficePrinci.CheckButton = false;
            btnAttivaOfficePrinci.Checked = false;
            btnAttivaOfficePrinci.CheckedBackground = Color.White;
            btnAttivaOfficePrinci.CheckedForeColor = Color.Black;
            btnAttivaOfficePrinci.CheckedImageTint = Color.White;
            btnAttivaOfficePrinci.CheckedOutline = Color.White;
            btnAttivaOfficePrinci.Content = "  Attiva Office";
            btnAttivaOfficePrinci.DialogResult = DialogResult.None;
            btnAttivaOfficePrinci.ForeColor = Color.White;
            btnAttivaOfficePrinci.HoverBackground = Color.White;
            btnAttivaOfficePrinci.HoverForeColor = Color.Black;
            btnAttivaOfficePrinci.HoverImageTint = Color.White;
            btnAttivaOfficePrinci.HoverOutline = Color.White;
            btnAttivaOfficePrinci.Image = Properties.Resources.pngAttivaWindows;
            btnAttivaOfficePrinci.ImageAutoCenter = true;
            btnAttivaOfficePrinci.ImageExpand = new Point(0, 0);
            btnAttivaOfficePrinci.ImageOffset = new Point(0, 0);
            btnAttivaOfficePrinci.Name = "btnAttivaOfficePrinci";
            btnAttivaOfficePrinci.NormalBackground = Color.FromArgb(37, 38, 39);
            btnAttivaOfficePrinci.NormalForeColor = Color.White;
            btnAttivaOfficePrinci.NormalImageTint = Color.White;
            btnAttivaOfficePrinci.NormalOutline = Color.White;
            btnAttivaOfficePrinci.OutlineThickness = 1F;
            btnAttivaOfficePrinci.PressedBackground = Color.White;
            btnAttivaOfficePrinci.PressedForeColor = Color.Black;
            btnAttivaOfficePrinci.PressedImageTint = Color.White;
            btnAttivaOfficePrinci.PressedOutline = Color.White;
            btnAttivaOfficePrinci.Rounding = new Padding(8);
            btnAttivaOfficePrinci.TextAlignment = StringAlignment.Center;
            btnAttivaOfficePrinci.TextOffset = new Point(0, 0);
            toolTip1.SetToolTip(btnAttivaOfficePrinci, resources.GetString("btnAttivaOfficePrinci.ToolTip"));
            btnAttivaOfficePrinci.Click += btnAttivaOffice_Click;
            // 
            // btnScrubberPrinci
            // 
            resources.ApplyResources(btnScrubberPrinci, "btnScrubberPrinci");
            btnScrubberPrinci.CheckButton = false;
            btnScrubberPrinci.Checked = false;
            btnScrubberPrinci.CheckedBackground = Color.White;
            btnScrubberPrinci.CheckedForeColor = Color.Black;
            btnScrubberPrinci.CheckedImageTint = Color.White;
            btnScrubberPrinci.CheckedOutline = Color.White;
            btnScrubberPrinci.Content = "  Disinstalla Office";
            btnScrubberPrinci.DialogResult = DialogResult.None;
            btnScrubberPrinci.ForeColor = Color.White;
            btnScrubberPrinci.HoverBackground = Color.White;
            btnScrubberPrinci.HoverForeColor = Color.Black;
            btnScrubberPrinci.HoverImageTint = Color.White;
            btnScrubberPrinci.HoverOutline = Color.White;
            btnScrubberPrinci.Image = Properties.Resources.pngDisinstallaOffice;
            btnScrubberPrinci.ImageAutoCenter = true;
            btnScrubberPrinci.ImageExpand = new Point(0, 0);
            btnScrubberPrinci.ImageOffset = new Point(0, 0);
            btnScrubberPrinci.Name = "btnScrubberPrinci";
            btnScrubberPrinci.NormalBackground = Color.FromArgb(37, 38, 39);
            btnScrubberPrinci.NormalForeColor = Color.White;
            btnScrubberPrinci.NormalImageTint = Color.White;
            btnScrubberPrinci.NormalOutline = Color.White;
            btnScrubberPrinci.OutlineThickness = 1F;
            btnScrubberPrinci.PressedBackground = Color.White;
            btnScrubberPrinci.PressedForeColor = Color.Black;
            btnScrubberPrinci.PressedImageTint = Color.White;
            btnScrubberPrinci.PressedOutline = Color.White;
            btnScrubberPrinci.Rounding = new Padding(8);
            btnScrubberPrinci.TextAlignment = StringAlignment.Center;
            btnScrubberPrinci.TextOffset = new Point(0, 0);
            toolTip1.SetToolTip(btnScrubberPrinci, resources.GetString("btnScrubberPrinci.ToolTip"));
            btnScrubberPrinci.Click += btnScrubber_Click;
            // 
            // panel70
            // 
            resources.ApplyResources(panel70, "panel70");
            panel70.Controls.Add(label2);
            panel70.Controls.Add(Checkbox_Salva);
            panel70.Controls.Add(Checkbox_Installa);
            panel70.Controls.Add(btn_cambiaBianco);
            panel70.Controls.Add(comboBox_Lingua);
            panel70.Controls.Add(label1);
            panel70.Controls.Add(comboBoxVerOffice);
            panel70.Controls.Add(labelversione);
            panel70.Controls.Add(labelTipoInstallazione);
            panel70.Controls.Add(labelavviso);
            panel70.Controls.Add(comboBoxInstallazione);
            panel70.Controls.Add(labelpercorso);
            panel70.Controls.Add(lblSelezionLingua);
            panel70.Controls.Add(progressBar1);
            panel70.Controls.Add(btnDownloadVerdi);
            panel70.Name = "panel70";
            toolTip1.SetToolTip(panel70, resources.GetString("panel70.ToolTip"));
            panel70.Resize += panel70_Resize;
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.BackColor = Color.Transparent;
            label2.ForeColor = Color.White;
            label2.Name = "label2";
            toolTip1.SetToolTip(label2, resources.GetString("label2.ToolTip"));
            // 
            // Checkbox_Salva
            // 
            resources.ApplyResources(Checkbox_Salva, "Checkbox_Salva");
            Checkbox_Salva.ForeColor = Color.White;
            Checkbox_Salva.Name = "Checkbox_Salva";
            toolTip1.SetToolTip(Checkbox_Salva, resources.GetString("Checkbox_Salva.ToolTip"));
            Checkbox_Salva.UseVisualStyleBackColor = true;
            Checkbox_Salva.CheckedChanged += Checkbox_Salva_CheckedChanged;
            // 
            // Checkbox_Installa
            // 
            resources.ApplyResources(Checkbox_Installa, "Checkbox_Installa");
            Checkbox_Installa.ForeColor = Color.White;
            Checkbox_Installa.Name = "Checkbox_Installa";
            toolTip1.SetToolTip(Checkbox_Installa, resources.GetString("Checkbox_Installa.ToolTip"));
            Checkbox_Installa.UseVisualStyleBackColor = true;
            Checkbox_Installa.CheckedChanged += Checkbox_Installa_CheckedChanged;
            // 
            // btn_cambiaBianco
            // 
            resources.ApplyResources(btn_cambiaBianco, "btn_cambiaBianco");
            btn_cambiaBianco.CheckButton = false;
            btn_cambiaBianco.Checked = false;
            btn_cambiaBianco.CheckedBackground = Color.White;
            btn_cambiaBianco.CheckedForeColor = Color.Black;
            btn_cambiaBianco.CheckedImageTint = Color.Black;
            btn_cambiaBianco.CheckedOutline = Color.White;
            btn_cambiaBianco.Content = "Change";
            btn_cambiaBianco.DialogResult = DialogResult.None;
            btn_cambiaBianco.ForeColor = Color.White;
            btn_cambiaBianco.HoverBackground = Color.White;
            btn_cambiaBianco.HoverForeColor = Color.Black;
            btn_cambiaBianco.HoverImageTint = Color.Black;
            btn_cambiaBianco.HoverOutline = Color.White;
            btn_cambiaBianco.Image = null;
            btn_cambiaBianco.ImageAutoCenter = true;
            btn_cambiaBianco.ImageExpand = new Point(0, 0);
            btn_cambiaBianco.ImageOffset = new Point(0, 0);
            btn_cambiaBianco.Name = "btn_cambiaBianco";
            btn_cambiaBianco.NormalBackground = Color.FromArgb(37, 38, 39);
            btn_cambiaBianco.NormalForeColor = Color.White;
            btn_cambiaBianco.NormalImageTint = Color.White;
            btn_cambiaBianco.NormalOutline = Color.White;
            btn_cambiaBianco.OutlineThickness = 1F;
            btn_cambiaBianco.PressedBackground = Color.White;
            btn_cambiaBianco.PressedForeColor = Color.Black;
            btn_cambiaBianco.PressedImageTint = Color.Black;
            btn_cambiaBianco.PressedOutline = Color.White;
            btn_cambiaBianco.Rounding = new Padding(8);
            btn_cambiaBianco.TextAlignment = StringAlignment.Center;
            btn_cambiaBianco.TextOffset = new Point(0, 0);
            toolTip1.SetToolTip(btn_cambiaBianco, resources.GetString("btn_cambiaBianco.ToolTip"));
            btn_cambiaBianco.Click += btn_cambia_Click;
            // 
            // comboBox_Lingua
            // 
            resources.ApplyResources(comboBox_Lingua, "comboBox_Lingua");
            comboBox_Lingua.Cursor = Cursors.Hand;
            comboBox_Lingua.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_Lingua.FormattingEnabled = true;
            comboBox_Lingua.Items.AddRange(new object[] { resources.GetString("comboBox_Lingua.Items"), resources.GetString("comboBox_Lingua.Items1") });
            comboBox_Lingua.Name = "comboBox_Lingua";
            toolTip1.SetToolTip(comboBox_Lingua, resources.GetString("comboBox_Lingua.ToolTip"));
            comboBox_Lingua.SelectedIndexChanged += comboBox_Lingua_SelectedIndexChanged;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.FromArgb(0, 126, 249);
            label1.Name = "label1";
            toolTip1.SetToolTip(label1, resources.GetString("label1.ToolTip"));
            // 
            // comboBoxVerOffice
            // 
            resources.ApplyResources(comboBoxVerOffice, "comboBoxVerOffice");
            comboBoxVerOffice.BackColor = Color.White;
            comboBoxVerOffice.Cursor = Cursors.Hand;
            comboBoxVerOffice.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxVerOffice.FormattingEnabled = true;
            comboBoxVerOffice.Items.AddRange(new object[] { resources.GetString("comboBoxVerOffice.Items"), resources.GetString("comboBoxVerOffice.Items1"), resources.GetString("comboBoxVerOffice.Items2"), resources.GetString("comboBoxVerOffice.Items3") });
            comboBoxVerOffice.Name = "comboBoxVerOffice";
            toolTip1.SetToolTip(comboBoxVerOffice, resources.GetString("comboBoxVerOffice.ToolTip"));
            comboBoxVerOffice.SelectedIndexChanged += comboBoxVerOffice_SelectedIndexChanged;
            // 
            // labelversione
            // 
            resources.ApplyResources(labelversione, "labelversione");
            labelversione.ForeColor = Color.FromArgb(0, 126, 249);
            labelversione.Name = "labelversione";
            toolTip1.SetToolTip(labelversione, resources.GetString("labelversione.ToolTip"));
            // 
            // labelTipoInstallazione
            // 
            resources.ApplyResources(labelTipoInstallazione, "labelTipoInstallazione");
            labelTipoInstallazione.ForeColor = Color.FromArgb(0, 126, 249);
            labelTipoInstallazione.Name = "labelTipoInstallazione";
            toolTip1.SetToolTip(labelTipoInstallazione, resources.GetString("labelTipoInstallazione.ToolTip"));
            // 
            // labelavviso
            // 
            resources.ApplyResources(labelavviso, "labelavviso");
            labelavviso.ForeColor = Color.FromArgb(192, 0, 0);
            labelavviso.Name = "labelavviso";
            toolTip1.SetToolTip(labelavviso, resources.GetString("labelavviso.ToolTip"));
            // 
            // comboBoxInstallazione
            // 
            resources.ApplyResources(comboBoxInstallazione, "comboBoxInstallazione");
            comboBoxInstallazione.Cursor = Cursors.Hand;
            comboBoxInstallazione.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxInstallazione.FormattingEnabled = true;
            comboBoxInstallazione.Items.AddRange(new object[] { resources.GetString("comboBoxInstallazione.Items"), resources.GetString("comboBoxInstallazione.Items1"), resources.GetString("comboBoxInstallazione.Items2"), resources.GetString("comboBoxInstallazione.Items3") });
            comboBoxInstallazione.Name = "comboBoxInstallazione";
            toolTip1.SetToolTip(comboBoxInstallazione, resources.GetString("comboBoxInstallazione.ToolTip"));
            comboBoxInstallazione.SelectedIndexChanged += comboBoxInstallazione_SelectedIndexChanged;
            // 
            // labelpercorso
            // 
            resources.ApplyResources(labelpercorso, "labelpercorso");
            labelpercorso.ForeColor = Color.FromArgb(0, 126, 249);
            labelpercorso.Name = "labelpercorso";
            toolTip1.SetToolTip(labelpercorso, resources.GetString("labelpercorso.ToolTip"));
            // 
            // lblSelezionLingua
            // 
            resources.ApplyResources(lblSelezionLingua, "lblSelezionLingua");
            lblSelezionLingua.ForeColor = Color.FromArgb(0, 126, 249);
            lblSelezionLingua.Name = "lblSelezionLingua";
            toolTip1.SetToolTip(lblSelezionLingua, resources.GetString("lblSelezionLingua.ToolTip"));
            // 
            // progressBar1
            // 
            resources.ApplyResources(progressBar1, "progressBar1");
            progressBar1.Background = Color.FromArgb(64, 128, 128, 128);
            progressBar1.Flipped = false;
            progressBar1.Foreground = Color.FromArgb(46, 125, 60);
            progressBar1.MaxValue = 100;
            progressBar1.Name = "progressBar1";
            progressBar1.Rounding = 8;
            toolTip1.SetToolTip(progressBar1, resources.GetString("progressBar1.ToolTip"));
            progressBar1.Value = 0;
            // 
            // btnDownloadVerdi
            // 
            resources.ApplyResources(btnDownloadVerdi, "btnDownloadVerdi");
            btnDownloadVerdi.CheckButton = false;
            btnDownloadVerdi.Checked = false;
            btnDownloadVerdi.CheckedBackground = Color.FromArgb(46, 125, 60);
            btnDownloadVerdi.CheckedForeColor = Color.FromArgb(46, 125, 60);
            btnDownloadVerdi.CheckedImageTint = Color.FromArgb(46, 125, 60);
            btnDownloadVerdi.CheckedOutline = Color.FromArgb(46, 125, 60);
            btnDownloadVerdi.Content = "  Download";
            btnDownloadVerdi.DialogResult = DialogResult.None;
            btnDownloadVerdi.ForeColor = Color.White;
            btnDownloadVerdi.HoverBackground = Color.FromArgb(46, 125, 50);
            btnDownloadVerdi.HoverForeColor = Color.White;
            btnDownloadVerdi.HoverImageTint = Color.White;
            btnDownloadVerdi.HoverOutline = Color.FromArgb(46, 125, 50);
            btnDownloadVerdi.Image = Properties.Resources.pngScaricaOffice;
            btnDownloadVerdi.ImageAutoCenter = true;
            btnDownloadVerdi.ImageExpand = new Point(0, 0);
            btnDownloadVerdi.ImageOffset = new Point(0, 0);
            btnDownloadVerdi.Name = "btnDownloadVerdi";
            btnDownloadVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btnDownloadVerdi.NormalForeColor = Color.White;
            btnDownloadVerdi.NormalImageTint = Color.White;
            btnDownloadVerdi.NormalOutline = Color.FromArgb(46, 125, 50);
            btnDownloadVerdi.OutlineThickness = 1F;
            btnDownloadVerdi.PressedBackground = Color.FromArgb(46, 125, 50);
            btnDownloadVerdi.PressedForeColor = Color.Black;
            btnDownloadVerdi.PressedImageTint = Color.Black;
            btnDownloadVerdi.PressedOutline = Color.FromArgb(46, 125, 50);
            btnDownloadVerdi.Rounding = new Padding(8);
            btnDownloadVerdi.TextAlignment = StringAlignment.Center;
            btnDownloadVerdi.TextOffset = new Point(0, 0);
            toolTip1.SetToolTip(btnDownloadVerdi, resources.GetString("btnDownloadVerdi.ToolTip"));
            btnDownloadVerdi.Click += btnDownload_Click;
            // 
            // FormOffice
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(panel50);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormOffice";
            toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            Load += FormOffice_Load;
            Resize += FormOffice_Resize;
            panel50.ResumeLayout(false);
            tableLayoutPanel50.ResumeLayout(false);
            panel60.ResumeLayout(false);
            panel70.ResumeLayout(false);
            panel70.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private RichTextBox richTextBoxInfo;
        private Panel panel50;
        private TableLayoutPanel tableLayoutPanel50;
        private Panel panel70;
        private Panel panel60;
        private cuiButton btnPersonalizzaOfficePrinci;
        private cuiButton btnAggRimAppOfficePrinci;
        private cuiButton btnAttivaOfficePrinci;
        private cuiButton btnScrubberPrinci;
        private cuiButton btn_cambiaBianco;
        private Label label1;
        private Label labelavviso;
        private Label labelpercorso;
        private cuiProgressBarHorizontal progressBar1;
        private cuiButton btnDownloadVerdi;
        private ComboBox comboBox_Lingua;
        private ComboBox comboBoxVerOffice;
        private Label labelversione;
        private Label labelTipoInstallazione;
        private ComboBox comboBoxInstallazione;
        private Label lblSelezionLingua;
        private CheckBox Checkbox_Installa;
        private CheckBox Checkbox_Salva;
        private Label label2;
        private ToolTip toolTip1;
    }
}