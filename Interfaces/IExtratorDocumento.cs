using lerXML.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace lerXML.Interface
{
    public interface IExtratorDocumento
    {
        public interface IExtratorDocumento<T>
        {
            List<T> Extrair(XDocument xml, string nomeArquivo);
        }
    }
}
