using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lerXML.Classes
{
    public class CupomFiscal
    {
        [JsonIgnore] public string? nCFE { get; set; }
        [JsonIgnore] public DateTime? data { get; set; }
        [JsonIgnore] public string? CNPJ { get; set; }
        [JsonIgnore] public string? chCFE { get; set; }
        [JsonIgnore] public decimal vProd { get; set; }
        [JsonIgnore] public decimal vDesc { get; set; }
        [JsonIgnore] public decimal vOutro { get; set; }
        [JsonIgnore] public decimal vCFE { get; set; }
        [JsonIgnore] public string status { get; set; }
        [JsonIgnore] public List<string> cfop { get; set; }
        [JsonIgnore] public List<string> csosn { get; set; }
        [JsonIgnore] public List<decimal> vProdItem { get; set; }
        [JsonIgnore] public string? nserieSAT { get; set; }

        public string? caminhoCupom { get; set; }

    }
}
