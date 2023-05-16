using System.ComponentModel.DataAnnotations;

namespace delivery_backend_advanced.Models.Entities;

public class DishBasketEntity
{
    public Guid Id { get; set; }
    
    [Required]
    public Customer Customer { get; set; }
    
    [Required]
    public DishEntity Dish { get; set; }

    [Required]
    public int Amount { get; set; }
    
    [Required]
    public RestaurantEntity Restaurant { get; set; }

    [Required]
    public bool IsInOrder { get; set; }
}