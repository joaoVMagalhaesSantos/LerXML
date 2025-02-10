namespace lerXML.View
{
    partial class EnvioArquivos
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
            Button button1;
            dataGridView1 = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewButtonColumn();
            Column3 = new DataGridViewButtonColumn();
            Column4 = new DataGridViewCheckBoxColumn();
            Column5 = new DataGridViewTextBoxColumn();
            label1 = new Label();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(524, 65);
            button1.Name = "button1";
            button1.Size = new Size(106, 34);
            button1.TabIndex = 2;
            button1.Text = "Configuração";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2, Column3, Column4, Column5 });
            dataGridView1.Location = new Point(0, 0);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(489, 340);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellClick += dataGridView1_CellClick;
            // 
            // Column1
            // 
            Column1.HeaderText = "Mês Competencia";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            Column1.Width = 117;
            // 
            // Column2
            // 
            Column2.HeaderText = "Fechamento Total";
            Column2.Name = "Column2";
            Column2.Text = "Enviar";
            Column2.UseColumnTextForButtonValue = true;
            Column2.Width = 96;
            // 
            // Column3
            // 
            Column3.HeaderText = "Fechamento Parcial";
            Column3.Name = "Column3";
            Column3.ReadOnly = true;
            Column3.Text = "Enviar";
            Column3.UseColumnTextForButtonValue = true;
            Column3.Width = 105;
            // 
            // Column4
            // 
            Column4.HeaderText = "Check Total";
            Column4.Name = "Column4";
            Column4.Width = 67;
            // 
            // Column5
            // 
            Column5.HeaderText = "Historico";
            Column5.Name = "Column5";
            Column5.Resizable = DataGridViewTriState.True;
            Column5.SortMode = DataGridViewColumnSortMode.NotSortable;
            Column5.Width = 61;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(507, 6);
            label1.Name = "label1";
            label1.Size = new Size(148, 21);
            label1.TabIndex = 1;
            label1.Text = "Envio de Arquivos";
            // 
            // EnvioArquivos
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(690, 342);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(dataGridView1);
            Name = "EnvioArquivos";
            Text = "Form2";
            Load += Form2_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Label label1;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewButtonColumn Column2;
        private DataGridViewButtonColumn Column3;
        private DataGridViewCheckBoxColumn Column4;
        private DataGridViewTextBoxColumn Column5;
    }
}