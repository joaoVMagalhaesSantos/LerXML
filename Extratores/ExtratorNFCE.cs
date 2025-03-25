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
                if(VerificaAtuorizado(xml))
                {
                    // Processar cupons autorizados
                    return _servicesXML.ExtrairNFCeAutorizado(xml, nomeArquivo);
                }
                else
                {
                    return _servicesXML.ExtrairNFCeNaoAutorizado(xml, nomeArquivo);   
                }
            }
        }

        private bool VerificarSeCancelado(XDocument xml)
        {
            return xml.Root?.Name.LocalName == "CFeCanc";
        }

        private bool VerificaAtuorizado(XDocument xml)
        {
            try
            {
                XNamespace ns = xml.Root.GetDefaultNamespace();
                string motivo = "";

                foreach (var nfeElement in xml.Descendants(ns + "infProt"))
                {
                    motivo = nfeElement.Element(ns + "xMotivo")?.Value;
                }

                if (motivo == "Autorizado o uso da NF-e")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show($"Erro ao tentar ler o xml: {ex.Message}");
                return false;
            }
        }
    }
}
