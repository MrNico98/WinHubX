namespace WinHubX.Forms.Settaggi
{
    partial class FormUpdate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUpdate));
            DisabilitaUpdate = new CheckedListBox();
            AbilitaUpdate = new CheckedListBox();
            label2 = new Label();
            label1 = new Label();
            toolTip1 = new ToolTip(components);
            cuiPanel2 = new CuoreUI.Controls.cuiPanel();
            pictureBoxPowerPoint = new PictureBox();
            btnSuggeritiVerdi = new CuoreUI.Controls.cuiButton();
            label3 = new Label();
            label5 = new Label();
            btnRipristinaWinUpdateVerdi = new CuoreUI.Controls.cuiButton();
            btnUpdateEssenzialeVerdi = new CuoreUI.Controls.cuiButton();
            btnAvviaSelezionatiVerdi = new CuoreUI.Controls.cuiButton();
            progressBar2 = new CuoreUI.Controls.cuiProgressBarHorizontal();
            btnResetVerdi = new CuoreUI.Controls.cuiButton();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            cuiPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPowerPoint).BeginInit();
            SuspendLayout();
            // 
            // DisabilitaUpdate
            // 
            resources.ApplyResources(DisabilitaUpdate, "DisabilitaUpdate");
            DisabilitaUpdate.BackColor = Color.FromArgb(37, 38, 39);
            DisabilitaUpdate.BorderStyle = BorderStyle.None;
            DisabilitaUpdate.Cursor = Cursors.Hand;
            DisabilitaUpdate.ForeColor = Color.White;
            DisabilitaUpdate.FormattingEnabled = true;
            DisabilitaUpdate.Items.AddRange(new object[] { resources.GetString("DisabilitaUpdate.Items"), resources.GetString("DisabilitaUpdate.Items1"), resources.GetString("DisabilitaUpdate.Items2"), resources.GetString("DisabilitaUpdate.Items3"), resources.GetString("DisabilitaUpdate.Items4") });
            DisabilitaUpdate.Name = "DisabilitaUpdate";
            DisabilitaUpdate.ItemCheck += DisabilitaUpdate_ItemCheck;
            DisabilitaUpdate.MouseDown += DisabilitaUpdate_MouseDown;
            // 
            // AbilitaUpdate
            // 
            resources.ApplyResources(AbilitaUpdate, "AbilitaUpdate");
            AbilitaUpdate.BackColor = Color.FromArgb(37, 38, 39);
            AbilitaUpdate.BorderStyle = BorderStyle.None;
            AbilitaUpdate.Cursor = Cursors.Hand;
            AbilitaUpdate.ForeColor = Color.White;
            AbilitaUpdate.FormattingEnabled = true;
            AbilitaUpdate.Items.AddRange(new object[] { resources.GetString("AbilitaUpdate.Items"), resources.GetString("AbilitaUpdate.Items1"), resources.GetString("AbilitaUpdate.Items2"), resources.GetString("AbilitaUpdate.Items3"), resources.GetString("AbilitaUpdate.Items4") });
            AbilitaUpdate.Name = "AbilitaUpdate";
            AbilitaUpdate.ItemCheck += AbilitaUpdate_ItemCheck;
            AbilitaUpdate.MouseDown += AbilitaUpdate_MouseDown;
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.ForeColor = Color.FromArgb(0, 126, 249);
            label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.FromArgb(0, 126, 249);
            label1.Name = "label1";
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
            // btnRipristinaWinUpdateVerdi
            // 
            resources.ApplyResources(btnRipristinaWinUpdateVerdi, "btnRipristinaWinUpdateVerdi");
            btnRipristinaWinUpdateVerdi.CheckButton = false;
            btnRipristinaWinUpdateVerdi.Checked = false;
            btnRipristinaWinUpdateVerdi.CheckedBackground = Color.FromArgb(0, 126, 249);
            btnRipristinaWinUpdateVerdi.CheckedForeColor = Color.FromArgb(0, 126, 249);
            btnRipristinaWinUpdateVerdi.CheckedImageTint = Color.FromArgb(0, 126, 249);
            btnRipristinaWinUpdateVerdi.CheckedOutline = Color.FromArgb(0, 126, 249);
            btnRipristinaWinUpdateVerdi.Content = "  Ripristina";
            btnRipristinaWinUpdateVerdi.DialogResult = DialogResult.None;
            btnRipristinaWinUpdateVerdi.ForeColor = Color.White;
            btnRipristinaWinUpdateVerdi.HoverBackground = Color.FromArgb(0, 126, 249);
            btnRipristinaWinUpdateVerdi.HoverForeColor = Color.White;
            btnRipristinaWinUpdateVerdi.HoverImageTint = Color.White;
            btnRipristinaWinUpdateVerdi.HoverOutline = Color.FromArgb(0, 126, 249);
            btnRipristinaWinUpdateVerdi.Image = Properties.Resources.pngRipristinaDefenderTweaks;
            btnRipristinaWinUpdateVerdi.ImageAutoCenter = true;
            btnRipristinaWinUpdateVerdi.ImageExpand = new Point(0, 0);
            btnRipristinaWinUpdateVerdi.ImageOffset = new Point(0, 0);
            btnRipristinaWinUpdateVerdi.Name = "btnRipristinaWinUpdateVerdi";
            btnRipristinaWinUpdateVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btnRipristinaWinUpdateVerdi.NormalForeColor = Color.White;
            btnRipristinaWinUpdateVerdi.NormalImageTint = Color.White;
            btnRipristinaWinUpdateVerdi.NormalOutline = Color.FromArgb(0, 126, 249);
            btnRipristinaWinUpdateVerdi.OutlineThickness = 1F;
            btnRipristinaWinUpdateVerdi.PressedBackground = Color.FromArgb(0, 126, 249);
            btnRipristinaWinUpdateVerdi.PressedForeColor = Color.Black;
            btnRipristinaWinUpdateVerdi.PressedImageTint = Color.Black;
            btnRipristinaWinUpdateVerdi.PressedOutline = Color.FromArgb(0, 126, 249);
            btnRipristinaWinUpdateVerdi.Rounding = new Padding(8);
            btnRipristinaWinUpdateVerdi.TextAlignment = StringAlignment.Center;
            btnRipristinaWinUpdateVerdi.TextOffset = new Point(0, 0);
            btnRipristinaWinUpdateVerdi.Click += btnResetUpdate_Click;
            // 
            // btnUpdateEssenzialeVerdi
            // 
            resources.ApplyResources(btnUpdateEssenzialeVerdi, "btnUpdateEssenzialeVerdi");
            btnUpdateEssenzialeVerdi.CheckButton = false;
            btnUpdateEssenzialeVerdi.Checked = false;
            btnUpdateEssenzialeVerdi.CheckedBackground = Color.FromArgb(192, 0, 0);
            btnUpdateEssenzialeVerdi.CheckedForeColor = Color.FromArgb(192, 0, 0);
            btnUpdateEssenzialeVerdi.CheckedImageTint = Color.FromArgb(192, 0, 0);
            btnUpdateEssenzialeVerdi.CheckedOutline = Color.FromArgb(192, 0, 0);
            btnUpdateEssenzialeVerdi.Content = "  Update essenziale";
            btnUpdateEssenzialeVerdi.DialogResult = DialogResult.None;
            btnUpdateEssenzialeVerdi.ForeColor = Color.White;
            btnUpdateEssenzialeVerdi.HoverBackground = Color.FromArgb(192, 0, 0);
            btnUpdateEssenzialeVerdi.HoverForeColor = Color.White;
            btnUpdateEssenzialeVerdi.HoverImageTint = Color.White;
            btnUpdateEssenzialeVerdi.HoverOutline = Color.FromArgb(192, 0, 0);
            btnUpdateEssenzialeVerdi.Image = Properties.Resources.pngProtezioneMinimaDefender;
            btnUpdateEssenzialeVerdi.ImageAutoCenter = true;
            btnUpdateEssenzialeVerdi.ImageExpand = new Point(0, 0);
            btnUpdateEssenzialeVerdi.ImageOffset = new Point(0, 0);
            btnUpdateEssenzialeVerdi.Name = "btnUpdateEssenzialeVerdi";
            btnUpdateEssenzialeVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btnUpdateEssenzialeVerdi.NormalForeColor = Color.White;
            btnUpdateEssenzialeVerdi.NormalImageTint = Color.White;
            btnUpdateEssenzialeVerdi.NormalOutline = Color.FromArgb(192, 0, 0);
            btnUpdateEssenzialeVerdi.OutlineThickness = 1F;
            btnUpdateEssenzialeVerdi.PressedBackground = Color.FromArgb(192, 0, 0);
            btnUpdateEssenzialeVerdi.PressedForeColor = Color.Black;
            btnUpdateEssenzialeVerdi.PressedImageTint = Color.Black;
            btnUpdateEssenzialeVerdi.PressedOutline = Color.FromArgb(192, 0, 0);
            btnUpdateEssenzialeVerdi.Rounding = new Padding(8);
            btnUpdateEssenzialeVerdi.TextAlignment = StringAlignment.Center;
            btnUpdateEssenzialeVerdi.TextOffset = new Point(0, 0);
            btnUpdateEssenzialeVerdi.Click += btnUpdateEssential_Click;
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
            btnAvviaSelezionatiVerdi.Click += btnAvviaSelezionatiUpda_Click;
            // 
            // progressBar2
            // 
            resources.ApplyResources(progressBar2, "progressBar2");
            progressBar2.Background = Color.FromArgb(64, 128, 128, 128);
            progressBar2.Flipped = false;
            progressBar2.Foreground = Color.FromArgb(46, 125, 60);
            progressBar2.MaxValue = 100;
            progressBar2.Name = "progressBar2";
            progressBar2.Rounding = 8;
            progressBar2.Value = 0;
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
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // FormUpdate
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(btnResetVerdi);
            Controls.Add(cuiPanel2);
            Controls.Add(btnRipristinaWinUpdateVerdi);
            Controls.Add(btnUpdateEssenzialeVerdi);
            Controls.Add(btnAvviaSelezionatiVerdi);
            Controls.Add(progressBar2);
            Controls.Add(label1);
            Controls.Add(label2);
            Controls.Add(AbilitaUpdate);
            Controls.Add(DisabilitaUpdate);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormUpdate";
            cuiPanel2.ResumeLayout(false);
            cuiPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPowerPoint).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private CheckedListBox DisabilitaUpdate;
        private CheckedListBox AbilitaUpdate;
        private Label label2;
        private Label label1;
        private ProgressBar progressBar1;
        private ToolTip toolTip1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private CuoreUI.Controls.cuiButton btnRipristinaWinUpdateVerdi;
        private CuoreUI.Controls.cuiButton btnUpdateEssenzialeVerdi;
        private CuoreUI.Controls.cuiButton btnAvviaSelezionatiVerdi;
        private CuoreUI.Controls.cuiProgressBarHorizontal progressBar2;
        private CuoreUI.Controls.cuiPanel cuiPanel2;
        private PictureBox pictureBoxPowerPoint;
        private CuoreUI.Controls.cuiButton btnSuggeritiVerdi;
        private Label label3;
        private Label label5;
        private CuoreUI.Controls.cuiButton btnResetVerdi;
    }
}