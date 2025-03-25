using System;
using System.Windows.Forms;
using lerXML.Application.Services;
using lerXML.View;

namespace lerXML
{
    static class Program
    {
        [STAThread]
        static void Main()
        {

            global::System.Windows.Forms.Application.EnableVisualStyles();
            global::System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            // Criar as inst�ncias dos servi�os necess�rios
            JsonServices jsonServices = new JsonServices("C:\\tolsistemas\\lerXML\\configEnvArq.json");
            PdfService pdfService = new PdfService();
            HtmlGenerator htmlGenerator = new HtmlGenerator();
            RarService rarService = new RarService();
            EmailServices emailService = new EmailServices(jsonServices);
            ArquivoService arquivoService = new ArquivoService();
            XmlServices xmlService = new XmlServices();

            // Criar inst�ncia do servi�o de relat�rios
            RelatorioService relatorioService = new RelatorioService(jsonServices, pdfService, htmlGenerator, rarService, emailService, arquivoService, xmlService);

            // Passar o servi�o para o Form
            global::System.Windows.Forms.Application.Run(new EnvioArquivos(relatorioService));
        }
    }
}