using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lerXML.Classes
{
    public class NotaFiscal
    {
        [JsonIgnore] public string? cNF { get; set; }
        [JsonIgnore] public string? chNFe { get; set; }
        [JsonIgnore] public string? natOp { get; set; }
        [JsonIgnore] public string? nNf { get; set; }
        [JsonIgnore] public DateTime? dhEmi { get; set; }
        [JsonIgnore] public decimal vNF { get; set; }
        [JsonIgnore] public string? CNPJ { get; set; }
        [JsonIgnore] public string? status { get; set; }
        [JsonIgnore] public List<string>? cfop { get; set; }
        [JsonIgnore] public List<string>? csosn { get; set; }
        [JsonIgnore] public List<decimal>? vProdItem { get; set; }
        public string? caminhoNota { get; set; }

    }
}
