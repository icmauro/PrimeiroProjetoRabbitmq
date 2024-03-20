using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnviandoMsgRabbitMq.Exemplo2
{
    class Send
    {
        //MODO DIRECT - MANDO UMA MENSAGEM PRA UMA/VARIAS FILAS ESPECIFICA
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //criamos 2 filas Queue

                    //fila que processa a imagem
                    channel.QueueDeclare(queue: "imageProcess",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null
                                         );

                    //fila que arquiva essa imagem
                    channel.QueueDeclare(queue: "imageArchive",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null
                                         );

                    //criamos uma exchange, chamada Logs do tipo direct e quando utilizamos ele
                    //passamos a routingKey tambem
                    channel.ExchangeDeclare(exchange: "image", type: "direct");

                    //vinculamos as 2 filas que criamos (bind) que irá monitorar o
                    //Exchange logs do tipo fanout
                    channel.QueueBind(queue: "imageProcess", exchange: "image", routingKey: "crop");
                    channel.QueueBind(queue: "imageProcess", exchange: "image", routingKey: "resize");
                    channel.QueueBind(queue: "imageArchive", exchange: "image", routingKey: "resize");

                    var count = 0;
                    while (true)
                    {
                        string message = $"Image direct process message: {count++}";

                        var body1 = Encoding.UTF8.GetBytes($"{message} - crop");
                        var body2 = Encoding.UTF8.GetBytes($"{message} - resize");

                        //Exchage imagem que vai cair somente na fila imageProcess (1 mensagem)
                        //plugado pelo routingKey
                        channel.BasicPublish(exchange: "image",
                                             routingKey: "crop",
                                             basicProperties: null,
                                             body: body1);

                        //Exchange imagem vai cair na fila imageProcess e imageArchive (2 mensagem)
                        //plugado pelo routingKey
                        channel.BasicPublish(exchange: "image",
                                              routingKey: "resize",
                                              basicProperties: null,
                                              body: body1);

                        Console.WriteLine($" [x] Sent: {message} ");

                        System.Threading.Thread.Sleep(100);


                    }

                }
            }
        }
    }
}
