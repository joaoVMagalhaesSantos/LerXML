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
            PreencheGrid();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void PreencheGrid()
        {
            for (int i = 1; i <= 12; i++)
            {
                string nomeMes = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);
                dataGridView1.Rows.Add(nomeMes.ToUpper());
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

        private async void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int month;
            int year;
            int day = 1;
            
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                month = e.RowIndex + 1;
                year = DateTime.Now.Year;

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
                year = DateTime.Now.Year;
                


                Periodo periodo = new Periodo(year, month, day, _relatorioService);
                periodo.ShowDialog();
                
            }
        }
    }
}
