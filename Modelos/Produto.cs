using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lerXML.Classes
{
    public class Produto
    {
        public string? codProd { get; set; }
        public string? xProd { get; set; }
        public string? ncm { get; set; }
        public string? cfop { get; set; }
        public string? csosn { get; set; }
        public string? uCom { get; set; }
        public string? uTrib { get; set; }
        public decimal? qCom { get; set; }
        public decimal? vUnCom { get; set; }
        public decimal? vProd { get; set; }
        public decimal? qTrib { get; set; }
        public decimal? vUnTrib { get; set; }
        public string? cst { get; set; }
        public decimal? pRedBC { get; set; }
        public decimal? vBC { get; set; }
        public decimal? pICMS { get; set; }
        public decimal? vICMS { get; set; }
    }
}
