using lerXML.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static lerXML.Interface.IExtratorDocumento;

namespace lerXML.Application.Services
{
    public class ArquivoService
    {
        private readonly XmlServices _servicesXML;

        public ArquivoService()
        {
            _servicesXML = new XmlServices();
        }
        public bool VerificarCaminhoAcessivel(string caminho)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(caminho)) // 🔹 Se estiver vazio, retorna verdadeiro para seguir o fluxo
                    return true;

                if (Directory.Exists(caminho))
                {
                    Directory.GetFiles(caminho); // Tenta acessar os arquivos para testar a conexão
                    return true;
                }
            }
            catch (IOException) // Exceção comum para redes inacessíveis
            {
                return false;
            }
            catch (UnauthorizedAccessException) // Caso não tenha permissão
            {
                return false;
            }
            return false;
        }

        public void ApagarArquivosGerados(string pastaDestino)
        {
            try
            {
                if (Directory.Exists(pastaDestino))
                {
                    // Deleta apenas arquivos .pdf e .rar
                    foreach (string arquivo in Directory.GetFiles(pastaDestino, "*.*"))
                    {
                        if (arquivo.EndsWith(".pdf") || arquivo.EndsWith(".rar"))
                        {
                            File.Delete(arquivo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao apagar arquivos: {ex.Message}");
            }
        }

        /*
        public string SelecionarCaminho()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;

                return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : string.Empty;
            }
        }
        public DataTable ListaArquivos(DataTable tabela, string caminho)
        {
            var diretorios = Directory.GetDirectories(caminho);

            foreach (var dir in diretorios)
            {
                tabela.Rows.Add(new DirectoryInfo(dir).Name, "Pasta", "-");
            }

            var arquivos = Directory.GetFiles(caminho);

            foreach (var arq in arquivos)
            {
                FileInfo file = new FileInfo(arq);
                tabela.Rows.Add(file.Name, file.Extension, file.Length / 1024);
            }

            return tabela;
        }
        public DataTable ListaCaminhos(DataTable tabela, string caminho)
        {
            var diretorios = Directory.GetDirectories(caminho);

            foreach (var dir in diretorios)
            {
                tabela.Rows.Add(new DirectoryInfo(dir).Name, "Pasta", "-");
            }

            var arquivos = Directory.GetDirectories(caminho);

            foreach (var arq in arquivos)
            {
                tabela.Rows.Add(arq);
            }

            return tabela;
        }
        public List<T> LerDocumentos<T>(string caminhoAutorizado, string caminhoCancelado, DataGridView dataGrid, IExtratorDocumento<T> extrator)
        {
            List<T> documentos = new List<T>();

            for (int i = 0; i < dataGrid.Rows.Count - 1; i++)
            {
                var row = dataGrid.Rows[i];

                string nomeArquivo = row.Cells[0].Value?.ToString();

                if (string.IsNullOrEmpty(nomeArquivo))
                {
                    MessageBox.Show("O nome do arquivo está vazio ou nulo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                string caminhoArquivoAutorizado = Path.Combine(caminhoAutorizado, nomeArquivo);
                string caminhoArquivoCancelado = Path.Combine(caminhoCancelado, nomeArquivo);
                try
                {
                    if (File.Exists(caminhoArquivoAutorizado))
                    {
                        XDocument xml = XDocument.Load(caminhoArquivoAutorizado);
                        documentos.AddRange(extrator.Extrair(xml, nomeArquivo));
                    }
                    if (File.Exists(caminhoArquivoCancelado))
                    {
                        XDocument xml = XDocument.Load(caminhoArquivoCancelado);
                        documentos.AddRange(extrator.Extrair(xml, nomeArquivo));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao processar {nomeArquivo}: {ex.Message}");
                }
            }

            return documentos;
        }
        */

    }

}
