using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using NotificationAPI.Hubs;
using NotificationAPI.Interfaces;
using NotificationAPI.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationAPI.Services;

public class NotificationService : INotificationService
{
    private IHubContext<NotificationHub> _hubContext { get; }

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotification()
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

        var consumer = new EventingBasicConsumer(channel);
        
        var body = channel.BasicGet("statuses", true).Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var notification = JsonConvert.DeserializeObject<Notification>(message);

        /*await _hubContext.Clients.User(notification.UserId.ToString()).SendAsync("ReceiveMessage",
            $"{DateTime.UtcNow.ToString("s")} UTC: {notification.Text}");*/
        
        channel.BasicConsume("statuses", true, consumer);
    }
}