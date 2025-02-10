using lerXML.Application.Services;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lerXML.View
{
    public partial class Periodo : Form
    {
        private int year;
        private int month;
        private int day;

        private readonly RelatorioService _relatorioService;
        public Periodo(int ano, int mes, int dia, RelatorioService relatorioService)
        {
            InitializeComponent();

            year = ano;
            month = mes;
            day = dia;
            _relatorioService = relatorioService;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtInicial_Leave(object sender, EventArgs e)
        {
            AtualizarCalendario();
        }

        private void txtFinal_Leave(object sender, EventArgs e)
        {
            AtualizarCalendario();
        }

        private void AtualizarCalendario()
        {
            if (DateTime.TryParseExact(txtInicial.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataInicial) &&
                DateTime.TryParseExact(txtFinal.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataFinal))
            {
                if (dataInicial <= dataFinal)
                {
                    txtCalendario.SelectionStart = dataInicial;
                    txtCalendario.SelectionEnd = dataFinal;
                    txtCalendario.SetSelectionRange(dataInicial,dataFinal);
                }
                else
                {
                    MessageBox.Show("A data inicial deve ser menor ou igual à data final.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            DateTime dtInicial = Convert.ToDateTime(txtInicial.Text);
            DateTime dtFinal = Convert.ToDateTime(txtFinal.Text);

            var progressoForm = new ProgressoView(null);

            progressoForm = new ProgressoView(async () =>
            {
                await _relatorioService.GerarRelatorioPorPeriodo(dtInicial, dtFinal, progressoForm.AtualizarProgresso);
            });

            progressoForm.ShowDialog();

        }

        private void Periodo_Load(object sender, EventArgs e)
        {
            DateTime dataInicial = new DateTime(year,month,day);
            DateTime datafinal = new DateTime(year,month,day);

            txtInicial.Text = dataInicial.ToString("dd/MM/yyyy");
            txtFinal.Text = datafinal.ToString("dd/MM/yyyy");

            txtCalendario.SelectionStart = dataInicial;
            txtCalendario.SelectionEnd = datafinal;
        }
    }
}
