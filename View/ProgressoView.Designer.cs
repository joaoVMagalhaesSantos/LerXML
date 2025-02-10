namespace lerXML.View
{
    partial class ProgressoView
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
            lblStatus = new Label();
            progressBar = new ProgressBar();
            SuspendLayout();
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(12, 9);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(83, 15);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "Processando...";
            // 
            // progressBar
            // 
            progressBar.Location = new Point(12, 27);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(618, 23);
            progressBar.TabIndex = 1;
            // 
            // ProgressoView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(642, 67);
            Controls.Add(progressBar);
            Controls.Add(lblStatus);
            Name = "ProgressoView";
            Text = "ProgressoView";
            Load += ProgressoView_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblStatus;
        private ProgressBar progressBar;
    }
}