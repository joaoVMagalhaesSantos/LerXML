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
using System.Diagnostics;

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
        private readonly XmlServices _xmlServices;

        public RelatorioService(JsonServices jsonServices, PdfService pdfService, HtmlGenerator htmlGenerator, RarService rarService, EmailServices emailService, ArquivoService arquivoService, XmlServices xmlServices)
        {
            _jsonServices = jsonServices;
            _pdfService = pdfService;
            _htmlGenerator = htmlGenerator;
            _rarService = rarService;
            _emailService = emailService;
            _arquivoService = arquivoService;
            _xmlServices = xmlServices;
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

                List<string> tarefasDeCompactacao = new List<string>();
                string x;

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
                        List<string> xmlFilesDoCaminho = _arquivoService.BuscarArquivosXML(basePath, folderName, subFolders, indicePasta, totalPastas);


                        if (xmlFilesDoCaminho.Count == 0)
                        {
                            Console.WriteLine($"Nenhum arquivo XML encontrado para {basePath} no mês {folderName}.");
                            continue;
                        }
                        
                        var html = "";
                        //string pastaDestino;
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
                                
                                var cupons = await _xmlServices.ProcessarXMLs(xmlFilesDoCaminho, extratorCupom, indicePasta, totalPastas);
                                html = _htmlGenerator.GerarRelatorioCupons(cupons, "Relatorio completo de Cupons Ficais - SAT");

                                atualizarProgresso(50, "Gerando PDF...");
                                await Task.Delay(500);
                                
                                await _pdfService.GerarPDF(html, year, month, "SAT", cnpj, nserieSAT);

                                x = await _rarService.CompactarArquivos(basePath, year, month, "SAT", cnpj, nserieSAT);

                                tarefasDeCompactacao.Add(x);

                                break;

                            case "_notaFiscais":
                                ExtratorNotaFiscal extratorNota = new ExtratorNotaFiscal();
                                
                                atualizarProgresso(40, "Extraindo informações...");
                                await Task.Delay(500);
                                
                                var notas = await _xmlServices.ProcessarNotasFiscais(xmlFilesDoCaminho, extratorNota, indicePasta, totalPastas);
                                html = _htmlGenerator.GerarRelatorioNotas(notas, "Relatorio completo de Notas Fiscais - NFe");

                                atualizarProgresso(50, "Gerando PDF...");
                                await Task.Delay(500);
                                
                                await _pdfService.GerarPDF(html, year, month, "NFE", cnpj, "");

                                x = await _rarService.CompactarArquivos(basePath, year, month, "NFe", cnpj, "");

                                tarefasDeCompactacao.Add(x);

                                break;

                            case "_nfces":
                                IExtratorDocumento<NFCE> extratorNFCE = new ExtratorNFCE();
                                
                                atualizarProgresso(40, "Extraindo informações...");
                                await Task.Delay(500);
                                
                                var nfces = await _xmlServices.ProcessarXMLs(xmlFilesDoCaminho, extratorNFCE, indicePasta, totalPastas);
                                html = await _htmlGenerator.GerarRelatorioCuponsNCFE(nfces, "Relatorio completo de Cupons Ficais - NFCe");

                                atualizarProgresso(50, "Gerando PDF...");
                                await Task.Delay(500);
                                
                                await _pdfService.GerarPDF(html, year, month, "NFCE", cnpj, "");

                                x = await _rarService.CompactarArquivos(basePath, year, month, "NFCe", cnpj, "");

                                tarefasDeCompactacao.Add(x);

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
                var caminhosCompactados = tarefasDeCompactacao;

                atualizarProgresso(80, "Enviando e-mail...");
                await Task.Delay(500);
                await _emailService.EnviarEmailComAnexos(caminhosCompactados[0], year, month);
                
                atualizarProgresso(100, "Processo concluído!");
                await Task.Delay(500);

                GravarEnvioSucesso(caminhosCompactados[0]);
                _arquivoService.AbrirArquivosPDF(caminhosCompactados[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro durante a geração do relatório: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public async Task GerarRelatorioMensal14032025(int year, int month, Action<int, string> atualizarProgresso)
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

                List<string> tarefasDeCompactacao = new List<string>();
                string x;

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
                        List<string> xmlFilesDoCaminho = _arquivoService.BuscarArquivosXML(basePath, folderName, subFolders, indicePasta, totalPastas);


                        if (xmlFilesDoCaminho.Count == 0)
                        {
                            Console.WriteLine($"Nenhum arquivo XML encontrado para {basePath} no mês {folderName}.");
                            continue;
                        }

                        string html;
                        //string pastaDestino;
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

                                var cupons = await _xmlServices.ProcessarXMLs(xmlFilesDoCaminho, extratorCupom, indicePasta, totalPastas);
                                html = _htmlGenerator.GerarRelatorioCupons(cupons, "Relatorio completo de Cupons Ficais - SAT");

                                atualizarProgresso(50, "Gerando PDF...");
                                await Task.Delay(500);

                                await _pdfService.GerarPDF(html, year, month, "SAT", cnpj, nserieSAT);

                                x = await _rarService.CompactarArquivos(basePath, year, month, "SAT", cnpj, nserieSAT);

                                tarefasDeCompactacao.Add(x);

                                break;

                            case "_notaFiscais":
                                ExtratorNotaFiscal extratorNota = new ExtratorNotaFiscal();

                                atualizarProgresso(40, "Extraindo informações...");
                                await Task.Delay(500);

                                var notas = await _xmlServices.ProcessarNotasFiscais(xmlFilesDoCaminho, extratorNota, indicePasta, totalPastas);
                                html = _htmlGenerator.GerarRelatorioNotas(notas, "Relatorio completo de Notas Fiscais - NFe");

                                atualizarProgresso(50, "Gerando PDF...");
                                await Task.Delay(500);

                                await _pdfService.GerarPDF(html, year, month, "NFE", cnpj, "");

                                x = await _rarService.CompactarArquivos(basePath, year, month, "NFE", cnpj, "");

                                tarefasDeCompactacao.Add(x);

                                break;

                            case "_nfces":
                                IExtratorDocumento<NFCE> extratorNFCE = new ExtratorNFCE();

                                atualizarProgresso(40, "Extraindo informações...");
                                await Task.Delay(500);

                                var nfces = await _xmlServices.ProcessarXMLs(xmlFilesDoCaminho, extratorNFCE, indicePasta, totalPastas);
                                html = await _htmlGenerator.GerarRelatorioCuponsNCFE(nfces, "Relatorio completo de Cupons Ficais - NFCe");

                                atualizarProgresso(50, "Gerando PDF...");
                                await Task.Delay(500);

                                await _pdfService.GerarPDF(html, year, month, "NFCE", cnpj, "");

                                x = await _rarService.CompactarArquivos(basePath, year, month, "NFCE", cnpj, "");

                                tarefasDeCompactacao.Add(x);

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
                var caminhosCompactados = tarefasDeCompactacao;

                atualizarProgresso(80, "Enviando e-mail...");
                await Task.Delay(500);
                await _emailService.EnviarEmailComAnexos(caminhosCompactados[0], year, month);

                atualizarProgresso(100, "Processo concluído!");
                await Task.Delay(500);

                GravarEnvioSucesso(caminhosCompactados[0]);
                _arquivoService.AbrirArquivosPDF(caminhosCompactados[0]);
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
                        List<string> xmlFilesDoCaminho = _arquivoService.BuscarArquivosXMLPorPeriodo(basePath, folderName, subFolders, indicePasta, totalPastas,dataInicial,dataFinal);


                        if (xmlFilesDoCaminho.Count == 0)
                        {
                            Console.WriteLine($"Nenhum arquivo XML encontrado para {basePath} no mês {folderName}.");
                            continue;
                        }

                        string html;
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

                                var cupons = await _xmlServices.ProcessarXMLs(xmlFilesDoCaminho, extratorCupom, indicePasta, totalPastas);
                                html = _htmlGenerator.GerarRelatorioCupons(cupons, $"Relatorio parcial de Cupons Ficais - SAT \n De: {dataInicial.Date.ToString("dd/MM/yyyy")} à {dataFinal.Date.ToString("dd/MM/yyyy")}");

                                atualizarProgresso(50, "Gerando PDF...");
                                await Task.Delay(500);

                                await _pdfService.GerarPDF(html, year, month, "SAT", cnpj, nserieSAT);

                                tarefasDeCompactacao.Add(_rarService.CompactarArquivos(basePath, year, month, "SAT", cnpj, nserieSAT));

                                break;

                            case "_notaFiscais":
                                ExtratorNotaFiscal extratorNota = new ExtratorNotaFiscal();

                                atualizarProgresso(40, "Extraindo informações...");
                                await Task.Delay(500);

                                var notas = await _xmlServices.ProcessarNotasFiscais(xmlFilesDoCaminho, extratorNota, indicePasta, totalPastas);
                                html = _htmlGenerator.GerarRelatorioNotas(notas, $"Relatorio parcial de Notas Fiscais - NFe \n De: {dataInicial.Date.ToString("dd/MM/yyyy")} à {dataFinal.Date.ToString("dd/MM/yyyy")}");

                                atualizarProgresso(50, "Gerando PDF...");
                                await Task.Delay(500);

                                await _pdfService.GerarPDF(html, year, month, "NFE", cnpj, "");

                                tarefasDeCompactacao.Add(_rarService.CompactarArquivos(basePath, year, month, "NFE", cnpj, ""));

                                break;

                            case "_nfces":
                                IExtratorDocumento<NFCE> extratorNFCE = new ExtratorNFCE();

                                atualizarProgresso(40, "Extraindo informações...");
                                await Task.Delay(500);

                                var nfces = await _xmlServices.ProcessarXMLs(xmlFilesDoCaminho, extratorNFCE, indicePasta, totalPastas);
                                html = await _htmlGenerator.GerarRelatorioCuponsNCFE(nfces, $"Relatorio parcial de Cupons Fiscais - NFCe \n De: {dataInicial.Date.ToString("dd/MM/yyyy")} à {dataFinal.Date.ToString("dd/MM/yyyy")}");

                                atualizarProgresso(50, "Gerando PDF...");
                                await Task.Delay(500);

                                await _pdfService.GerarPDF(html, year, month, "NFCE", cnpj, "");

                                tarefasDeCompactacao.Add(_rarService.CompactarArquivos(basePath, year, month, "NFCE", cnpj, ""));

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
                await Task.Delay(500);

                GravarEnvioSucesso(caminhosCompactados[0]);

                _arquivoService.AbrirArquivosPDF(caminhosCompactados[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro durante a geração do relatório: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public void GravarEnvioSucesso(string caminho)
        {
            string caminhoSucesso = caminho;
            string nomeArquivo = "status.txt";

            string caminhoArquivo = Path.Combine(caminhoSucesso, nomeArquivo);

            string conteudo = "OK: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try 
            {
                File.WriteAllText(caminhoArquivo, conteudo);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Erro ao gravar o arquivo: " + ex.Message);
            }
        }
    }
}