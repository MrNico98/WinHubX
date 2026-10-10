using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;

namespace WinHubX.Forms.Base
{
    partial class ProgressForm
    {
        private ProgressBar progressBar;
        private Label lblStatus;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
            progressBar = new ProgressBar();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // progressBar
            // 
            resources.ApplyResources(progressBar, "progressBar");
            progressBar.Name = "progressBar";
            // 
            // lblStatus
            // 
            resources.ApplyResources(lblStatus, "lblStatus");
            lblStatus.ForeColor = Color.WhiteSmoke;
            lblStatus.Name = "lblStatus";
            // 
            // ProgressForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(64, 60, 59);
            Controls.Add(lblStatus);
            Controls.Add(progressBar);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "ProgressForm";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
