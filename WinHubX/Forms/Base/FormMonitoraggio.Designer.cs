using WinHubX.Bottoni;

namespace WinHubX.Forms.Base
{
    partial class FormMonitoraggio
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMonitoraggio));
            pic_termcpu = new PictureBox();
            pic_termgpu = new PictureBox();
            labelCpuTemp = new Label();
            labelGpuTemp = new Label();
            BarRAM = new CircularProgressBar();
            BarCPU = new CircularProgressBar();
            swapButton1 = new BottoniSwap();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            btn_pulisciram = new Button();
            btn_puliscicpu = new Button();
            radioButton_taskbar = new RadioButton();
            radioButton_notifica = new RadioButton();
            CartellaTemp = new CircularProgressBar();
            label4 = new Label();
            comboBox_gb = new ComboBox();
            btnPulisciTemp = new Button();
            tempMonitorTimer = new System.Windows.Forms.Timer(components);
            label5 = new Label();
            panel10 = new Panel();
            ((System.ComponentModel.ISupportInitialize)pic_termcpu).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pic_termgpu).BeginInit();
            panel10.SuspendLayout();
            SuspendLayout();
            // 
            // pic_termcpu
            // 
            resources.ApplyResources(pic_termcpu, "pic_termcpu");
            pic_termcpu.Image = Properties.Resources.term_verde;
            pic_termcpu.Name = "pic_termcpu";
            pic_termcpu.TabStop = false;
            // 
            // pic_termgpu
            // 
            resources.ApplyResources(pic_termgpu, "pic_termgpu");
            pic_termgpu.Image = Properties.Resources.term_giallo;
            pic_termgpu.Name = "pic_termgpu";
            pic_termgpu.TabStop = false;
            // 
            // labelCpuTemp
            // 
            resources.ApplyResources(labelCpuTemp, "labelCpuTemp");
            labelCpuTemp.ForeColor = Color.FromArgb(224, 224, 224);
            labelCpuTemp.Name = "labelCpuTemp";
            // 
            // labelGpuTemp
            // 
            resources.ApplyResources(labelGpuTemp, "labelGpuTemp");
            labelGpuTemp.ForeColor = Color.FromArgb(224, 224, 224);
            labelGpuTemp.Name = "labelGpuTemp";
            // 
            // BarRAM
            // 
            resources.ApplyResources(BarRAM, "BarRAM");
            BarRAM.Maximum = 100;
            BarRAM.Minimum = 0;
            BarRAM.Name = "BarRAM";
            BarRAM.Value = 30;
            // 
            // BarCPU
            // 
            resources.ApplyResources(BarCPU, "BarCPU");
            BarCPU.Maximum = 100;
            BarCPU.Minimum = 0;
            BarCPU.Name = "BarCPU";
            BarCPU.Value = 30;
            // 
            // swapButton1
            // 
            resources.ApplyResources(swapButton1, "swapButton1");
            swapButton1.Name = "swapButton1";
            swapButton1.OffBackColor = Color.Gray;
            swapButton1.OffToggleColor = Color.Gainsboro;
            swapButton1.OnBackColor = Color.MediumSlateBlue;
            swapButton1.OnToggleColor = Color.WhiteSmoke;
            swapButton1.UseVisualStyleBackColor = true;
            swapButton1.CheckedChanged += swapButton1_CheckedChanged;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.FromArgb(224, 224, 224);
            label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.ForeColor = Color.FromArgb(224, 224, 224);
            label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = Color.FromArgb(224, 224, 224);
            label3.Name = "label3";
            // 
            // btn_pulisciram
            // 
            resources.ApplyResources(btn_pulisciram, "btn_pulisciram");
            btn_pulisciram.BackColor = Color.FromArgb(64, 64, 64);
            btn_pulisciram.FlatAppearance.BorderSize = 0;
            btn_pulisciram.ForeColor = Color.FromArgb(224, 224, 224);
            btn_pulisciram.Name = "btn_pulisciram";
            btn_pulisciram.UseVisualStyleBackColor = false;
            btn_pulisciram.Click += btn_pulisciram_Click;
            // 
            // btn_puliscicpu
            // 
            resources.ApplyResources(btn_puliscicpu, "btn_puliscicpu");
            btn_puliscicpu.BackColor = Color.FromArgb(64, 64, 64);
            btn_puliscicpu.FlatAppearance.BorderSize = 0;
            btn_puliscicpu.ForeColor = Color.FromArgb(224, 224, 224);
            btn_puliscicpu.Name = "btn_puliscicpu";
            btn_puliscicpu.UseVisualStyleBackColor = false;
            btn_puliscicpu.Click += btn_puliscicpu_Click;
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
            // CartellaTemp
            // 
            resources.ApplyResources(CartellaTemp, "CartellaTemp");
            CartellaTemp.Maximum = 100;
            CartellaTemp.Minimum = 0;
            CartellaTemp.Name = "CartellaTemp";
            CartellaTemp.Value = 30;
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.ForeColor = Color.FromArgb(224, 224, 224);
            label4.Name = "label4";
            // 
            // comboBox_gb
            // 
            resources.ApplyResources(comboBox_gb, "comboBox_gb");
            comboBox_gb.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_gb.FormattingEnabled = true;
            comboBox_gb.Items.AddRange(new object[] { resources.GetString("comboBox_gb.Items"), resources.GetString("comboBox_gb.Items1"), resources.GetString("comboBox_gb.Items2"), resources.GetString("comboBox_gb.Items3"), resources.GetString("comboBox_gb.Items4"), resources.GetString("comboBox_gb.Items5"), resources.GetString("comboBox_gb.Items6"), resources.GetString("comboBox_gb.Items7"), resources.GetString("comboBox_gb.Items8"), resources.GetString("comboBox_gb.Items9") });
            comboBox_gb.Name = "comboBox_gb";
            comboBox_gb.SelectedIndexChanged += comboBox_gb_SelectedIndexChanged;
            // 
            // btnPulisciTemp
            // 
            resources.ApplyResources(btnPulisciTemp, "btnPulisciTemp");
            btnPulisciTemp.BackColor = Color.FromArgb(64, 64, 64);
            btnPulisciTemp.FlatAppearance.BorderSize = 0;
            btnPulisciTemp.ForeColor = Color.FromArgb(224, 224, 224);
            btnPulisciTemp.Name = "btnPulisciTemp";
            btnPulisciTemp.UseVisualStyleBackColor = false;
            btnPulisciTemp.Click += btnPulisciTemp_Click;
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.ForeColor = Color.FromArgb(224, 224, 224);
            label5.Name = "label5";
            // 
            // panel10
            // 
            resources.ApplyResources(panel10, "panel10");
            panel10.Controls.Add(radioButton_notifica);
            panel10.Controls.Add(radioButton_taskbar);
            panel10.Name = "panel10";
            // 
            // FormMonitoraggio
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(panel10);
            Controls.Add(label5);
            Controls.Add(btnPulisciTemp);
            Controls.Add(comboBox_gb);
            Controls.Add(label4);
            Controls.Add(CartellaTemp);
            Controls.Add(btn_puliscicpu);
            Controls.Add(btn_pulisciram);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(swapButton1);
            Controls.Add(BarCPU);
            Controls.Add(BarRAM);
            Controls.Add(labelGpuTemp);
            Controls.Add(labelCpuTemp);
            Controls.Add(pic_termgpu);
            Controls.Add(pic_termcpu);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormMonitoraggio";
            FormClosing += FormMonitoraggio_FormClosing;
            Load += FormMonitoraggio_Load;
            ((System.ComponentModel.ISupportInitialize)pic_termcpu).EndInit();
            ((System.ComponentModel.ISupportInitialize)pic_termgpu).EndInit();
            panel10.ResumeLayout(false);
            panel10.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CircularProgressBar BarRAM;
        private PictureBox pic_termcpu;
        private PictureBox pic_termgpu;
        private Label labelCpuTemp;
        private Label labelGpuTemp;
        private CircularProgressBar BarCPU;
        private BottoniSwap swapButton1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button btn_pulisciram;
        private Button btn_puliscicpu;
        private RadioButton radioButton_taskbar;
        private RadioButton radioButton_notifica;
        private CircularProgressBar CartellaTemp;
        private Label label4;
        private ComboBox comboBox_gb;
        private Button btnPulisciTemp;
        private System.Windows.Forms.Timer tempMonitorTimer;
        private Label label5;
        private Panel panel10;
    }
}