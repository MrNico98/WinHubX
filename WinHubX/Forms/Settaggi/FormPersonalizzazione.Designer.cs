namespace WinHubX.Forms.Settaggi
{
    partial class FormPersonalizzazione
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPersonalizzazione));
            panel23 = new Panel();
            radio_mostraoradata = new RadioButton();
            radio_nascondioradata = new RadioButton();
            radio_orologiostandard = new RadioButton();
            radio_mostradatasecondi = new RadioButton();
            radio_mostrasecondi = new RadioButton();
            label4 = new Label();
            btnAvviaSelezionati = new Button();
            panel24 = new Panel();
            panel14 = new Panel();
            radio_abilitaendtask = new RadioButton();
            radio_disabilitaendtask = new RadioButton();
            panel5 = new Panel();
            radio_apripowershell = new RadioButton();
            radio_eliminapowershell = new RadioButton();
            panel4 = new Panel();
            radio_apricmd = new RadioButton();
            radio_eliminaapricmd = new RadioButton();
            panel32 = new Panel();
            radio_destrolegacy = new RadioButton();
            radio_destrodefault = new RadioButton();
            label1 = new Label();
            panel7 = new Panel();
            panel10 = new Panel();
            radio_abilicopilot = new RadioButton();
            radio_disacopilot = new RadioButton();
            panel6 = new Panel();
            radio_abilitarecall = new RadioButton();
            radio_disabilitarecall = new RadioButton();
            panel13 = new Panel();
            radio_attivafx = new RadioButton();
            radio_disattivafx = new RadioButton();
            panel9 = new Panel();
            radio_abilitasuggeriti = new RadioButton();
            radio_disabilitasuggeriti = new RadioButton();
            panel11 = new Panel();
            radio_disabilitaricercainternet = new RadioButton();
            panel12 = new Panel();
            radio_ripristinaottimizzazionewin = new RadioButton();
            radio_ottimizzawindows = new RadioButton();
            label2 = new Label();
            panel8 = new Panel();
            radio_ottimizzaricerca = new RadioButton();
            btnBack = new Button();
            btn_resetselezione = new Button();
            label3 = new Label();
            label5 = new Label();
            progressBar1 = new ProgressBar();
            toolTip1 = new ToolTip(components);
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            btnSettaggiExplorer = new Button();
            panel23.SuspendLayout();
            panel24.SuspendLayout();
            panel14.SuspendLayout();
            panel5.SuspendLayout();
            panel4.SuspendLayout();
            panel32.SuspendLayout();
            panel7.SuspendLayout();
            panel10.SuspendLayout();
            panel6.SuspendLayout();
            panel13.SuspendLayout();
            panel9.SuspendLayout();
            panel11.SuspendLayout();
            panel12.SuspendLayout();
            panel8.SuspendLayout();
            SuspendLayout();
            // 
            // panel23
            // 
            resources.ApplyResources(panel23, "panel23");
            panel23.Controls.Add(radio_mostraoradata);
            panel23.Controls.Add(radio_nascondioradata);
            panel23.Controls.Add(radio_orologiostandard);
            panel23.Controls.Add(radio_mostradatasecondi);
            panel23.Controls.Add(radio_mostrasecondi);
            panel23.Controls.Add(label4);
            panel23.Name = "panel23";
            // 
            // radio_mostraoradata
            // 
            resources.ApplyResources(radio_mostraoradata, "radio_mostraoradata");
            radio_mostraoradata.ForeColor = Color.White;
            radio_mostraoradata.Name = "radio_mostraoradata";
            radio_mostraoradata.TabStop = true;
            toolTip1.SetToolTip(radio_mostraoradata, resources.GetString("radio_mostraoradata.ToolTip"));
            radio_mostraoradata.UseVisualStyleBackColor = true;
            // 
            // radio_nascondioradata
            // 
            resources.ApplyResources(radio_nascondioradata, "radio_nascondioradata");
            radio_nascondioradata.ForeColor = Color.White;
            radio_nascondioradata.Name = "radio_nascondioradata";
            radio_nascondioradata.TabStop = true;
            toolTip1.SetToolTip(radio_nascondioradata, resources.GetString("radio_nascondioradata.ToolTip"));
            radio_nascondioradata.UseVisualStyleBackColor = true;
            // 
            // radio_orologiostandard
            // 
            resources.ApplyResources(radio_orologiostandard, "radio_orologiostandard");
            radio_orologiostandard.ForeColor = Color.White;
            radio_orologiostandard.Name = "radio_orologiostandard";
            radio_orologiostandard.TabStop = true;
            toolTip1.SetToolTip(radio_orologiostandard, resources.GetString("radio_orologiostandard.ToolTip"));
            radio_orologiostandard.UseVisualStyleBackColor = true;
            // 
            // radio_mostradatasecondi
            // 
            resources.ApplyResources(radio_mostradatasecondi, "radio_mostradatasecondi");
            radio_mostradatasecondi.ForeColor = Color.White;
            radio_mostradatasecondi.Name = "radio_mostradatasecondi";
            radio_mostradatasecondi.TabStop = true;
            toolTip1.SetToolTip(radio_mostradatasecondi, resources.GetString("radio_mostradatasecondi.ToolTip"));
            radio_mostradatasecondi.UseVisualStyleBackColor = true;
            // 
            // radio_mostrasecondi
            // 
            resources.ApplyResources(radio_mostrasecondi, "radio_mostrasecondi");
            radio_mostrasecondi.ForeColor = Color.White;
            radio_mostrasecondi.Name = "radio_mostrasecondi";
            radio_mostrasecondi.TabStop = true;
            toolTip1.SetToolTip(radio_mostrasecondi, resources.GetString("radio_mostrasecondi.ToolTip"));
            radio_mostrasecondi.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.ForeColor = Color.Coral;
            label4.Name = "label4";
            // 
            // btnAvviaSelezionati
            // 
            resources.ApplyResources(btnAvviaSelezionati, "btnAvviaSelezionati");
            btnAvviaSelezionati.Cursor = Cursors.Hand;
            btnAvviaSelezionati.FlatAppearance.BorderSize = 0;
            btnAvviaSelezionati.ForeColor = Color.White;
            btnAvviaSelezionati.Name = "btnAvviaSelezionati";
            btnAvviaSelezionati.UseVisualStyleBackColor = true;
            btnAvviaSelezionati.Click += btnAvviaSelezionati_Click;
            // 
            // panel24
            // 
            resources.ApplyResources(panel24, "panel24");
            panel24.Controls.Add(panel14);
            panel24.Controls.Add(panel5);
            panel24.Controls.Add(panel4);
            panel24.Controls.Add(panel32);
            panel24.Controls.Add(label1);
            panel24.Name = "panel24";
            // 
            // panel14
            // 
            panel14.Controls.Add(radio_abilitaendtask);
            panel14.Controls.Add(radio_disabilitaendtask);
            resources.ApplyResources(panel14, "panel14");
            panel14.Name = "panel14";
            // 
            // radio_abilitaendtask
            // 
            resources.ApplyResources(radio_abilitaendtask, "radio_abilitaendtask");
            radio_abilitaendtask.ForeColor = Color.White;
            radio_abilitaendtask.Name = "radio_abilitaendtask";
            radio_abilitaendtask.TabStop = true;
            toolTip1.SetToolTip(radio_abilitaendtask, resources.GetString("radio_abilitaendtask.ToolTip"));
            radio_abilitaendtask.UseVisualStyleBackColor = true;
            // 
            // radio_disabilitaendtask
            // 
            resources.ApplyResources(radio_disabilitaendtask, "radio_disabilitaendtask");
            radio_disabilitaendtask.ForeColor = Color.White;
            radio_disabilitaendtask.Name = "radio_disabilitaendtask";
            radio_disabilitaendtask.TabStop = true;
            toolTip1.SetToolTip(radio_disabilitaendtask, resources.GetString("radio_disabilitaendtask.ToolTip"));
            radio_disabilitaendtask.UseVisualStyleBackColor = true;
            // 
            // panel5
            // 
            panel5.Controls.Add(radio_apripowershell);
            panel5.Controls.Add(radio_eliminapowershell);
            resources.ApplyResources(panel5, "panel5");
            panel5.Name = "panel5";
            // 
            // radio_apripowershell
            // 
            resources.ApplyResources(radio_apripowershell, "radio_apripowershell");
            radio_apripowershell.ForeColor = Color.White;
            radio_apripowershell.Name = "radio_apripowershell";
            radio_apripowershell.TabStop = true;
            toolTip1.SetToolTip(radio_apripowershell, resources.GetString("radio_apripowershell.ToolTip"));
            radio_apripowershell.UseVisualStyleBackColor = true;
            // 
            // radio_eliminapowershell
            // 
            resources.ApplyResources(radio_eliminapowershell, "radio_eliminapowershell");
            radio_eliminapowershell.ForeColor = Color.White;
            radio_eliminapowershell.Name = "radio_eliminapowershell";
            radio_eliminapowershell.TabStop = true;
            toolTip1.SetToolTip(radio_eliminapowershell, resources.GetString("radio_eliminapowershell.ToolTip"));
            radio_eliminapowershell.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            panel4.Controls.Add(radio_apricmd);
            panel4.Controls.Add(radio_eliminaapricmd);
            resources.ApplyResources(panel4, "panel4");
            panel4.Name = "panel4";
            // 
            // radio_apricmd
            // 
            resources.ApplyResources(radio_apricmd, "radio_apricmd");
            radio_apricmd.ForeColor = Color.White;
            radio_apricmd.Name = "radio_apricmd";
            radio_apricmd.TabStop = true;
            toolTip1.SetToolTip(radio_apricmd, resources.GetString("radio_apricmd.ToolTip"));
            radio_apricmd.UseVisualStyleBackColor = true;
            // 
            // radio_eliminaapricmd
            // 
            resources.ApplyResources(radio_eliminaapricmd, "radio_eliminaapricmd");
            radio_eliminaapricmd.ForeColor = Color.White;
            radio_eliminaapricmd.Name = "radio_eliminaapricmd";
            radio_eliminaapricmd.TabStop = true;
            toolTip1.SetToolTip(radio_eliminaapricmd, resources.GetString("radio_eliminaapricmd.ToolTip"));
            radio_eliminaapricmd.UseVisualStyleBackColor = true;
            // 
            // panel32
            // 
            panel32.Controls.Add(radio_destrolegacy);
            panel32.Controls.Add(radio_destrodefault);
            resources.ApplyResources(panel32, "panel32");
            panel32.Name = "panel32";
            // 
            // radio_destrolegacy
            // 
            resources.ApplyResources(radio_destrolegacy, "radio_destrolegacy");
            radio_destrolegacy.ForeColor = Color.White;
            radio_destrolegacy.Name = "radio_destrolegacy";
            radio_destrolegacy.TabStop = true;
            toolTip1.SetToolTip(radio_destrolegacy, resources.GetString("radio_destrolegacy.ToolTip"));
            radio_destrolegacy.UseVisualStyleBackColor = true;
            // 
            // radio_destrodefault
            // 
            resources.ApplyResources(radio_destrodefault, "radio_destrodefault");
            radio_destrodefault.ForeColor = Color.White;
            radio_destrodefault.Name = "radio_destrodefault";
            radio_destrodefault.TabStop = true;
            toolTip1.SetToolTip(radio_destrodefault, resources.GetString("radio_destrodefault.ToolTip"));
            radio_destrodefault.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.Coral;
            label1.Name = "label1";
            // 
            // panel7
            // 
            resources.ApplyResources(panel7, "panel7");
            panel7.Controls.Add(panel10);
            panel7.Controls.Add(panel6);
            panel7.Controls.Add(panel13);
            panel7.Controls.Add(panel9);
            panel7.Controls.Add(panel11);
            panel7.Controls.Add(panel12);
            panel7.Controls.Add(label2);
            panel7.Controls.Add(panel8);
            panel7.Name = "panel7";
            // 
            // panel10
            // 
            panel10.Controls.Add(radio_abilicopilot);
            panel10.Controls.Add(radio_disacopilot);
            resources.ApplyResources(panel10, "panel10");
            panel10.Name = "panel10";
            // 
            // radio_abilicopilot
            // 
            resources.ApplyResources(radio_abilicopilot, "radio_abilicopilot");
            radio_abilicopilot.ForeColor = Color.White;
            radio_abilicopilot.Name = "radio_abilicopilot";
            radio_abilicopilot.TabStop = true;
            toolTip1.SetToolTip(radio_abilicopilot, resources.GetString("radio_abilicopilot.ToolTip"));
            radio_abilicopilot.UseVisualStyleBackColor = true;
            // 
            // radio_disacopilot
            // 
            resources.ApplyResources(radio_disacopilot, "radio_disacopilot");
            radio_disacopilot.ForeColor = Color.White;
            radio_disacopilot.Name = "radio_disacopilot";
            radio_disacopilot.TabStop = true;
            toolTip1.SetToolTip(radio_disacopilot, resources.GetString("radio_disacopilot.ToolTip"));
            radio_disacopilot.UseVisualStyleBackColor = true;
            // 
            // panel6
            // 
            panel6.Controls.Add(radio_abilitarecall);
            panel6.Controls.Add(radio_disabilitarecall);
            resources.ApplyResources(panel6, "panel6");
            panel6.Name = "panel6";
            // 
            // radio_abilitarecall
            // 
            resources.ApplyResources(radio_abilitarecall, "radio_abilitarecall");
            radio_abilitarecall.ForeColor = Color.White;
            radio_abilitarecall.Name = "radio_abilitarecall";
            radio_abilitarecall.TabStop = true;
            toolTip1.SetToolTip(radio_abilitarecall, resources.GetString("radio_abilitarecall.ToolTip"));
            radio_abilitarecall.UseVisualStyleBackColor = true;
            // 
            // radio_disabilitarecall
            // 
            resources.ApplyResources(radio_disabilitarecall, "radio_disabilitarecall");
            radio_disabilitarecall.ForeColor = Color.White;
            radio_disabilitarecall.Name = "radio_disabilitarecall";
            radio_disabilitarecall.TabStop = true;
            toolTip1.SetToolTip(radio_disabilitarecall, resources.GetString("radio_disabilitarecall.ToolTip"));
            radio_disabilitarecall.UseVisualStyleBackColor = true;
            // 
            // panel13
            // 
            panel13.Controls.Add(radio_attivafx);
            panel13.Controls.Add(radio_disattivafx);
            resources.ApplyResources(panel13, "panel13");
            panel13.Name = "panel13";
            // 
            // radio_attivafx
            // 
            resources.ApplyResources(radio_attivafx, "radio_attivafx");
            radio_attivafx.ForeColor = Color.White;
            radio_attivafx.Name = "radio_attivafx";
            radio_attivafx.TabStop = true;
            toolTip1.SetToolTip(radio_attivafx, resources.GetString("radio_attivafx.ToolTip"));
            radio_attivafx.UseVisualStyleBackColor = true;
            // 
            // radio_disattivafx
            // 
            resources.ApplyResources(radio_disattivafx, "radio_disattivafx");
            radio_disattivafx.ForeColor = Color.White;
            radio_disattivafx.Name = "radio_disattivafx";
            radio_disattivafx.TabStop = true;
            toolTip1.SetToolTip(radio_disattivafx, resources.GetString("radio_disattivafx.ToolTip"));
            radio_disattivafx.UseVisualStyleBackColor = true;
            // 
            // panel9
            // 
            panel9.Controls.Add(radio_abilitasuggeriti);
            panel9.Controls.Add(radio_disabilitasuggeriti);
            resources.ApplyResources(panel9, "panel9");
            panel9.Name = "panel9";
            // 
            // radio_abilitasuggeriti
            // 
            resources.ApplyResources(radio_abilitasuggeriti, "radio_abilitasuggeriti");
            radio_abilitasuggeriti.ForeColor = Color.White;
            radio_abilitasuggeriti.Name = "radio_abilitasuggeriti";
            radio_abilitasuggeriti.TabStop = true;
            toolTip1.SetToolTip(radio_abilitasuggeriti, resources.GetString("radio_abilitasuggeriti.ToolTip"));
            radio_abilitasuggeriti.UseVisualStyleBackColor = true;
            // 
            // radio_disabilitasuggeriti
            // 
            resources.ApplyResources(radio_disabilitasuggeriti, "radio_disabilitasuggeriti");
            radio_disabilitasuggeriti.ForeColor = Color.White;
            radio_disabilitasuggeriti.Name = "radio_disabilitasuggeriti";
            radio_disabilitasuggeriti.TabStop = true;
            toolTip1.SetToolTip(radio_disabilitasuggeriti, resources.GetString("radio_disabilitasuggeriti.ToolTip"));
            radio_disabilitasuggeriti.UseVisualStyleBackColor = true;
            // 
            // panel11
            // 
            panel11.Controls.Add(radio_disabilitaricercainternet);
            resources.ApplyResources(panel11, "panel11");
            panel11.Name = "panel11";
            // 
            // radio_disabilitaricercainternet
            // 
            resources.ApplyResources(radio_disabilitaricercainternet, "radio_disabilitaricercainternet");
            radio_disabilitaricercainternet.ForeColor = Color.White;
            radio_disabilitaricercainternet.Name = "radio_disabilitaricercainternet";
            radio_disabilitaricercainternet.TabStop = true;
            toolTip1.SetToolTip(radio_disabilitaricercainternet, resources.GetString("radio_disabilitaricercainternet.ToolTip"));
            radio_disabilitaricercainternet.UseVisualStyleBackColor = true;
            // 
            // panel12
            // 
            panel12.Controls.Add(radio_ripristinaottimizzazionewin);
            panel12.Controls.Add(radio_ottimizzawindows);
            resources.ApplyResources(panel12, "panel12");
            panel12.Name = "panel12";
            // 
            // radio_ripristinaottimizzazionewin
            // 
            resources.ApplyResources(radio_ripristinaottimizzazionewin, "radio_ripristinaottimizzazionewin");
            radio_ripristinaottimizzazionewin.ForeColor = Color.White;
            radio_ripristinaottimizzazionewin.Name = "radio_ripristinaottimizzazionewin";
            radio_ripristinaottimizzazionewin.TabStop = true;
            radio_ripristinaottimizzazionewin.UseVisualStyleBackColor = true;
            // 
            // radio_ottimizzawindows
            // 
            resources.ApplyResources(radio_ottimizzawindows, "radio_ottimizzawindows");
            radio_ottimizzawindows.ForeColor = Color.White;
            radio_ottimizzawindows.Name = "radio_ottimizzawindows";
            radio_ottimizzawindows.TabStop = true;
            radio_ottimizzawindows.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.ForeColor = Color.Coral;
            label2.Name = "label2";
            // 
            // panel8
            // 
            panel8.Controls.Add(radio_ottimizzaricerca);
            resources.ApplyResources(panel8, "panel8");
            panel8.Name = "panel8";
            // 
            // radio_ottimizzaricerca
            // 
            resources.ApplyResources(radio_ottimizzaricerca, "radio_ottimizzaricerca");
            radio_ottimizzaricerca.ForeColor = Color.White;
            radio_ottimizzaricerca.Name = "radio_ottimizzaricerca";
            radio_ottimizzaricerca.TabStop = true;
            radio_ottimizzaricerca.UseVisualStyleBackColor = true;
            // 
            // btnBack
            // 
            btnBack.Cursor = Cursors.Hand;
            btnBack.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(btnBack, "btnBack");
            btnBack.Image = Properties.Resources.pngBackArrow;
            btnBack.Name = "btnBack";
            btnBack.UseMnemonic = false;
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            // 
            // btn_resetselezione
            // 
            resources.ApplyResources(btn_resetselezione, "btn_resetselezione");
            btn_resetselezione.Cursor = Cursors.Hand;
            btn_resetselezione.FlatAppearance.BorderSize = 0;
            btn_resetselezione.ForeColor = Color.White;
            btn_resetselezione.Name = "btn_resetselezione";
            btn_resetselezione.UseVisualStyleBackColor = true;
            btn_resetselezione.Click += btn_resetselezione_Click;
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = Color.Coral;
            label3.Name = "label3";
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.ForeColor = Color.Coral;
            label5.Name = "label5";
            // 
            // progressBar1
            // 
            resources.ApplyResources(progressBar1, "progressBar1");
            progressBar1.Name = "progressBar1";
            progressBar1.Style = ProgressBarStyle.Continuous;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // btnSettaggiExplorer
            // 
            resources.ApplyResources(btnSettaggiExplorer, "btnSettaggiExplorer");
            btnSettaggiExplorer.Cursor = Cursors.Hand;
            btnSettaggiExplorer.FlatAppearance.BorderSize = 0;
            btnSettaggiExplorer.ForeColor = Color.White;
            btnSettaggiExplorer.Name = "btnSettaggiExplorer";
            btnSettaggiExplorer.UseVisualStyleBackColor = true;
            btnSettaggiExplorer.Click += btnSettaggiExplorer_Click;
            // 
            // FormPersonalizzazione
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(btnSettaggiExplorer);
            Controls.Add(progressBar1);
            Controls.Add(label5);
            Controls.Add(label3);
            Controls.Add(btn_resetselezione);
            Controls.Add(btnBack);
            Controls.Add(panel7);
            Controls.Add(panel24);
            Controls.Add(btnAvviaSelezionati);
            Controls.Add(panel23);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormPersonalizzazione";
            Load += FormPersonalizzazione_Load;
            panel23.ResumeLayout(false);
            panel23.PerformLayout();
            panel24.ResumeLayout(false);
            panel24.PerformLayout();
            panel14.ResumeLayout(false);
            panel14.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel32.ResumeLayout(false);
            panel32.PerformLayout();
            panel7.ResumeLayout(false);
            panel7.PerformLayout();
            panel10.ResumeLayout(false);
            panel10.PerformLayout();
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            panel13.ResumeLayout(false);
            panel13.PerformLayout();
            panel9.ResumeLayout(false);
            panel9.PerformLayout();
            panel11.ResumeLayout(false);
            panel11.PerformLayout();
            panel12.ResumeLayout(false);
            panel12.PerformLayout();
            panel8.ResumeLayout(false);
            panel8.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel23;
        private Label label4;
        private RadioButton radio_orologiostandard;
        private RadioButton radio_mostradatasecondi;
        private RadioButton radio_mostrasecondi;
        private RadioButton radio_mostraoradata;
        private RadioButton radio_nascondioradata;
        private Button btnAvviaSelezionati;
        private Panel panel24;
        private Panel panel32;
        private Label label1;
        private RadioButton radio_destrodefault;
        private RadioButton radio_destrolegacy;
        private Panel panel5;
        private RadioButton radio_apripowershell;
        private RadioButton radio_eliminapowershell;
        private Panel panel4;
        private RadioButton radio_apricmd;
        private RadioButton radio_eliminaapricmd;
        private Panel panel7;
        private Panel panel8;
        private RadioButton radio_ottimizzaricerca;
        private Panel panel9;
        private RadioButton radio_abilitasuggeriti;
        private RadioButton radio_disabilitasuggeriti;
        private Panel panel11;
        private RadioButton radio_disabilitaricercainternet;
        private Label label2;
        private Button btnBack;
        private Button btn_resetselezione;
        private Panel panel6;
        private RadioButton radio_abilitarecall;
        private RadioButton radio_disabilitarecall;
        private Panel panel12;
        private RadioButton radio_ottimizzawindows;
        private RadioButton radio_ripristinaottimizzazionewin;
        private Panel panel13;
        private RadioButton radio_attivafx;
        private RadioButton radio_disattivafx;
        private Label label3;
        private Label label5;
        private ProgressBar progressBar1;
        private Panel panel10;
        private RadioButton radio_abilicopilot;
        private RadioButton radio_disacopilot;
        private ToolTip toolTip1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Panel panel14;
        private RadioButton radio_abilitaendtask;
        private RadioButton radio_disabilitaendtask;
        private Button btnSettaggiExplorer;
    }
}