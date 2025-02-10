using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lerXML.Modelos
{
    public class NFCE
    {
        [JsonIgnore] public string? nNF { get; set; }
        [JsonIgnore] public DateTime? dhEmi { get; set; }
        [JsonIgnore] public string? CNPJ { get; set; }
        [JsonIgnore] public string? chCFE { get; set; }
        [JsonIgnore] public decimal? vProd { get; set; }
        [JsonIgnore] public decimal? vDesc { get; set; }
        [JsonIgnore] public decimal? vOutro { get; set; }
        [JsonIgnore] public decimal vNF { get; set; }
        [JsonIgnore] public string? status { get; set; }
        [JsonIgnore] public List<string>? cfop { get; set; }
        [JsonIgnore] public List<string>? csosn { get; set; }
        [JsonIgnore] public List<decimal>? vProdItem { get; set; }
        public string? caminhoNFCE { get; set; }
    }
}
