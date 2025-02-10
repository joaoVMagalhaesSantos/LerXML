namespace lerXML.View
{
    partial class Periodo
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
            label1 = new Label();
            txtCalendario = new MonthCalendar();
            label2 = new Label();
            label3 = new Label();
            txtInicial = new MaskedTextBox();
            txtFinal = new MaskedTextBox();
            button1 = new Button();
            button2 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(224, 15);
            label1.TabIndex = 0;
            label1.Text = "Selecione o periodo para gerar o relatório";
            label1.Click += label1_Click;
            // 
            // txtCalendario
            // 
            txtCalendario.Location = new Point(18, 161);
            txtCalendario.MaxSelectionCount = 31;
            txtCalendario.Name = "txtCalendario";
            txtCalendario.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 68);
            label2.Name = "label2";
            label2.Size = new Size(41, 15);
            label2.TabIndex = 2;
            label2.Text = "Inicial:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 105);
            label3.Name = "label3";
            label3.Size = new Size(35, 15);
            label3.TabIndex = 3;
            label3.Text = "Final:";
            // 
            // txtInicial
            // 
            txtInicial.Location = new Point(59, 65);
            txtInicial.Mask = "00/00/0000";
            txtInicial.Name = "txtInicial";
            txtInicial.Size = new Size(100, 23);
            txtInicial.TabIndex = 4;
            txtInicial.ValidatingType = typeof(DateTime);
            txtInicial.Leave += txtInicial_Leave;
            // 
            // txtFinal
            // 
            txtFinal.Location = new Point(59, 102);
            txtFinal.Mask = "00/00/0000";
            txtFinal.Name = "txtFinal";
            txtFinal.Size = new Size(100, 23);
            txtFinal.TabIndex = 5;
            txtFinal.ValidatingType = typeof(DateTime);
            txtFinal.Leave += txtFinal_Leave;
            // 
            // button1
            // 
            button1.Location = new Point(18, 335);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 6;
            button1.Text = "Confirmar";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_ClickAsync;
            // 
            // button2
            // 
            button2.Location = new Point(170, 335);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 7;
            button2.Text = "Sair";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // Periodo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(265, 416);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(txtFinal);
            Controls.Add(txtInicial);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(txtCalendario);
            Controls.Add(label1);
            Name = "Periodo";
            Text = "Periodo";
            Load += Periodo_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private MonthCalendar txtCalendario;
        private Label label2;
        private Label label3;
        private MaskedTextBox txtInicial;
        private MaskedTextBox txtFinal;
        private Button button1;
        private Button button2;
    }
}