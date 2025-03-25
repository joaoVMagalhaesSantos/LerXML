using lerXML.Classes;
using lerXML.Extratores;
using lerXML.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static lerXML.Interface.IExtratorDocumento;

namespace lerXML.Application.Services
{
    public class XmlServices
    {
        public List<NotaFiscal> ExtrairNotasFiscais(XDocument xml)
        {
            List<NotaFiscal> notas = new List<NotaFiscal>();

            XNamespace ns = xml.Root.GetDefaultNamespace();

            foreach (var nfeElement in xml.Descendants(ns + "nfeProc"))
            {
                NotaFiscal nfe = new NotaFiscal
                {
                    cNF = nfeElement.Element(ns + "NFe")?.Element(ns + "infNFe")?.Element(ns + "ide")?.Element(ns + "cNF")?.Value,
                    natOp = nfeElement.Element(ns + "NFe")?.Element(ns + "infNFe")?.Element(ns + "ide")?.Element(ns + "natOp")?.Value,
                    nNf = nfeElement.Element(ns + "NFe")?.Element(ns + "infNFe")?.Element(ns + "ide")?.Element(ns + "nNF")?.Value,
                    dhEmi = DateTime.Parse(nfeElement.Element(ns + "NFe")?.Element(ns + "infNFe")?.Element(ns + "ide") ?. Element(ns + "dhEmi")?.Value, null, System.Globalization.DateTimeStyles.RoundtripKind),
                    CNPJ = nfeElement.Element(ns + "NFe")?.Element(ns + "infNFe")?.Element(ns + "dest")?.Element(ns + "CNPJ")?.Value,
                    vNF = decimal.Parse(nfeElement.Element(ns + "NFe").Element(ns + "infNFe").Element(ns + "total").Element(ns + "ICMSTot").Element(ns + "vNF").Value.Replace(".", ",")),
                    chNFe = nfeElement.Element(ns + "protNFe")?.Element(ns + "infProt")?.Element(ns + "chNFe")?.Value,
                    status = "Autorizado",
                    cfop = new List<string>(),
                    csosn = new List<string>(),
                    vProdItem = new List<decimal>()

                };

                foreach (var nfeElementItem in xml.Descendants(ns + "det"))
                {
                    var cfopValue = nfeElementItem.Element(ns + "prod")?.Element(ns + "CFOP")?.Value;
                    if (!string.IsNullOrEmpty(cfopValue))
                    {
                        nfe.cfop.Add(cfopValue);
                    }

                    var csosnValue = nfeElementItem.Element(ns + "imposto")?.Element(ns + "ICMS")?.Element(ns + "ICMSSN102")?.Element(ns + "CSOSN")?.Value;
                    if (!string.IsNullOrEmpty(csosnValue))
                    {
                        nfe.csosn.Add(csosnValue);
                    }

                    var vProdValue = nfeElementItem.Element(ns + "prod")?.Element(ns + "vProd")?.Value; ;
                    if (!string.IsNullOrEmpty(vProdValue))
                    {
                        nfe.vProdItem.Add(decimal.Parse(vProdValue.Replace(".", ",")));
                    }
                }

                notas.Add(nfe);
            }

            return notas;
        }

        public List<NotaFiscal> ExtrairNotasFiscaisCanceladas(XDocument xml)
        {
            List<NotaFiscal> notas = new List<NotaFiscal>();

            // 🔹 Obtém o namespace do XML
            XNamespace ns = "http://www.portalfiscal.inf.br/nfe";

            // 🔹 Verifica se há elementos <retEvento> dentro de <retEnvEvento>
            foreach (var retEvento in xml.Descendants(ns + "retEvento"))
            {
                var infEvento = retEvento.Element(ns + "infEvento");
                if (infEvento != null)
                {
                    NotaFiscal nfe = new NotaFiscal
                    {
                        chNFe = infEvento.Element(ns + "chNFe")?.Value,
                        CNPJ = infEvento.Element(ns + "CNPJDest")?.Value,
                        status = "Cancelado"
                    };

                    // 🔹 Tenta converter a data do evento, se existir
                    string dataEvento = infEvento.Element(ns + "dhRegEvento")?.Value;
                    if (!string.IsNullOrEmpty(dataEvento) && DateTime.TryParse(dataEvento, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime dataFormatada))
                    {
                        nfe.dhEmi = dataFormatada;
                    }
                    else
                    {
                        nfe.dhEmi = DateTime.MinValue; // Define um valor padrão para evitar erro
                        Console.WriteLine($"⚠️ Data inválida ou ausente para a NF-e cancelada: {nfe.chNFe}");
                    }

                    notas.Add(nfe);
                }
            }

            return notas;
        }

        public List<CupomFiscal> ExtrairCupomFiscalAutorizado(XDocument xml, string chaveCupom)
        {
            List<CupomFiscal> cupons = new List<CupomFiscal>();

            XNamespace ns = xml.Root.GetDefaultNamespace();

            foreach (var nfeElement in xml.Descendants(ns + "infCFe"))
            {
                CupomFiscal cupom = new CupomFiscal
                {
                    nCFE = nfeElement.Element(ns + "ide")?.Element(ns + "nCFe")?.Value,
                    data = DateTime.ParseExact(nfeElement.Element(ns + "ide")?.Element(ns + "dEmi")?.Value, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                    CNPJ = nfeElement.Element(ns + "emit")?.Element(ns + "CNPJ")?.Value,
                    nserieSAT = nfeElement.Element(ns + "ide")?.Element(ns + "nserieSAT")?.Value,
                    chCFE = chaveCupom,
                    vProd = decimal.Parse(nfeElement.Element(ns + "total").Element(ns + "ICMSTot").Element(ns + "vProd").Value.Replace(".", ",")),
                    vDesc = decimal.Parse(nfeElement.Element(ns + "total").Element(ns + "ICMSTot").Element(ns + "vDesc").Value.Replace(".", ",")),
                    vOutro = decimal.Parse(nfeElement.Element(ns + "total").Element(ns + "ICMSTot").Element(ns + "vOutro").Value.Replace(".", ",")),
                    vCFE = decimal.Parse(nfeElement.Element(ns + "total").Element(ns + "vCFe").Value.Replace(".", ",")),
                    status = "Autorizado",
                    cfop = new List<string>(),
                    csosn = new List<string>(),
                    vProdItem = new List<decimal>()
                };

                foreach (var nfeElementItem in xml.Descendants(ns + "det"))
                {
                    var cfopValue = nfeElementItem.Element(ns + "prod")?.Element(ns + "CFOP")?.Value;
                    if (!string.IsNullOrEmpty(cfopValue))
                    {
                        cupom.cfop.Add(cfopValue);
                    }

                    var csosnValue = nfeElementItem.Element(ns + "imposto")?.Element(ns + "ICMS")?.Element(ns + "ICMSSN102")?.Element(ns + "CSOSN")?.Value;
                    if (!string.IsNullOrEmpty(csosnValue))
                    {
                        cupom.csosn.Add(csosnValue);
                    }

                    var vProdValue = nfeElementItem.Element(ns + "prod")?.Element(ns + "vProd")?.Value; ;
                    if (!string.IsNullOrEmpty(vProdValue))
                    {
                        cupom.vProdItem.Add(decimal.Parse(vProdValue.Replace(".", ",")));
                    }
                }

                cupons.Add(cupom);
            }
            return cupons;
        }

        public List<CupomFiscal> ExtrairCupomFiscalCancelado(XDocument xml, string chaveCupom)
        {
            List<CupomFiscal> cupons = new List<CupomFiscal>();

            XNamespace ns = xml.Root.GetDefaultNamespace();

            foreach (var nfeElement in xml.Descendants(ns + "infCFe"))
            {
                CupomFiscal cupom = new CupomFiscal
                {
                    nCFE = nfeElement.Element(ns + "ide")?.Element(ns + "nCFe")?.Value,
                    nserieSAT = nfeElement.Element(ns + "ide")?.Element(ns + "nserieSAT")?.Value,
                    data = DateTime.ParseExact(nfeElement.Element(ns + "ide")?.Element(ns + "dEmi")?.Value, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                    CNPJ = nfeElement.Element(ns + "emit")?.Element(ns + "CNPJ")?.Value,
                    chCFE = chaveCupom,
                    vCFE = decimal.Parse(nfeElement.Element(ns + "total").Element(ns + "vCFe").Value.Replace(".", ",")),
                    status = "Cancelado",
                };

                cupons.Add(cupom);
            }
            return cupons;
        }

        public List<NFCE> ExtrairNFCeAutorizado(XDocument xml, string chaveCupom)
        {
            List<NFCE> cupons = new List<NFCE>();

            if (xml?.Root == null)
            {
                throw new ArgumentException("O documento XML está vazio ou malformado.");
            }

            XNamespace ns = xml.Root.GetDefaultNamespace();

            foreach (var nfeElement in xml.Descendants(ns + "infNFe"))
            {
                NFCE cupom = new NFCE
                {
                    nNF = nfeElement.Element(ns + "ide")?.Element(ns + "nNF")?.Value,
                    dhEmi = DateTime.TryParseExact(nfeElement.Element(ns + "ide")?.Element(ns + "dhEmi")?.Value, "yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dhEmiParsed) ? dhEmiParsed.Date : (DateTime?) null,
                    CNPJ = nfeElement.Element(ns + "emit")?.Element(ns + "CNPJ")?.Value,
                    chCFE = chaveCupom,
                    vProd = TryParseDecimal(nfeElement.Element(ns + "total")?.Element(ns + "ICMSTot")?.Element(ns + "vProd")?.Value),
                    vDesc = TryParseDecimal(nfeElement.Element(ns + "total")?.Element(ns + "ICMSTot")?.Element(ns + "vDesc")?.Value),
                    vOutro = TryParseDecimal(nfeElement.Element(ns + "total")?.Element(ns + "ICMSTot")?.Element(ns + "vOutro")?.Value),
                    vNF = TryParseDecimal(nfeElement.Element(ns + "total").Element(ns + "ICMSTot").Element(ns + "vNF").Value),
                    status = "Autorizado",
                    cfop = new List<string>(),
                    csosn = new List<string>(),
                    vProdItem = new List<decimal>()
                };

                foreach (var nfeElementItem in nfeElement.Descendants(ns + "det"))
                {
                    var cfopValue = nfeElementItem.Element(ns + "prod")?.Element(ns + "CFOP")?.Value;
                    if (!string.IsNullOrEmpty(cfopValue))
                    {
                        cupom.cfop.Add(cfopValue);
                    }

                    var icmsElement = nfeElementItem.Element(ns + "imposto")?.Element(ns + "ICMS");
                    if (icmsElement != null)
                    {
                        var csosnElement = icmsElement.Elements().FirstOrDefault(e => e.Element(ns + "CSOSN") != null);
                        var csosnValue = csosnElement?.Element(ns + "CSOSN")?.Value;
                        if (!string.IsNullOrEmpty(csosnValue))
                        {
                            cupom.csosn.Add(csosnValue);
                        }
                    }

                    var vProdValue = nfeElementItem.Element(ns + "prod")?.Element(ns + "vProd")?.Value;
                    if (!string.IsNullOrEmpty(vProdValue))
                    {
                        if (decimal.TryParse(vProdValue.Replace(".", ","), out var vProdParsed))
                        {
                            cupom.vProdItem.Add(vProdParsed);
                        }
                    }
                }

                cupons.Add(cupom);
            }
            return cupons;

        }
        public List<NFCE> ExtrairNFCeNaoAutorizado(XDocument xml, string chaveCupom)
        {
            List<NFCE> cupons = new List<NFCE>();

            if (xml?.Root == null)
            {
                throw new ArgumentException("O documento XML está vazio ou malformado.");
            }

            XNamespace ns = xml.Root.GetDefaultNamespace();

            foreach (var nfeElement in xml.Descendants(ns + "infNFe"))
            {
                NFCE cupom = new NFCE
                {
                    nNF = nfeElement.Element(ns + "ide")?.Element(ns + "nNF")?.Value,
                    dhEmi = DateTime.TryParseExact(nfeElement.Element(ns + "ide")?.Element(ns + "dhEmi")?.Value, "yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dhEmiParsed) ? dhEmiParsed.Date : (DateTime?)null,
                    CNPJ = nfeElement.Element(ns + "emit")?.Element(ns + "CNPJ")?.Value,
                    chCFE = chaveCupom,
                    vProd = TryParseDecimal(nfeElement.Element(ns + "total")?.Element(ns + "ICMSTot")?.Element(ns + "vProd")?.Value),
                    vDesc = TryParseDecimal(nfeElement.Element(ns + "total")?.Element(ns + "ICMSTot")?.Element(ns + "vDesc")?.Value),
                    vOutro = TryParseDecimal(nfeElement.Element(ns + "total")?.Element(ns + "ICMSTot")?.Element(ns + "vOutro")?.Value),
                    vNF = TryParseDecimal(nfeElement.Element(ns + "total").Element(ns + "ICMSTot").Element(ns + "vNF").Value),
                    status = "Não Autorizado",
                    cfop = new List<string>(),
                    csosn = new List<string>(),
                    vProdItem = new List<decimal>()
                };

                foreach (var nfeElementItem in nfeElement.Descendants(ns + "det"))
                {
                    var cfopValue = nfeElementItem.Element(ns + "prod")?.Element(ns + "CFOP")?.Value;
                    if (!string.IsNullOrEmpty(cfopValue))
                    {
                        cupom.cfop.Add(cfopValue);
                    }

                    var icmsElement = nfeElementItem.Element(ns + "imposto")?.Element(ns + "ICMS");
                    if (icmsElement != null)
                    {
                        var csosnElement = icmsElement.Elements().FirstOrDefault(e => e.Element(ns + "CSOSN") != null);
                        var csosnValue = csosnElement?.Element(ns + "CSOSN")?.Value;
                        if (!string.IsNullOrEmpty(csosnValue))
                        {
                            cupom.csosn.Add(csosnValue);
                        }
                    }

                    var vProdValue = nfeElementItem.Element(ns + "prod")?.Element(ns + "vProd")?.Value;
                    if (!string.IsNullOrEmpty(vProdValue))
                    {
                        if (decimal.TryParse(vProdValue.Replace(".", ","), out var vProdParsed))
                        {
                            cupom.vProdItem.Add(vProdParsed);
                        }
                    }
                }

                cupons.Add(cupom);
            }
            return cupons;

        }

        public List<NFCE> ExtrairNCFeCancelado(XDocument xml, string chaveCupom)
        {
            List<NFCE> cupons = new List<NFCE>();

            XNamespace ns = xml.Root.GetDefaultNamespace();

            foreach (var nfeElement in xml.Descendants(ns + "infCFe"))
            {
                NFCE cupom = new NFCE
                {
                    nNF = nfeElement.Element(ns + "ide")?.Element(ns + "nNF")?.Value,
                    dhEmi = DateTime.ParseExact(nfeElement.Element(ns + "ide")?.Element(ns + "dhEmi")?.Value, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                    CNPJ = nfeElement.Element(ns + "emit")?.Element(ns + "CNPJ")?.Value,
                    chCFE = chaveCupom,
                    vNF = decimal.Parse(nfeElement.Element(ns + "total").Element(ns + "vNF").Value.Replace(".", ",")),
                    status = "Cancelado",
                };

                cupons.Add(cupom);
            }
            return cupons;
        }

        private decimal TryParseDecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            if (decimal.TryParse(value.Replace(".", ","), out var parsedValue))
                return parsedValue;

            return 0;
        }

        public async Task<List<T>> ProcessarXMLs<T>(List<string> xmlFiles, IExtratorDocumento<T> extrator, int indicePasta, int totalPastas)
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

        public async Task<List<NotaFiscal>> ProcessarNotasFiscais(List<string> xmlFiles, ExtratorNotaFiscal extrator, int indicePasta, int totalPastas)
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

    }
}
