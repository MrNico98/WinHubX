namespace WinHubX.Forms.Settaggi
{
    partial class FormRipristinoSO
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRipristinoSO));
            checkBox_hw = new CheckBox();
            checkBox_sw = new CheckBox();
            dateTimePicker1 = new DateTimePicker();
            progressBar1 = new CuoreUI.Controls.cuiProgressBarHorizontal();
            label3 = new Label();
            labeltempo = new Label();
            btn_CreaISOVerdi = new CuoreUI.Controls.cuiButton();
            richTextBox1 = new RichTextBox();
            richTextBox2 = new RichTextBox();
            SuspendLayout();
            // 
            // checkBox_hw
            // 
            resources.ApplyResources(checkBox_hw, "checkBox_hw");
            checkBox_hw.ForeColor = Color.White;
            checkBox_hw.Name = "checkBox_hw";
            checkBox_hw.UseVisualStyleBackColor = true;
            checkBox_hw.CheckedChanged += checkBox_hw_CheckedChanged;
            // 
            // checkBox_sw
            // 
            resources.ApplyResources(checkBox_sw, "checkBox_sw");
            checkBox_sw.ForeColor = Color.White;
            checkBox_sw.Name = "checkBox_sw";
            checkBox_sw.UseVisualStyleBackColor = true;
            // 
            // dateTimePicker1
            // 
            resources.ApplyResources(dateTimePicker1, "dateTimePicker1");
            dateTimePicker1.CalendarMonthBackground = Color.FromArgb(37, 38, 39);
            dateTimePicker1.CalendarTitleBackColor = Color.FromArgb(37, 38, 39);
            dateTimePicker1.CalendarTitleForeColor = Color.FromArgb(37, 38, 39);
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker1.Value = new DateTime(2025, 11, 12, 0, 30, 0, 0);
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
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = Color.FromArgb(0, 126, 249);
            label3.Name = "label3";
            // 
            // labeltempo
            // 
            resources.ApplyResources(labeltempo, "labeltempo");
            labeltempo.ForeColor = Color.White;
            labeltempo.Name = "labeltempo";
            // 
            // btn_CreaISOVerdi
            // 
            resources.ApplyResources(btn_CreaISOVerdi, "btn_CreaISOVerdi");
            btn_CreaISOVerdi.CheckButton = false;
            btn_CreaISOVerdi.Checked = false;
            btn_CreaISOVerdi.CheckedBackground = Color.FromArgb(46, 125, 60);
            btn_CreaISOVerdi.CheckedForeColor = Color.FromArgb(46, 125, 60);
            btn_CreaISOVerdi.CheckedImageTint = Color.FromArgb(46, 125, 60);
            btn_CreaISOVerdi.CheckedOutline = Color.FromArgb(46, 125, 60);
            btn_CreaISOVerdi.Content = "  Avvia";
            btn_CreaISOVerdi.DialogResult = DialogResult.None;
            btn_CreaISOVerdi.ForeColor = Color.White;
            btn_CreaISOVerdi.HoverBackground = Color.FromArgb(46, 125, 50);
            btn_CreaISOVerdi.HoverForeColor = Color.White;
            btn_CreaISOVerdi.HoverImageTint = Color.White;
            btn_CreaISOVerdi.HoverOutline = Color.FromArgb(46, 125, 50);
            btn_CreaISOVerdi.Image = Properties.Resources.pngCheckCreaISO;
            btn_CreaISOVerdi.ImageAutoCenter = true;
            btn_CreaISOVerdi.ImageExpand = new Point(0, 0);
            btn_CreaISOVerdi.ImageOffset = new Point(0, 0);
            btn_CreaISOVerdi.Name = "btn_CreaISOVerdi";
            btn_CreaISOVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btn_CreaISOVerdi.NormalForeColor = Color.White;
            btn_CreaISOVerdi.NormalImageTint = Color.White;
            btn_CreaISOVerdi.NormalOutline = Color.FromArgb(46, 125, 50);
            btn_CreaISOVerdi.OutlineThickness = 1F;
            btn_CreaISOVerdi.PressedBackground = Color.FromArgb(46, 125, 50);
            btn_CreaISOVerdi.PressedForeColor = Color.Black;
            btn_CreaISOVerdi.PressedImageTint = Color.Black;
            btn_CreaISOVerdi.PressedOutline = Color.FromArgb(46, 125, 50);
            btn_CreaISOVerdi.Rounding = new Padding(8);
            btn_CreaISOVerdi.TextAlignment = StringAlignment.Center;
            btn_CreaISOVerdi.TextOffset = new Point(0, 0);
            btn_CreaISOVerdi.Click += buttonStart_Click;
            // 
            // richTextBox1
            // 
            resources.ApplyResources(richTextBox1, "richTextBox1");
            richTextBox1.BackColor = Color.DimGray;
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.ForeColor = Color.White;
            richTextBox1.Name = "richTextBox1";
            // 
            // richTextBox2
            // 
            resources.ApplyResources(richTextBox2, "richTextBox2");
            richTextBox2.BackColor = Color.DimGray;
            richTextBox2.BorderStyle = BorderStyle.None;
            richTextBox2.ForeColor = Color.White;
            richTextBox2.Name = "richTextBox2";
            // 
            // FormRipristinoSO
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(richTextBox2);
            Controls.Add(richTextBox1);
            Controls.Add(btn_CreaISOVerdi);
            Controls.Add(labeltempo);
            Controls.Add(progressBar1);
            Controls.Add(label3);
            Controls.Add(dateTimePicker1);
            Controls.Add(checkBox_sw);
            Controls.Add(checkBox_hw);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormRipristinoSO";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private CheckBox checkBox_hw;
        private CheckBox checkBox_sw;
        private DateTimePicker dateTimePicker1;
        private CuoreUI.Controls.cuiProgressBarHorizontal progressBar1;
        private Label label3;
        private Label labeltempo;
        private CuoreUI.Controls.cuiButton btn_CreaISOVerdi;
        private RichTextBox richTextBox1;
        private RichTextBox richTextBox2;
    }
}