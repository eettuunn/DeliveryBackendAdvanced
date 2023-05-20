using System.Text;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using NotificationAPI.Hubs;
using NotificationAPI.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationAPI.Services;

public class RabbitMqListener : BackgroundService
{
    private readonly IConnection _connection;
    private IHubContext<NotificationsHub> HubContext { get; }

    private readonly IModel _channel;

    public RabbitMqListener(IConnection connection, IHubContext<NotificationsHub> hubContext)
    {
        _connection = connection;
        HubContext = hubContext;
        _channel = _connection.CreateModel();
    }

    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        
        _channel.QueueDeclare("statuses", durable: true, exclusive: false);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var notification = JsonConvert.DeserializeObject<Notification>(message);
            HubContext.Clients.User(notification.UserId.ToString()).SendAsync("ReceiveMessage", notification.Text, cancellationToken: stoppingToken);
        };

        _channel.BasicConsume("statuses", true, consumer);
        
        return Task.CompletedTask;
    }
    
    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}