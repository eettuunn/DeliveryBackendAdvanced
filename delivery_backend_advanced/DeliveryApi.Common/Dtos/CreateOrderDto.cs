using System.ComponentModel.DataAnnotations;

namespace delivery_backend_advanced.Models.Dtos;

public class CreateOrderDto
{
    public DateTime? deliveryTime { get; set; }
    
    public string? address { get; set; }
    
    [Required]
    public Guid restaurantId { get; set; }
}