using NotificationAPI.Models;

namespace NotificationAPI.Interfaces;

public interface INotificationService
{
    public Task SendNotification();
}