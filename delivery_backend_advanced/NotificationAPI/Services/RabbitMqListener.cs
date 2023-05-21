using System.Text;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using NotificationAPI.Hubs;
using NotificationAPI.Models;
using NotificationAPI.Models.Dtos;
using NotificationAPI.Models.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationAPI.Services;

public class RabbitMqListener : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IServiceProvider _serviceProvider;
    private IHubContext<NotificationsHub> HubContext { get; }

    private readonly IModel _channel;

    public RabbitMqListener(IConnection connection, IHubContext<NotificationsHub> hubContext, IServiceProvider serviceProvider)
    {
        _connection = connection;
        HubContext = hubContext;
        _serviceProvider = serviceProvider;
        _channel = _connection.CreateModel();
    }

    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        
        _channel.QueueDeclare("statuses", durable: true, exclusive: false);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (model, eventArgs) => UserNotification(eventArgs, stoppingToken);

        _channel.BasicConsume("statuses", true, consumer);
        
        return Task.CompletedTask;
    }
    
    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }



    private async Task UserNotification(BasicDeliverEventArgs eventArgs, CancellationToken stoppingToken)
    {
        var body = eventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var notification = JsonConvert.DeserializeObject<NotificationDto>(message);
        if (NotificationsHub.IsConnected(notification.UserId.ToString()))
        {
            await HubContext.Clients.User(notification.UserId.ToString()).SendAsync("ReceiveMessage", notification.Text,
                cancellationToken: stoppingToken);
        }
        else
        {
            var storeNot = new Notification()
            {
                Id = new Guid(),
                Status = NotificationStatus.New,
                OrderId = notification.OrderId,
                UserId = notification.UserId,
                Text = notification.Text
            };
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
                await context.Notifications.AddAsync(storeNot, stoppingToken);
                await context.SaveChangesAsync(stoppingToken);
            }
        }
    }
}