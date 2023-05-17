using delivery_backend_advanced.Models.Enums;

namespace delivery_backend_advanced.Models.Dtos;

public class OrderStatusMessage
{
    public Guid orderId { get; set; }
    
    public OrderStatus newStatus { get; set; }
    
    public string address { get; set; }
    
    public int number { get; set; }
}