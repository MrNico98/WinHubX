namespace WinHubX
{
    partial class FormWin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWin));
            btnAttivaWin = new Button();
            btnCambioEdizione = new Button();
            comboBox1 = new ComboBox();
            label5 = new Label();
            labelVersione = new Label();
            comboBoxVersione = new ComboBox();
            comboBoxArchitettura = new ComboBox();
            labelArchitettura = new Label();
            richTextBoxDescription = new RichTextBox();
            btnDownload = new Button();
            richTextBoxInfo = new RichTextBox();
            pictureBox4 = new PictureBox();
            comboBox_SelezionaLingua = new ComboBox();
            lblSelezionaLingua = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            SuspendLayout();
            // 
            // btnAttivaWin
            // 
            resources.ApplyResources(btnAttivaWin, "btnAttivaWin");
            btnAttivaWin.Cursor = Cursors.Hand;
            btnAttivaWin.FlatAppearance.BorderSize = 0;
            btnAttivaWin.ForeColor = Color.White;
            btnAttivaWin.Image = Properties.Resources.pngAttivaWin;
            btnAttivaWin.Name = "btnAttivaWin";
            btnAttivaWin.UseVisualStyleBackColor = true;
            btnAttivaWin.Click += btnAttivaWin_Click;
            // 
            // btnCambioEdizione
            // 
            resources.ApplyResources(btnCambioEdizione, "btnCambioEdizione");
            btnCambioEdizione.Cursor = Cursors.Hand;
            btnCambioEdizione.FlatAppearance.BorderSize = 0;
            btnCambioEdizione.ForeColor = Color.White;
            btnCambioEdizione.Image = Properties.Resources.pngCambioEdizione;
            btnCambioEdizione.Name = "btnCambioEdizione";
            btnCambioEdizione.UseVisualStyleBackColor = true;
            btnCambioEdizione.Click += btnCambioEdizione_Click;
            // 
            // comboBox1
            // 
            comboBox1.Cursor = Cursors.Hand;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            resources.ApplyResources(comboBox1, "comboBox1");
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { resources.GetString("comboBox1.Items"), resources.GetString("comboBox1.Items1"), resources.GetString("comboBox1.Items2"), resources.GetString("comboBox1.Items3"), resources.GetString("comboBox1.Items4"), resources.GetString("comboBox1.Items5") });
            comboBox1.Name = "comboBox1";
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.ForeColor = Color.Coral;
            label5.Name = "label5";
            // 
            // labelVersione
            // 
            resources.ApplyResources(labelVersione, "labelVersione");
            labelVersione.ForeColor = Color.Coral;
            labelVersione.Name = "labelVersione";
            // 
            // comboBoxVersione
            // 
            comboBoxVersione.Cursor = Cursors.Hand;
            comboBoxVersione.DropDownStyle = ComboBoxStyle.DropDownList;
            resources.ApplyResources(comboBoxVersione, "comboBoxVersione");
            comboBoxVersione.FormattingEnabled = true;
            comboBoxVersione.Items.AddRange(new object[] { resources.GetString("comboBoxVersione.Items"), resources.GetString("comboBoxVersione.Items1") });
            comboBoxVersione.Name = "comboBoxVersione";
            comboBoxVersione.SelectedIndexChanged += comboBoxVersione_SelectedIndexChanged;
            // 
            // comboBoxArchitettura
            // 
            comboBoxArchitettura.Cursor = Cursors.Hand;
            comboBoxArchitettura.DropDownStyle = ComboBoxStyle.DropDownList;
            resources.ApplyResources(comboBoxArchitettura, "comboBoxArchitettura");
            comboBoxArchitettura.FormattingEnabled = true;
            comboBoxArchitettura.Name = "comboBoxArchitettura";
            comboBoxArchitettura.SelectedIndexChanged += comboBoxArchitettura_SelectedIndexChanged;
            // 
            // labelArchitettura
            // 
            resources.ApplyResources(labelArchitettura, "labelArchitettura");
            labelArchitettura.ForeColor = Color.Coral;
            labelArchitettura.Name = "labelArchitettura";
            // 
            // richTextBoxDescription
            // 
            richTextBoxDescription.BackColor = Color.FromArgb(37, 38, 39);
            richTextBoxDescription.BorderStyle = BorderStyle.None;
            richTextBoxDescription.ForeColor = Color.White;
            resources.ApplyResources(richTextBoxDescription, "richTextBoxDescription");
            richTextBoxDescription.Name = "richTextBoxDescription";
            // 
            // btnDownload
            // 
            resources.ApplyResources(btnDownload, "btnDownload");
            btnDownload.Cursor = Cursors.Hand;
            btnDownload.FlatAppearance.BorderSize = 0;
            btnDownload.ForeColor = Color.White;
            btnDownload.Image = Properties.Resources.pngDownloadWindows;
            btnDownload.Name = "btnDownload";
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += buttonDownload_Click;
            // 
            // richTextBoxInfo
            // 
            richTextBoxInfo.BackColor = Color.FromArgb(37, 38, 39);
            richTextBoxInfo.BorderStyle = BorderStyle.None;
            richTextBoxInfo.ForeColor = Color.White;
            resources.ApplyResources(richTextBoxInfo, "richTextBoxInfo");
            richTextBoxInfo.Name = "richTextBoxInfo";
            // 
            // pictureBox4
            // 
            resources.ApplyResources(pictureBox4, "pictureBox4");
            pictureBox4.Name = "pictureBox4";
            pictureBox4.TabStop = false;
            // 
            // comboBox_SelezionaLingua
            // 
            comboBox_SelezionaLingua.Cursor = Cursors.Hand;
            comboBox_SelezionaLingua.DropDownStyle = ComboBoxStyle.DropDownList;
            resources.ApplyResources(comboBox_SelezionaLingua, "comboBox_SelezionaLingua");
            comboBox_SelezionaLingua.FormattingEnabled = true;
            comboBox_SelezionaLingua.Items.AddRange(new object[] { resources.GetString("comboBox_SelezionaLingua.Items"), resources.GetString("comboBox_SelezionaLingua.Items1") });
            comboBox_SelezionaLingua.Name = "comboBox_SelezionaLingua";
            comboBox_SelezionaLingua.SelectedIndexChanged += comboBox_SelezionaLingua_SelectedIndexChanged;
            // 
            // lblSelezionaLingua
            // 
            resources.ApplyResources(lblSelezionaLingua, "lblSelezionaLingua");
            lblSelezionaLingua.ForeColor = Color.Coral;
            lblSelezionaLingua.Name = "lblSelezionaLingua";
            // 
            // FormWin
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(comboBox_SelezionaLingua);
            Controls.Add(lblSelezionaLingua);
            Controls.Add(pictureBox4);
            Controls.Add(richTextBoxInfo);
            Controls.Add(btnDownload);
            Controls.Add(richTextBoxDescription);
            Controls.Add(comboBoxArchitettura);
            Controls.Add(labelArchitettura);
            Controls.Add(comboBoxVersione);
            Controls.Add(labelVersione);
            Controls.Add(label5);
            Controls.Add(comboBox1);
            Controls.Add(btnCambioEdizione);
            Controls.Add(btnAttivaWin);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormWin";
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnAttivaWin;
        private Button btnCambioEdizione;
        private ComboBox comboBox1;
        private Label label5;
        private Label labelVersione;
        private ComboBox comboBoxVersione;
        private ComboBox comboBoxArchitettura;
        private Label labelArchitettura;
        private RichTextBox richTextBoxDescription;
        private Button btnDownload;
        private RichTextBox richTextBoxInfo;
        private PictureBox pictureBox4;
        private ComboBox comboBox_SelezionaLingua;
        private Label lblSelezionaLingua;
    }
}