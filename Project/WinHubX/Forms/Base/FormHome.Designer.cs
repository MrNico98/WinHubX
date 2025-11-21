namespace WinHubX
{
    partial class FormHome
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHome));
            cuiProgressTrackerHorizontal1 = new CuoreUI.Controls.cuiProgressTrackerHorizontal();
            cuiSpinner1 = new CuoreUI.Controls.cuiSpinner();
            cuiSeparator1 = new CuoreUI.Controls.cuiSeparator();
            labelverifica = new Label();
            label1 = new Label();
            labelcpu = new Label();
            pictureBox1 = new PictureBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            labelram = new Label();
            labeldisco = new Label();
            labelos = new Label();
            labelwindows = new Label();
            labeloffice = new Label();
            label7 = new Label();
            btnVerificaVerdi = new CuoreUI.Controls.cuiButton();
            label5 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // cuiProgressTrackerHorizontal1
            // 
            resources.ApplyResources(cuiProgressTrackerHorizontal1, "cuiProgressTrackerHorizontal1");
            cuiProgressTrackerHorizontal1.AutoRounding = true;
            cuiProgressTrackerHorizontal1.CompletedColor = Color.FromArgb(0, 126, 249);
            cuiProgressTrackerHorizontal1.CurrentTaskForeColor = Color.FromArgb(128, 128, 128);
            cuiProgressTrackerHorizontal1.ForeColor = Color.White;
            cuiProgressTrackerHorizontal1.LineThickness = 13;
            cuiProgressTrackerHorizontal1.Name = "cuiProgressTrackerHorizontal1";
            cuiProgressTrackerHorizontal1.Rounding = 10;
            cuiProgressTrackerHorizontal1.ShowSymbols = true;
            cuiProgressTrackerHorizontal1.TaskForeColor = Color.FromArgb(128, 128, 128);
            cuiProgressTrackerHorizontal1.Tasks = new string[]
    {
    "Task1",
    "Task2",
    "Task3"
    };
            cuiProgressTrackerHorizontal1.TasksProgress = 0;
            cuiProgressTrackerHorizontal1.TrackColor = Color.FromArgb(64, 128, 128, 128);
            // 
            // cuiSpinner1
            // 
            resources.ApplyResources(cuiSpinner1, "cuiSpinner1");
            cuiSpinner1.ArcColor = Color.FromArgb(0, 126, 249);
            cuiSpinner1.Cursor = Cursors.WaitCursor;
            cuiSpinner1.ForeColor = Color.FromArgb(0, 126, 249);
            cuiSpinner1.Name = "cuiSpinner1";
            cuiSpinner1.RingColor = Color.FromArgb(64, 128, 128, 128);
            cuiSpinner1.RotateSpeed = 6F;
            cuiSpinner1.Rotation = 221.250717F;
            cuiSpinner1.Thickness = 5F;
            // 
            // cuiSeparator1
            // 
            resources.ApplyResources(cuiSeparator1, "cuiSeparator1");
            cuiSeparator1.ForeColor = Color.FromArgb(128, 128, 128, 128);
            cuiSeparator1.Name = "cuiSeparator1";
            cuiSeparator1.SeparatorMargin = 8;
            cuiSeparator1.Thickness = 0.5F;
            cuiSeparator1.Vertical = false;
            // 
            // labelverifica
            // 
            resources.ApplyResources(labelverifica, "labelverifica");
            labelverifica.ForeColor = Color.White;
            labelverifica.Name = "labelverifica";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.White;
            label1.Name = "label1";
            // 
            // labelcpu
            // 
            resources.ApplyResources(labelcpu, "labelcpu");
            labelcpu.ForeColor = Color.White;
            labelcpu.Image = Properties.Resources.pngStatoWindowsBlackFormHome;
            labelcpu.Name = "labelcpu";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = Properties.Resources.support_me_on_kofi_badge_red;
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            pictureBox1.Click += btnKofi_Click;
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.ForeColor = Color.White;
            label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = Color.White;
            label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.ForeColor = Color.FromArgb(0, 126, 249);
            label4.Name = "label4";
            // 
            // labelram
            // 
            resources.ApplyResources(labelram, "labelram");
            labelram.ForeColor = Color.White;
            labelram.Image = Properties.Resources.pngRamHome;
            labelram.Name = "labelram";
            // 
            // labeldisco
            // 
            resources.ApplyResources(labeldisco, "labeldisco");
            labeldisco.ForeColor = Color.White;
            labeldisco.Image = Properties.Resources.pngHDDHome;
            labeldisco.Name = "labeldisco";
            // 
            // labelos
            // 
            resources.ApplyResources(labelos, "labelos");
            labelos.ForeColor = Color.White;
            labelos.Image = Properties.Resources.pngOSHome;
            labelos.Name = "labelos";
            // 
            // labelwindows
            // 
            resources.ApplyResources(labelwindows, "labelwindows");
            labelwindows.ForeColor = Color.White;
            labelwindows.Image = Properties.Resources.pngStatoWindowsHome;
            labelwindows.Name = "labelwindows";
            // 
            // labeloffice
            // 
            resources.ApplyResources(labeloffice, "labeloffice");
            labeloffice.ForeColor = Color.White;
            labeloffice.Image = Properties.Resources.pngStatoOfficeHome;
            labeloffice.Name = "labeloffice";
            // 
            // label7
            // 
            resources.ApplyResources(label7, "label7");
            label7.ForeColor = Color.White;
            label7.Name = "label7";
            // 
            // btnVerificaVerdi
            // 
            resources.ApplyResources(btnVerificaVerdi, "btnVerificaVerdi");
            btnVerificaVerdi.CheckButton = false;
            btnVerificaVerdi.Checked = false;
            btnVerificaVerdi.CheckedBackground = Color.FromArgb(46, 125, 60);
            btnVerificaVerdi.CheckedForeColor = Color.FromArgb(46, 125, 60);
            btnVerificaVerdi.CheckedImageTint = Color.FromArgb(46, 125, 60);
            btnVerificaVerdi.CheckedOutline = Color.FromArgb(46, 125, 60);
            btnVerificaVerdi.Content = "  Rerun PC verification";
            btnVerificaVerdi.DialogResult = DialogResult.None;
            btnVerificaVerdi.ForeColor = Color.White;
            btnVerificaVerdi.HoverBackground = Color.FromArgb(46, 125, 50);
            btnVerificaVerdi.HoverForeColor = Color.White;
            btnVerificaVerdi.HoverImageTint = Color.White;
            btnVerificaVerdi.HoverOutline = Color.FromArgb(46, 125, 50);
            btnVerificaVerdi.Image = Properties.Resources.pngclick;
            btnVerificaVerdi.ImageAutoCenter = true;
            btnVerificaVerdi.ImageExpand = new Point(0, 0);
            btnVerificaVerdi.ImageOffset = new Point(0, 0);
            btnVerificaVerdi.Name = "btnVerificaVerdi";
            btnVerificaVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btnVerificaVerdi.NormalForeColor = Color.White;
            btnVerificaVerdi.NormalImageTint = Color.White;
            btnVerificaVerdi.NormalOutline = Color.FromArgb(46, 125, 50);
            btnVerificaVerdi.OutlineThickness = 1F;
            btnVerificaVerdi.PressedBackground = Color.FromArgb(46, 125, 50);
            btnVerificaVerdi.PressedForeColor = Color.Black;
            btnVerificaVerdi.PressedImageTint = Color.Black;
            btnVerificaVerdi.PressedOutline = Color.FromArgb(46, 125, 50);
            btnVerificaVerdi.Rounding = new Padding(8);
            btnVerificaVerdi.TextAlignment = StringAlignment.Center;
            btnVerificaVerdi.TextOffset = new Point(0, 0);
            btnVerificaVerdi.Click += btnVerifica_Click;
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.ForeColor = Color.White;
            label5.Name = "label5";
            // 
            // FormHome
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(label5);
            Controls.Add(btnVerificaVerdi);
            Controls.Add(label7);
            Controls.Add(labeloffice);
            Controls.Add(labelwindows);
            Controls.Add(labelos);
            Controls.Add(labeldisco);
            Controls.Add(labelram);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(pictureBox1);
            Controls.Add(labelcpu);
            Controls.Add(label1);
            Controls.Add(labelverifica);
            Controls.Add(cuiSeparator1);
            Controls.Add(cuiSpinner1);
            Controls.Add(cuiProgressTrackerHorizontal1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormHome";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private CuoreUI.Controls.cuiProgressTrackerHorizontal cuiProgressTrackerHorizontal1;
        private CuoreUI.Controls.cuiSpinner cuiSpinner1;
        private CuoreUI.Controls.cuiSeparator cuiSeparator1;
        private Label labelverifica;
        private Button button1;
        private Label label1;
        private Label labelcpu;
        private PictureBox pictureBox1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label labelram;
        private Label labeldisco;
        private Label labelos;
        private Label labelwindows;
        private Label labeloffice;
        private Label label7;
        private CuoreUI.Controls.cuiButton btnVerificaVerdi;
        private Label label5;
    }
}