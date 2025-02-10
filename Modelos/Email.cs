using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lerXML.Modelos
{
    public class Email
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string ServidorSMTP { get; set; }
        public int Porta { get; set; }
        public string Destinatario { get; set; }
        public string Copia { get; set; }
        public string Assunto { get; set; }
        public string Mensagem { get; set; }
        public string Anexo { get; set; }

    }
}
