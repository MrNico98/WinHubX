namespace WinHubX.Forms.DriverRST
{
    partial class FormDriverRST
    {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDriverRST));
            label3 = new Label();
            btnInstallaVerdi = new HartUI.Controls.cuiButton();
            richTextBox1 = new RichTextBox();
            SuspendLayout();
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = Color.FromArgb(0, 126, 249);
            label3.Name = "label3";
            // 
            // btnInstallaVerdi
            // 
            resources.ApplyResources(btnInstallaVerdi, "btnInstallaVerdi");
            btnInstallaVerdi.CheckButton = false;
            btnInstallaVerdi.Checked = false;
            btnInstallaVerdi.CheckedBackground = Color.FromArgb(46, 125, 60);
            btnInstallaVerdi.CheckedForeColor = Color.FromArgb(46, 125, 60);
            btnInstallaVerdi.CheckedImageTint = Color.FromArgb(46, 125, 60);
            btnInstallaVerdi.CheckedOutline = Color.FromArgb(46, 125, 60);
            btnInstallaVerdi.Content = "  Download";
            btnInstallaVerdi.DialogResult = DialogResult.None;
            btnInstallaVerdi.ForeColor = Color.White;
            btnInstallaVerdi.HoverBackground = Color.FromArgb(46, 125, 50);
            btnInstallaVerdi.HoverForeColor = Color.White;
            btnInstallaVerdi.HoverImageTint = Color.White;
            btnInstallaVerdi.HoverOutline = Color.FromArgb(46, 125, 50);
            btnInstallaVerdi.Image = Properties.Resources.pngScaricaOffice;
            btnInstallaVerdi.ImageExpand = new Point(0, 0);
            btnInstallaVerdi.Name = "btnInstallaVerdi";
            btnInstallaVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btnInstallaVerdi.NormalForeColor = Color.White;
            btnInstallaVerdi.NormalImageTint = Color.White;
            btnInstallaVerdi.NormalOutline = Color.FromArgb(46, 125, 50);
            btnInstallaVerdi.OutlineThickness = 1F;
            btnInstallaVerdi.PressedBackground = Color.FromArgb(46, 125, 50);
            btnInstallaVerdi.PressedForeColor = Color.Black;
            btnInstallaVerdi.PressedImageTint = Color.Black;
            btnInstallaVerdi.PressedOutline = Color.FromArgb(46, 125, 50);
            btnInstallaVerdi.Rounding = new Padding(8);
            btnInstallaVerdi.TextAlignment = StringAlignment.Center;
            btnInstallaVerdi.Click += btnDriverRSTPrinci_Click;
            // 
            // richTextBox1
            // 
            resources.ApplyResources(richTextBox1, "richTextBox1");
            richTextBox1.BackColor = Color.FromArgb(37, 38, 39);
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.ForeColor = Color.White;
            richTextBox1.Name = "richTextBox1";
            // 
            // FormDriverRST
            // 
            resources.ApplyResources(this, "$this");
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(richTextBox1);
            Controls.Add(btnInstallaVerdi);
            Controls.Add(label3);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormDriverRST";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label3;
        private HartUI.Controls.cuiButton btnInstallaVerdi;
        private RichTextBox richTextBox1;
    }
}
