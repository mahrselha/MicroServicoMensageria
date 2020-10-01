using System;
using System.Collections.Generic;
using System.Text;

namespace MicroServicoRecebeMsg.Domain
{
    class EnvioMensagem
    {
        public int Id { get; set; }

        public string Mensagem { get; set; }

        public string IdMS { get; set; }

        public string HoraEnvio { get; set; }
    }
}
