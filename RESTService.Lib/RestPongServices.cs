using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Threading.Tasks;

namespace RESTService.Lib
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RestPongServices:IRESTPongServices
    {
        public int numeromensajesrecibidos = 0;
        public int numeromensajescontestados = 0;
        public bool servicioniciado = false;

        public  string GetEstadisticas()
        {
            string estadisticas = "";
            estadisticas = numeromensajesrecibidos + "," + numeromensajescontestados;
            
            return estadisticas;
        }

        public void Iniciar()
        {
            Task.Run(async () =>
            {
                if (!servicioniciado)
                {
                    InicializarPong();
                }
            });
        }

        public void InicializarPong()
        {
            servicioniciado = true;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "pingpong_queue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);


                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queue: "pingpong_queue",
                                     noAck: false,
                                     consumer: consumer);

                Console.WriteLine(" [x] Waiting for work.");

                while (true)
                {
                    string response = null;
                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body);
                        int n = int.Parse(message);
                        numeromensajesrecibidos++;
                        Thread.Sleep(2 * 1000);
                        Console.WriteLine(" [.] fib({0})", message);
                        response = fib(n).ToString();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                        response = "";
                    }
                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        channel.BasicPublish(exchange: "",
                                             routingKey: props.ReplyTo,
                                             basicProperties: replyProps,
                                             body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                                         multiple: false);
                        numeromensajescontestados++;
                        Console.WriteLine("Mensajes reicibidos {0}", numeromensajesrecibidos);
                        Console.WriteLine("Mensajes contestados {0}", numeromensajescontestados);
                    }
                }
            }
        }

        /// <summary>
        /// Valores positivos.
        /// </summary>
        private static int fib(int n)
        {
            if (n == 0 || n == 1)
            {
                return n;
            }

            return fib(n - 1) + fib(n - 2);
        }
    }
}
