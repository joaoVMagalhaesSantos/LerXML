using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace lerXML.View
{
    public partial class ProgressoView : Form
    {
        public BackgroundWorker Worker { get; private set; }
        private Func<Task> _processamento;

        public ProgressoView(Func<Task> processamento)
        {
            InitializeComponent();
            _processamento = processamento;
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            try
            {
                await _processamento();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void AtualizarProgresso(int valor, string mensagem)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AtualizarProgresso(valor, mensagem)));
            }
            else
            {
                progressBar.Value = valor;
                lblStatus.Text = mensagem;
                progressBar.Refresh();
            }
        }

        private void ProgressoView_Load(object sender, EventArgs e)
        {

        }
    }
}
