using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lerXML.View
{
    public partial class ArquivosView : Form
    {
        public ArquivosView(string caminho)
        {
            InitializeComponent();
            CarregarArquivos(caminho);
        }

        private void ArquivosView_Load(object sender, EventArgs e)
        {

        }

        private void CarregarArquivos(string caminho)
        {
            if (!Directory.Exists(caminho))
            {
                MessageBox.Show($"Diretório não encontrado: {caminho}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var tabela = new DataTable();
            tabela.Columns.Add("Arquivo");

            foreach (var arquivo in Directory.GetDirectories(caminho))
            {
                tabela.Rows.Add(arquivo); // Adiciona apenas o nome do arquivo
            }
            if (Directory.GetDirectories(caminho).Length == 0)
            {
                foreach (var arquivo in Directory.GetFiles(caminho))
                {
                    tabela.Rows.Add(arquivo); // Adiciona apenas o nome do arquivo
                }
            }

            dtGridArquivos.DataSource = tabela;
        }

        private void dtGridArquivos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dtGridArquivos.Rows[e.RowIndex].Cells[0].Value != null)
            {
                string caminhoSelecionado = dtGridArquivos.Rows[e.RowIndex].Cells[0].Value.ToString();
                MostrarArquivos(caminhoSelecionado);
            }
        }

        private void MostrarArquivos(string caminho)
        {
            var arquivosView = new ArquivosView(caminho);
            arquivosView.ShowDialog();
        }
    }
}
