using System.Text;
using System.Text.Json;
using delivery_backend_advanced.Services.Interfaces;
using RabbitMQ.Client;

namespace DeliveryApi.BL.Services;

public class MessageProducer : IMessageProducer
{
    public void SendMessage<T>(T message)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "user",
            Password = "1234",
            VirtualHost = "/"
        };

        var connection = factory.CreateConnection();

        using var channel = connection.CreateModel();

        channel.QueueDeclare("statuses", durable: true, exclusive: false);

        var jsonStr = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonStr);
        
        channel.BasicPublish("", "statuses", body: body);
    }
}