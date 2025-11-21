namespace WinHubX.Forms.Settaggi
{
    partial class FormPrivacy
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrivacy));
            DisabilitaPrivacy = new CheckedListBox();
            lblWin7Lite = new Label();
            label1 = new Label();
            AbilitaPrivacy = new CheckedListBox();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            toolTip1 = new ToolTip(components);
            cuiPanel2 = new CuoreUI.Controls.cuiPanel();
            pictureBoxPowerPoint = new PictureBox();
            btnSuggeritiVerdi = new CuoreUI.Controls.cuiButton();
            label3 = new Label();
            label2 = new Label();
            btnAvviaSelezionatiVerdi = new CuoreUI.Controls.cuiButton();
            progressBar1 = new CuoreUI.Controls.cuiProgressBarHorizontal();
            btnResetVerdi = new CuoreUI.Controls.cuiButton();
            cuiPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPowerPoint).BeginInit();
            SuspendLayout();
            // 
            // DisabilitaPrivacy
            // 
            resources.ApplyResources(DisabilitaPrivacy, "DisabilitaPrivacy");
            DisabilitaPrivacy.BackColor = Color.FromArgb(37, 38, 39);
            DisabilitaPrivacy.BorderStyle = BorderStyle.None;
            DisabilitaPrivacy.Cursor = Cursors.Hand;
            DisabilitaPrivacy.ForeColor = Color.White;
            DisabilitaPrivacy.FormattingEnabled = true;
            DisabilitaPrivacy.Items.AddRange(new object[] { resources.GetString("DisabilitaPrivacy.Items"), resources.GetString("DisabilitaPrivacy.Items1"), resources.GetString("DisabilitaPrivacy.Items2"), resources.GetString("DisabilitaPrivacy.Items3"), resources.GetString("DisabilitaPrivacy.Items4"), resources.GetString("DisabilitaPrivacy.Items5"), resources.GetString("DisabilitaPrivacy.Items6"), resources.GetString("DisabilitaPrivacy.Items7"), resources.GetString("DisabilitaPrivacy.Items8"), resources.GetString("DisabilitaPrivacy.Items9"), resources.GetString("DisabilitaPrivacy.Items10"), resources.GetString("DisabilitaPrivacy.Items11"), resources.GetString("DisabilitaPrivacy.Items12"), resources.GetString("DisabilitaPrivacy.Items13"), resources.GetString("DisabilitaPrivacy.Items14"), resources.GetString("DisabilitaPrivacy.Items15"), resources.GetString("DisabilitaPrivacy.Items16") });
            DisabilitaPrivacy.Name = "DisabilitaPrivacy";
            DisabilitaPrivacy.ItemCheck += DisabilitaPrivacy_ItemCheck;
            DisabilitaPrivacy.MouseDown += DisabilitaPrivacy_MouseDown;
            // 
            // lblWin7Lite
            // 
            resources.ApplyResources(lblWin7Lite, "lblWin7Lite");
            lblWin7Lite.ForeColor = Color.FromArgb(0, 126, 249);
            lblWin7Lite.Name = "lblWin7Lite";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.FromArgb(0, 126, 249);
            label1.Name = "label1";
            // 
            // AbilitaPrivacy
            // 
            resources.ApplyResources(AbilitaPrivacy, "AbilitaPrivacy");
            AbilitaPrivacy.BackColor = Color.FromArgb(37, 38, 39);
            AbilitaPrivacy.BorderStyle = BorderStyle.None;
            AbilitaPrivacy.Cursor = Cursors.Hand;
            AbilitaPrivacy.ForeColor = Color.White;
            AbilitaPrivacy.FormattingEnabled = true;
            AbilitaPrivacy.Items.AddRange(new object[] { resources.GetString("AbilitaPrivacy.Items"), resources.GetString("AbilitaPrivacy.Items1"), resources.GetString("AbilitaPrivacy.Items2"), resources.GetString("AbilitaPrivacy.Items3"), resources.GetString("AbilitaPrivacy.Items4"), resources.GetString("AbilitaPrivacy.Items5"), resources.GetString("AbilitaPrivacy.Items6"), resources.GetString("AbilitaPrivacy.Items7"), resources.GetString("AbilitaPrivacy.Items8"), resources.GetString("AbilitaPrivacy.Items9"), resources.GetString("AbilitaPrivacy.Items10"), resources.GetString("AbilitaPrivacy.Items11"), resources.GetString("AbilitaPrivacy.Items12"), resources.GetString("AbilitaPrivacy.Items13"), resources.GetString("AbilitaPrivacy.Items14"), resources.GetString("AbilitaPrivacy.Items15"), resources.GetString("AbilitaPrivacy.Items16") });
            AbilitaPrivacy.Name = "AbilitaPrivacy";
            AbilitaPrivacy.ItemCheck += AbilitaPrivacy_ItemCheck;
            AbilitaPrivacy.MouseDown += AbilitaPrivacy_MouseDown;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // cuiPanel2
            // 
            resources.ApplyResources(cuiPanel2, "cuiPanel2");
            cuiPanel2.Controls.Add(pictureBoxPowerPoint);
            cuiPanel2.Controls.Add(btnSuggeritiVerdi);
            cuiPanel2.Controls.Add(label3);
            cuiPanel2.Controls.Add(label2);
            cuiPanel2.Name = "cuiPanel2";
            cuiPanel2.OutlineThickness = 1F;
            cuiPanel2.PanelColor = Color.FromArgb(37, 38, 39);
            cuiPanel2.PanelOutlineColor = Color.WhiteSmoke;
            cuiPanel2.Rounding = new Padding(8);
            toolTip1.SetToolTip(cuiPanel2, resources.GetString("cuiPanel2.ToolTip"));
            // 
            // pictureBoxPowerPoint
            // 
            pictureBoxPowerPoint.Cursor = Cursors.Hand;
            pictureBoxPowerPoint.Image = Properties.Resources.pngSuggeritiTweaks;
            resources.ApplyResources(pictureBoxPowerPoint, "pictureBoxPowerPoint");
            pictureBoxPowerPoint.Name = "pictureBoxPowerPoint";
            pictureBoxPowerPoint.TabStop = false;
            // 
            // btnSuggeritiVerdi
            // 
            resources.ApplyResources(btnSuggeritiVerdi, "btnSuggeritiVerdi");
            btnSuggeritiVerdi.CheckButton = false;
            btnSuggeritiVerdi.Checked = false;
            btnSuggeritiVerdi.CheckedBackground = Color.FromArgb(46, 125, 60);
            btnSuggeritiVerdi.CheckedForeColor = Color.FromArgb(46, 125, 60);
            btnSuggeritiVerdi.CheckedImageTint = Color.FromArgb(46, 125, 60);
            btnSuggeritiVerdi.CheckedOutline = Color.FromArgb(46, 125, 60);
            btnSuggeritiVerdi.Content = " Applica";
            btnSuggeritiVerdi.DialogResult = DialogResult.None;
            btnSuggeritiVerdi.ForeColor = Color.White;
            btnSuggeritiVerdi.HoverBackground = Color.FromArgb(46, 125, 50);
            btnSuggeritiVerdi.HoverForeColor = Color.White;
            btnSuggeritiVerdi.HoverImageTint = Color.White;
            btnSuggeritiVerdi.HoverOutline = Color.FromArgb(46, 125, 50);
            btnSuggeritiVerdi.Image = Properties.Resources.pngCheckCreaISO;
            btnSuggeritiVerdi.ImageAutoCenter = true;
            btnSuggeritiVerdi.ImageExpand = new Point(0, 0);
            btnSuggeritiVerdi.ImageOffset = new Point(0, 0);
            btnSuggeritiVerdi.Name = "btnSuggeritiVerdi";
            btnSuggeritiVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btnSuggeritiVerdi.NormalForeColor = Color.White;
            btnSuggeritiVerdi.NormalImageTint = Color.White;
            btnSuggeritiVerdi.NormalOutline = Color.FromArgb(46, 125, 50);
            btnSuggeritiVerdi.OutlineThickness = 1F;
            btnSuggeritiVerdi.PressedBackground = Color.FromArgb(46, 125, 50);
            btnSuggeritiVerdi.PressedForeColor = Color.Black;
            btnSuggeritiVerdi.PressedImageTint = Color.Black;
            btnSuggeritiVerdi.PressedOutline = Color.FromArgb(46, 125, 50);
            btnSuggeritiVerdi.Rounding = new Padding(8);
            btnSuggeritiVerdi.TextAlignment = StringAlignment.Center;
            btnSuggeritiVerdi.TextOffset = new Point(0, 0);
            btnSuggeritiVerdi.Click += btnSuggeriti_Click;
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = Color.White;
            label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.ForeColor = Color.White;
            label2.Name = "label2";
            // 
            // btnAvviaSelezionatiVerdi
            // 
            resources.ApplyResources(btnAvviaSelezionatiVerdi, "btnAvviaSelezionatiVerdi");
            btnAvviaSelezionatiVerdi.CheckButton = false;
            btnAvviaSelezionatiVerdi.Checked = false;
            btnAvviaSelezionatiVerdi.CheckedBackground = Color.FromArgb(46, 125, 60);
            btnAvviaSelezionatiVerdi.CheckedForeColor = Color.FromArgb(46, 125, 60);
            btnAvviaSelezionatiVerdi.CheckedImageTint = Color.FromArgb(46, 125, 60);
            btnAvviaSelezionatiVerdi.CheckedOutline = Color.FromArgb(46, 125, 60);
            btnAvviaSelezionatiVerdi.Content = "  Avvia";
            btnAvviaSelezionatiVerdi.DialogResult = DialogResult.None;
            btnAvviaSelezionatiVerdi.ForeColor = Color.White;
            btnAvviaSelezionatiVerdi.HoverBackground = Color.FromArgb(46, 125, 50);
            btnAvviaSelezionatiVerdi.HoverForeColor = Color.White;
            btnAvviaSelezionatiVerdi.HoverImageTint = Color.White;
            btnAvviaSelezionatiVerdi.HoverOutline = Color.FromArgb(46, 125, 50);
            btnAvviaSelezionatiVerdi.Image = Properties.Resources.pngCheckCreaISO;
            btnAvviaSelezionatiVerdi.ImageAutoCenter = true;
            btnAvviaSelezionatiVerdi.ImageExpand = new Point(0, 0);
            btnAvviaSelezionatiVerdi.ImageOffset = new Point(0, 0);
            btnAvviaSelezionatiVerdi.Name = "btnAvviaSelezionatiVerdi";
            btnAvviaSelezionatiVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btnAvviaSelezionatiVerdi.NormalForeColor = Color.White;
            btnAvviaSelezionatiVerdi.NormalImageTint = Color.White;
            btnAvviaSelezionatiVerdi.NormalOutline = Color.FromArgb(46, 125, 50);
            btnAvviaSelezionatiVerdi.OutlineThickness = 1F;
            btnAvviaSelezionatiVerdi.PressedBackground = Color.FromArgb(46, 125, 50);
            btnAvviaSelezionatiVerdi.PressedForeColor = Color.Black;
            btnAvviaSelezionatiVerdi.PressedImageTint = Color.Black;
            btnAvviaSelezionatiVerdi.PressedOutline = Color.FromArgb(46, 125, 50);
            btnAvviaSelezionatiVerdi.Rounding = new Padding(8);
            btnAvviaSelezionatiVerdi.TextAlignment = StringAlignment.Center;
            btnAvviaSelezionatiVerdi.TextOffset = new Point(0, 0);
            btnAvviaSelezionatiVerdi.Click += btnAvviaSelezionati_Click;
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
            progressBar1.Value = 0;
            // 
            // btnResetVerdi
            // 
            resources.ApplyResources(btnResetVerdi, "btnResetVerdi");
            btnResetVerdi.CheckButton = false;
            btnResetVerdi.Checked = false;
            btnResetVerdi.CheckedBackground = Color.FromArgb(0, 126, 249);
            btnResetVerdi.CheckedForeColor = Color.FromArgb(0, 126, 249);
            btnResetVerdi.CheckedImageTint = Color.FromArgb(0, 126, 249);
            btnResetVerdi.CheckedOutline = Color.FromArgb(0, 126, 249);
            btnResetVerdi.Content = "  Reset";
            btnResetVerdi.DialogResult = DialogResult.None;
            btnResetVerdi.ForeColor = Color.White;
            btnResetVerdi.HoverBackground = Color.FromArgb(0, 126, 249);
            btnResetVerdi.HoverForeColor = Color.White;
            btnResetVerdi.HoverImageTint = Color.White;
            btnResetVerdi.HoverOutline = Color.FromArgb(0, 126, 249);
            btnResetVerdi.Image = Properties.Resources.pngRipristinaDefenderTweaks;
            btnResetVerdi.ImageAutoCenter = true;
            btnResetVerdi.ImageExpand = new Point(0, 0);
            btnResetVerdi.ImageOffset = new Point(0, 0);
            btnResetVerdi.Name = "btnResetVerdi";
            btnResetVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btnResetVerdi.NormalForeColor = Color.White;
            btnResetVerdi.NormalImageTint = Color.White;
            btnResetVerdi.NormalOutline = Color.FromArgb(0, 126, 249);
            btnResetVerdi.OutlineThickness = 1F;
            btnResetVerdi.PressedBackground = Color.FromArgb(0, 126, 249);
            btnResetVerdi.PressedForeColor = Color.Black;
            btnResetVerdi.PressedImageTint = Color.Black;
            btnResetVerdi.PressedOutline = Color.FromArgb(0, 126, 249);
            btnResetVerdi.Rounding = new Padding(8);
            btnResetVerdi.TextAlignment = StringAlignment.Center;
            btnResetVerdi.TextOffset = new Point(0, 0);
            btnResetVerdi.Click += btnReset_Click;
            // 
            // FormPrivacy
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(btnResetVerdi);
            Controls.Add(cuiPanel2);
            Controls.Add(btnAvviaSelezionatiVerdi);
            Controls.Add(progressBar1);
            Controls.Add(AbilitaPrivacy);
            Controls.Add(label1);
            Controls.Add(lblWin7Lite);
            Controls.Add(DisabilitaPrivacy);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormPrivacy";
            cuiPanel2.ResumeLayout(false);
            cuiPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPowerPoint).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private CheckedListBox DisabilitaPrivacy;
        private Label lblWin7Lite;
        private Label label1;
        private CheckedListBox AbilitaPrivacy;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private ToolTip toolTip1;
        private CuoreUI.Controls.cuiButton btnAvviaSelezionatiVerdi;
        private CuoreUI.Controls.cuiProgressBarHorizontal progressBar1;
        private CuoreUI.Controls.cuiPanel cuiPanel2;
        private CuoreUI.Controls.cuiButton btnSuggeritiVerdi;
        private Label label3;
        private Label label2;
        private PictureBox pictureBoxPowerPoint;
        private CuoreUI.Controls.cuiButton btnResetVerdi;
    }
}