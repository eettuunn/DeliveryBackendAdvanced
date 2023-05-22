using System.Security.Claims;
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
        await Clients.User(notification.UserId.ToString()).SendAsync("ReceiveMessage",notification.Text);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        if (!_connectedClients.Contains(userId))
        {
            _connectedClients.Add(userId);
        }

        var userNotifications = await _context
            .Notifications
            .Where(n => n.UserId == Guid.Parse(userId) && n.Status == NotificationStatus.New)
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
        _connectedClients.Remove(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        return base.OnDisconnectedAsync(exception);
    }

    public static bool IsConnected(string userId)
    {
        return _connectedClients.Contains(userId);
    }
}