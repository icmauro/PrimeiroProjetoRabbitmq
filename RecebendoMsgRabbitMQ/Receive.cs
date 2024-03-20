using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecebendoMsgRabbitMQ
{
    internal class Receive
    {
        //MODO SIMPLES
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //declarando  queue que contem o nome chave que é o mesmo que o pubisher
                    channel.QueueDeclare(queue: "hello",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null
                                         );

                    //consumindo o canal Consumer/Consumidor baseado em eventos
                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {
                        try
                        {
                            //resgatando a mensagem
                            var body = ea.Body;

                            // transformando de bytes para string
                            var message = Encoding.UTF8.GetString(body.ToArray());

                            Console.WriteLine($"[x] Received: {message}");

                            // ack tudo que eu recebo e
                            // processei/comfirmei - retiro a mensagem da fila
                            channel.BasicAck(ea.DeliveryTag, false);

                        }
                        catch (Exception ex)
                        {
                            //caso tenha causado algum erro ussamos o Nack não
                            //nao processei - devolve a mensagem pra fila
                            channel.BasicNack(ea.DeliveryTag, false, true);
                        }

                        //vinculamos nossa fila ao cosumidor, autoack(tudo que eu recebo ja tiro da fila = true)(recebi processei)
                        channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);

                    };

                }
            }
        }
    }
}
