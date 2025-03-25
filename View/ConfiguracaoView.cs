using lerXML.Application.Services;
using lerXML.Modelos;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace lerXML.View
{
    public partial class ConfiguracaoView : Form
    {
        private readonly ConfiguracaoService _configuracaoService;
        private readonly EmailServices _emailService;
        private Configuracao _configuracao;

        public ConfiguracaoView(ConfiguracaoService configuracaoService, EmailServices emailService)
        {
            InitializeComponent();

            _configuracaoService = configuracaoService;
            _emailService = emailService;

            CarregarConfiguracao();
        }

        private void CarregarConfiguracao()
        {
            try
            {
                _configuracao = _configuracaoService.CarregarConfiguracao();

                if (_configuracao.email != null)
                {
                    txtUsuario.Text = _configuracao.email.Usuario;
                    txtSenha.Text = _configuracao.email.Senha;
                    txtServidorSMTP.Text = _configuracao.email.ServidorSMTP;
                    txtPorta.Text = _configuracao.email.Porta.ToString();
                    txtDestinatario.Text = _configuracao.email.Destinatario;
                    txtCopia.Text = _configuracao.email.Copia;
                    txtAssunto.Text = _configuracao.email.Assunto;
                    txtMensagem.Text = _configuracao.email.Mensagem;
                }

                AtualizarGrids();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar configurações: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarGrids()
        {
            AtualizarGrid(dtGridCupons, _configuracao.caminhosFiscais._cuponsFiscais.Select(x => x.caminhoCupom).ToList());
            AtualizarGrid(dtGridNotasFiscais, _configuracao.caminhosFiscais._notaFiscais.Select(x => x.caminhoNota).ToList());
            AtualizarGrid(dtGridNFCEs, _configuracao.caminhosFiscais._nfces.Select(x => x.caminhoNFCE).ToList());
        }

        private void AtualizarGrid(DataGridView grid, List<string> caminhos)
        {
            var tabela = new DataTable();
            tabela.Columns.Add("Caminho"); // Apenas uma coluna

            foreach (var caminho in caminhos)
            {
                tabela.Rows.Add(caminho);
            }

            grid.DataSource = tabela;
            grid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void buttonSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                _configuracao.email = new Email
                {
                    Usuario = txtUsuario.Text,
                    Senha = txtSenha.Text,
                    ServidorSMTP = txtServidorSMTP.Text,
                    Porta = Convert.ToInt32(txtPorta.Text),
                    Destinatario = txtDestinatario.Text,
                    Copia = txtCopia.Text,
                    Assunto = txtAssunto.Text,
                    Mensagem = txtMensagem.Text
                };

                _configuracaoService.SalvarConfiguracao(_configuracao);
                MessageBox.Show("Configurações salvas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar configurações: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEnviarTeste_Click(object sender, EventArgs e)
        {
            try
            {
                _emailService.EnviarEmail(_configuracao.email);
                MessageBox.Show("E-mail de teste enviado!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar e-mail: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 🔹 Eventos corrigidos para chamar as grids corretas
        private void dtGridCupons_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            AbrirDetalhesDiretorio(dtGridCupons, e);
        }

        private void dtGridNotasFiscais_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            AbrirDetalhesDiretorio(dtGridNotasFiscais, e);
        }

        private void dtGridNFCEs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            AbrirDetalhesDiretorio(dtGridNFCEs, e);
        }

        private void AbrirDetalhesDiretorio(DataGridView grid, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && grid.Rows[e.RowIndex].Cells[0].Value != null)
            {
                string caminhoSelecionado = grid.Rows[e.RowIndex].Cells[0].Value.ToString();

                if (Directory.Exists(caminhoSelecionado))
                {
                    var arquivosView = new ArquivosView(caminhoSelecionado);
                    arquivosView.ShowDialog();
                }
                else
                {
                    MessageBox.Show("O diretório selecionado não existe!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelecionaCaminho_Click(object sender, EventArgs e)
        {

        }
    }
}
