using System.ComponentModel.DataAnnotations;
using delivery_backend_advanced.Models.Enums;

namespace delivery_backend_advanced.Models.Entities;

public class RatingEntity
{
    public Guid Id { get; set; }
    
    [Required]
    public int Value { get; set; }

    [Required]
    public DishEntity Dish { get; set; }
 
    [Required]
    public Customer Customer { get; set; }
}