using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HotBag.Service.EmailServices.Worker
{
    public class WorkerService : BackgroundService
    {
        IConnectionFactory _factory { get; set; }
        IConnection _connection { get; set; }
        IModel _channel { get; set; }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost",
                VirtualHost = "/",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.BasicQos(0, 1, false);
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (_connection != null) _connection.Close();
            if (_channel != null) _channel.Close();
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //_channel.ExchangeDeclare(
            //    exchange: "HotBag.Service.EmailService",
            //    type: "direct", // "topic", "direct", "headers", "fanout"
            //    durable: true,
            //    autoDelete: false,
            //    null
            //    );

            //_channel.QueueDeclare(queue: "EmailService.Default",
            //    durable: false,
            //    exclusive: false,
            //    autoDelete: false,
            //    arguments: null);

            //_channel.QueueDeclare(queue: "EmailService.Extended",
            //   durable: false,
            //   exclusive: false,
            //   autoDelete: false,
            //   arguments: null);

            //_channel.QueueBind("EmailService.Default", "HotBag.Service.EmailService", "default", null);
            //_channel.QueueBind("EmailService.Extended", "HotBag.Service.EmailService", "extended", null);

            var consumerDefault = new EventingBasicConsumer(_channel);
            consumerDefault.Received += ReceiveDefaultMessage;
            _channel.BasicConsume("EmailService.Default", autoAck: false, consumer: consumerDefault);

            var consumerExtended = new EventingBasicConsumer(_channel);
            consumerExtended.Received += ReceiveExtendedMessage;
            _channel.BasicConsume("EmailService.Extended", autoAck: false, consumer: consumerExtended);

            await Task.FromResult(true);
        }

        public void ReceiveDefaultMessage(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received: {message}");

            //if any error is happend
            // set as Nak : done by BasicNack
            //_channel.BasicNack(ea.DeliveryTag, false, true);

            //after successfully manupulating data
            // set as ack : done by BasicAck
            _channel.BasicAck(ea.DeliveryTag, false);
        }

        public void ReceiveExtendedMessage(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received: {message}");

            //if any error is happend
            // set as Nak : done by BasicNack
            //_channel.BasicNack(ea.DeliveryTag, false, true);

            //after successfully manupulating data
            // set as ack : done by BasicAck
            _channel.BasicAck(ea.DeliveryTag, false);
        }

        public void ReceiveMessage(object model, BasicDeliverEventArgs ea)
        {
           
        }
    }
}
