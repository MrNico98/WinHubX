using CuoreUI.Controls;

namespace WinHubX.Forms.CreaISO
{
    partial class FormCreazioneISO
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCreazioneISO));
            richTextBox1 = new RichTextBox();
            label3 = new Label();
            label1 = new Label();
            progressBar1 = new cuiProgressBarHorizontal();
            progressBar2 = new cuiProgressBarHorizontal();
            btnStopVerdi = new cuiButton();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            resources.ApplyResources(richTextBox1, "richTextBox1");
            richTextBox1.BackColor = Color.DimGray;
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.ForeColor = Color.White;
            richTextBox1.Name = "richTextBox1";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = Color.White;
            label3.Name = "label3";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.White;
            label1.Name = "label1";
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
            // btnStopVerdi
            // 
            resources.ApplyResources(btnStopVerdi, "btnStopVerdi");
            btnStopVerdi.CheckButton = false;
            btnStopVerdi.Checked = false;
            btnStopVerdi.CheckedBackground = Color.FromArgb(192, 0, 0);
            btnStopVerdi.CheckedForeColor = Color.FromArgb(192, 0, 0);
            btnStopVerdi.CheckedImageTint = Color.FromArgb(192, 0, 0);
            btnStopVerdi.CheckedOutline = Color.FromArgb(192, 0, 0);
            btnStopVerdi.Content = "   STOP";
            btnStopVerdi.DialogResult = DialogResult.None;
            btnStopVerdi.ForeColor = Color.White;
            btnStopVerdi.HoverBackground = Color.FromArgb(192, 0, 0);
            btnStopVerdi.HoverForeColor = Color.White;
            btnStopVerdi.HoverImageTint = Color.White;
            btnStopVerdi.HoverOutline = Color.FromArgb(192, 0, 0);
            btnStopVerdi.Image = Properties.Resources.pngCloseCreazioneISO;
            btnStopVerdi.ImageAutoCenter = true;
            btnStopVerdi.ImageExpand = new Point(0, 0);
            btnStopVerdi.ImageOffset = new Point(0, 0);
            btnStopVerdi.Name = "btnStopVerdi";
            btnStopVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btnStopVerdi.NormalForeColor = Color.White;
            btnStopVerdi.NormalImageTint = Color.White;
            btnStopVerdi.NormalOutline = Color.FromArgb(192, 0, 0);
            btnStopVerdi.OutlineThickness = 1F;
            btnStopVerdi.PressedBackground = Color.FromArgb(192, 0, 0);
            btnStopVerdi.PressedForeColor = Color.Black;
            btnStopVerdi.PressedImageTint = Color.Black;
            btnStopVerdi.PressedOutline = Color.FromArgb(192, 0, 0);
            btnStopVerdi.Rounding = new Padding(8);
            btnStopVerdi.TextAlignment = StringAlignment.Center;
            btnStopVerdi.TextOffset = new Point(0, 0);
            btnStopVerdi.Click += btnStop_Click;
            // 
            // FormCreazioneISO
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(btnStopVerdi);
            Controls.Add(progressBar2);
            Controls.Add(progressBar1);
            Controls.Add(label1);
            Controls.Add(label3);
            Controls.Add(richTextBox1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormCreazioneISO";
            Shown += FormCreazioneISO_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox richTextBox1;
        private Label label3;
        private Label label1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private cuiProgressBarHorizontal progressBar1;
        private CuoreUI.Controls.cuiProgressBarHorizontal cuiProgressBarHorizontal1;
        private CuoreUI.Controls.cuiProgressBarHorizontal progressBar2;
        private cuiButton btnStopVerdi;
    }
}