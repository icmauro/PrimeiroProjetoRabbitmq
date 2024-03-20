using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace EnviandoMsgRabbitMq.Exemplo2
{
    class Send
    {
        //MODO FANOUT
        static void Main(string[] args)
        {
            ///
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //criamos 2 filas Queue
                    channel.QueueDeclare(queue: "MyQueuei1",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null
                                         );

                    channel.QueueDeclare(queue: "MyQueuei2",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null
                                         );

                    //criamos uma exchange, chamada Logs do tipo fanout
                    channel.ExchangeDeclare(exchange: "logs", type: "fanout");

                    //vinculamos as 2 filas que criamos (bind) que irá monitorar o
                    //Exchange logs do tipo fanout
                    channel.QueueBind(queue: "MyQueuei1", exchange: "logs", "");

                    var count = 0;
                    while (true)
                    {
                        string message = $"Fanout World: {count++}";

                        var body = Encoding.UTF8.GetBytes(message);

                        //colocamos o exchange apontando pro Logs que ira mandar
                        //mensagem pro Exchange Logs
                        channel.BasicPublish(exchange: "logs",
                                             routingKey: "",
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
