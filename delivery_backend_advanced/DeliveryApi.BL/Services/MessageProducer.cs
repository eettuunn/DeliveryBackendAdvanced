using System.Text;
using System.Text.Json;
using delivery_backend_advanced.Services.Interfaces;
using RabbitMQ.Client;

namespace DeliveryApi.BL.Services;

public class MessageProducer : IMessageProducer
{
    private readonly IConnection _connection;

    public MessageProducer(IConnection connection)
    {
        _connection = connection;
    }

    public void SendMessage<T>(T message)
    {
        using var channel = _connection.CreateModel();

        channel.QueueDeclare("statuses", durable: true, exclusive: false);

        var jsonStr = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonStr);
        
        channel.BasicPublish("", "statuses", body: body);
    }
}