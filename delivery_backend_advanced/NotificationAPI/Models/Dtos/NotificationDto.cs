using NotificationAPI.Models.Enums;

namespace NotificationAPI.Models.Dtos;

public class NotificationDto
{
    public Guid UserId { get; set; }
    
    public Guid OrderId { get; set; }
    
    public string Text { get; set; }
    
    public NotificationStatus Status { get; set; }
}