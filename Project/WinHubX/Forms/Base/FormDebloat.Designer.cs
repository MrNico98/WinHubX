namespace WinHubX.Forms.Base
{
    partial class FormDebloat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDebloat));
            flowLayoutPanel1 = new FlowLayoutPanel();
            textBox1 = new TextBox();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            progressBar1 = new CuoreUI.Controls.cuiProgressBarHorizontal();
            btnAvviaSelezionatiVerdi = new CuoreUI.Controls.cuiButton();
            btnModificaServiziDisattivo = new CuoreUI.Controls.cuiButton();
            cuiPanel2 = new CuoreUI.Controls.cuiPanel();
            pictureBoxPowerPoint = new PictureBox();
            btnDebloatAutomaticoVerdi = new CuoreUI.Controls.cuiButton();
            label3 = new Label();
            label5 = new Label();
            cuiPanel1 = new CuoreUI.Controls.cuiPanel();
            label1 = new Label();
            cuiSwitch1 = new CuoreUI.Controls.cuiSwitch();
            btnInstallaComponentiVerdi = new CuoreUI.Controls.cuiButton();
            cuiPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPowerPoint).BeginInit();
            cuiPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // textBox1
            // 
            resources.ApplyResources(textBox1, "textBox1");
            textBox1.BackColor = Color.FromArgb(37, 38, 39);
            textBox1.ForeColor = Color.White;
            textBox1.Name = "textBox1";
            textBox1.TextChanged += textBox1_TextChanged;
            textBox1.KeyDown += textBox1_KeyDown;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
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
            btnAvviaSelezionatiVerdi.Click += btnAvviaSelezionatiDebloat_Click;
            // 
            // btnModificaServiziDisattivo
            // 
            resources.ApplyResources(btnModificaServiziDisattivo, "btnModificaServiziDisattivo");
            btnModificaServiziDisattivo.CheckButton = false;
            btnModificaServiziDisattivo.Checked = false;
            btnModificaServiziDisattivo.CheckedBackground = Color.FromArgb(0, 126, 249);
            btnModificaServiziDisattivo.CheckedForeColor = Color.FromArgb(0, 126, 249);
            btnModificaServiziDisattivo.CheckedImageTint = Color.FromArgb(0, 126, 249);
            btnModificaServiziDisattivo.CheckedOutline = Color.FromArgb(0, 126, 249);
            btnModificaServiziDisattivo.Content = "  Modifica servizi";
            btnModificaServiziDisattivo.DialogResult = DialogResult.None;
            btnModificaServiziDisattivo.ForeColor = Color.Gray;
            btnModificaServiziDisattivo.HoverBackground = Color.FromArgb(0, 126, 249);
            btnModificaServiziDisattivo.HoverForeColor = Color.White;
            btnModificaServiziDisattivo.HoverImageTint = Color.White;
            btnModificaServiziDisattivo.HoverOutline = Color.FromArgb(192, 0, 0);
            btnModificaServiziDisattivo.Image = Properties.Resources.pngModificaServiziDebloat;
            btnModificaServiziDisattivo.ImageAutoCenter = true;
            btnModificaServiziDisattivo.ImageExpand = new Point(0, 0);
            btnModificaServiziDisattivo.ImageOffset = new Point(0, 0);
            btnModificaServiziDisattivo.Name = "btnModificaServiziDisattivo";
            btnModificaServiziDisattivo.NormalBackground = Color.FromArgb(37, 38, 39);
            btnModificaServiziDisattivo.NormalForeColor = Color.Gray;
            btnModificaServiziDisattivo.NormalImageTint = Color.Gray;
            btnModificaServiziDisattivo.NormalOutline = Color.FromArgb(0, 126, 249);
            btnModificaServiziDisattivo.OutlineThickness = 1F;
            btnModificaServiziDisattivo.PressedBackground = Color.FromArgb(0, 126, 249);
            btnModificaServiziDisattivo.PressedForeColor = Color.Black;
            btnModificaServiziDisattivo.PressedImageTint = Color.Black;
            btnModificaServiziDisattivo.PressedOutline = Color.FromArgb(0, 126, 249);
            btnModificaServiziDisattivo.Rounding = new Padding(8);
            btnModificaServiziDisattivo.TextAlignment = StringAlignment.Center;
            btnModificaServiziDisattivo.TextOffset = new Point(0, 0);
            btnModificaServiziDisattivo.Click += btnServizi_Click;
            // 
            // cuiPanel2
            // 
            resources.ApplyResources(cuiPanel2, "cuiPanel2");
            cuiPanel2.Controls.Add(pictureBoxPowerPoint);
            cuiPanel2.Controls.Add(btnDebloatAutomaticoVerdi);
            cuiPanel2.Controls.Add(label3);
            cuiPanel2.Controls.Add(label5);
            cuiPanel2.Name = "cuiPanel2";
            cuiPanel2.OutlineThickness = 1F;
            cuiPanel2.PanelColor = Color.FromArgb(37, 38, 39);
            cuiPanel2.PanelOutlineColor = Color.WhiteSmoke;
            cuiPanel2.Rounding = new Padding(8);
            // 
            // pictureBoxPowerPoint
            // 
            resources.ApplyResources(pictureBoxPowerPoint, "pictureBoxPowerPoint");
            pictureBoxPowerPoint.Cursor = Cursors.Hand;
            pictureBoxPowerPoint.Image = Properties.Resources.pngDebloatAutoaticoDebloat;
            pictureBoxPowerPoint.Name = "pictureBoxPowerPoint";
            pictureBoxPowerPoint.TabStop = false;
            // 
            // btnDebloatAutomaticoVerdi
            // 
            resources.ApplyResources(btnDebloatAutomaticoVerdi, "btnDebloatAutomaticoVerdi");
            btnDebloatAutomaticoVerdi.CheckButton = false;
            btnDebloatAutomaticoVerdi.Checked = false;
            btnDebloatAutomaticoVerdi.CheckedBackground = Color.FromArgb(46, 125, 60);
            btnDebloatAutomaticoVerdi.CheckedForeColor = Color.FromArgb(46, 125, 60);
            btnDebloatAutomaticoVerdi.CheckedImageTint = Color.FromArgb(46, 125, 60);
            btnDebloatAutomaticoVerdi.CheckedOutline = Color.FromArgb(46, 125, 60);
            btnDebloatAutomaticoVerdi.Content = " Avvia";
            btnDebloatAutomaticoVerdi.DialogResult = DialogResult.None;
            btnDebloatAutomaticoVerdi.ForeColor = Color.White;
            btnDebloatAutomaticoVerdi.HoverBackground = Color.FromArgb(46, 125, 50);
            btnDebloatAutomaticoVerdi.HoverForeColor = Color.White;
            btnDebloatAutomaticoVerdi.HoverImageTint = Color.White;
            btnDebloatAutomaticoVerdi.HoverOutline = Color.FromArgb(46, 125, 50);
            btnDebloatAutomaticoVerdi.Image = Properties.Resources.pngCheckCreaISO;
            btnDebloatAutomaticoVerdi.ImageAutoCenter = true;
            btnDebloatAutomaticoVerdi.ImageExpand = new Point(0, 0);
            btnDebloatAutomaticoVerdi.ImageOffset = new Point(0, 0);
            btnDebloatAutomaticoVerdi.Name = "btnDebloatAutomaticoVerdi";
            btnDebloatAutomaticoVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btnDebloatAutomaticoVerdi.NormalForeColor = Color.White;
            btnDebloatAutomaticoVerdi.NormalImageTint = Color.White;
            btnDebloatAutomaticoVerdi.NormalOutline = Color.FromArgb(46, 125, 50);
            btnDebloatAutomaticoVerdi.OutlineThickness = 1F;
            btnDebloatAutomaticoVerdi.PressedBackground = Color.FromArgb(46, 125, 50);
            btnDebloatAutomaticoVerdi.PressedForeColor = Color.Black;
            btnDebloatAutomaticoVerdi.PressedImageTint = Color.Black;
            btnDebloatAutomaticoVerdi.PressedOutline = Color.FromArgb(46, 125, 50);
            btnDebloatAutomaticoVerdi.Rounding = new Padding(8);
            btnDebloatAutomaticoVerdi.TextAlignment = StringAlignment.Center;
            btnDebloatAutomaticoVerdi.TextOffset = new Point(0, 0);
            btnDebloatAutomaticoVerdi.Click += btnDebloatAuto_Click;
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = Color.White;
            label3.Name = "label3";
            label3.Click += label3_Click;
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.ForeColor = Color.White;
            label5.Name = "label5";
            // 
            // cuiPanel1
            // 
            resources.ApplyResources(cuiPanel1, "cuiPanel1");
            cuiPanel1.Controls.Add(label1);
            cuiPanel1.Controls.Add(cuiSwitch1);
            cuiPanel1.Name = "cuiPanel1";
            cuiPanel1.OutlineThickness = 1F;
            cuiPanel1.PanelColor = Color.FromArgb(37, 38, 39);
            cuiPanel1.PanelOutlineColor = Color.FromArgb(0, 126, 249);
            cuiPanel1.Rounding = new Padding(8);
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.White;
            label1.Name = "label1";
            // 
            // cuiSwitch1
            // 
            resources.ApplyResources(cuiSwitch1, "cuiSwitch1");
            cuiSwitch1.Checked = false;
            cuiSwitch1.CheckedBackground = Color.FromArgb(0, 126, 249);
            cuiSwitch1.CheckedForeground = Color.White;
            cuiSwitch1.CheckedOutlineColor = Color.Empty;
            cuiSwitch1.CheckedSymbolColor = Color.FromArgb(0, 126, 249);
            cuiSwitch1.Name = "cuiSwitch1";
            cuiSwitch1.OutlineThickness = 1F;
            cuiSwitch1.ShowSymbols = false;
            cuiSwitch1.ThumbSizeModifier = new Size(0, 0);
            cuiSwitch1.UncheckedBackground = Color.FromArgb(64, 128, 128, 128);
            cuiSwitch1.UncheckedForeground = Color.White;
            cuiSwitch1.UncheckedOutlineColor = Color.Empty;
            cuiSwitch1.UncheckedSymbolColor = Color.Gray;
            cuiSwitch1.CheckedChanged += cuiSwitch1_CheckedChanged;
            // 
            // btnInstallaComponentiVerdi
            // 
            resources.ApplyResources(btnInstallaComponentiVerdi, "btnInstallaComponentiVerdi");
            btnInstallaComponentiVerdi.CheckButton = false;
            btnInstallaComponentiVerdi.Checked = false;
            btnInstallaComponentiVerdi.CheckedBackground = Color.FromArgb(46, 125, 60);
            btnInstallaComponentiVerdi.CheckedForeColor = Color.FromArgb(46, 125, 60);
            btnInstallaComponentiVerdi.CheckedImageTint = Color.FromArgb(46, 125, 60);
            btnInstallaComponentiVerdi.CheckedOutline = Color.FromArgb(46, 125, 60);
            btnInstallaComponentiVerdi.Content = "  Aggiungi componenti";
            btnInstallaComponentiVerdi.DialogResult = DialogResult.None;
            btnInstallaComponentiVerdi.ForeColor = Color.White;
            btnInstallaComponentiVerdi.HoverBackground = Color.FromArgb(46, 125, 50);
            btnInstallaComponentiVerdi.HoverForeColor = Color.White;
            btnInstallaComponentiVerdi.HoverImageTint = Color.White;
            btnInstallaComponentiVerdi.HoverOutline = Color.FromArgb(46, 125, 50);
            btnInstallaComponentiVerdi.Image = Properties.Resources.pngInstallaAppDebloat;
            btnInstallaComponentiVerdi.ImageAutoCenter = true;
            btnInstallaComponentiVerdi.ImageExpand = new Point(0, 0);
            btnInstallaComponentiVerdi.ImageOffset = new Point(0, 0);
            btnInstallaComponentiVerdi.Name = "btnInstallaComponentiVerdi";
            btnInstallaComponentiVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btnInstallaComponentiVerdi.NormalForeColor = Color.White;
            btnInstallaComponentiVerdi.NormalImageTint = Color.White;
            btnInstallaComponentiVerdi.NormalOutline = Color.FromArgb(46, 125, 50);
            btnInstallaComponentiVerdi.OutlineThickness = 1F;
            btnInstallaComponentiVerdi.PressedBackground = Color.FromArgb(46, 125, 50);
            btnInstallaComponentiVerdi.PressedForeColor = Color.Black;
            btnInstallaComponentiVerdi.PressedImageTint = Color.Black;
            btnInstallaComponentiVerdi.PressedOutline = Color.FromArgb(46, 125, 50);
            btnInstallaComponentiVerdi.Rounding = new Padding(8);
            btnInstallaComponentiVerdi.TextAlignment = StringAlignment.Center;
            btnInstallaComponentiVerdi.TextOffset = new Point(0, 0);
            btnInstallaComponentiVerdi.Click += btnInstallaComponentiVerdi_Click;
            // 
            // FormDebloat
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(cuiPanel2);
            Controls.Add(btnInstallaComponentiVerdi);
            Controls.Add(cuiPanel1);
            Controls.Add(btnModificaServiziDisattivo);
            Controls.Add(progressBar1);
            Controls.Add(btnAvviaSelezionatiVerdi);
            Controls.Add(textBox1);
            Controls.Add(flowLayoutPanel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormDebloat";
            cuiPanel2.ResumeLayout(false);
            cuiPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPowerPoint).EndInit();
            cuiPanel1.ResumeLayout(false);
            cuiPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnDebloatAuto;
        private Label lblInfoWin12;
        private FlowLayoutPanel flowLayoutPanel1;
        private TextBox textBox1;
        private CuoreUI.Controls.cuiButton btnModificaServiziDisattivo;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private CuoreUI.Controls.cuiProgressBarHorizontal progressBar1;
        private CuoreUI.Controls.cuiButton btnAvviaSelezionatiVerdi;
        private CuoreUI.Controls.cuiButton btnRiprisitinoDefenderVerdi;
        private CuoreUI.Controls.cuiButton btnProtezioneMinimaVerdi;
        private CuoreUI.Controls.cuiPanel cuiPanel2;
        private PictureBox pictureBoxPowerPoint;
        private CuoreUI.Controls.cuiButton btnDebloatAutomaticoVerdi;
        private Label label3;
        private Label label5;
        private CuoreUI.Controls.cuiPanel cuiPanel1;
        private Label label1;
        private CuoreUI.Controls.cuiSwitch cuiSwitch1;
        private CuoreUI.Controls.cuiButton btnInstallaComponentiVerdi;
    }
}