using System.ComponentModel.DataAnnotations;
using delivery_backend_advanced.Models.Enums;

namespace delivery_backend_advanced.Models.Entities;

public class MenuEntity
{
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public RestaurantEntity Restaurant { get; set; }

    public List<DishEntity> Dishes { get; set; } = new();
}