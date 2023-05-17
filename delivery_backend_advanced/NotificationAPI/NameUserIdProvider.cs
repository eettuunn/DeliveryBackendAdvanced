using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace NotificationAPI;

public class NameUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
   
}
