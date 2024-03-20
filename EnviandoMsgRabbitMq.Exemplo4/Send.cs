using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnviandoMsgRabbitMq.Exemplo4
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

                    //3 filas independentes
                    channel.QueueDeclare(queue: "email",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null
                                         );

                    channel.QueueDeclare(queue: "whatsapp",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null
                                         );

                    channel.QueueDeclare(queue: "external",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null
                                         );

                    //criamos uma exchange, chamada Topic do tipo direct e quando utilizamos ele
                    //passamos a routingKey tambem nas queyebinds
                    channel.ExchangeDeclare(exchange: "alerts", type: "topic");

                    //3 binds de fila apontando pro Exchange e e cada uma aborda uma rountingKey
                    //diferente, essa routing key pode ser abordada pr varias mensagens diferentes nas
                    //Publish
                    channel.QueueBind(queue: "email", exchange: "alerts", routingKey: "*.web.*"); // * só uma palavra
                    channel.QueueBind(queue: "whatsapp", exchange: "alerts", routingKey: "*.mobile.*"); 
                    channel.QueueBind(queue: "external", exchange: "alerts", routingKey: "game.#"); // # quaquer coisa

                    var count = 0;
                    while (true)
                    {
                        string message = $"Alert direct process message: {count++}";

                        var body1 = Encoding.UTF8.GetBytes($"{message} - payments");
                        var body2 = Encoding.UTF8.GetBytes($"{message} - orders");
                        var body3 = Encoding.UTF8.GetBytes($"{message} - clients");
                        var body4 = Encoding.UTF8.GetBytes($"{message} - game");

                        // essa cai na fila de email
                        channel.BasicPublish(exchange: "alerts",
                                              routingKey: "paymenst.web.exec",
                                              basicProperties: null,
                                              body: body1);

                        // essa cai na fila de email
                        channel.BasicPublish(exchange: "alerts",
                                              routingKey: "orders.web.exec",
                                              basicProperties: null,
                                              body: body2);

                        // essa cai na fila de email
                        channel.BasicPublish(exchange: "alerts",
                                              routingKey: "clients.web.exec",
                                              basicProperties: null,
                                              body: body3);

                        // essa cai na fila de whatsapp e external
                        channel.BasicPublish(exchange: "alerts",
                                              routingKey: "game.mobile.playing",
                                              basicProperties: null,
                                              body: body4);

                        Console.WriteLine($" [x] Sent: {message} ");

                        System.Threading.Thread.Sleep(100);


                    }

                }
            }
        }
    }
}
