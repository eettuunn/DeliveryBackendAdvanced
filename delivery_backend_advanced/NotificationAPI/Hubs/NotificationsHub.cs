using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using NotificationAPI.Models;
using NotificationAPI.Models.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationAPI.Hubs;

[Authorize]
public class NotificationsHub : Hub
{
    public async Task SendNotification(Notification notification)
    {
        await Clients.User(notification.UserId.ToString()).SendAsync("ReceiveMessage",
            $"{DateTime.UtcNow.ToString("s")} UTC: {notification.Text}");
    }
}