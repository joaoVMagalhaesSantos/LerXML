namespace lerXML.View
{
    partial class ArquivosView
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
            dtGridArquivos = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dtGridArquivos).BeginInit();
            SuspendLayout();
            // 
            // dtGridArquivos
            // 
            dtGridArquivos.AllowUserToAddRows = false;
            dtGridArquivos.AllowUserToDeleteRows = false;
            dtGridArquivos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dtGridArquivos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dtGridArquivos.Dock = DockStyle.Fill;
            dtGridArquivos.Location = new Point(0, 0);
            dtGridArquivos.Name = "dtGridArquivos";
            dtGridArquivos.Size = new Size(800, 450);
            dtGridArquivos.TabIndex = 0;
            dtGridArquivos.CellDoubleClick += dtGridArquivos_CellDoubleClick;
            // 
            // ArquivosView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dtGridArquivos);
            Name = "ArquivosView";
            Text = "ArquivosView";
            Load += ArquivosView_Load;
            ((System.ComponentModel.ISupportInitialize)dtGridArquivos).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dtGridArquivos;
    }
}