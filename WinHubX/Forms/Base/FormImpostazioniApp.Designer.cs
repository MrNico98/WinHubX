namespace WinHubX.Forms.Base
{
    partial class FormImpostazioniApp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImpostazioniApp));
            label1 = new Label();
            bottoniSwap1 = new WinHubX.Bottoni.BottoniSwap();
            pictureBox3 = new PictureBox();
            comboBox1 = new ComboBox();
            radioButton_notifica = new RadioButton();
            radioButton_taskbar = new RadioButton();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // bottoniSwap1
            // 
            resources.ApplyResources(bottoniSwap1, "bottoniSwap1");
            bottoniSwap1.Name = "bottoniSwap1";
            bottoniSwap1.OffBackColor = Color.Gray;
            bottoniSwap1.OffToggleColor = Color.Gainsboro;
            bottoniSwap1.OnBackColor = Color.MediumSlateBlue;
            bottoniSwap1.OnToggleColor = Color.WhiteSmoke;
            bottoniSwap1.UseVisualStyleBackColor = true;
            bottoniSwap1.CheckedChanged += bottoniSwap1_CheckedChanged;
            // 
            // pictureBox3
            // 
            pictureBox3.Cursor = Cursors.Hand;
            pictureBox3.Image = Properties.Resources.italias;
            resources.ApplyResources(pictureBox3, "pictureBox3");
            pictureBox3.Name = "pictureBox3";
            pictureBox3.TabStop = false;
            pictureBox3.Click += pictureBox3_Click;
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { resources.GetString("comboBox1.Items"), resources.GetString("comboBox1.Items1") });
            resources.ApplyResources(comboBox1, "comboBox1");
            comboBox1.Name = "comboBox1";
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // radioButton_notifica
            // 
            radioButton_notifica.Cursor = Cursors.Hand;
            radioButton_notifica.ForeColor = Color.White;
            resources.ApplyResources(radioButton_notifica, "radioButton_notifica");
            radioButton_notifica.Name = "radioButton_notifica";
            radioButton_notifica.TabStop = true;
            radioButton_notifica.UseVisualStyleBackColor = true;
            radioButton_notifica.CheckedChanged += radioButton_notifica_CheckedChanged;
            // 
            // radioButton_taskbar
            // 
            resources.ApplyResources(radioButton_taskbar, "radioButton_taskbar");
            radioButton_taskbar.ForeColor = Color.White;
            radioButton_taskbar.Name = "radioButton_taskbar";
            radioButton_taskbar.TabStop = true;
            radioButton_taskbar.UseVisualStyleBackColor = true;
            radioButton_taskbar.CheckedChanged += radioButton_taskbar_CheckedChanged;
            // 
            // FormImpostazioniApp
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(radioButton_notifica);
            Controls.Add(radioButton_taskbar);
            Controls.Add(label1);
            Controls.Add(bottoniSwap1);
            Controls.Add(pictureBox3);
            Controls.Add(comboBox1);
            Name = "FormImpostazioniApp";
            Load += FormImpostazioniApp_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Bottoni.BottoniSwap bottoniSwap1;
        public PictureBox pictureBox3;
        public ComboBox comboBox1;
        private RadioButton radioButton_notifica;
        private RadioButton radioButton_taskbar;
    }
}