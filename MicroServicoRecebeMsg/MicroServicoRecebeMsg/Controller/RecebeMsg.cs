using MicroServicoRecebeMsg.Domain;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace MicroServicoRecebeMsg
{
    class RecebeMsg
    {
        static void Main(string[] args)
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

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine(" [x] Recebido {0}", message);

                    var mensagem = System.Text.Json.JsonSerializer.Deserialize<EnvioMensagem>(message);
                };
                channel.BasicConsume(queue: "MensagemQueue",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Pressione 0 para sair");
                Console.ReadLine();
            }
        }
    }
}
