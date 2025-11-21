namespace WinHubX.DialogBlock
{
    partial class Form_DialogBlock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_DialogBlock));
            btnVerificaVerdi = new CuoreUI.Controls.cuiButton();
            btnClose = new Button();
            label4 = new Label();
            label3 = new Label();
            pictureBox1 = new PictureBox();
            cuiFormRounder1 = new CuoreUI.Components.cuiFormRounder();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
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
            btnVerificaVerdi.Content = "  Esegui check";
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
            btnVerificaVerdi.NormalBackground = Color.FromArgb(50, 50, 50);
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
            btnVerificaVerdi.Click += btnVerificaVerdi_Click;
            // 
            // btnClose
            // 
            resources.ApplyResources(btnClose, "btnClose");
            btnClose.Cursor = Cursors.Hand;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Image = Properties.Resources.pngChiudiForm1;
            btnClose.Name = "btnClose";
            btnClose.UseMnemonic = false;
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.ForeColor = Color.FromArgb(0, 126, 249);
            label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = Color.White;
            label3.Name = "label3";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Image = Properties.Resources.pngDialogBlock;
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // cuiFormRounder1
            // 
            cuiFormRounder1.OutlineColor = Color.FromArgb(32, 128, 128, 128);
            cuiFormRounder1.Rounding = 20;
            cuiFormRounder1.TargetForm = this;
            // 
            // Form_DialogBlock
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(50, 50, 50);
            Controls.Add(btnVerificaVerdi);
            Controls.Add(btnClose);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form_DialogBlock";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CuoreUI.Controls.cuiButton btnVerificaVerdi;
        private Button btnClose;
        private Label label4;
        private Label label3;
        private PictureBox pictureBox1;
        private CuoreUI.Components.cuiFormRounder cuiFormRounder1;
    }
}