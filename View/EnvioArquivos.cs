using lerXML.Classes;
using lerXML.Extratores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static lerXML.Interface.IExtratorDocumento;
using System.Xml.Linq;
using System.Reflection.Emit;
using lerXML.Application.Services;

namespace lerXML.View
{
    public partial class EnvioArquivos : Form
    {
        private readonly RelatorioService _relatorioService;

        public EnvioArquivos(RelatorioService relatorioService)
        {
            InitializeComponent();
            _relatorioService = relatorioService;
            int year;

            year = DateTime.Now.Year;

            comboBox1.Text = year.ToString();

            PreencheGrid(year);
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void PreencheGrid(int year)
        {
            dataGridView1.Rows.Clear();
            string caminhoPadraoContabil = @"C:\tolsistemas\contabil";
            string arquivoTXT = "status.txt";
            int month;

            if (Directory.Exists(caminhoPadraoContabil))
            {
                for (int i = 1; i <= 12; i++)
                {
                    month = i;

                    string folderName = $"{year}{month:00}";

                    string caminho = Path.Combine(caminhoPadraoContabil, folderName);

                    if (Directory.Exists(caminho))
                    {
                        if (File.Exists(Path.Combine(caminho, arquivoTXT)))
                        {
                            string conteudo = File.ReadAllText(Path.Combine(caminho, arquivoTXT));

                            if (conteudo.Length >= 3) // Garante que há pelo menos 3 caracteres
                            {
                                if (conteudo.Substring(0, 3) == "OK:")
                                {
                                    string nomeMes = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);

                                    int rowIndex = dataGridView1.Rows.Add(nomeMes.ToUpper());
                                    dataGridView1.Rows[rowIndex].Cells["Column4"].Value = true;
                                }
                            }
                            else
                            {
                                string nomeMes = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);

                                int rowIndex = dataGridView1.Rows.Add(nomeMes.ToUpper());
                                dataGridView1.Rows[rowIndex].Cells["Column4"].Value = false;
                            }
                        }
                        else
                        {
                            File.Create(Path.Combine(caminho, arquivoTXT));
                        }
                    }
                    else
                    {
                        string nomeMes = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);

                        int rowIndex = dataGridView1.Rows.Add(nomeMes.ToUpper());
                        dataGridView1.Rows[rowIndex].Cells["Column4"].Value = false;
                    }
                }
            }
            else
            {
                MessageBox.Show("Pasta Contabil não encontrada!", "Alerta", MessageBoxButtons.OK);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string jsonFilePath = @"C:\\tolsistemas\\lerXML\\configEnvArq.json";

            var jsonServices = new JsonServices(jsonFilePath);

            var configuracaoService = new ConfiguracaoService(jsonFilePath);
            var emailService = new EmailServices(jsonServices);

            ConfiguracaoView configView = new ConfiguracaoView(configuracaoService, emailService);
            configView.ShowDialog();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int month;
            int year = Convert.ToInt32(comboBox1.Text);
            int day = 1;

            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                month = e.RowIndex + 1;

                var progressoForm = new ProgressoView(null);

                progressoForm = new ProgressoView(async () =>
                {
                    await _relatorioService.GerarRelatorioMensal(year, month, progressoForm.AtualizarProgresso);
                });

                progressoForm.ShowDialog();
            }

            if (e.ColumnIndex == 2 && e.RowIndex >= 0)
            {
                month = e.RowIndex + 1;

                Periodo periodo = new Periodo(year, month, day, _relatorioService);
                periodo.ShowDialog();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            PreencheGrid(Convert.ToInt32(comboBox1.Text));
        }
    }
}
