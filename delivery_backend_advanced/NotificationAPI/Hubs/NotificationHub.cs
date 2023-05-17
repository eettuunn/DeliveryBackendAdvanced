using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace NotificationAPI.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public async Task SendNotification(string user, string message)
    {
        await Clients.User(user).SendAsync("ReceiveMessage",
            $"{DateTime.UtcNow.ToString("s")} UTC: {message}");
    }

}