using lerXML.Classes;
using lerXML.Extratores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static lerXML.Interface.IExtratorDocumento;
using System.Xml.Linq;
using System.ComponentModel;
using lerXML.Interface;
using lerXML.Modelos;

namespace lerXML.Application.Services
{
    public class RelatorioService
    {
        private readonly JsonServices _jsonServices;
        private readonly PdfService _pdfService;
        private readonly HtmlGenerator _htmlGenerator;
        private readonly RarService _rarService;
        private readonly EmailServices _emailService;
        private readonly ArquivoService _arquivoService;
        public RelatorioService(JsonServices jsonServices, PdfService pdfService, HtmlGenerator htmlGenerator, RarService rarService, EmailServices emailService, ArquivoService arquivoService)
        {
            _jsonServices = jsonServices;
            _pdfService = pdfService;
            _htmlGenerator = htmlGenerator;
            _rarService = rarService;
            _emailService = emailService;
            _arquivoService = arquivoService;
        }

        public async Task GerarRelatorioMensal(int year, int month, Action<int, string> atualizarProgresso)
        {
            try
            {
                string folderName = $"{year}{month:00}";

                atualizarProgresso(10, "Lendo arquivos XML...");
                await Task.Delay(500);

                Dictionary<string, string[]> documentosFiscais = new Dictionary<string, string[]>
                {
                    { "_cuponsFiscais", new string[] { "Vendas", "Cancelamentos" } },  // Cupons tem subpastas
                    { "_notaFiscais", new string[] { "CNPJ", "NFe", "AAAAMM", "NFE" } }, // Notas Fiscais têm subpastas
                    { "_nfces", new string[] { "CNPJ", "NFCe", "AAAAMM", "NFCe" } } // NFCes têm subpastas
                };

                List<Task<string>> tarefasDeCompactacao = new List<Task<string>>();

                foreach (var docTipo in documentosFiscais)
                {
                    string tipoDocumento = docTipo.Key;
                    string[] subFolders = docTipo.Value;

                    atualizarProgresso(20, $"Processando {tipoDocumento}...");
                    await Task.Delay(500);

                    List<string> basePaths = _jsonServices.PegarCaminhosFiscais(tipoDocumento);

                    if (basePaths == null || basePaths.Count == 0)
                    {
                        Console.WriteLine($"Nenhum caminho configurado para {tipoDocumento}. Pulando processamento...");
                        continue; // Pula para o próximo tipo de documento
                    }


                    int totalPastas = basePaths.Count;
                    int indicePasta = 0;

                    foreach (string basePath in basePaths)
                    {
                        if (string.IsNullOrWhiteSpace(basePath))
                        {
                            Console.WriteLine($"Caminho vazio detectado para {tipoDocumento}. Pulando...");
                            continue; // Pula para o próximo caminho sem erro
                        }

                        if (!_arquivoService.VerificarCaminhoAcessivel(basePath))
                        {
                            DialogResult result = MessageBox.Show(
                                $"O caminho '{basePath}' está inacessível.\nDeseja continuar a operação?",
                                "Aviso",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning
                            );

                            if (result == DialogResult.No)
                            {
                                string pastaDestinox = Path.Combine(@"C:\tolsistemas\contabil", folderName);
                                _arquivoService.ApagarArquivosGerados(pastaDestinox);

                                MessageBox.Show("Operação cancelada. Os arquivos gerados foram apagados.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }

                        atualizarProgresso(30, "Lendo arquivos XML...");
                        await Task.Delay(500);
                        List<string> xmlFilesDoCaminho = BuscarArquivosXML(basePath, folderName, subFolders, indicePasta, totalPastas);


                        if (xmlFilesDoCaminho.Count == 0)
                        {
                            Console.WriteLine($"Nenhum arquivo XML encontrado para {basePath} no mês {folderName}.");
                            continue;
                        }
                        
                        string html;
                        string pastaDestino;
                        string cnpj = "00000000000000";
                        string nserieSAT = "000000";

                        if (xmlFilesDoCaminho.Count > 0)
                        {
                            XDocument xml = XDocument.Load(xmlFilesDoCaminho.First());
                            (cnpj, nserieSAT) = new ExtratorCupomFiscal().ObterDadosXML(xml);
                        }

                        switch (tipoDocumento)
                        {
                            case "_cuponsFiscais":
                                IExtratorDocumento<CupomFiscal> extratorCupom = new ExtratorCupomFiscal();

                                atualizarProgresso(40, "Extraindo informações...");
                                await Task.Delay(500);
                                
                                var cupons = await ProcessarXMLs(xmlFilesDoCaminho, extratorCupom, indicePasta, totalPastas);
                                html = _htmlGenerator.GerarRelatorioCupons(cupons, "Relatorio completo de Cupons Ficais - SAT");

                                atualizarProgresso(50, "Gerando PDF...");
                                await Task.Delay(500);
                                
                                await _pdfService.GerarPDF(html, year, month, "SAT", cnpj, nserieSAT);
                                
                                tarefasDeCompactacao.Add(CompactarArquivos(basePath, year, month, "SAT", cnpj, nserieSAT));

                                break;

                            case "_notaFiscais":
                                ExtratorNotaFiscal extratorNota = new ExtratorNotaFiscal();
                                
                                atualizarProgresso(40, "Extraindo informações...");
                                await Task.Delay(500);
                                
                                var notas = await ProcessarNotasFiscais(xmlFilesDoCaminho, extratorNota, indicePasta, totalPastas);
                                html = _htmlGenerator.GerarRelatorioNotas(notas, "Relatorio completo de Notas Fiscais - NFe");

                                atualizarProgresso(50, "Gerando PDF...");
                                await Task.Delay(500);
                                
                                await _pdfService.GerarPDF(html, year, month, "NFE", cnpj, "");

                                tarefasDeCompactacao.Add(CompactarArquivos(basePath, year, month, "NFE", cnpj, ""));

                                break;

                            case "_nfces":
                                IExtratorDocumento<NFCE> extratorNFCE = new ExtratorNFCE();
                                
                                atualizarProgresso(40, "Extraindo informações...");
                                await Task.Delay(500);
                                
                                var nfces = await ProcessarXMLs(xmlFilesDoCaminho, extratorNFCE, indicePasta, totalPastas);
                                html = _htmlGenerator.GerarRelatorioCuponsNCFE(nfces, "Relatorio completo de Cupons Ficais - NFCe");

                                atualizarProgresso(50, "Gerando PDF...");
                                await Task.Delay(500);
                                
                                await _pdfService.GerarPDF(html, year, month, "NFCE", cnpj, "");

                                tarefasDeCompactacao.Add(CompactarArquivos(basePath, year, month, "NFCE", cnpj, ""));

                                break;

                            default:
                                Console.WriteLine($"Tipo de documento não reconhecido: {tipoDocumento}");
                                continue;
                        }

                        indicePasta++;
                    }
                }

                atualizarProgresso(60, "Compactando arquivos...");
                await Task.Delay(500);
                var caminhosCompactados = await Task.WhenAll(tarefasDeCompactacao);

                atualizarProgresso(80, "Enviando e-mail...");
                await Task.Delay(500);
                await _emailService.EnviarEmailComAnexos(caminhosCompactados[0], year, month);
                
                atualizarProgresso(100, "Processo concluído!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro durante a geração do relatório: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public async Task GerarRelatorioPorPeriodo(DateTime dataInicial,DateTime dataFinal, Action<int, string> atualizarProgresso)
        {
            int year = dataInicial.Year;
            int month = dataInicial.Month;

            try
            {
                string folderName = $"{year}{month:00}";

                atualizarProgresso(10, "Lendo arquivos XML...");
                await Task.Delay(500);

                Dictionary<string, string[]> documentosFiscais = new Dictionary<string, string[]>
                {
                    { "_cuponsFiscais", new string[] { "Vendas", "Cancelamentos" } },  // Cupons tem subpastas
                    { "_notaFiscais", new string[] { "CNPJ", "NFe", "AAAAMM", "NFE" } }, // Notas Fiscais têm subpastas
                    { "_nfces", new string[] { "CNPJ", "NFCe", "AAAAMM", "NFCe" } } // NFCes têm subpastas
                };

                List<Task<string>> tarefasDeCompactacao = new List<Task<string>>();

                foreach (var docTipo in documentosFiscais)
                {
                    string tipoDocumento = docTipo.Key;
                    string[] subFolders = docTipo.Value;

                    atualizarProgresso(20, $"Processando {tipoDocumento}...");
                    await Task.Delay(500);

                    List<string> basePaths = _jsonServices.PegarCaminhosFiscais(tipoDocumento);

                    if (basePaths == null || basePaths.Count == 0)
                    {
                        Console.WriteLine($"Nenhum caminho configurado para {tipoDocumento}. Pulando processamento...");
                        continue; // Pula para o próximo tipo de documento
                    }


                    int totalPastas = basePaths.Count;
                    int indicePasta = 0;

                    foreach (string basePath in basePaths)
                    {
                        if (string.IsNullOrWhiteSpace(basePath))
                        {
                            Console.WriteLine($"Caminho vazio detectado para {tipoDocumento}. Pulando...");
                            continue; // Pula para o próximo caminho sem erro
                        }

                        if (!_arquivoService.VerificarCaminhoAcessivel(basePath))
                        {
                            DialogResult result = MessageBox.Show(
                                $"O caminho '{basePath}' está inacessível.\nDeseja continuar a operação?",
                                "Aviso",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning
                            );

                            if (result == DialogResult.No)
                            {
                                string pastaDestinox = Path.Combine(@"C:\tolsistemas\contabil", folderName);
                                _arquivoService.ApagarArquivosGerados(pastaDestinox);

                                MessageBox.Show("Operação cancelada. Os arquivos gerados foram apagados.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }

                        atualizarProgresso(30, "Lendo arquivos XML...");
                        await Task.Delay(500);
                        List<string> xmlFilesDoCaminho = BuscarArquivosXMLPorPeriodo(basePath, folderName, subFolders, indicePasta, totalPastas,dataInicial,dataFinal);


                        if (xmlFilesDoCaminho.Count == 0)
                        {
                            Console.WriteLine($"Nenhum arquivo XML encontrado para {basePath} no mês {folderName}.");
                            continue;
                        }

                        string html;
                        string pastaDestino;
                        string cnpj = "00000000000000";
                        string nserieSAT = "000000";

                        if (xmlFilesDoCaminho.Count > 0)
                        {
                            XDocument xml = XDocument.Load(xmlFilesDoCaminho.First());
                            (cnpj, nserieSAT) = new ExtratorCupomFiscal().ObterDadosXML(xml);
                        }

                        switch (tipoDocumento)
                        {
                            case "_cuponsFiscais":
                                IExtratorDocumento<CupomFiscal> extratorCupom = new ExtratorCupomFiscal();

                                atualizarProgresso(40, "Extraindo informações...");
                                await Task.Delay(500);

                                var cupons = await ProcessarXMLs(xmlFilesDoCaminho, extratorCupom, indicePasta, totalPastas);
                                html = _htmlGenerator.GerarRelatorioCupons(cupons, $"Relatorio parcial de Cupons Ficais - SAT \n De: {dataInicial.Date} à {dataFinal.Date}");

                                atualizarProgresso(50, "Gerando PDF...");
                                await Task.Delay(500);

                                await _pdfService.GerarPDF(html, year, month, "SAT", cnpj, nserieSAT);

                                tarefasDeCompactacao.Add(CompactarArquivos(basePath, year, month, "SAT", cnpj, nserieSAT));

                                break;

                            case "_notaFiscais":
                                ExtratorNotaFiscal extratorNota = new ExtratorNotaFiscal();

                                atualizarProgresso(40, "Extraindo informações...");
                                await Task.Delay(500);

                                var notas = await ProcessarNotasFiscais(xmlFilesDoCaminho, extratorNota, indicePasta, totalPastas);
                                html = _htmlGenerator.GerarRelatorioNotas(notas, $"Relatorio parcial de Notas Fiscais - NFe \n De: {dataInicial.Date} à {dataFinal.Date}");

                                atualizarProgresso(50, "Gerando PDF...");
                                await Task.Delay(500);

                                await _pdfService.GerarPDF(html, year, month, "NFE", cnpj, "");

                                tarefasDeCompactacao.Add(CompactarArquivos(basePath, year, month, "NFE", cnpj, ""));

                                break;

                            case "_nfces":
                                IExtratorDocumento<NFCE> extratorNFCE = new ExtratorNFCE();

                                atualizarProgresso(40, "Extraindo informações...");
                                await Task.Delay(500);

                                var nfces = await ProcessarXMLs(xmlFilesDoCaminho, extratorNFCE, indicePasta, totalPastas);
                                html = _htmlGenerator.GerarRelatorioCuponsNCFE(nfces, $"Relatorio parcial de Cupons Fiscais - NFCe \n De: {dataInicial.Date} à {dataFinal.Date}");

                                atualizarProgresso(50, "Gerando PDF...");
                                await Task.Delay(500);

                                await _pdfService.GerarPDF(html, year, month, "NFCE", cnpj, "");

                                tarefasDeCompactacao.Add(CompactarArquivos(basePath, year, month, "NFCE", cnpj, ""));

                                break;

                            default:
                                Console.WriteLine($"Tipo de documento não reconhecido: {tipoDocumento}");
                                continue;
                        }

                        indicePasta++;
                    }
                }

                atualizarProgresso(60, "Compactando arquivos...");
                await Task.Delay(500);
                var caminhosCompactados = await Task.WhenAll(tarefasDeCompactacao);

                atualizarProgresso(80, "Enviando e-mail...");
                await Task.Delay(500);
                await _emailService.EnviarEmailComAnexos(caminhosCompactados[0], year, month);

                atualizarProgresso(100, "Processo concluído!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro durante a geração do relatório: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private List<string> BuscarArquivosXMLPorPeriodo(string basePath, string folderName, string[] subFolders, int indicePasta, int totalPastas, DateTime dataInicial, DateTime dataFinal)
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

        private List<string> BuscarArquivosXML(string basePath, string folderName, string[] subFolders, int indicePasta, int totalPastas)
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
                string cnpj = Path.GetFileName(basePath);
                string nfeFolderPath = Path.Combine(basePath, "NFe", folderName, "NFE");

                string baseEventoPath = Directory.GetParent(basePath).FullName;
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
                string nfceFolderPath = Path.Combine(basePath, "NFCe", folderName, "NFCE");

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

        private async Task<List<T>> ProcessarXMLs<T>(List<string> xmlFiles, IExtratorDocumento<T> extrator, int indicePasta, int totalPastas)
        {
            List<T> documentos = new List<T>();
            int totalArquivos = xmlFiles.Count;
            int progressoAtual = (indicePasta * 100) / totalPastas;
            int progressoPorArquivo = totalArquivos > 0 ? 80 / totalArquivos : 1;

            foreach (string xmlFile in xmlFiles)
            {
                try
                {
                    if (File.Exists(xmlFile))
                    {
                        XDocument xml = XDocument.Load(xmlFile);
                        string nomeArquivo = Path.GetFileName(xmlFile);
                        documentos.AddRange(extrator.Extrair(xml, nomeArquivo));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar {xmlFile}: {ex.Message}");
                }
            }
            return documentos;
        }

        private async Task<List<NotaFiscal>> ProcessarNotasFiscais(List<string> xmlFiles, ExtratorNotaFiscal extrator, int indicePasta, int totalPastas)
        {
            List<NotaFiscal> notasFiscais = new List<NotaFiscal>();
            List<XDocument> autorizadasXml = new List<XDocument>();
            List<XDocument> canceladasXml = new List<XDocument>();

            int totalArquivos = xmlFiles.Count;
            int progressoAtual = (indicePasta * 100) / totalPastas;
            int progressoPorArquivo = totalArquivos > 0 ? 80 / totalArquivos : 1;

            // 🔹 Separa os XMLs entre autorizados e cancelados
            foreach (string xmlFile in xmlFiles)
            {
                try
                {
                    if (File.Exists(xmlFile))
                    {
                        XDocument xml = XDocument.Load(xmlFile);
                        if (extrator.VerificarSeCancelado(xml))
                        {
                            canceladasXml.Add(xml);
                        }
                        else if (extrator.VerificarSeAutorizado(xml))
                        {
                            autorizadasXml.Add(xml);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar {xmlFile}: {ex.Message}");
                }
            }

            HashSet<string> chavesAutorizadas = new HashSet<string>(extrator.ExtrairChavesDeAcesso(autorizadasXml));

            foreach (var xml in autorizadasXml)
            {
                notasFiscais.AddRange(extrator.Extrair(xml, "", chavesAutorizadas.ToList()));
            }

            HashSet<string> chavesCanceladas = new HashSet<string>();

            foreach (var xml in canceladasXml)
            {
                string chaveNotaCancelada = extrator.ExtrairChaveAcesso(xml);
                if (!string.IsNullOrEmpty(chaveNotaCancelada))
                {
                    chavesCanceladas.Add(chaveNotaCancelada);
                }
            }

            notasFiscais = notasFiscais
            .Where(nfe => !(nfe.status == "Autorizado" && chavesCanceladas.Contains(nfe.chNFe)))
            .ToList();

            foreach (var xml in canceladasXml)
            {
                string chaveNotaCancelada = extrator.ExtrairChaveAcesso(xml);

                if (chavesAutorizadas.Contains(chaveNotaCancelada))
                {
                    notasFiscais.AddRange(extrator.Extrair(xml, "", chavesAutorizadas.ToList()));
                }
            }
            return notasFiscais;
        }

        private async Task<string> CompactarArquivos(string basePath, int year, int month, string tipoDocumento, string cnpj, string nserieSAT)
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

            bool isNotaFiscal = Directory.Exists(Path.Combine(basePath, "NFe"));
            bool isNFCe = Directory.Exists(Path.Combine(basePath, "NFCe"));

            List<string> diretoriosDoMes = new List<string>();

            if (isNotaFiscal)
            {
                string cnpjFolder = Path.GetFileName(basePath);
                string caminhoNotas = Path.Combine(basePath, "NFe", folderName, "NFE");

                string baseEventoPath = Directory.GetParent(basePath)?.FullName ?? "";
                string caminhoEventos = Path.Combine(baseEventoPath, "evento", cnpjFolder, "NFe", folderName, "Evento", "Cancelamento");

                if (Directory.Exists(caminhoNotas))
                {
                    diretoriosDoMes.Add(caminhoNotas);
                }
                else
                {
                    Console.WriteLine($"⚠ Pasta de notas autorizadas não encontrada: {caminhoNotas}");
                }

                // 🔹 Adiciona a pasta de eventos (cancelamentos), se existir
                if (Directory.Exists(caminhoEventos))
                {
                    diretoriosDoMes.Add(caminhoEventos);
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
                    diretoriosDoMes.Add(caminhoNotas);
                }
                else
                {
                    Console.WriteLine($"⚠ Pasta de NFCe autorizadas não encontrada: {caminhoNotas}");
                }

                // 🔹 Adiciona a pasta de eventos (canceladas), se existir
                if (Directory.Exists(caminhoEventos))
                {
                    diretoriosDoMes.Add(caminhoEventos);
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
                Console.WriteLine($"Nenhum arquivo encontrado para compactar no mês {folderName}.");
                return destinoPasta;
            }

            _rarService.CompactarDiretorios(diretoriosDoMes.ToArray(), rarPath);
            return destinoPasta;
        }

        private async Task<string> CompactarArquivosPorPeriodo(string basePath, int year, int month, string tipoDocumento, string cnpj, string nserieSAT, DateTime dataInicial, DateTime dataFinal)
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

            _rarService.CompactarDiretorios(arquivosParaCompactar.ToArray(), rarPath);
            return destinoPasta;
        }
    }
}