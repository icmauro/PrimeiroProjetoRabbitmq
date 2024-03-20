using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecebendoMsgRabbitMq.Exemplo3
{
    class Receive
    {
        static void Main(string[] args)
        {
            //MODO DIRECT
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //não pegar a fila toda, sempre pegar de 2 filas em 2 filas
                    channel.BasicQos(0, prefetchCount: 2, false);

                    var queueName1 = "imagemProcess";
                    var queueName2 = "imagemArchive";

                    //declarando  queue que contem o nome chave que é o mesmo que o pubisher
                    channel.QueueDeclare(queue: queueName1,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null
                                         );

                    channel.QueueDeclare(queue: queueName2,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null
                                         );

                    //consumindo o canal Consumer/Consumidor baseado em eventos
                    var consumer1 = new EventingBasicConsumer(channel);

                    var consumer2 = new EventingBasicConsumer(channel);

                    consumer1.Received += (model, ea) =>
                    {
                        try
                        {
                            //resgatando a mensagem
                            var body = ea.Body;

                            // transformando de bytes para string
                            var message = Encoding.UTF8.GetString(body.ToArray());

                            Console.WriteLine($"[x] C1 - Process: {message}");

                            // ack tudo que eu recebo e
                            // processei/comfirmei - retiro a mensagem da fila
                            channel.BasicAck(ea.DeliveryTag, false);

                        }
                        catch (Exception ex)
                        {
                            //caso tenha causado algum erro ussamos o Nack não
                            //processei - devolve a mensagem pra fila
                            channel.BasicNack(ea.DeliveryTag, false, true);
                        }

                    };

                    consumer2.Received += (model, ea) =>
                    {
                        try
                        {
                            //resgatando a mensagem
                            var body = ea.Body;

                            // transformando de bytes para string
                            var message = Encoding.UTF8.GetString(body.ToArray());

                            Console.WriteLine($"[x] C2 - Archive: {message}");

                            // ack tudo que eu recebo e
                            // processei/comfirmei - retiro a mensagem da fila
                            channel.BasicAck(ea.DeliveryTag, false);

                        }
                        catch (Exception ex)
                        {
                            //caso tenha causado algum erro ussamos o Nack não
                            //processei - devolve a mensagem pra fila
                            channel.BasicNack(ea.DeliveryTag, false, true);
                        }

                    };

                    channel.BasicConsume(queue: queueName1, autoAck: false, consumer: consumer1);
                    channel.BasicConsume(queue: queueName2, autoAck: false, consumer: consumer1);

                }
            }
        }
    }
}
