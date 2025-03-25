using lerXML.Classes;
using lerXML.Modelos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace lerXML.Application.Services
{
    public class HtmlGenerator
    {

        public HtmlGenerator()
        {

        }

        public string GerarRelatorioNotas(List<NotaFiscal> notas, string nomeRelatorio)
        {
            string nomeRel = nomeRelatorio;

            string html = $@"
                <!DOCTYPE html>
                <html lang='pt-BR'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Relatório de Faturamento</title>
                    <style>
                        body {{ 
                            font-family: Arial, sans-serif; 
                            margin: 20px; 
                        }}
                        h1 {{ 
                            color: #333; 
                            text-align: center; 
                        }}
                        h2 {{ 
                            color: #333; 
                            text-align: right; 
                        }}
                        table {{
                            width: 100%;
                            border-collapse: collapse;
                            margin-bottom: 20px;
                        }}
                        th, td {{
                            border: 1px solid black;
                            padding: 8px;
                            text-align: center;
                        }}
                        th {{
                            background-color: #f2f2f2;
                        }}
                        div {{
                            page-break-before: always;
                        }}
                    </style>
                </head>
                <body>
                    <h1>{nomeRel}</h1>
                    <h1>Notas fiscais Autorizadas </h1>
            ";

            var notasAgrupadasPorDia = notas
                .Where(nota => nota.status == "Autorizado")
                .GroupBy(nota => nota.dhEmi?.Date)
                .Select(grupox => new
                {
                    Data = grupox.Key,
                    Total = grupox.Sum(cuponsDatax => cuponsDatax.vNF)
                })
                .OrderBy(grupox => grupox.Data);

            foreach (var dataxy in notasAgrupadasPorDia)
            {
                html += $@"
                    
                    <table>
                        <tr>
                            <th>NF-e</th>
                            <th>CPF/CNPJ</th>
                            <th>Data Emissão</th>
                            <th>Chave de Acesso</th>
                            <th>Valor</th>
                            <th>Status</th>
                        </tr>
                ";

                decimal totalNFE = 0;
                foreach (var nota in notas)
                {
                    totalNFE += nota.vNF;
                    html += $@"
                        <tr>
                            <td>{nota.nNf}</td>
                            <td>{nota.CNPJ}</td>
                            <td>{nota.dhEmi}</td>
                            <td>{nota.chNFe}</td>
                            <td>R$ {nota.vNF:F2}</td>
                            <td>{nota.status}</td>
                        </tr>
                     ";
                }

                html += $@"
                    </table>
                    <h2>R$ {totalNFE:F2}</h2>
                ";
            }

            var notasCanceladas = notas.Where(n => n.status == "Cancelado").ToList();
            if (notasCanceladas.Any())
            {
                html += $@"
                    <h1>Notas Canceladas</h1>
                    <table>
                        <tr>
                            <th>CPF/CNPJ</th>
                            <th>Chave de Acesso</th>
                            <th>Status</th>
                        </tr>
                ";

                foreach (var nota in notasCanceladas)
                {
                    html += $@"
                        <tr>
                            <td>{nota.CNPJ}</td>
                            <td>{nota.chNFe}</td>
                            <td>{nota.status}</td>
                        </tr>
                    ";
                }

                html += "</table>";
            }

            var resumoCFOP = notas
                .Where(n => n.status == "Autorizado")
                .SelectMany(n => n.cfop.Zip(n.vProdItem, (cfop, valor) => new { CFOP = cfop, Valor = valor }))
                .GroupBy(item => item.CFOP)
                .Select(g => new { CFOP = g.Key, SomaValores = g.Sum(i => i.Valor) });

            html += $@"
                <h1>Resumo por CFOP</h1>
                <table>
                    <tr><th>CFOP</th><th>Valor</th></tr>
            ";

            foreach (var item in resumoCFOP)
            {
                html += $@"
                    <tr>
                        <td>{item.CFOP}</td>
                        <td>R$ {item.SomaValores:F2}</td>
                    </tr>
                ";
            }

            html += "</table>";

            var resumoCSOSN = notas
                .Where(n => n.status == "Autorizado")
                .SelectMany(n => n.csosn.Zip(n.vProdItem, (csosn, valor) => new { CSOSN = csosn, Valor = valor }))
                .GroupBy(item => item.CSOSN)
                .Select(g => new { CSOSN = g.Key, SomaValores = g.Sum(i => i.Valor) });

            html += $@"
                <h1>Resumo por CSOSN</h1>
                <table>
                    <tr><th>CSOSN</th><th>Valor</th></tr>
            ";

            foreach (var item in resumoCSOSN)
            {
                html += $@"
                    <tr>
                        <td>{item.CSOSN}</td>
                        <td>R$ {item.SomaValores:F2}</td>
                    </tr>
                ";
            }

            html += "</table>";

            List<int> numerosNotas = notas
            .Where(n => n.status == "Autorizado")
            .Select(n => int.Parse(n.nNf))
            .ToList();

            if (numerosNotas.Any())
            {
                int menor = numerosNotas.Min();
                int maior = numerosNotas.Max();
                List<int> sequencia = Enumerable.Range(menor, maior - menor + 1).ToList();

                
                List<int> numerosNotasCanceladas = notas
                .Where(n => n.status == "Cancelado" && !string.IsNullOrEmpty(n.chNFe) && n.chNFe.Length >= 31)
                .Select(n =>
                {
                    string numeroNota = n.chNFe.Substring(25, 9); 
                    if (int.TryParse(numeroNota.TrimStart('0'), out int numero))
                    {
                        return numero;
                    }
                    return -1; 
                })
                .Where(num => num > 0)
                .Distinct() 
                .ToList();

                
                List<int> faltantes = sequencia.Except(numerosNotas).ToList();

                
                faltantes = faltantes.Except(numerosNotasCanceladas).ToList();

                html += "<h1>Notas Fiscais Faltantes na Sequência</h1><ol>";

                if (faltantes.Any())
                {
                    foreach (var numero in faltantes)
                    {
                        html += $"<li>{numero}</li>";
                    }
                }
                else
                {
                    html += "<li>Nenhuma nota faltando na sequência.</li>";
                }

                html += "</ol>";
            }

            html += "</body></html>";

            return html;
        }

        public string GerarRelatorioCupons(List<CupomFiscal> cupons, string nomeRelatorio)
        {
            string nomeRel = nomeRelatorio;

            string html = $@"
                <!DOCTYPE html>
                <html lang='pt-BR'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Relatorio de faturamento</title>
                    <style>
                        body {{ 
                            font-family: Arial, sans-serif; 
                            margin: 20px; 
                        }}
                        h1 {{ 
                            color: #333; 
                            text-align: center; 
                        }}
                        h2 {{ 
                            color: #333; 
                            text-align: right; 
                        }}
                        
                        footer {{ 
                            text-align: center; 
                            margin-top: 20px; 
                            font-size: 0.8em; 
                            color: #777; 
                        }}
                        th{{
                            border-bottom: 2px solid;
                            font-size: 20px;
                            padding-left: 20px;
                            padding-right: 20px;
                            padding-bottom: 10px;
                        }}
                        td {{
                            font-size: 15px;
                            padding-left: 20px;
                            padding-right: 20px;
                            padding-bottom: 10px;
                        }}
                        div {{
                            page-break-before: always;
                        }}
                        
                    </style>
                </head>
                <body>
                    <h1>{nomeRel}</h1>

            ";

            var cuponsAgrupadosPorDiax = cupons
            .GroupBy(cuponsDatax => cuponsDatax.data.Value)
            .Select(grupox => new
            {
                Data = grupox.Key,
                Total = grupox.Sum(cuponsDatax => cuponsDatax.vCFE)
            })
            .OrderBy(grupox => grupox.Data);

            foreach (var datax in cuponsAgrupadosPorDiax)
            {
                html += $@"



                    <table>
                        <caption>Cupons Autorizados</caption>
                        <tr>
                            <th>CF-e</th>
                            <th>CPF/CNPJ</th>
                            <th>Chave de acesso</th>
                            <th>Bruto</th>
                            <th>Desc.</th>
                            <th>Acres.</th>
                            <th>Valor</th>
                        </tr>
                ";

                decimal totalCupom = 0;
                foreach (var cupom in cupons)
                {
                    if (cupom.data == datax.Data)
                    {
                        if (cupom.status == "Autorizado")
                        {
                            totalCupom += cupom.vCFE;
                            html += $@"
                                    <tr>
                                        <td>{cupom.nCFE}</td>
                                        <td>{cupom.CNPJ}</td>
                                        <td>{cupom.chCFE}</td>
                                        <td>R$ {cupom.vProd}</td>
                                        <td>R$ {cupom.vDesc}</td>
                                        <td>R$ {cupom.vOutro}</td>
                                        <td>R$ {cupom.vCFE}</td>
                                    </tr>";
                        }
                    }
                }
                html += $@"
                        </table>
                        <h2>{datax.Data:dd/MM/yyyy} Total em Cupons autorizados: R$ {datax.Total:F2}</h2>
                ";
            }

            html += $@"
                
                
                <table>
                    <caption>Cupons Cancelados</caption>
                    <tr>
                        <caption>Cupons Cancelados</caption>
                        <th>CF-e</th>
                        <th>CPF/CNPJ</th>
                        <th>Chave de acesso</th>
                        <th>Valor</th>
                    </tr>
                ";

            decimal totalCupomCancelado = 0;
            foreach (var cupom in cupons)
            {
                if (cupom.status == "Cancelado")
                {
                    totalCupomCancelado += cupom.vCFE;
                    html += $@"
                        
                        <tr>
                            <td>{cupom.nCFE}</td>
                            <td>{cupom.CNPJ}</td>
                            <td>{cupom.chCFE}</td>
                            <td>R$ {cupom.vCFE}</td>
                        </tr>";
                }
            }

            html += $@"
                    </table>
                    <h2>Total em Cupons cancelados: R$ {totalCupomCancelado.ToString("#,##0.00", new CultureInfo("pt-BR"))}</h2>
                    

                ";


            html += $@"
                    <div></div>

                    <h1>Resumo por Dia</h1>
                    
                    <table>
                        <tr>
                            <th>Dia</th>
                            <th>Valor</th>
                        </tr>
            ";
            decimal totalGeral = 0;
            var cuponsAgrupadosPorDia = cupons
            .GroupBy(cuponsData => cuponsData.data.Value)
            .Select(grupo => new
            {
                Data = grupo.Key,
                Total = grupo.Sum(cuponsData => cuponsData.vCFE)
            })
            .OrderBy(grupo => grupo.Data);

            foreach (var data in cuponsAgrupadosPorDia)
            {
                totalGeral += data.Total;

                html += $@"
                        <tr>
                            <td>{data.Data:dd/MM/yyyy}</td>
                            <td>R$ {data.Total:F2}</td>
                        </tr>
                ";
            }



            html += $@"
                    </table>


                    <h2>Total Geral no periodo: R$ {totalGeral:F2}</h2>

                    <div></div>

                    <h1>Resumo por natureza de operação</h1>
                    
                    <table>
                        <tr>
                            <th>Nat. Operação</th>
                            <th>Valor</th>
                        </tr>
             ";

            var resumo = cupons
            .Where(cupomx => cupomx.status == "Autorizado")
            .SelectMany(cupomx => cupomx.cfop.Zip(cupomx.vProdItem, (cfop, valor) => new { CFOP = cfop, Valor = valor }))
            .GroupBy(item => item.CFOP)
            .Select(grupo => new
            {
                CFOP = grupo.Key,
                SomaValores = grupo.Sum(item => item.Valor)
            });
            foreach (var item in resumo)
            {

                html += $@"
                        <tr>
                            <td>{item.CFOP}</td>
                            <td>{item.SomaValores:C}</td>
                        </tr>
                        ";
            }
            html += $@"
                    </table>
                    
                    <h1>Resumo por CSOSN</h1>
                    <table>
                        <tr>
                            <th>CSOSN/CST</th>
                            <th>Valor</th>
                        </tr>
            ";
            var resumoCSOSN = cupons
            .Where(cupomxy => cupomxy.status == "Autorizado")
            .SelectMany(cupomxy => cupomxy.csosn.Zip(cupomxy.vProdItem, (csosn, valor) => new { CSOSN = csosn, Valor = valor }))
            .GroupBy(itemx => itemx.CSOSN)
            .Select(grupo => new
            {
                CSOSN = grupo.Key,
                SomaValores = grupo.Sum(item => item.Valor)
            });
            foreach (var itemx in resumoCSOSN)
            {

                html += $@"
                        <tr>
                            <td>{itemx.CSOSN}</td>
                            <td>{itemx.SomaValores:C}</td>
                        </tr>
                        ";
            }

            html += $@"
                    </table>
                    
                    <h1>Cupons Faltantes na sequencia</h1>

                    <ol>
                    
            ";

            List<int> numerosCupons = new List<int>();
            foreach (var cupom in cupons)
            {
                numerosCupons.Add(Convert.ToInt32(cupom.nCFE));

            }

            int menor = numerosCupons.Min();
            int maior = numerosCupons.Max();

            List<int> sequencia = Enumerable.Range(menor, maior - menor + 1).ToList();

            List<int> faltantes = sequencia.Except(numerosCupons).ToList();

            if (faltantes.Any())
            {

                faltantes.ForEach(numero => html += $@"<li>{numero}</li>");
            }

            html += $@"
                    </ol>
                </body>
                </html>";

            return html;
        }

        public async Task<string> GerarRelatorioCuponsNCFE(List<NFCE> cupons, string nomeRelatorio)
        {
            string nomeRel = nomeRelatorio;

            string html = $@"
                <!DOCTYPE html>
                <html lang='pt-BR'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Relatorio de faturamento</title>
                    <style>
                        body {{ 
                            font-family: Arial, sans-serif; 
                            margin: 20px; 
                        }}
                        h1 {{ 
                            color: #333; 
                            text-align: center; 
                        }}
                        h2 {{ 
                            color: #333; 
                            text-align: right; 
                        }}
                        
                        footer {{ 
                            text-align: center; 
                            margin-top: 20px; 
                            font-size: 0.8em; 
                            color: #777; 
                        }}
                        th{{
                            border-bottom: 2px solid;
                            font-size: 20px;
                            padding-left: 20px;
                            padding-right: 20px;
                            padding-bottom: 10px;
                        }}
                        td {{
                            font-size: 15px;
                            padding-left: 20px;
                            padding-right: 20px;
                            padding-bottom: 10px;
                        }}
                        div {{
                            page-break-before: always;
                        }}
                        
                    </style>
                </head>
                <body>
                    <h1>{nomeRel}</h1>

            ";

            var cuponsAgrupadosPorDiax = cupons
            .GroupBy(cuponsDatax => cuponsDatax.dhEmi.Value.Date)
            .Select(grupox => new
            {
                Data = grupox.Key,
                Total = grupox.Sum(cuponsDatax => cuponsDatax.vNF)
            })
            .OrderBy(grupox => grupox.Data);

            html += $@"
                    <table>
                        <caption>Cupons Autorizados</caption>
                        <tr>
                            <th>NFC-e</th>
                            <th>CPF/CNPJ</th>
                            <th>Chave de acesso</th>
                            <th>Bruto</th>
                            <th>Desc.</th>
                            <th>Acres.</th>
                            <th>Valor</th>
                        </tr>
                ";

            foreach (var datax in cuponsAgrupadosPorDiax)
            {
                decimal totalCupom = 0;
                foreach (var cupom in cupons)
                {
                    if (cupom.dhEmi == datax.Data)
                    {
                        if (cupom.status == "Autorizado")
                        {
                            totalCupom += cupom.vNF;
                            html += $@"
                                    <tr>
                                        <td>{cupom.nNF}</td>
                                        <td>{cupom.CNPJ}</td>
                                        <td>{cupom.chCFE}</td>
                                        <td>R$ {cupom.vProd}</td>
                                        <td>R$ {cupom.vDesc}</td>
                                        <td>R$ {cupom.vOutro}</td>
                                        <td>R$ {cupom.vNF}</td>
                                    </tr>";

                            
                        }
                    }
                }

                /*html += $@"
                                </table>
                                <h2>{datax.Data:dd/MM/yyyy} Total em Cupons autorizados: R$ {totalCupom:F2}</h2>
                            ";*/

            }

            html += $@"
                </table>
                ";


            /*
            html += $@"
                
                
                <table>
                    <caption>Cupons sem tag de Autorizados</caption>
                    <tr>
                        <caption>Cupons não Autorizados</caption>
                        <th>NFC-e</th>
                        <th>CPF/CNPJ</th>
                        <th>Chave de acesso</th>
                        <th>Valor</th>
                    </tr>
                ";
            decimal totalCupomNaoAutorizado = 0;
            foreach (var cupom in cupons)
            {
                if (cupom.status == "Não Autorizado")
                {
                    totalCupomNaoAutorizado += cupom.vNF;
                    html += $@"
                        
                        <tr>
                            <td>{cupom.nNF}</td>
                            <td>{cupom.CNPJ}</td>
                            <td>{cupom.chCFE}</td>
                            <td>R$ {cupom.vNF}</td>
                        </tr>";
                }
            }
            */

            html += $@"
                <table>
                    <caption>Cupons Cancelados</caption>
                    <tr>
                        <caption>Cupons Cancelados</caption>
                        <th>NFC-e</th>
                        <th>CPF/CNPJ</th>
                        <th>Chave de acesso</th>
                        <th>Valor</th>
                    </tr>
                ";

            decimal totalCupomCancelado = 0;
            foreach (var cupom in cupons)
            {
                if (cupom.status == "Cancelado")
                {
                    totalCupomCancelado += cupom.vNF;
                    html += $@"
                        
                        <tr>
                            <td>{cupom.nNF}</td>
                            <td>{cupom.CNPJ}</td>
                            <td>{cupom.chCFE}</td>
                            <td>R$ {cupom.vNF}</td>
                        </tr>";
                }
            }

            html += $@"
                    </table>
                    <h2>Total em Cupons cancelados: R$ {totalCupomCancelado.ToString("#,##0.00", new CultureInfo("pt-BR"))}</h2>
                ";


            html += $@"
                    <div></div>

                    <h1>Resumo por Dia</h1>
                    
                    <table>
                        <tr>
                            <th>Dia</th>
                            <th>Valor</th>
                        </tr>
            ";

            decimal totalGeral = 0;

            var cuponsAgrupadosPorDia = cupons
            .GroupBy(cuponsData => cuponsData.dhEmi.Value)
            .Select(grupo => new
            {
                Data = grupo.Key,
                Total = grupo.Sum(cuponsData => cuponsData.vNF)
            })
            .OrderBy(grupo => grupo.Data);

            foreach (var data in cuponsAgrupadosPorDia)
            {
                totalGeral += data.Total;

                html += $@"
                        <tr>
                            <td>{data.Data:dd/MM/yyyy}</td>
                            <td>R$ {data.Total:F2}</td>
                        </tr>
                ";
            }



            html += $@"
                    </table>
                    
                    <h2>Total Geral no periodo: R$ {totalGeral:F2}</h2>

                    <div></div>

                    <h1>Resumo por natureza de operação</h1>
                    
                    <table>
                        <tr>
                            <th>Nat. Operação</th>
                            <th>Valor</th>
                        </tr>
             ";

            var resumo = cupons
            .Where(cupomx => cupomx.status == "Autorizado")
            .SelectMany(cupomx => cupomx.cfop.Zip(cupomx.vProdItem, (cfop, valor) => new { CFOP = cfop, Valor = valor }))
            .GroupBy(item => item.CFOP)
            .Select(grupo => new
            {
                CFOP = grupo.Key,
                SomaValores = grupo.Sum(item => item.Valor)
            });
            foreach (var item in resumo)
            {

                html += $@"
                        <tr>
                            <td>{item.CFOP}</td>
                            <td>{item.SomaValores:C}</td>
                        </tr>
                        ";
            }
            html += $@"
                    </table>
                    
                    <h1>Resumo por CSOSN</h1>
                    <table>
                        <tr>
                            <th>CSOSN/CST</th>
                            <th>Valor</th>
                        </tr>
            ";
            var resumoCSOSN = cupons
            .Where(cupomxy => cupomxy.status == "Autorizado")
            .SelectMany(cupomxy => cupomxy.csosn.Zip(cupomxy.vProdItem, (csosn, valor) => new { CSOSN = csosn, Valor = valor }))
            .GroupBy(itemx => itemx.CSOSN)
            .Select(grupo => new
            {
                CSOSN = grupo.Key,
                SomaValores = grupo.Sum(item => item.Valor)
            });
            foreach (var itemx in resumoCSOSN)
            {

                html += $@"
                        <tr>
                            <td>{itemx.CSOSN}</td>
                            <td>{itemx.SomaValores:C}</td>
                        </tr>
                        ";
            }

            html += $@"
                    </table>
                    
                    <h1>Cupons Faltantes na sequencia</h1>

                    <ol>
                    
            ";

            List<int> numerosCupons = new List<int>();
            foreach (var cupomx in cupons)
            {
                numerosCupons.Add(Convert.ToInt32(cupomx.nNF));

            }

            int menor = numerosCupons.Min();
            int maior = numerosCupons.Max();

            List<int> sequencia = Enumerable.Range(menor, maior - menor + 1).ToList();

            List<int> faltantes = sequencia.Except(numerosCupons).ToList();

            if (faltantes.Any())
            {

                faltantes.ForEach(numero => html += $@"<li>{numero}</li>");
            }

            html += $@"
                    </ol>
                </body>
                </html>";

            return html;
        }
    }
}