using lerXML.Application.Services;
using lerXML.Classes;
using lerXML.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static lerXML.Interface.IExtratorDocumento;

namespace lerXML.Extratores
{
    public class ExtratorNotaFiscal
    {
        private readonly XmlServices _servicesXML;

        public ExtratorNotaFiscal()
        {
            _servicesXML = new XmlServices();
        }

        public List<NotaFiscal> Extrair(XDocument xml, string nomeArquivo, List<string> chavesAutorizadas)
        {
            if (VerificarSeCancelado(xml))
            {
                return _servicesXML.ExtrairNotasFiscaisCanceladas(xml);
            }
            else if (VerificarSeAutorizado(xml))
            {
                return _servicesXML.ExtrairNotasFiscais(xml);
            }
            else
            {
                Console.WriteLine($"Arquivo {nomeArquivo} ignorado - Não é uma NFe válida.");
                return new List<NotaFiscal>(); // Retorna uma lista vazia se não for uma NFe válida
            }
        }

        public bool VerificarSeAutorizado(XDocument xml)
        {
            XNamespace nsNFe = "http://www.portalfiscal.inf.br/nfe";

            // Verifica se o XML contém a estrutura de uma NFe autorizada
            return xml.Descendants(nsNFe + "infNFe").Any();
        }

        public bool VerificarSeCancelado(XDocument xml)
        {
            XNamespace nsNFe = "http://www.portalfiscal.inf.br/nfe";
            XNamespace nsSoap = "http://www.w3.org/2003/05/soap-envelope";

            foreach (var evento in xml.Descendants(nsNFe + "retEvento"))
            {
                var infEvento = evento.Element(nsNFe + "infEvento");

                if (infEvento != null)
                {
                    string tipoEvento = infEvento.Element(nsNFe + "tpEvento")?.Value.Trim();
                    string descricaoEvento = infEvento.Element(nsNFe + "xEvento")?.Value.Trim();

                    if (tipoEvento == "110111" || descricaoEvento.Contains("Cancelamento", StringComparison.OrdinalIgnoreCase))
                    {
                        return true; 
                    }
                }
            }

            return false;
        }

        public List<string> ExtrairChavesDeAcesso(List<XDocument> documentos)
        {
            List<string> chavesDeAcesso = new List<string>();

            foreach (var xml in documentos)
            {
                XNamespace ns = xml.Root?.GetDefaultNamespace();

                var chaveAcesso = xml.Descendants(ns + "infNFe")
                                      .FirstOrDefault()?
                                      .Attribute("Id")?.Value.Replace("NFe", ""); // Remove o prefixo "NFe"

                if (!string.IsNullOrEmpty(chaveAcesso))
                {
                    chavesDeAcesso.Add(chaveAcesso);
                }
            }

            return chavesDeAcesso;
        }

        public string ExtrairChaveAcesso(XDocument xml)
        {
            XNamespace ns = xml.Root?.GetDefaultNamespace();
            return xml.Descendants(ns + "chNFe").FirstOrDefault()?.Value ?? "";
        }
    }
}
