namespace lerXML.View
{
    partial class ConfiguracaoView
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
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            txtArqConfig = new TextBox();
            btnSelecionaCaminho = new Button();
            txtUsuario = new TextBox();
            txtSenha = new TextBox();
            txtServidorSMTP = new TextBox();
            txtPorta = new TextBox();
            txtDestinatario = new TextBox();
            txtCopia = new TextBox();
            txtAssunto = new TextBox();
            txtMensagem = new TextBox();
            txtAnexo = new TextBox();
            button2 = new Button();
            btnSalvar = new Button();
            button4 = new Button();
            dtGridCupons = new DataGridView();
            dtGridNotasFiscais = new DataGridView();
            label11 = new Label();
            label12 = new Label();
            label13 = new Label();
            dtGridNFCEs = new DataGridView();
            btnEnviarTeste = new Button();
            ((System.ComponentModel.ISupportInitialize)dtGridCupons).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dtGridNotasFiscais).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dtGridNFCEs).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(205, 15);
            label1.TabIndex = 0;
            label1.Text = "Caminho do arquivo de configuração";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 81);
            label2.Name = "label2";
            label2.Size = new Size(98, 15);
            label2.TabIndex = 1;
            label2.Text = "Email de Origem:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(336, 81);
            label3.Name = "label3";
            label3.Size = new Size(39, 15);
            label3.TabIndex = 2;
            label3.Text = "Senha";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 125);
            label4.Name = "label4";
            label4.Size = new Size(83, 15);
            label4.TabIndex = 3;
            label4.Text = "Servidor SMTP";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(336, 125);
            label5.Name = "label5";
            label5.Size = new Size(35, 15);
            label5.TabIndex = 4;
            label5.Text = "Porta";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 190);
            label6.Name = "label6";
            label6.Size = new Size(119, 15);
            label6.TabIndex = 5;
            label6.Text = "Email do Destinatario";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 234);
            label7.Name = "label7";
            label7.Size = new Size(86, 15);
            label7.TabIndex = 6;
            label7.Text = "Email de Copia";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(12, 278);
            label8.Name = "label8";
            label8.Size = new Size(50, 15);
            label8.TabIndex = 7;
            label8.Text = "Assunto";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(336, 190);
            label9.Name = "label9";
            label9.Size = new Size(66, 15);
            label9.TabIndex = 8;
            label9.Text = "Mensagem";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(12, 322);
            label10.Name = "label10";
            label10.Size = new Size(167, 15);
            label10.TabIndex = 9;
            label10.Text = "Caminho do arquivo de anexo";
            // 
            // txtArqConfig
            // 
            txtArqConfig.Location = new Point(12, 27);
            txtArqConfig.Name = "txtArqConfig";
            txtArqConfig.Size = new Size(707, 23);
            txtArqConfig.TabIndex = 10;
            // 
            // btnSelecionaCaminho
            // 
            btnSelecionaCaminho.Enabled = false;
            btnSelecionaCaminho.Location = new Point(725, 27);
            btnSelecionaCaminho.Name = "btnSelecionaCaminho";
            btnSelecionaCaminho.Size = new Size(40, 23);
            btnSelecionaCaminho.TabIndex = 11;
            btnSelecionaCaminho.Text = "...";
            btnSelecionaCaminho.UseVisualStyleBackColor = true;
            btnSelecionaCaminho.Click += btnSelecionaCaminho_Click;
            // 
            // txtUsuario
            // 
            txtUsuario.Location = new Point(12, 99);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(289, 23);
            txtUsuario.TabIndex = 12;
            // 
            // txtSenha
            // 
            txtSenha.Location = new Point(336, 99);
            txtSenha.Name = "txtSenha";
            txtSenha.Size = new Size(195, 23);
            txtSenha.TabIndex = 13;
            // 
            // txtServidorSMTP
            // 
            txtServidorSMTP.Location = new Point(12, 143);
            txtServidorSMTP.Name = "txtServidorSMTP";
            txtServidorSMTP.Size = new Size(289, 23);
            txtServidorSMTP.TabIndex = 14;
            // 
            // txtPorta
            // 
            txtPorta.Location = new Point(336, 143);
            txtPorta.Name = "txtPorta";
            txtPorta.Size = new Size(195, 23);
            txtPorta.TabIndex = 15;
            // 
            // txtDestinatario
            // 
            txtDestinatario.Location = new Point(12, 208);
            txtDestinatario.Name = "txtDestinatario";
            txtDestinatario.Size = new Size(289, 23);
            txtDestinatario.TabIndex = 16;
            // 
            // txtCopia
            // 
            txtCopia.Location = new Point(12, 252);
            txtCopia.Name = "txtCopia";
            txtCopia.Size = new Size(289, 23);
            txtCopia.TabIndex = 17;
            // 
            // txtAssunto
            // 
            txtAssunto.Location = new Point(12, 296);
            txtAssunto.Name = "txtAssunto";
            txtAssunto.Size = new Size(289, 23);
            txtAssunto.TabIndex = 18;
            // 
            // txtMensagem
            // 
            txtMensagem.Location = new Point(336, 208);
            txtMensagem.Multiline = true;
            txtMensagem.Name = "txtMensagem";
            txtMensagem.Size = new Size(383, 155);
            txtMensagem.TabIndex = 19;
            // 
            // txtAnexo
            // 
            txtAnexo.Location = new Point(12, 340);
            txtAnexo.Name = "txtAnexo";
            txtAnexo.Size = new Size(243, 23);
            txtAnexo.TabIndex = 20;
            // 
            // button2
            // 
            button2.Location = new Point(261, 340);
            button2.Name = "button2";
            button2.Size = new Size(40, 23);
            button2.TabIndex = 21;
            button2.Text = "...";
            button2.UseVisualStyleBackColor = true;
            // 
            // btnSalvar
            // 
            btnSalvar.Location = new Point(23, 485);
            btnSalvar.Name = "btnSalvar";
            btnSalvar.Size = new Size(75, 23);
            btnSalvar.TabIndex = 22;
            btnSalvar.Text = "Salvar";
            btnSalvar.UseVisualStyleBackColor = true;
            btnSalvar.Click += buttonSalvar_Click;
            // 
            // button4
            // 
            button4.Location = new Point(104, 485);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 23;
            button4.Text = "Sair";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // dtGridCupons
            // 
            dtGridCupons.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dtGridCupons.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dtGridCupons.EditMode = DataGridViewEditMode.EditOnEnter;
            dtGridCupons.Location = new Point(771, 27);
            dtGridCupons.Name = "dtGridCupons";
            dtGridCupons.Size = new Size(595, 204);
            dtGridCupons.TabIndex = 24;
            dtGridCupons.CellDoubleClick += dtGridCupons_CellDoubleClick;
            // 
            // dtGridNotasFiscais
            // 
            dtGridNotasFiscais.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dtGridNotasFiscais.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dtGridNotasFiscais.EditMode = DataGridViewEditMode.EditOnEnter;
            dtGridNotasFiscais.Location = new Point(771, 252);
            dtGridNotasFiscais.Name = "dtGridNotasFiscais";
            dtGridNotasFiscais.Size = new Size(595, 212);
            dtGridNotasFiscais.TabIndex = 25;
            dtGridNotasFiscais.CellDoubleClick += dtGridNotasFiscais_CellDoubleClick;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(771, 9);
            label11.Name = "label11";
            label11.Size = new Size(85, 15);
            label11.TabIndex = 26;
            label11.Text = "Cupons Fiscais";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(771, 234);
            label12.Name = "label12";
            label12.Size = new Size(75, 15);
            label12.TabIndex = 27;
            label12.Text = "Notas Fiscais";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(771, 479);
            label13.Name = "label13";
            label13.Size = new Size(41, 15);
            label13.TabIndex = 28;
            label13.Text = "NFCEs";
            // 
            // dtGridNFCEs
            // 
            dtGridNFCEs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dtGridNFCEs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dtGridNFCEs.EditMode = DataGridViewEditMode.EditOnEnter;
            dtGridNFCEs.Location = new Point(771, 497);
            dtGridNFCEs.Name = "dtGridNFCEs";
            dtGridNFCEs.Size = new Size(595, 210);
            dtGridNFCEs.TabIndex = 29;
            dtGridNFCEs.CellDoubleClick += dtGridNFCEs_CellDoubleClick;
            // 
            // btnEnviarTeste
            // 
            btnEnviarTeste.Location = new Point(644, 369);
            btnEnviarTeste.Name = "btnEnviarTeste";
            btnEnviarTeste.Size = new Size(75, 23);
            btnEnviarTeste.TabIndex = 30;
            btnEnviarTeste.Text = "Email Teste";
            btnEnviarTeste.UseVisualStyleBackColor = true;
            btnEnviarTeste.Click += btnEnviarTeste_Click;
            // 
            // ConfiguracaoView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1378, 719);
            Controls.Add(btnEnviarTeste);
            Controls.Add(dtGridNFCEs);
            Controls.Add(label13);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(dtGridNotasFiscais);
            Controls.Add(dtGridCupons);
            Controls.Add(button4);
            Controls.Add(btnSalvar);
            Controls.Add(button2);
            Controls.Add(txtAnexo);
            Controls.Add(txtMensagem);
            Controls.Add(txtAssunto);
            Controls.Add(txtCopia);
            Controls.Add(txtDestinatario);
            Controls.Add(txtPorta);
            Controls.Add(txtServidorSMTP);
            Controls.Add(txtSenha);
            Controls.Add(txtUsuario);
            Controls.Add(btnSelecionaCaminho);
            Controls.Add(txtArqConfig);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "ConfiguracaoView";
            Text = "Configuracao";
            WindowState = FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)dtGridCupons).EndInit();
            ((System.ComponentModel.ISupportInitialize)dtGridNotasFiscais).EndInit();
            ((System.ComponentModel.ISupportInitialize)dtGridNFCEs).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private TextBox txtArqConfig;
        private Button btnSelecionaCaminho;
        private TextBox txtUsuario;
        private TextBox txtSenha;
        private TextBox txtServidorSMTP;
        private TextBox txtPorta;
        private TextBox txtDestinatario;
        private TextBox txtCopia;
        private TextBox txtAssunto;
        private TextBox txtMensagem;
        private TextBox txtAnexo;
        private Button button2;
        private Button btnSalvar;
        private Button button4;
        private DataGridView dtGridCupons;
        private DataGridView dtGridNotasFiscais;
        private Label label11;
        private Label label12;
        private Label label13;
        private DataGridView dtGridNFCEs;
        private Button btnEnviarTeste;
    }
}