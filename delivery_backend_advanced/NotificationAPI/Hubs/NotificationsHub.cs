using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotificationAPI.Models.Dtos;
using NotificationAPI.Models.Enums;

namespace NotificationAPI.Hubs;

[Authorize]
public class NotificationsHub : Hub
{
    private static List<string> _connectedClients = new();

    private readonly NotificationDbContext _context;

    public NotificationsHub(NotificationDbContext context)
    {
        _context = context;
    }

    private async Task SendNotification(NotificationDto notification)
    {
        await Clients.User(notification.UserId.ToString()).SendAsync("ReceiveMessage",
            $"{DateTime.UtcNow.ToString("s")} UTC: {notification.Text}");
    }

    public override async Task OnConnectedAsync()
    {
        _connectedClients.Add(Context.UserIdentifier);

        var userNotifications = await _context
            .Notifications
            .Where(n => n.UserId == Guid.Parse(Context.UserIdentifier))
            .ToListAsync();

        foreach (var n in userNotifications)
        {
            var notDto = new NotificationDto()
            {
                UserId = n.UserId,
                OrderId = n.OrderId,
                Text = n.Text,
                Status = n.Status
            };

            await SendNotification(notDto);

            n.Status = NotificationStatus.Sent;
            await _context.SaveChangesAsync();
        }
        
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _connectedClients.Remove(Context.UserIdentifier);
        return base.OnDisconnectedAsync(exception);
    }

    public static bool IsConnected(string userId)
    {
        return _connectedClients.Contains(userId);
    }
}