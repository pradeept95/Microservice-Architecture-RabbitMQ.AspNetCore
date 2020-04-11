using RabbitMQ.Client;
using System;
using System.Text;

namespace TestPublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("----------- Hi, I am Publisher using RabbitMQ ----------------");
            var message = "";
            while (true)
            {
                Console.WriteLine("Enter message");
                message = Console.ReadLine();
                PublishMessage(message);
                Console.WriteLine("Type exit to Exit or pressa Enter to continue");
                var key = Console.ReadLine();
                if (key == "exit")
                {
                    break;
                }
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void PublishMessage(string message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(
                     exchange: "HotBag.Service.EmailService",
                     type: "direct", // "topic", "direct", "headers", "fanout"
                     durable: true,
                     autoDelete: false,
                     null
                     );

                channel.QueueDeclare(queue: "EmailService.Default",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.QueueDeclare(queue: "EmailService.Extended",
                   durable: false,
                   exclusive: false,
                   autoDelete: false,
                   arguments: null);

                channel.QueueBind("EmailService.Default", "HotBag.Service.EmailService", "default", null);
                channel.QueueBind("EmailService.Extended", "HotBag.Service.EmailService", "extended", null);


                var defaultBody = Encoding.UTF8.GetBytes($"Default : {message}");
                var extendedBody = Encoding.UTF8.GetBytes($"Extended : {message}");

                channel.BasicPublish(exchange: "HotBag.Service.EmailService",
                    routingKey: "default",
                    basicProperties: null,
                    body: defaultBody);

                channel.BasicPublish(exchange: "HotBag.Service.EmailService",
                    routingKey: "extended",
                    basicProperties: null,
                    body: extendedBody);
            }
        }
    }
}
