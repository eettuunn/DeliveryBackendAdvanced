using System.ComponentModel.DataAnnotations;
using delivery_backend_advanced.Models.Enums;

namespace delivery_backend_advanced.Models.Entities;

public class OrderEntity
{
    public Guid Id { get; set; }
    
    [Required]
    public DateTime DeliveryTime { get; set; }
    
    [Required]
    public DateTime OrderTime { get; set; }
    
    [Required]
    public double Price { get; set; }
    
    [Required]
    public string Address { get; set; }
    
    [Required]
    public OrderStatus Status { get; set; }
    
    [Required]
    public int Number { get; set; }
    
    [Required]
    public RestaurantEntity Restaurant { get; set; }

    public List<DishBasketEntity> Dishes { get; set; } = new();
    
    // [Required]
    // public CustomerEntity Customer { get; set; }
    
    // public CookEntity? Cook { get; set; }

    // public CourierEntity? Courier { get; set; }
}