using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EnviandoMsgRabbitMq
{
    class Send
    {
        //MODO SIMPLES
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null
                                         );
                    var count = 0;
                    while (true)
                    {
                        string message = $"Hello World: {count++}";
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                             routingKey: "hello",
                                             basicProperties: null,
                                             body: body);

                        Console.WriteLine($" [x] Sent: {message} ");

                        System.Threading.Thread.Sleep(100);

                    }

                }
            }

        }
    }
}
