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
            DisabilitaDefender = new CheckedListBox();
            AbilitaDefender = new CheckedListBox();
            label1 = new Label();
            label2 = new Label();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            toolTip1 = new ToolTip(components);
            cuiPanel2 = new CuoreUI.Controls.cuiPanel();
            pictureBoxPowerPoint = new PictureBox();
            btnSuggeritiVerdi = new CuoreUI.Controls.cuiButton();
            label3 = new Label();
            label5 = new Label();
            btnAvviaSelezionatiVerdi = new CuoreUI.Controls.cuiButton();
            progressBar1 = new CuoreUI.Controls.cuiProgressBarHorizontal();
            btnProtezioneMinimaVerdi = new CuoreUI.Controls.cuiButton();
            btnRiprisitinoDefenderVerdi = new CuoreUI.Controls.cuiButton();
            btnResetVerdi = new CuoreUI.Controls.cuiButton();
            cuiPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPowerPoint).BeginInit();
            SuspendLayout();
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
            DisabilitaDefender.ItemCheck += DisabilitaDefender_ItemCheck;
            DisabilitaDefender.MouseDown += DisabilitaDefender_MouseDown;
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
            AbilitaDefender.ItemCheck += AbilitaDefender_ItemCheck;
            AbilitaDefender.MouseDown += AbilitaDefender_MouseDown;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.FromArgb(0, 126, 249);
            label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.ForeColor = Color.FromArgb(0, 126, 249);
            label2.Name = "label2";
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
            cuiPanel2.Controls.Add(label5);
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
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.ForeColor = Color.White;
            label5.Name = "label5";
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
            btnAvviaSelezionatiVerdi.Click += btnAvviaSelezionatiDef_Click;
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
            // btnProtezioneMinimaVerdi
            // 
            resources.ApplyResources(btnProtezioneMinimaVerdi, "btnProtezioneMinimaVerdi");
            btnProtezioneMinimaVerdi.CheckButton = false;
            btnProtezioneMinimaVerdi.Checked = false;
            btnProtezioneMinimaVerdi.CheckedBackground = Color.FromArgb(192, 0, 0);
            btnProtezioneMinimaVerdi.CheckedForeColor = Color.FromArgb(192, 0, 0);
            btnProtezioneMinimaVerdi.CheckedImageTint = Color.FromArgb(192, 0, 0);
            btnProtezioneMinimaVerdi.CheckedOutline = Color.FromArgb(192, 0, 0);
            btnProtezioneMinimaVerdi.Content = "Protezione minima";
            btnProtezioneMinimaVerdi.DialogResult = DialogResult.None;
            btnProtezioneMinimaVerdi.ForeColor = Color.White;
            btnProtezioneMinimaVerdi.HoverBackground = Color.FromArgb(192, 0, 0);
            btnProtezioneMinimaVerdi.HoverForeColor = Color.White;
            btnProtezioneMinimaVerdi.HoverImageTint = Color.White;
            btnProtezioneMinimaVerdi.HoverOutline = Color.FromArgb(192, 0, 0);
            btnProtezioneMinimaVerdi.Image = Properties.Resources.pngProtezioneMinimaDefender;
            btnProtezioneMinimaVerdi.ImageAutoCenter = true;
            btnProtezioneMinimaVerdi.ImageExpand = new Point(0, 0);
            btnProtezioneMinimaVerdi.ImageOffset = new Point(0, 0);
            btnProtezioneMinimaVerdi.Name = "btnProtezioneMinimaVerdi";
            btnProtezioneMinimaVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btnProtezioneMinimaVerdi.NormalForeColor = Color.White;
            btnProtezioneMinimaVerdi.NormalImageTint = Color.White;
            btnProtezioneMinimaVerdi.NormalOutline = Color.FromArgb(192, 0, 0);
            btnProtezioneMinimaVerdi.OutlineThickness = 1F;
            btnProtezioneMinimaVerdi.PressedBackground = Color.FromArgb(192, 0, 0);
            btnProtezioneMinimaVerdi.PressedForeColor = Color.Black;
            btnProtezioneMinimaVerdi.PressedImageTint = Color.Black;
            btnProtezioneMinimaVerdi.PressedOutline = Color.FromArgb(192, 0, 0);
            btnProtezioneMinimaVerdi.Rounding = new Padding(8);
            btnProtezioneMinimaVerdi.TextAlignment = StringAlignment.Center;
            btnProtezioneMinimaVerdi.TextOffset = new Point(0, 0);
            btnProtezioneMinimaVerdi.Click += btnProtezioneMinima_Click;
            // 
            // btnRiprisitinoDefenderVerdi
            // 
            resources.ApplyResources(btnRiprisitinoDefenderVerdi, "btnRiprisitinoDefenderVerdi");
            btnRiprisitinoDefenderVerdi.CheckButton = false;
            btnRiprisitinoDefenderVerdi.Checked = false;
            btnRiprisitinoDefenderVerdi.CheckedBackground = Color.FromArgb(0, 126, 249);
            btnRiprisitinoDefenderVerdi.CheckedForeColor = Color.FromArgb(0, 126, 249);
            btnRiprisitinoDefenderVerdi.CheckedImageTint = Color.FromArgb(0, 126, 249);
            btnRiprisitinoDefenderVerdi.CheckedOutline = Color.FromArgb(0, 126, 249);
            btnRiprisitinoDefenderVerdi.Content = "  Rirpristina";
            btnRiprisitinoDefenderVerdi.DialogResult = DialogResult.None;
            btnRiprisitinoDefenderVerdi.ForeColor = Color.White;
            btnRiprisitinoDefenderVerdi.HoverBackground = Color.FromArgb(0, 126, 249);
            btnRiprisitinoDefenderVerdi.HoverForeColor = Color.White;
            btnRiprisitinoDefenderVerdi.HoverImageTint = Color.White;
            btnRiprisitinoDefenderVerdi.HoverOutline = Color.FromArgb(0, 126, 249);
            btnRiprisitinoDefenderVerdi.Image = Properties.Resources.pngRipristinaDefenderTweaks;
            btnRiprisitinoDefenderVerdi.ImageAutoCenter = true;
            btnRiprisitinoDefenderVerdi.ImageExpand = new Point(0, 0);
            btnRiprisitinoDefenderVerdi.ImageOffset = new Point(0, 0);
            btnRiprisitinoDefenderVerdi.Name = "btnRiprisitinoDefenderVerdi";
            btnRiprisitinoDefenderVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btnRiprisitinoDefenderVerdi.NormalForeColor = Color.White;
            btnRiprisitinoDefenderVerdi.NormalImageTint = Color.White;
            btnRiprisitinoDefenderVerdi.NormalOutline = Color.FromArgb(0, 126, 249);
            btnRiprisitinoDefenderVerdi.OutlineThickness = 1F;
            btnRiprisitinoDefenderVerdi.PressedBackground = Color.FromArgb(0, 126, 249);
            btnRiprisitinoDefenderVerdi.PressedForeColor = Color.Black;
            btnRiprisitinoDefenderVerdi.PressedImageTint = Color.Black;
            btnRiprisitinoDefenderVerdi.PressedOutline = Color.FromArgb(0, 126, 249);
            btnRiprisitinoDefenderVerdi.Rounding = new Padding(8);
            btnRiprisitinoDefenderVerdi.TextAlignment = StringAlignment.Center;
            btnRiprisitinoDefenderVerdi.TextOffset = new Point(0, 0);
            btnRiprisitinoDefenderVerdi.Click += btnRipristinaDefender_Click;
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
            // FormDefender
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(btnResetVerdi);
            Controls.Add(btnRiprisitinoDefenderVerdi);
            Controls.Add(btnProtezioneMinimaVerdi);
            Controls.Add(cuiPanel2);
            Controls.Add(btnAvviaSelezionatiVerdi);
            Controls.Add(progressBar1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(AbilitaDefender);
            Controls.Add(DisabilitaDefender);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormDefender";
            cuiPanel2.ResumeLayout(false);
            cuiPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPowerPoint).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private CheckedListBox DisabilitaDefender;
        private CheckedListBox AbilitaDefender;
        private Label label1;
        private Label label2;
        private Label label4;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private ToolTip toolTip1;
        private CuoreUI.Controls.cuiButton btnAvviaSelezionatiVerdi;
        private CuoreUI.Controls.cuiProgressBarHorizontal progressBar1;
        private CuoreUI.Controls.cuiPanel cuiPanel2;
        private PictureBox pictureBoxPowerPoint;
        private CuoreUI.Controls.cuiButton btnSuggeritiVerdi;
        private Label label3;
        private Label label5;
        private CuoreUI.Controls.cuiButton btnProtezioneMinimaVerdi;
        private CuoreUI.Controls.cuiButton btnRiprisitinoDefenderVerdi;
        private CuoreUI.Controls.cuiButton btnResetVerdi;
    }
}