namespace WinHubX.Forms.Settaggi
{
    partial class FormDefender
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDefender));
            btnBack = new Button();
            DisabilitaDefender = new CheckedListBox();
            btnAvviaSelezionatiDef = new Button();
            btnProtezioneMinima = new Button();
            btnRipristinaDefender = new Button();
            AbilitaDefender = new CheckedListBox();
            label1 = new Label();
            label2 = new Label();
            label4 = new Label();
            progressBar1 = new ProgressBar();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            toolTip1 = new ToolTip(components);
            SuspendLayout();
            // 
            // btnBack
            // 
            btnBack.Cursor = Cursors.Hand;
            btnBack.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(btnBack, "btnBack");
            btnBack.Image = Properties.Resources.pngBackArrow;
            btnBack.Name = "btnBack";
            btnBack.UseMnemonic = false;
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            // 
            // DisabilitaDefender
            // 
            resources.ApplyResources(DisabilitaDefender, "DisabilitaDefender");
            DisabilitaDefender.BackColor = Color.FromArgb(37, 38, 39);
            DisabilitaDefender.BorderStyle = BorderStyle.None;
            DisabilitaDefender.Cursor = Cursors.Hand;
            DisabilitaDefender.ForeColor = Color.White;
            DisabilitaDefender.FormattingEnabled = true;
            DisabilitaDefender.Items.AddRange(new object[] { resources.GetString("DisabilitaDefender.Items"), resources.GetString("DisabilitaDefender.Items1"), resources.GetString("DisabilitaDefender.Items2"), resources.GetString("DisabilitaDefender.Items3"), resources.GetString("DisabilitaDefender.Items4"), resources.GetString("DisabilitaDefender.Items5"), resources.GetString("DisabilitaDefender.Items6"), resources.GetString("DisabilitaDefender.Items7"), resources.GetString("DisabilitaDefender.Items8"), resources.GetString("DisabilitaDefender.Items9"), resources.GetString("DisabilitaDefender.Items10"), resources.GetString("DisabilitaDefender.Items11"), resources.GetString("DisabilitaDefender.Items12") });
            DisabilitaDefender.Name = "DisabilitaDefender";
            // 
            // btnAvviaSelezionatiDef
            // 
            resources.ApplyResources(btnAvviaSelezionatiDef, "btnAvviaSelezionatiDef");
            btnAvviaSelezionatiDef.Cursor = Cursors.Hand;
            btnAvviaSelezionatiDef.FlatAppearance.BorderSize = 0;
            btnAvviaSelezionatiDef.ForeColor = Color.White;
            btnAvviaSelezionatiDef.Name = "btnAvviaSelezionatiDef";
            btnAvviaSelezionatiDef.UseVisualStyleBackColor = true;
            btnAvviaSelezionatiDef.Click += btnAvviaSelezionatiDef_Click;
            // 
            // btnProtezioneMinima
            // 
            resources.ApplyResources(btnProtezioneMinima, "btnProtezioneMinima");
            btnProtezioneMinima.Cursor = Cursors.Hand;
            btnProtezioneMinima.FlatAppearance.BorderSize = 0;
            btnProtezioneMinima.ForeColor = Color.White;
            btnProtezioneMinima.Name = "btnProtezioneMinima";
            btnProtezioneMinima.UseVisualStyleBackColor = true;
            btnProtezioneMinima.Click += btnProtezioneMinima_Click;
            // 
            // btnRipristinaDefender
            // 
            resources.ApplyResources(btnRipristinaDefender, "btnRipristinaDefender");
            btnRipristinaDefender.Cursor = Cursors.Hand;
            btnRipristinaDefender.FlatAppearance.BorderSize = 0;
            btnRipristinaDefender.ForeColor = Color.White;
            btnRipristinaDefender.Name = "btnRipristinaDefender";
            btnRipristinaDefender.UseVisualStyleBackColor = true;
            btnRipristinaDefender.Click += btnRipristinaDefender_Click;
            // 
            // AbilitaDefender
            // 
            resources.ApplyResources(AbilitaDefender, "AbilitaDefender");
            AbilitaDefender.BackColor = Color.FromArgb(37, 38, 39);
            AbilitaDefender.BorderStyle = BorderStyle.None;
            AbilitaDefender.Cursor = Cursors.Hand;
            AbilitaDefender.ForeColor = Color.White;
            AbilitaDefender.FormattingEnabled = true;
            AbilitaDefender.Items.AddRange(new object[] { resources.GetString("AbilitaDefender.Items"), resources.GetString("AbilitaDefender.Items1"), resources.GetString("AbilitaDefender.Items2"), resources.GetString("AbilitaDefender.Items3"), resources.GetString("AbilitaDefender.Items4"), resources.GetString("AbilitaDefender.Items5"), resources.GetString("AbilitaDefender.Items6"), resources.GetString("AbilitaDefender.Items7"), resources.GetString("AbilitaDefender.Items8"), resources.GetString("AbilitaDefender.Items9"), resources.GetString("AbilitaDefender.Items10"), resources.GetString("AbilitaDefender.Items11"), resources.GetString("AbilitaDefender.Items12") });
            AbilitaDefender.Name = "AbilitaDefender";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.White;
            label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.ForeColor = Color.White;
            label2.Name = "label2";
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.ForeColor = Color.Coral;
            label4.Name = "label4";
            // 
            // progressBar1
            // 
            resources.ApplyResources(progressBar1, "progressBar1");
            progressBar1.Name = "progressBar1";
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // FormDefender
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(label4);
            Controls.Add(progressBar1);
            Controls.Add(btnProtezioneMinima);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(AbilitaDefender);
            Controls.Add(btnRipristinaDefender);
            Controls.Add(btnAvviaSelezionatiDef);
            Controls.Add(DisabilitaDefender);
            Controls.Add(btnBack);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormDefender";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnBack;
        private CheckedListBox DisabilitaDefender;
        private Button btnAvviaSelezionatiDef;
        private Button btnProtezioneMinima;
        private Button btnRipristinaDefender;
        private CheckedListBox AbilitaDefender;
        private Label label1;
        private Label label2;
        private Label label4;
        private ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private ToolTip toolTip1;
    }
}