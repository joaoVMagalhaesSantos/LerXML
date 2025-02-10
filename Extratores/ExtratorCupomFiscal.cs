using lerXML.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static lerXML.Interface.IExtratorDocumento;
using System.Xml.Linq;
using lerXML.Application.Services;

namespace lerXML.Extratores
{
    public class ExtratorCupomFiscal : IExtratorDocumento<CupomFiscal>
    {
        private readonly XmlServices _servicesXML;

        public ExtratorCupomFiscal() 
        {
            _servicesXML = new XmlServices();
        }
        public List<CupomFiscal> Extrair(XDocument xml, string nomeArquivo)
        {
            if (VerificarSeCancelado(xml))
            {
                // Processar cupons cancelados
                return _servicesXML.ExtrairCupomFiscalCancelado(xml, nomeArquivo);
            }
            else
            {
                // Processar cupons autorizados
                return _servicesXML.ExtrairCupomFiscalAutorizado(xml, nomeArquivo);
            }
        }

        private bool VerificarSeCancelado(XDocument xml)
        {
            return xml.Root?.Name.LocalName == "CFeCanc";
        }

        public (string cnpj, string nserieSAT) ObterDadosXML(XDocument xml)
        {
            XNamespace ns = xml.Root.GetDefaultNamespace();

            string cnpj = xml.Descendants(ns + "emit").Elements(ns + "CNPJ").FirstOrDefault()?.Value ?? "00000000000000";
            string nserieSAT = xml.Descendants(ns + "ide").Elements(ns + "nserieSAT").FirstOrDefault()?.Value ?? "000000";

            return (cnpj, nserieSAT);
        }
    }
}
