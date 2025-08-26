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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOffice));
            btnAttivaOffice = new Button();
            btnScrubber = new Button();
            btnPersonalizzaOffice = new Button();
            comboBoxInstallazione = new ComboBox();
            labelTipoInstallazione = new Label();
            labelversione = new Label();
            comboBoxVerOffice = new ComboBox();
            pictureBox4 = new PictureBox();
            richTextBoxInfo = new RichTextBox();
            richTextBoxDescription = new RichTextBox();
            btnDownload = new Button();
            comboBox_Lingua = new ComboBox();
            lblSelezionLingua = new Label();
            btnAggRimAppOffice = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            SuspendLayout();
            // 
            // btnAttivaOffice
            // 
            resources.ApplyResources(btnAttivaOffice, "btnAttivaOffice");
            btnAttivaOffice.Cursor = Cursors.Hand;
            btnAttivaOffice.FlatAppearance.BorderSize = 0;
            btnAttivaOffice.ForeColor = Color.White;
            btnAttivaOffice.Image = Properties.Resources.pngAttivaOffice;
            btnAttivaOffice.Name = "btnAttivaOffice";
            btnAttivaOffice.UseVisualStyleBackColor = true;
            btnAttivaOffice.Click += btnAttivaOffice_Click;
            // 
            // btnScrubber
            // 
            resources.ApplyResources(btnScrubber, "btnScrubber");
            btnScrubber.Cursor = Cursors.Hand;
            btnScrubber.FlatAppearance.BorderSize = 0;
            btnScrubber.ForeColor = Color.White;
            btnScrubber.Image = Properties.Resources.pngDisinstallaOffice;
            btnScrubber.Name = "btnScrubber";
            btnScrubber.UseVisualStyleBackColor = true;
            btnScrubber.Click += btnScrubber_Click;
            // 
            // btnPersonalizzaOffice
            // 
            resources.ApplyResources(btnPersonalizzaOffice, "btnPersonalizzaOffice");
            btnPersonalizzaOffice.Cursor = Cursors.Hand;
            btnPersonalizzaOffice.FlatAppearance.BorderSize = 0;
            btnPersonalizzaOffice.ForeColor = Color.White;
            btnPersonalizzaOffice.Image = Properties.Resources.pngPersonalizzaOffice;
            btnPersonalizzaOffice.Name = "btnPersonalizzaOffice";
            btnPersonalizzaOffice.UseVisualStyleBackColor = true;
            btnPersonalizzaOffice.Click += btnPersonalizzaOffice_Click;
            // 
            // comboBoxInstallazione
            // 
            resources.ApplyResources(comboBoxInstallazione, "comboBoxInstallazione");
            comboBoxInstallazione.Cursor = Cursors.Hand;
            comboBoxInstallazione.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxInstallazione.FormattingEnabled = true;
            comboBoxInstallazione.Items.AddRange(new object[] { resources.GetString("comboBoxInstallazione.Items"), resources.GetString("comboBoxInstallazione.Items1"), resources.GetString("comboBoxInstallazione.Items2"), resources.GetString("comboBoxInstallazione.Items3") });
            comboBoxInstallazione.Name = "comboBoxInstallazione";
            comboBoxInstallazione.SelectedIndexChanged += comboBoxInstallazione_SelectedIndexChanged;
            // 
            // labelTipoInstallazione
            // 
            resources.ApplyResources(labelTipoInstallazione, "labelTipoInstallazione");
            labelTipoInstallazione.ForeColor = Color.Coral;
            labelTipoInstallazione.Name = "labelTipoInstallazione";
            // 
            // labelversione
            // 
            resources.ApplyResources(labelversione, "labelversione");
            labelversione.ForeColor = Color.Coral;
            labelversione.Name = "labelversione";
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
            comboBoxVerOffice.SelectedIndexChanged += comboBoxVerOffice_SelectedIndexChanged;
            // 
            // pictureBox4
            // 
            resources.ApplyResources(pictureBox4, "pictureBox4");
            pictureBox4.Name = "pictureBox4";
            pictureBox4.TabStop = false;
            // 
            // richTextBoxInfo
            // 
            resources.ApplyResources(richTextBoxInfo, "richTextBoxInfo");
            richTextBoxInfo.BackColor = Color.FromArgb(37, 38, 39);
            richTextBoxInfo.BorderStyle = BorderStyle.None;
            richTextBoxInfo.ForeColor = Color.White;
            richTextBoxInfo.Name = "richTextBoxInfo";
            // 
            // richTextBoxDescription
            // 
            resources.ApplyResources(richTextBoxDescription, "richTextBoxDescription");
            richTextBoxDescription.BackColor = Color.FromArgb(37, 38, 39);
            richTextBoxDescription.BorderStyle = BorderStyle.None;
            richTextBoxDescription.ForeColor = Color.White;
            richTextBoxDescription.Name = "richTextBoxDescription";
            // 
            // btnDownload
            // 
            resources.ApplyResources(btnDownload, "btnDownload");
            btnDownload.Cursor = Cursors.Hand;
            btnDownload.FlatAppearance.BorderSize = 0;
            btnDownload.ForeColor = Color.White;
            btnDownload.Image = Properties.Resources.pngDownloadOffice;
            btnDownload.Name = "btnDownload";
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_Click;
            // 
            // comboBox_Lingua
            // 
            resources.ApplyResources(comboBox_Lingua, "comboBox_Lingua");
            comboBox_Lingua.Cursor = Cursors.Hand;
            comboBox_Lingua.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_Lingua.FormattingEnabled = true;
            comboBox_Lingua.Items.AddRange(new object[] { resources.GetString("comboBox_Lingua.Items"), resources.GetString("comboBox_Lingua.Items1") });
            comboBox_Lingua.Name = "comboBox_Lingua";
            comboBox_Lingua.SelectedIndexChanged += comboBox_Lingua_SelectedIndexChanged;
            // 
            // lblSelezionLingua
            // 
            resources.ApplyResources(lblSelezionLingua, "lblSelezionLingua");
            lblSelezionLingua.ForeColor = Color.Coral;
            lblSelezionLingua.Name = "lblSelezionLingua";
            // 
            // btnAggRimAppOffice
            // 
            resources.ApplyResources(btnAggRimAppOffice, "btnAggRimAppOffice");
            btnAggRimAppOffice.Cursor = Cursors.Hand;
            btnAggRimAppOffice.FlatAppearance.BorderSize = 0;
            btnAggRimAppOffice.ForeColor = Color.White;
            btnAggRimAppOffice.Image = Properties.Resources.pngAggiungiRimuoviOffice;
            btnAggRimAppOffice.Name = "btnAggRimAppOffice";
            btnAggRimAppOffice.UseVisualStyleBackColor = true;
            btnAggRimAppOffice.Click += btnAggRimAppOffice_Click;
            // 
            // FormOffice
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(btnAggRimAppOffice);
            Controls.Add(comboBox_Lingua);
            Controls.Add(lblSelezionLingua);
            Controls.Add(btnDownload);
            Controls.Add(richTextBoxInfo);
            Controls.Add(richTextBoxDescription);
            Controls.Add(pictureBox4);
            Controls.Add(comboBoxInstallazione);
            Controls.Add(labelTipoInstallazione);
            Controls.Add(labelversione);
            Controls.Add(comboBoxVerOffice);
            Controls.Add(btnPersonalizzaOffice);
            Controls.Add(btnScrubber);
            Controls.Add(btnAttivaOffice);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormOffice";
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnAttivaOffice;
        private Button btnScrubber;
        private Button btnPersonalizzaOffice;
        private ComboBox comboBoxInstallazione;
        private Label labelTipoInstallazione;
        private Label labelversione;
        private ComboBox comboBoxVerOffice;
        private PictureBox pictureBox4;
        private RichTextBox richTextBoxInfo;
        private RichTextBox richTextBoxDescription;
        private Button btnDownload;
        private ComboBox comboBox_Lingua;
        private Label lblSelezionLingua;
        private Button btnAggRimAppOffice;
    }
}