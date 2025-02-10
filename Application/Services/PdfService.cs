using DinkToPdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lerXML.Application.Services
{
    public class PdfService
    {
        private readonly ArquivoService _arquivoService;
        private readonly SynchronizedConverter _converter = new SynchronizedConverter(new PdfTools());


        public PdfService()
        {
            _arquivoService = new ArquivoService();
            _converter = new SynchronizedConverter(new PdfTools());
        }

        public async Task GerarPDF(string html, int year, int month, string tipoDocumento, string cnpj, string nserieSAT)
        {
            try
            {
                string destinoPasta = Path.Combine(@"C:\tolsistemas\contabil", $"{year}{month:00}");

                if (!Directory.Exists(destinoPasta))
                {
                    Directory.CreateDirectory(destinoPasta);
                }

                string nomeArquivoPDF = GerarNomeArquivo(tipoDocumento, year, month, cnpj, nserieSAT, true);
                string pdfPath = Path.Combine(destinoPasta, nomeArquivoPDF);

                // 🔹 Garante que o _converter não é null
                if (_converter == null)
                {
                    Console.WriteLine("Erro: _converter não foi inicializado.");
                    //_converter = new SynchronizedConverter(new PdfTools());
                }

                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = new GlobalSettings
                    {
                        ColorMode = ColorMode.Color,
                        Orientation = DinkToPdf.Orientation.Landscape,
                        PaperSize = PaperKind.A4,
                        Out = pdfPath // 🔹 Define o caminho do PDF diretamente
                    },
                    Objects =
            {
                new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
                }
            }
                };

                Console.WriteLine($"Iniciando conversão do PDF para: {pdfPath}");

                byte[] pdfBytes = _converter.Convert(doc);

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    Console.WriteLine("Erro: O PDF gerado está vazio.");
                    return;
                }

                File.WriteAllBytes(pdfPath, pdfBytes);
                Console.WriteLine($"PDF gerado com sucesso: {pdfPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar PDF: {ex.Message}");
                MessageBox.Show($"Erro ao gerar PDF: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string GerarNomeArquivo(string tipoDocumento, int year, int month, string cnpj, string nserieSAT, bool isPDF)
        {
            string folderName = $"{year}{month:00}";
            string tipoArquivo = isPDF ? "Relatório" : "ArquivosFiscais"; // Define se é PDF ou RAR
            string nomeArquivo;

            if (tipoDocumento == "SAT")
            {
                nomeArquivo = $"{tipoArquivo}_{folderName}_SAT_{nserieSAT}_{cnpj}";
            }
            else
            {
                nomeArquivo = $"{tipoArquivo}_{folderName}_{tipoDocumento}_{cnpj}";
            }

            nomeArquivo += isPDF ? ".pdf" : ".7z"; // Adiciona a extensão correta

            return nomeArquivo;
        }

        
    }
}
