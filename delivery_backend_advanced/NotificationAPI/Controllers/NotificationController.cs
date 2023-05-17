using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NotificationAPI.Interfaces;
using NotificationAPI.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationAPI.Controllers;

[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification()
    {
        await _notificationService.SendNotification();

        return Ok();
    }
}