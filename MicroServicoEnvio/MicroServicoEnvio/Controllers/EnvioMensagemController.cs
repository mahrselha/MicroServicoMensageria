using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MicroServicoEnvio.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using RabbitMQ.Client;
using Serilog.Data;

namespace MicroServicoEnvio.Controllers
{
    [Route("api/controller")]
    [ApiController]
    public class EnvioMensagemController : ControllerBase
    {
        //Geração de Log 
        private static ILogger<EnvioMensagemController> _logger;

        public EnvioMensagemController(ILogger<EnvioMensagemController> logger)
        {
            _logger = logger;
        }
        //Cria a mensagem automaticamente a cada 5 segundos
        public static void AutoMsg()
        {
            //Utilizado RabbitMQ para serviço de mensageria 
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "MensagemQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                    string message = JsonSerializer.Serialize(CriarMsg());

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "MensagemQueue",
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Enviado {0}", message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao tentar criar/enviar a mensagem", ex);
            }
        }

        public static EnvioMensagem CriarMsg()
        {
            EnvioMensagem msg = new EnvioMensagem();

            Random randNum = new Random();

            msg.Id = randNum.Next(1000);
            msg.IdMS = "MicroServico de envio de Mensagens";
            msg.Mensagem = "Hello World";
            msg.HoraEnvio = DateTime.Now.ToString();

            return msg;
        }

        //Ler a mensagem à partir de um Json
        public IActionResult LerMsg(EnvioMensagem msg)
        {
            try
            {
                //Utilizado RabbitMQ para serviço de mensageria 
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "MensagemQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                    string message = JsonSerializer.Serialize(msg);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "MensagemQueue",
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Enviado {0}", message);

                }

                return Accepted(msg);

            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao criar/enviar mensagem: ", ex);

                return new StatusCodeResult(500);
            }
        }
    }
}
