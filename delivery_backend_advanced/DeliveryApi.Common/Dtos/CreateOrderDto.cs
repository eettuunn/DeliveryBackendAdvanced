using System.ComponentModel.DataAnnotations;

namespace delivery_backend_advanced.Models.Dtos;

public class CreateOrderDto
{
    public DateTime? deliveryTime { get; set; }

    [Required(ErrorMessage = "address field is required")]
    public string address { get; set; }
    
    [Required]
    public Guid restaurantId { get; set; }
}