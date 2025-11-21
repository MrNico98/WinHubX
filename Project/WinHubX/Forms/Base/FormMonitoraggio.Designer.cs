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
            tempMonitorTimer = new System.Windows.Forms.Timer(components);
            tableLayoutPanel17 = new TableLayoutPanel();
            cuiPanel5 = new CuoreUI.Controls.cuiPanel();
            domainUpDown1 = new DomainUpDown();
            BarTEMPtext = new Label();
            BarTEMP = new CuoreUI.Controls.cuiCircleProgressBar();
            label21 = new Label();
            label3 = new Label();
            label11 = new Label();
            label12 = new Label();
            btnSvuotaTemp = new CuoreUI.Controls.cuiButton();
            BarDISCOtext = new Label();
            BarDISCO = new CuoreUI.Controls.cuiCircleProgressBar();
            label18 = new Label();
            cuiPanel1 = new CuoreUI.Controls.cuiPanel();
            label17 = new Label();
            label16 = new Label();
            cuiSwitch_gradicpu = new CuoreUI.Controls.cuiSwitch();
            limiteCPU = new NumericUpDown();
            puliziaautomaticoCPU = new CuoreUI.Controls.cuiSwitch();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            btnPulisciCPU = new CuoreUI.Controls.cuiButton();
            labelCpuTemp = new Label();
            BarCPUtext = new Label();
            pic_termcpu = new PictureBox();
            BarCPU = new CuoreUI.Controls.cuiCircleProgressBar();
            label1 = new Label();
            label4 = new Label();
            cuiPanel2 = new CuoreUI.Controls.cuiPanel();
            limiteRAM = new NumericUpDown();
            puliziaautomaticRAM = new CuoreUI.Controls.cuiSwitch();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            btnPulisciRam = new CuoreUI.Controls.cuiButton();
            BarRAMtext = new Label();
            BarRAM = new CuoreUI.Controls.cuiCircleProgressBar();
            label14 = new Label();
            cuiPanel3 = new CuoreUI.Controls.cuiPanel();
            label22 = new Label();
            label23 = new Label();
            cuiSwitch_gputemperatura = new CuoreUI.Controls.cuiSwitch();
            label2 = new Label();
            BarGPUtext = new Label();
            BarGPU = new CuoreUI.Controls.cuiCircleProgressBar();
            labelGpuTemp = new Label();
            pic_termgpu = new PictureBox();
            label19 = new Label();
            label20 = new Label();
            cuiPanel4 = new CuoreUI.Controls.cuiPanel();
            label13 = new Label();
            labelReteUtilizzo = new Label();
            progressbarRete = new CuoreUI.Controls.cuiCircleProgressBar();
            lblDonwload = new Label();
            lblUpload = new Label();
            labelVelocitaRete = new Label();
            label15 = new Label();
            tableLayoutPanel17.SuspendLayout();
            cuiPanel5.SuspendLayout();
            cuiPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)limiteCPU).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pic_termcpu).BeginInit();
            cuiPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)limiteRAM).BeginInit();
            cuiPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pic_termgpu).BeginInit();
            cuiPanel4.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel17
            // 
            resources.ApplyResources(tableLayoutPanel17, "tableLayoutPanel17");
            tableLayoutPanel17.Controls.Add(cuiPanel5, 2, 0);
            tableLayoutPanel17.Controls.Add(cuiPanel1, 0, 0);
            tableLayoutPanel17.Controls.Add(cuiPanel2, 1, 0);
            tableLayoutPanel17.Controls.Add(cuiPanel3, 0, 1);
            tableLayoutPanel17.Controls.Add(cuiPanel4, 1, 1);
            tableLayoutPanel17.Name = "tableLayoutPanel17";
            // 
            // cuiPanel5
            // 
            cuiPanel5.Controls.Add(domainUpDown1);
            cuiPanel5.Controls.Add(BarTEMPtext);
            cuiPanel5.Controls.Add(BarTEMP);
            cuiPanel5.Controls.Add(label21);
            cuiPanel5.Controls.Add(label3);
            cuiPanel5.Controls.Add(label11);
            cuiPanel5.Controls.Add(label12);
            cuiPanel5.Controls.Add(btnSvuotaTemp);
            cuiPanel5.Controls.Add(BarDISCOtext);
            cuiPanel5.Controls.Add(BarDISCO);
            cuiPanel5.Controls.Add(label18);
            resources.ApplyResources(cuiPanel5, "cuiPanel5");
            cuiPanel5.Name = "cuiPanel5";
            cuiPanel5.OutlineThickness = 1F;
            cuiPanel5.PanelColor = Color.FromArgb(37, 38, 39);
            cuiPanel5.PanelOutlineColor = Color.WhiteSmoke;
            cuiPanel5.Rounding = new Padding(8);
            // 
            // domainUpDown1
            // 
            resources.ApplyResources(domainUpDown1, "domainUpDown1");
            domainUpDown1.Name = "domainUpDown1";
            domainUpDown1.SelectedItemChanged += domainUpDown1_SelectedItemChanged;
            // 
            // BarTEMPtext
            // 
            resources.ApplyResources(BarTEMPtext, "BarTEMPtext");
            BarTEMPtext.ForeColor = Color.White;
            BarTEMPtext.Name = "BarTEMPtext";
            // 
            // BarTEMP
            // 
            resources.ApplyResources(BarTEMP, "BarTEMP");
            BarTEMP.BorderWidth = 12;
            BarTEMP.MaximumValue = 100;
            BarTEMP.MinimumValue = 0;
            BarTEMP.Name = "BarTEMP";
            BarTEMP.NormalColor = Color.FromArgb(64, 128, 128, 128);
            BarTEMP.ProgressColor = Color.FromArgb(0, 126, 249);
            BarTEMP.ProgressValue = 20;
            BarTEMP.RoundedEnds = true;
            // 
            // label21
            // 
            resources.ApplyResources(label21, "label21");
            label21.ForeColor = Color.White;
            label21.Name = "label21";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = Color.White;
            label3.Name = "label3";
            // 
            // label11
            // 
            resources.ApplyResources(label11, "label11");
            label11.ForeColor = Color.White;
            label11.Name = "label11";
            // 
            // label12
            // 
            resources.ApplyResources(label12, "label12");
            label12.ForeColor = Color.White;
            label12.Name = "label12";
            // 
            // btnSvuotaTemp
            // 
            resources.ApplyResources(btnSvuotaTemp, "btnSvuotaTemp");
            btnSvuotaTemp.CheckButton = false;
            btnSvuotaTemp.Checked = false;
            btnSvuotaTemp.CheckedBackground = Color.FromArgb(46, 125, 60);
            btnSvuotaTemp.CheckedForeColor = Color.FromArgb(46, 125, 60);
            btnSvuotaTemp.CheckedImageTint = Color.FromArgb(46, 125, 60);
            btnSvuotaTemp.CheckedOutline = Color.FromArgb(46, 125, 60);
            btnSvuotaTemp.Content = "  Svuota";
            btnSvuotaTemp.DialogResult = DialogResult.None;
            btnSvuotaTemp.ForeColor = Color.White;
            btnSvuotaTemp.HoverBackground = Color.FromArgb(46, 125, 50);
            btnSvuotaTemp.HoverForeColor = Color.White;
            btnSvuotaTemp.HoverImageTint = Color.White;
            btnSvuotaTemp.HoverOutline = Color.FromArgb(46, 125, 50);
            btnSvuotaTemp.Image = Properties.Resources.pngSvuotaMonitoraggio;
            btnSvuotaTemp.ImageAutoCenter = true;
            btnSvuotaTemp.ImageExpand = new Point(0, 0);
            btnSvuotaTemp.ImageOffset = new Point(0, 0);
            btnSvuotaTemp.Name = "btnSvuotaTemp";
            btnSvuotaTemp.NormalBackground = Color.FromArgb(37, 38, 39);
            btnSvuotaTemp.NormalForeColor = Color.White;
            btnSvuotaTemp.NormalImageTint = Color.White;
            btnSvuotaTemp.NormalOutline = Color.FromArgb(46, 125, 50);
            btnSvuotaTemp.OutlineThickness = 1F;
            btnSvuotaTemp.PressedBackground = Color.FromArgb(46, 125, 50);
            btnSvuotaTemp.PressedForeColor = Color.Black;
            btnSvuotaTemp.PressedImageTint = Color.Black;
            btnSvuotaTemp.PressedOutline = Color.FromArgb(46, 125, 50);
            btnSvuotaTemp.Rounding = new Padding(8);
            btnSvuotaTemp.TextAlignment = StringAlignment.Center;
            btnSvuotaTemp.TextOffset = new Point(0, 0);
            btnSvuotaTemp.Click += btnSvuotaTemp_Click;
            // 
            // BarDISCOtext
            // 
            resources.ApplyResources(BarDISCOtext, "BarDISCOtext");
            BarDISCOtext.ForeColor = Color.White;
            BarDISCOtext.Name = "BarDISCOtext";
            // 
            // BarDISCO
            // 
            resources.ApplyResources(BarDISCO, "BarDISCO");
            BarDISCO.BorderWidth = 12;
            BarDISCO.MaximumValue = 100;
            BarDISCO.MinimumValue = 0;
            BarDISCO.Name = "BarDISCO";
            BarDISCO.NormalColor = Color.FromArgb(64, 128, 128, 128);
            BarDISCO.ProgressColor = Color.FromArgb(0, 126, 249);
            BarDISCO.ProgressValue = 20;
            BarDISCO.RoundedEnds = true;
            // 
            // label18
            // 
            resources.ApplyResources(label18, "label18");
            label18.ForeColor = Color.FromArgb(0, 126, 249);
            label18.Image = Properties.Resources.pngHardDiskMonitoraggio;
            label18.Name = "label18";
            // 
            // cuiPanel1
            // 
            cuiPanel1.Controls.Add(label17);
            cuiPanel1.Controls.Add(label16);
            cuiPanel1.Controls.Add(cuiSwitch_gradicpu);
            cuiPanel1.Controls.Add(limiteCPU);
            cuiPanel1.Controls.Add(puliziaautomaticoCPU);
            cuiPanel1.Controls.Add(label7);
            cuiPanel1.Controls.Add(label6);
            cuiPanel1.Controls.Add(label5);
            cuiPanel1.Controls.Add(btnPulisciCPU);
            cuiPanel1.Controls.Add(labelCpuTemp);
            cuiPanel1.Controls.Add(BarCPUtext);
            cuiPanel1.Controls.Add(pic_termcpu);
            cuiPanel1.Controls.Add(BarCPU);
            cuiPanel1.Controls.Add(label1);
            cuiPanel1.Controls.Add(label4);
            resources.ApplyResources(cuiPanel1, "cuiPanel1");
            cuiPanel1.Name = "cuiPanel1";
            cuiPanel1.OutlineThickness = 1F;
            cuiPanel1.PanelColor = Color.FromArgb(37, 38, 39);
            cuiPanel1.PanelOutlineColor = Color.WhiteSmoke;
            cuiPanel1.Rounding = new Padding(8);
            // 
            // label17
            // 
            resources.ApplyResources(label17, "label17");
            label17.ForeColor = Color.White;
            label17.Name = "label17";
            // 
            // label16
            // 
            resources.ApplyResources(label16, "label16");
            label16.ForeColor = Color.White;
            label16.Name = "label16";
            // 
            // cuiSwitch_gradicpu
            // 
            resources.ApplyResources(cuiSwitch_gradicpu, "cuiSwitch_gradicpu");
            cuiSwitch_gradicpu.Checked = false;
            cuiSwitch_gradicpu.CheckedBackground = Color.FromArgb(0, 126, 249);
            cuiSwitch_gradicpu.CheckedForeground = Color.White;
            cuiSwitch_gradicpu.CheckedOutlineColor = Color.Empty;
            cuiSwitch_gradicpu.CheckedSymbolColor = Color.FromArgb(0, 126, 249);
            cuiSwitch_gradicpu.Name = "cuiSwitch_gradicpu";
            cuiSwitch_gradicpu.OutlineThickness = 1F;
            cuiSwitch_gradicpu.ShowSymbols = false;
            cuiSwitch_gradicpu.ThumbSizeModifier = new Size(0, 0);
            cuiSwitch_gradicpu.UncheckedBackground = Color.FromArgb(64, 128, 128, 128);
            cuiSwitch_gradicpu.UncheckedForeground = Color.White;
            cuiSwitch_gradicpu.UncheckedOutlineColor = Color.Empty;
            cuiSwitch_gradicpu.UncheckedSymbolColor = Color.Gray;
            cuiSwitch_gradicpu.CheckedChanged += cuiSwitch_gradicpu_CheckedChanged;
            // 
            // limiteCPU
            // 
            resources.ApplyResources(limiteCPU, "limiteCPU");
            limiteCPU.Name = "limiteCPU";
            limiteCPU.Value = new decimal(new int[] { 40, 0, 0, 0 });
            // 
            // puliziaautomaticoCPU
            // 
            resources.ApplyResources(puliziaautomaticoCPU, "puliziaautomaticoCPU");
            puliziaautomaticoCPU.Checked = false;
            puliziaautomaticoCPU.CheckedBackground = Color.FromArgb(0, 126, 249);
            puliziaautomaticoCPU.CheckedForeground = Color.White;
            puliziaautomaticoCPU.CheckedOutlineColor = Color.Empty;
            puliziaautomaticoCPU.CheckedSymbolColor = Color.FromArgb(0, 126, 249);
            puliziaautomaticoCPU.Name = "puliziaautomaticoCPU";
            puliziaautomaticoCPU.OutlineThickness = 1F;
            puliziaautomaticoCPU.ShowSymbols = false;
            puliziaautomaticoCPU.ThumbSizeModifier = new Size(0, 0);
            puliziaautomaticoCPU.UncheckedBackground = Color.FromArgb(64, 128, 128, 128);
            puliziaautomaticoCPU.UncheckedForeground = Color.White;
            puliziaautomaticoCPU.UncheckedOutlineColor = Color.Empty;
            puliziaautomaticoCPU.UncheckedSymbolColor = Color.Gray;
            // 
            // label7
            // 
            resources.ApplyResources(label7, "label7");
            label7.ForeColor = Color.White;
            label7.Name = "label7";
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            label6.ForeColor = Color.White;
            label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.ForeColor = Color.White;
            label5.Name = "label5";
            // 
            // btnPulisciCPU
            // 
            resources.ApplyResources(btnPulisciCPU, "btnPulisciCPU");
            btnPulisciCPU.CheckButton = false;
            btnPulisciCPU.Checked = false;
            btnPulisciCPU.CheckedBackground = Color.FromArgb(46, 125, 60);
            btnPulisciCPU.CheckedForeColor = Color.FromArgb(46, 125, 60);
            btnPulisciCPU.CheckedImageTint = Color.FromArgb(46, 125, 60);
            btnPulisciCPU.CheckedOutline = Color.FromArgb(46, 125, 60);
            btnPulisciCPU.Content = "  Pulizia";
            btnPulisciCPU.DialogResult = DialogResult.None;
            btnPulisciCPU.ForeColor = Color.White;
            btnPulisciCPU.HoverBackground = Color.FromArgb(46, 125, 50);
            btnPulisciCPU.HoverForeColor = Color.White;
            btnPulisciCPU.HoverImageTint = Color.White;
            btnPulisciCPU.HoverOutline = Color.FromArgb(46, 125, 50);
            btnPulisciCPU.Image = Properties.Resources.pngPulisciMonitoraggio;
            btnPulisciCPU.ImageAutoCenter = true;
            btnPulisciCPU.ImageExpand = new Point(0, 0);
            btnPulisciCPU.ImageOffset = new Point(0, 0);
            btnPulisciCPU.Name = "btnPulisciCPU";
            btnPulisciCPU.NormalBackground = Color.FromArgb(37, 38, 39);
            btnPulisciCPU.NormalForeColor = Color.White;
            btnPulisciCPU.NormalImageTint = Color.White;
            btnPulisciCPU.NormalOutline = Color.FromArgb(46, 125, 50);
            btnPulisciCPU.OutlineThickness = 1F;
            btnPulisciCPU.PressedBackground = Color.FromArgb(46, 125, 50);
            btnPulisciCPU.PressedForeColor = Color.Black;
            btnPulisciCPU.PressedImageTint = Color.Black;
            btnPulisciCPU.PressedOutline = Color.FromArgb(46, 125, 50);
            btnPulisciCPU.Rounding = new Padding(8);
            btnPulisciCPU.TextAlignment = StringAlignment.Center;
            btnPulisciCPU.TextOffset = new Point(0, 0);
            btnPulisciCPU.Click += btn_puliscicpu_Click;
            // 
            // labelCpuTemp
            // 
            resources.ApplyResources(labelCpuTemp, "labelCpuTemp");
            labelCpuTemp.ForeColor = Color.White;
            labelCpuTemp.Name = "labelCpuTemp";
            // 
            // BarCPUtext
            // 
            resources.ApplyResources(BarCPUtext, "BarCPUtext");
            BarCPUtext.ForeColor = Color.White;
            BarCPUtext.Name = "BarCPUtext";
            // 
            // pic_termcpu
            // 
            resources.ApplyResources(pic_termcpu, "pic_termcpu");
            pic_termcpu.Image = Properties.Resources.term_verde;
            pic_termcpu.Name = "pic_termcpu";
            pic_termcpu.TabStop = false;
            // 
            // BarCPU
            // 
            resources.ApplyResources(BarCPU, "BarCPU");
            BarCPU.BorderWidth = 12;
            BarCPU.MaximumValue = 100;
            BarCPU.MinimumValue = 0;
            BarCPU.Name = "BarCPU";
            BarCPU.NormalColor = Color.FromArgb(64, 128, 128, 128);
            BarCPU.ProgressColor = Color.FromArgb(0, 126, 249);
            BarCPU.ProgressValue = 20;
            BarCPU.RoundedEnds = true;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.White;
            label1.Name = "label1";
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.ForeColor = Color.FromArgb(0, 126, 249);
            label4.Image = Properties.Resources.pngCPUMonitoraggio;
            label4.Name = "label4";
            // 
            // cuiPanel2
            // 
            cuiPanel2.Controls.Add(limiteRAM);
            cuiPanel2.Controls.Add(puliziaautomaticRAM);
            cuiPanel2.Controls.Add(label8);
            cuiPanel2.Controls.Add(label9);
            cuiPanel2.Controls.Add(label10);
            cuiPanel2.Controls.Add(btnPulisciRam);
            cuiPanel2.Controls.Add(BarRAMtext);
            cuiPanel2.Controls.Add(BarRAM);
            cuiPanel2.Controls.Add(label14);
            resources.ApplyResources(cuiPanel2, "cuiPanel2");
            cuiPanel2.Name = "cuiPanel2";
            cuiPanel2.OutlineThickness = 1F;
            cuiPanel2.PanelColor = Color.FromArgb(37, 38, 39);
            cuiPanel2.PanelOutlineColor = Color.WhiteSmoke;
            cuiPanel2.Rounding = new Padding(8);
            // 
            // limiteRAM
            // 
            resources.ApplyResources(limiteRAM, "limiteRAM");
            limiteRAM.Name = "limiteRAM";
            limiteRAM.Value = new decimal(new int[] { 40, 0, 0, 0 });
            // 
            // puliziaautomaticRAM
            // 
            resources.ApplyResources(puliziaautomaticRAM, "puliziaautomaticRAM");
            puliziaautomaticRAM.Checked = false;
            puliziaautomaticRAM.CheckedBackground = Color.FromArgb(0, 126, 249);
            puliziaautomaticRAM.CheckedForeground = Color.White;
            puliziaautomaticRAM.CheckedOutlineColor = Color.Empty;
            puliziaautomaticRAM.CheckedSymbolColor = Color.FromArgb(0, 126, 249);
            puliziaautomaticRAM.Name = "puliziaautomaticRAM";
            puliziaautomaticRAM.OutlineThickness = 1F;
            puliziaautomaticRAM.ShowSymbols = false;
            puliziaautomaticRAM.ThumbSizeModifier = new Size(0, 0);
            puliziaautomaticRAM.UncheckedBackground = Color.FromArgb(64, 128, 128, 128);
            puliziaautomaticRAM.UncheckedForeground = Color.White;
            puliziaautomaticRAM.UncheckedOutlineColor = Color.Empty;
            puliziaautomaticRAM.UncheckedSymbolColor = Color.Gray;
            // 
            // label8
            // 
            resources.ApplyResources(label8, "label8");
            label8.ForeColor = Color.White;
            label8.Name = "label8";
            // 
            // label9
            // 
            resources.ApplyResources(label9, "label9");
            label9.ForeColor = Color.White;
            label9.Name = "label9";
            // 
            // label10
            // 
            resources.ApplyResources(label10, "label10");
            label10.ForeColor = Color.White;
            label10.Name = "label10";
            // 
            // btnPulisciRam
            // 
            resources.ApplyResources(btnPulisciRam, "btnPulisciRam");
            btnPulisciRam.CheckButton = false;
            btnPulisciRam.Checked = false;
            btnPulisciRam.CheckedBackground = Color.FromArgb(46, 125, 60);
            btnPulisciRam.CheckedForeColor = Color.FromArgb(46, 125, 60);
            btnPulisciRam.CheckedImageTint = Color.FromArgb(46, 125, 60);
            btnPulisciRam.CheckedOutline = Color.FromArgb(46, 125, 60);
            btnPulisciRam.Content = "  Pulizia";
            btnPulisciRam.DialogResult = DialogResult.None;
            btnPulisciRam.ForeColor = Color.White;
            btnPulisciRam.HoverBackground = Color.FromArgb(46, 125, 50);
            btnPulisciRam.HoverForeColor = Color.White;
            btnPulisciRam.HoverImageTint = Color.White;
            btnPulisciRam.HoverOutline = Color.FromArgb(46, 125, 50);
            btnPulisciRam.Image = Properties.Resources.pngPulisciMonitoraggio;
            btnPulisciRam.ImageAutoCenter = true;
            btnPulisciRam.ImageExpand = new Point(0, 0);
            btnPulisciRam.ImageOffset = new Point(0, 0);
            btnPulisciRam.Name = "btnPulisciRam";
            btnPulisciRam.NormalBackground = Color.FromArgb(37, 38, 39);
            btnPulisciRam.NormalForeColor = Color.White;
            btnPulisciRam.NormalImageTint = Color.White;
            btnPulisciRam.NormalOutline = Color.FromArgb(46, 125, 50);
            btnPulisciRam.OutlineThickness = 1F;
            btnPulisciRam.PressedBackground = Color.FromArgb(46, 125, 50);
            btnPulisciRam.PressedForeColor = Color.Black;
            btnPulisciRam.PressedImageTint = Color.Black;
            btnPulisciRam.PressedOutline = Color.FromArgb(46, 125, 50);
            btnPulisciRam.Rounding = new Padding(8);
            btnPulisciRam.TextAlignment = StringAlignment.Center;
            btnPulisciRam.TextOffset = new Point(0, 0);
            btnPulisciRam.Click += btn_pulisciram_Click;
            // 
            // BarRAMtext
            // 
            resources.ApplyResources(BarRAMtext, "BarRAMtext");
            BarRAMtext.ForeColor = Color.White;
            BarRAMtext.Name = "BarRAMtext";
            // 
            // BarRAM
            // 
            resources.ApplyResources(BarRAM, "BarRAM");
            BarRAM.BorderWidth = 12;
            BarRAM.MaximumValue = 100;
            BarRAM.MinimumValue = 0;
            BarRAM.Name = "BarRAM";
            BarRAM.NormalColor = Color.FromArgb(64, 128, 128, 128);
            BarRAM.ProgressColor = Color.FromArgb(0, 126, 249);
            BarRAM.ProgressValue = 20;
            BarRAM.RoundedEnds = true;
            // 
            // label14
            // 
            resources.ApplyResources(label14, "label14");
            label14.ForeColor = Color.FromArgb(0, 126, 249);
            label14.Image = Properties.Resources.pngRAMMonitoraggio;
            label14.Name = "label14";
            // 
            // cuiPanel3
            // 
            cuiPanel3.Controls.Add(label22);
            cuiPanel3.Controls.Add(label23);
            cuiPanel3.Controls.Add(cuiSwitch_gputemperatura);
            cuiPanel3.Controls.Add(label2);
            cuiPanel3.Controls.Add(BarGPUtext);
            cuiPanel3.Controls.Add(BarGPU);
            cuiPanel3.Controls.Add(labelGpuTemp);
            cuiPanel3.Controls.Add(pic_termgpu);
            cuiPanel3.Controls.Add(label19);
            cuiPanel3.Controls.Add(label20);
            resources.ApplyResources(cuiPanel3, "cuiPanel3");
            cuiPanel3.Name = "cuiPanel3";
            cuiPanel3.OutlineThickness = 1F;
            cuiPanel3.PanelColor = Color.FromArgb(37, 38, 39);
            cuiPanel3.PanelOutlineColor = Color.WhiteSmoke;
            cuiPanel3.Rounding = new Padding(8);
            // 
            // label22
            // 
            resources.ApplyResources(label22, "label22");
            label22.ForeColor = Color.White;
            label22.Name = "label22";
            // 
            // label23
            // 
            resources.ApplyResources(label23, "label23");
            label23.ForeColor = Color.White;
            label23.Name = "label23";
            // 
            // cuiSwitch_gputemperatura
            // 
            resources.ApplyResources(cuiSwitch_gputemperatura, "cuiSwitch_gputemperatura");
            cuiSwitch_gputemperatura.Checked = false;
            cuiSwitch_gputemperatura.CheckedBackground = Color.FromArgb(0, 126, 249);
            cuiSwitch_gputemperatura.CheckedForeground = Color.White;
            cuiSwitch_gputemperatura.CheckedOutlineColor = Color.Empty;
            cuiSwitch_gputemperatura.CheckedSymbolColor = Color.FromArgb(0, 126, 249);
            cuiSwitch_gputemperatura.Name = "cuiSwitch_gputemperatura";
            cuiSwitch_gputemperatura.OutlineThickness = 1F;
            cuiSwitch_gputemperatura.ShowSymbols = false;
            cuiSwitch_gputemperatura.ThumbSizeModifier = new Size(0, 0);
            cuiSwitch_gputemperatura.UncheckedBackground = Color.FromArgb(64, 128, 128, 128);
            cuiSwitch_gputemperatura.UncheckedForeground = Color.White;
            cuiSwitch_gputemperatura.UncheckedOutlineColor = Color.Empty;
            cuiSwitch_gputemperatura.UncheckedSymbolColor = Color.Gray;
            cuiSwitch_gputemperatura.CheckedChanged += cuiSwitch2_CheckedChanged;
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.ForeColor = Color.White;
            label2.Name = "label2";
            // 
            // BarGPUtext
            // 
            resources.ApplyResources(BarGPUtext, "BarGPUtext");
            BarGPUtext.ForeColor = Color.White;
            BarGPUtext.Name = "BarGPUtext";
            // 
            // BarGPU
            // 
            resources.ApplyResources(BarGPU, "BarGPU");
            BarGPU.BorderWidth = 12;
            BarGPU.MaximumValue = 100;
            BarGPU.MinimumValue = 0;
            BarGPU.Name = "BarGPU";
            BarGPU.NormalColor = Color.FromArgb(64, 128, 128, 128);
            BarGPU.ProgressColor = Color.FromArgb(0, 126, 249);
            BarGPU.ProgressValue = 20;
            BarGPU.RoundedEnds = true;
            // 
            // labelGpuTemp
            // 
            resources.ApplyResources(labelGpuTemp, "labelGpuTemp");
            labelGpuTemp.ForeColor = Color.White;
            labelGpuTemp.Name = "labelGpuTemp";
            // 
            // pic_termgpu
            // 
            resources.ApplyResources(pic_termgpu, "pic_termgpu");
            pic_termgpu.Image = Properties.Resources.term_verde;
            pic_termgpu.Name = "pic_termgpu";
            pic_termgpu.TabStop = false;
            // 
            // label19
            // 
            resources.ApplyResources(label19, "label19");
            label19.ForeColor = Color.White;
            label19.Name = "label19";
            // 
            // label20
            // 
            resources.ApplyResources(label20, "label20");
            label20.ForeColor = Color.FromArgb(0, 126, 249);
            label20.Image = Properties.Resources.pngGPUMonitoraggio;
            label20.Name = "label20";
            // 
            // cuiPanel4
            // 
            cuiPanel4.Controls.Add(label13);
            cuiPanel4.Controls.Add(labelReteUtilizzo);
            cuiPanel4.Controls.Add(progressbarRete);
            cuiPanel4.Controls.Add(lblDonwload);
            cuiPanel4.Controls.Add(lblUpload);
            cuiPanel4.Controls.Add(labelVelocitaRete);
            cuiPanel4.Controls.Add(label15);
            resources.ApplyResources(cuiPanel4, "cuiPanel4");
            cuiPanel4.Name = "cuiPanel4";
            cuiPanel4.OutlineThickness = 1F;
            cuiPanel4.PanelColor = Color.FromArgb(37, 38, 39);
            cuiPanel4.PanelOutlineColor = Color.WhiteSmoke;
            cuiPanel4.Rounding = new Padding(8);
            // 
            // label13
            // 
            resources.ApplyResources(label13, "label13");
            label13.ForeColor = Color.White;
            label13.Name = "label13";
            // 
            // labelReteUtilizzo
            // 
            resources.ApplyResources(labelReteUtilizzo, "labelReteUtilizzo");
            labelReteUtilizzo.ForeColor = Color.White;
            labelReteUtilizzo.Name = "labelReteUtilizzo";
            // 
            // progressbarRete
            // 
            resources.ApplyResources(progressbarRete, "progressbarRete");
            progressbarRete.BorderWidth = 12;
            progressbarRete.MaximumValue = 100;
            progressbarRete.MinimumValue = 0;
            progressbarRete.Name = "progressbarRete";
            progressbarRete.NormalColor = Color.FromArgb(64, 128, 128, 128);
            progressbarRete.ProgressColor = Color.FromArgb(0, 126, 249);
            progressbarRete.ProgressValue = 20;
            progressbarRete.RoundedEnds = true;
            // 
            // lblDonwload
            // 
            resources.ApplyResources(lblDonwload, "lblDonwload");
            lblDonwload.ForeColor = Color.White;
            lblDonwload.Image = Properties.Resources.pngDownloadMonitoraggio;
            lblDonwload.Name = "lblDonwload";
            // 
            // lblUpload
            // 
            resources.ApplyResources(lblUpload, "lblUpload");
            lblUpload.ForeColor = Color.White;
            lblUpload.Image = Properties.Resources.pngUploadMonitoraggio;
            lblUpload.Name = "lblUpload";
            // 
            // labelVelocitaRete
            // 
            resources.ApplyResources(labelVelocitaRete, "labelVelocitaRete");
            labelVelocitaRete.ForeColor = Color.White;
            labelVelocitaRete.Image = Properties.Resources.pngVelocitaMonitoraggio;
            labelVelocitaRete.Name = "labelVelocitaRete";
            // 
            // label15
            // 
            resources.ApplyResources(label15, "label15");
            label15.ForeColor = Color.FromArgb(0, 126, 249);
            label15.Image = Properties.Resources.pngReteMonitoraggio;
            label15.Name = "label15";
            // 
            // FormMonitoraggio
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(tableLayoutPanel17);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormMonitoraggio";
            FormClosing += FormMonitoraggio_FormClosing;
            Load += FormMonitoraggio_Load;
            tableLayoutPanel17.ResumeLayout(false);
            cuiPanel5.ResumeLayout(false);
            cuiPanel5.PerformLayout();
            cuiPanel1.ResumeLayout(false);
            cuiPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)limiteCPU).EndInit();
            ((System.ComponentModel.ISupportInitialize)pic_termcpu).EndInit();
            cuiPanel2.ResumeLayout(false);
            cuiPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)limiteRAM).EndInit();
            cuiPanel3.ResumeLayout(false);
            cuiPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pic_termgpu).EndInit();
            cuiPanel4.ResumeLayout(false);
            cuiPanel4.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer tempMonitorTimer;
        private TableLayoutPanel tableLayoutPanel17;
        private CuoreUI.Controls.cuiPanel cuiPanel3;
        private Label label2;
        private Label BarGPUtext;
        private CuoreUI.Controls.cuiCircleProgressBar BarGPU;
        private Label labelGpuTemp;
        private PictureBox pic_termgpu;
        private Label label19;
        private Label label20;
        private CuoreUI.Controls.cuiPanel cuiPanel2;
        private NumericUpDown limiteRAM;
        private CuoreUI.Controls.cuiSwitch puliziaautomaticRAM;
        private Label label8;
        private Label label9;
        private Label label10;
        private CuoreUI.Controls.cuiButton cuiButton1;
        private Label BarRAMtext;
        private CuoreUI.Controls.cuiCircleProgressBar BarRAM;
        private Label label14;
        private CuoreUI.Controls.cuiPanel cuiPanel1;
        private NumericUpDown limiteCPU;
        private CuoreUI.Controls.cuiSwitch puliziaautomaticoCPU;
        private Label label7;
        private Label label6;
        private Label label5;
        private CuoreUI.Controls.cuiButton btnPulisciCPU;
        private Label labelCpuTemp;
        private Label BarCPUtext;
        private PictureBox pic_termcpu;
        private CuoreUI.Controls.cuiCircleProgressBar BarCPU;
        private Label label1;
        private Label label4;
        private CuoreUI.Controls.cuiPanel cuiPanel5;
        private Label label21;
        private Label label3;
        private Label label11;
        private Label label12;
        private Label BarDISCOtext;
        private CuoreUI.Controls.cuiCircleProgressBar BarDISCO;
        private Label label18;
        private CuoreUI.Controls.cuiButton btnSvuotaTemp;
        private CuoreUI.Controls.cuiButton btnPulisciRam;        
        private Label BarTEMPtext;
        private CuoreUI.Controls.cuiCircleProgressBar BarTEMP;
        private DomainUpDown domainUpDown1;
        private Label label17;
        private Label label16;
        private CuoreUI.Controls.cuiSwitch cuiSwitch_gradicpu;
        private Label label22;
        private Label label23;
        private CuoreUI.Controls.cuiSwitch cuiSwitch_gputemperatura;
        private CuoreUI.Controls.cuiPanel cuiPanel4;
        private Label label13;
        private Label labelReteUtilizzo;
        private CuoreUI.Controls.cuiCircleProgressBar progressbarRete;
        private Label lblDonwload;
        private Label lblUpload;
        private Label labelVelocitaRete;
        private Label label15;
    }
}