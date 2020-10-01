using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicoEnvio.Controllers
{
    public class TimerController : IJob
    {
        //Classe implementando a interface do Quartz para controle de tempo
        //Configurações implementadas no Startup.cs
        public Task Execute(IJobExecutionContext context)
        {      
            //Chamando a classe que cria a mensagem automaticamente
            EnvioMensagemController.AutoMsg();

            return Task.CompletedTask;

        }

    }
}
