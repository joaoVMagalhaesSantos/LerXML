using lerXML.Application.Services;
using lerXML.Classes;
using lerXML.Interface;
using lerXML.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static lerXML.Interface.IExtratorDocumento;

namespace lerXML.Extratores
{
    internal class ExtratorNFCE : IExtratorDocumento<NFCE>
    {
        private readonly XmlServices _servicesXML;

        public ExtratorNFCE()
        {
            _servicesXML = new XmlServices();
        }
        public List<NFCE> Extrair(XDocument xml, string nomeArquivo)
        {
            if (VerificarSeCancelado(xml))
            {
                // Processar cupons cancelados
                return _servicesXML.ExtrairNCFeCancelado(xml, nomeArquivo);
            }
            else
            {
                // Processar cupons autorizados
                return _servicesXML.ExtrairNFCeAutorizado(xml, nomeArquivo);
            }
        }

        private bool VerificarSeCancelado(XDocument xml)
        {
            return xml.Root?.Name.LocalName == "CFeCanc";
        }
    }
}
