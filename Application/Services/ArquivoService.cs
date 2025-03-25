using lerXML.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
                if (string.IsNullOrWhiteSpace(caminho))
                    return true;

                if (Directory.Exists(caminho))
                {
                    Directory.GetFiles(caminho); 
                    return true;
                }
            }
            catch (IOException) 
            {
                return false;
            }
            catch (UnauthorizedAccessException) 
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

        public void AbrirArquivosPDF(string pastaArquivos)
        {
            if (!Directory.Exists(pastaArquivos))
            {
                Console.WriteLine("A pasta especificada não existe.");
                return;
            }

            string[] arquivos = Directory.GetFiles(pastaArquivos, "*.pdf"); // Filtro direto para PDF

            foreach (string arquivo in arquivos)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = arquivo,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao abrir o arquivo {arquivo}: {ex.Message}", "Alerta", MessageBoxButtons.OK);
                }
            }
        }

        public List<string> BuscarArquivosXMLPorPeriodo(string basePath, string folderName, string[] subFolders, int indicePasta, int totalPastas, DateTime dataInicial, DateTime dataFinal)
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                Console.WriteLine("BasePath está vazio. Nenhum XML será buscado.");
                return new List<string>();
            }

            List<string> xmlFilesDoCaminho = new List<string>();
            int totalArquivos = 0;

            if (subFolders.Length == 4 && subFolders.Contains("NFe"))
            {
                string cnpj = Path.GetFileName(basePath);
                string nfeFolderPath = Path.Combine(basePath, "NFe", folderName, "NFE");

                string baseEventoPath = Directory.GetParent(basePath).FullName;
                string eventoFolderPath = Path.Combine(baseEventoPath, "evento", cnpj, "NFe", folderName, "Evento", "Cancelamento");

                xmlFilesDoCaminho.AddRange(FiltrarArquivosPorPeriodo(nfeFolderPath, dataInicial, dataFinal, ref totalArquivos));
                xmlFilesDoCaminho.AddRange(FiltrarArquivosPorPeriodo(eventoFolderPath, dataInicial, dataFinal, ref totalArquivos));
            }
            else if (subFolders.Length == 4 && subFolders.Contains("NFCe"))
            {
                string cnpj = Path.GetFileName(basePath);
                string nfceFolderPath = Path.Combine(basePath, "NFCe", folderName, "NFCE");

                string baseEventoPath = Directory.GetParent(basePath).FullName;
                string eventoFolderPath = Path.Combine(baseEventoPath, "evento", cnpj, "NFCe", folderName, "Evento", "Cancelamento");

                xmlFilesDoCaminho.AddRange(FiltrarArquivosPorPeriodo(nfceFolderPath, dataInicial, dataFinal, ref totalArquivos));
                xmlFilesDoCaminho.AddRange(FiltrarArquivosPorPeriodo(eventoFolderPath, dataInicial, dataFinal, ref totalArquivos));
            }
            else
            {
                foreach (string subFolder in subFolders)
                {
                    string subFolderPath = Path.Combine(basePath, subFolder);

                    if (!Directory.Exists(subFolderPath))
                    {
                        Console.WriteLine($"⚠ Subpasta não encontrada: {subFolderPath} (Ignorando)");
                        continue;
                    }

                    string[] cnpjFolders = Directory.GetDirectories(subFolderPath);
                    foreach (string cnpjFolder in cnpjFolders)
                    {
                        string monthFolderPath = Path.Combine(cnpjFolder, folderName);

                        if (!Directory.Exists(monthFolderPath))
                        {
                            Console.WriteLine($"⚠ Pasta do mês '{folderName}' não encontrada em: {cnpjFolder} (Ignorando)");
                            continue;
                        }

                        xmlFilesDoCaminho.AddRange(FiltrarArquivosPorPeriodo(monthFolderPath, dataInicial, dataFinal, ref totalArquivos));
                    }
                }
            }

            Console.WriteLine($"Total de arquivos filtrados por período ({dataInicial:dd/MM/yyyy} - {dataFinal:dd/MM/yyyy}): {totalArquivos}");
            return xmlFilesDoCaminho;
        }

        private List<string> FiltrarArquivosPorPeriodo(string caminho, DateTime dataInicial, DateTime dataFinal, ref int totalArquivos)
        {
            List<string> arquivosFiltrados = new List<string>();

            if (Directory.Exists(caminho))
            {
                string[] arquivos = Directory.GetFiles(caminho, "*.xml");

                foreach (string arquivo in arquivos)
                {
                    DateTime dataCriacao = File.GetLastWriteTime(arquivo);

                    if (dataCriacao.Date >= dataInicial && dataCriacao.Date <= dataFinal)
                    {
                        arquivosFiltrados.Add(arquivo);
                        totalArquivos++;
                    }
                }
            }
            else
            {
                Console.WriteLine($"Pasta não encontrada: {caminho}");
            }

            return arquivosFiltrados;
        }

        public List<string> BuscarArquivosXML(string basePath, string folderName, string[] subFolders, int indicePasta, int totalPastas)
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                Console.WriteLine("BasePath está vazio. Nenhum XML será buscado.");
                return new List<string>(); // Retorna uma lista vazia ao invés de quebrar
            }

            List<string> xmlFilesDoCaminho = new List<string>();
            int totalArquivos = 0;

            if (subFolders.Length == 4 && subFolders.Contains("NFe"))
            {
                string ultimaPasta = Path.GetDirectoryName(basePath);
                string cnpj = Path.GetFileName(ultimaPasta);

                string nfeFolderPath = Path.Combine(basePath, folderName, "NFe");

                string baseEventoPath = Directory.GetParent(ultimaPasta).FullName;
                string eventoFolderPath = Path.Combine(baseEventoPath, "evento", cnpj, "NFe", folderName, "Evento", "Cancelamento");

                if (Directory.Exists(nfeFolderPath))
                {
                    string[] arquivos = Directory.GetFiles(nfeFolderPath, "*.xml");
                    xmlFilesDoCaminho.AddRange(arquivos);
                    totalArquivos += arquivos.Length;

                }
                else
                {
                    Console.WriteLine($"Pasta de autorizados não encontrada: {nfeFolderPath}");
                }

                if (Directory.Exists(eventoFolderPath))
                {
                    string[] arquivosEvento = Directory.GetFiles(eventoFolderPath, "*.xml");
                    xmlFilesDoCaminho.AddRange(arquivosEvento);
                    totalArquivos += arquivosEvento.Length;
                }
                else
                {
                    Console.WriteLine($"Pasta de eventos não encontrada: {eventoFolderPath}");
                }
            }
            else if (subFolders.Length == 4 && subFolders.Contains("NFCe"))
            {
                string cnpj = Path.GetFileName(basePath);
                string nfceFolderPath = Path.Combine(basePath, folderName, "NFCe");

                string baseEventoPath = Directory.GetParent(basePath).FullName;
                string eventoFolderPath = Path.Combine(baseEventoPath, "evento", cnpj, "NFCe", folderName, "Evento", "Cancelamento");

                if (Directory.Exists(nfceFolderPath))
                {
                    string[] arquivos = Directory.GetFiles(nfceFolderPath, "*.xml");
                    xmlFilesDoCaminho.AddRange(arquivos);
                    totalArquivos += arquivos.Length;

                }
                else
                {
                    Console.WriteLine($"Pasta de autorizados não encontrada: {nfceFolderPath}");
                }

                if (Directory.Exists(eventoFolderPath))
                {
                    string[] arquivosEvento = Directory.GetFiles(eventoFolderPath, "*.xml");
                    xmlFilesDoCaminho.AddRange(arquivosEvento);
                    totalArquivos += arquivosEvento.Length;
                }
                else
                {
                    Console.WriteLine($"Pasta de eventos não encontrada: {eventoFolderPath}");
                }
            }
            else
            {
                foreach (string subFolder in subFolders)
                {
                    string subFolderPath = Path.Combine(basePath, subFolder);

                    if (!Directory.Exists(subFolderPath))
                    {
                        Console.WriteLine($"⚠ Subpasta não encontrada: {subFolderPath} (Ignorando)");
                        continue;
                    }

                    string[] cnpjFolders = Directory.GetDirectories(subFolderPath);
                    foreach (string cnpjFolder in cnpjFolders)
                    {
                        string monthFolderPath = Path.Combine(cnpjFolder, folderName);

                        if (!Directory.Exists(monthFolderPath))
                        {
                            Console.WriteLine($"⚠ Pasta do mês '{folderName}' não encontrada em: {cnpjFolder} (Ignorando)");
                            continue;
                        }

                        string[] arquivos = Directory.GetFiles(monthFolderPath, "*.xml");
                        xmlFilesDoCaminho.AddRange(arquivos);
                        totalArquivos += arquivos.Length;
                    }
                }
            }
            return xmlFilesDoCaminho;
        }
    }
}
