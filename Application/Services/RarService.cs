using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lerXML.Application.Services
{
    public class RarService
    {
        private string _7zipPath = @"C:\Program Files\7-Zip\7z.exe";
        private readonly PdfService _pdfService;

        public RarService()
        {
            _pdfService = new PdfService();
        }

        public async Task CompactarDiretorios(string[] diretorios, string arquivoSaida)
        {
            try
            {
                if (diretorios == null || diretorios.Length == 0)
                {
                    MessageBox.Show("Nenhum diretório para compactar.");
                    return;
                }

                if (!File.Exists(_7zipPath))
                {
                    throw new FileNotFoundException("O 7-Zip não foi encontrado. Verifique se está instalado.");
                }

                
                List<string> diretoriosValidos = diretorios.Where(Directory.Exists).ToList();
                if (diretoriosValidos.Count == 0)
                {
                    MessageBox.Show("Nenhum dos diretórios especificados existe.");
                    return;
                }

                
                string raizComum = Path.GetPathRoot(diretoriosValidos[0]); // Obtém a raiz (ex: "Z:\")
                string caminhotolsistemas = "c:\\tolsistemas\\lerXML";
                
                string fileListPath = Path.Combine(caminhotolsistemas, "dirlist.txt");

                using (StreamWriter sw = new StreamWriter(fileListPath))
                {
                    foreach (var diretorio in diretoriosValidos)
                    {
                        //string relativePath = Path.GetRelativePath(raizComum, diretorio);
                        string absolutePath = Path.GetFullPath(diretorio);

                        sw.WriteLine($"\"{absolutePath}\""); // Adicionamos o caminho relativo
                        
                    }
                }

                if (!File.Exists(fileListPath))
                {
                    MessageBox.Show($"⚠ Erro: O arquivo de lista não foi encontrado: {fileListPath}");
                    return;
                }

                // Argumentos do 7-Zip:
                string argumentos = $"a -t7z \"{arquivoSaida}\" @{fileListPath} -mx=9 -spf";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = _7zipPath,
                    Arguments = argumentos,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = raizComum // Define a raiz como diretório de trabalho
                };

                using (Process proc = Process.Start(psi))
                {
                    string output = await proc.StandardOutput.ReadToEndAsync();
                    string error = await proc.StandardError.ReadToEndAsync();
                    await proc.WaitForExitAsync();

                    if (!string.IsNullOrEmpty(error))
                    {
                        MessageBox.Show($"⚠ Erro do 7-Zip: {error}");
                    }
                    else
                    {
                        Console.WriteLine($"✔ Diretórios compactados com sucesso: {arquivoSaida}");
                    }
                }

                // Remove o arquivo temporário após a compactação
                if (File.Exists(fileListPath))
                {
                    File.Delete(fileListPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao compactar diretórios: {ex.Message}","Alerta",MessageBoxButtons.OK);
            }
        }

        public async Task<string> CompactarArquivos(string basePath, int year, int month, string tipoDocumento, string cnpj, string nserieSAT)
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                Console.WriteLine("Caminho para compactação está vazio. Pulando...");
                return "";
            }

            string folderName = $"{year}{month:00}";
            string destinoPasta = Path.Combine(@"C:\tolsistemas\contabil", folderName);

            if (!Directory.Exists(destinoPasta))
            {
                Directory.CreateDirectory(destinoPasta);
            }

            string rarPath = Path.Combine(destinoPasta, _pdfService.GerarNomeArquivo(tipoDocumento, year, month, cnpj, nserieSAT, false));

            List<string> diretoriosDoMes = new List<string>();

            if (tipoDocumento == "NFe")
            {
                string cnpjFolder = Path.GetFileName(basePath);
                string caminhoNotas = Path.Combine(basePath, folderName, "NFe");

                string baseEventoPath = Directory.GetParent(basePath)?.FullName ?? "";
                string caminhoEventos = Path.Combine(baseEventoPath, "evento", cnpjFolder, "NFe", folderName, "Evento", "Cancelamento");

                if (Directory.Exists(caminhoNotas))
                {
                    diretoriosDoMes.Add(caminhoNotas);
                }
                else
                {
                    MessageBox.Show($"⚠ Pasta de notas autorizadas não encontrada: {caminhoNotas}");
                }

                if (Directory.Exists(caminhoEventos))
                {
                    diretoriosDoMes.Add(caminhoEventos);
                }
                else
                {
                    Console.WriteLine($"⚠ Pasta de eventos (canceladas) não encontrada: {caminhoEventos}");
                }
            }
            else if (tipoDocumento == "NFCe")
            {
                string cnpjFolder = Path.GetFileName(basePath);
                string caminhoNotas = Path.Combine(basePath, folderName, "NFCe");

                string baseEventoPath = Directory.GetParent(basePath)?.FullName ?? "";
                string caminhoEventos = Path.Combine(baseEventoPath, "evento", cnpjFolder, "NFCe", folderName, "Evento", "Cancelamento");

                if (Directory.Exists(caminhoNotas))
                {
                    diretoriosDoMes.Add(caminhoNotas);
                }
                else
                {
                    Console.WriteLine($"⚠ Pasta de NFCe autorizadas não encontrada: {caminhoNotas}", "Alerta", MessageBoxButtons.OK);
                }

                if (Directory.Exists(caminhoEventos))
                {
                    diretoriosDoMes.Add(caminhoEventos);
                }
                else
                {
                    Console.WriteLine($"⚠ Pasta de eventos (canceladas) não encontrada: {caminhoEventos}", "Alerta", MessageBoxButtons.OK);
                }
            }
            else
            {
                string[] subPastas = { "Vendas", "Cancelamentos" };

                foreach (string subPasta in subPastas)
                {
                    string caminhoSubPasta = Path.Combine(basePath, subPasta, cnpj, folderName);

                    if (Directory.Exists(caminhoSubPasta))
                    {
                        diretoriosDoMes.Add(caminhoSubPasta);
                    }
                    else
                    {
                        Console.WriteLine($"⚠ Pasta {subPasta} não encontrada: {caminhoSubPasta}");
                    }
                }
            }
            if (diretoriosDoMes.Count == 0)
            {
                MessageBox.Show($"Nenhum arquivo encontrado para compactar no mês {folderName}.");
                return destinoPasta;
            }

            CompactarDiretorios(diretoriosDoMes.ToArray(), rarPath);
            return destinoPasta;
        }

        /*private async Task<string> CompactarArquivosPorPeriodo(string basePath, int year, int month, string tipoDocumento, string cnpj, string nserieSAT, DateTime dataInicial, DateTime dataFinal)
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                Console.WriteLine("Caminho para compactação está vazio. Pulando...");
                return "";
            }

            string folderName = $"{year}{month:00}";
            string destinoPasta = Path.Combine(@"C:\tolsistemas\contabil", folderName);

            if (!Directory.Exists(destinoPasta))
            {
                Directory.CreateDirectory(destinoPasta);
            }

            int totalArquivos = 0;

            string rarPath = Path.Combine(destinoPasta, _pdfService.GerarNomeArquivo(tipoDocumento, year, month, cnpj, nserieSAT, false));

            bool isNotaFiscal = Directory.Exists(Path.Combine(basePath, "NFe"));
            bool isNFCe = Directory.Exists(Path.Combine(basePath, "NFCe"));

            List<string> arquivosParaCompactar = new List<string>();

            if (isNotaFiscal)
            {
                string cnpjFolder = Path.GetFileName(basePath);
                string caminhoNotas = Path.Combine(basePath, "NFe", folderName, "NFE");

                string baseEventoPath = Directory.GetParent(basePath)?.FullName ?? "";
                string caminhoEventos = Path.Combine(baseEventoPath, "evento", cnpjFolder, "NFe", folderName, "Evento", "Cancelamento");

                if (Directory.Exists(caminhoNotas))
                {
                    arquivosParaCompactar.AddRange(FiltrarArquivosPorPeriodo(caminhoNotas, dataInicial, dataFinal, ref totalArquivos));
                }
                else
                {
                    Console.WriteLine($"⚠ Pasta de notas autorizadas não encontrada: {caminhoNotas}");
                }

                // 🔹 Adiciona a pasta de eventos (cancelamentos), se existir
                if (Directory.Exists(caminhoEventos))
                {
                    arquivosParaCompactar.AddRange(FiltrarArquivosPorPeriodo(caminhoEventos, dataInicial, dataFinal, ref totalArquivos));
                }
                else
                {
                    Console.WriteLine($"⚠ Pasta de eventos (canceladas) não encontrada: {caminhoEventos}");
                }
            }
            else if (isNFCe)
            {
                string cnpjFolder = Path.GetFileName(basePath);
                string caminhoNotas = Path.Combine(basePath, "NFCe", folderName, "NFCe");

                string baseEventoPath = Directory.GetParent(basePath)?.FullName ?? "";
                string caminhoEventos = Path.Combine(baseEventoPath, "evento", cnpjFolder, "NFCe", folderName, "Evento", "Cancelamento");

                // 🔹 Adiciona a pasta de notas autorizadas, se existir
                if (Directory.Exists(caminhoNotas))
                {
                    arquivosParaCompactar.AddRange(FiltrarArquivosPorPeriodo(caminhoNotas, dataInicial, dataFinal, ref totalArquivos));
                }
                else
                {
                    Console.WriteLine($"⚠ Pasta de NFCe autorizadas não encontrada: {caminhoNotas}");
                }

                // 🔹 Adiciona a pasta de eventos (canceladas), se existir
                if (Directory.Exists(caminhoEventos))
                {
                    arquivosParaCompactar.AddRange(FiltrarArquivosPorPeriodo(caminhoEventos, dataInicial, dataFinal, ref totalArquivos));
                }
                else
                {
                    Console.WriteLine($"⚠ Pasta de eventos (canceladas) não encontrada: {caminhoEventos}");
                }
            }
            else
            {
                string[] subPastas = { "Vendas", "Cancelamentos" };

                foreach (string subPasta in subPastas)
                {
                    string caminhoSubPasta = Path.Combine(basePath, subPasta, cnpj, folderName);

                    if (Directory.Exists(caminhoSubPasta))
                    {
                        arquivosParaCompactar.AddRange(FiltrarArquivosPorPeriodo(caminhoSubPasta, dataInicial, dataFinal, ref totalArquivos));
                    }
                    else
                    {
                        Console.WriteLine($"⚠ Pasta {subPasta} não encontrada: {caminhoSubPasta}");
                    }
                }
            }
            if (arquivosParaCompactar.Count == 0)
            {
                Console.WriteLine($"Nenhum arquivo encontrado para compactar no mês {folderName}.");
                return destinoPasta;
            }

            await _rarService.CompactarDiretorios(arquivosParaCompactar.ToArray(), rarPath);
            return destinoPasta;
        }*/
    }
}
