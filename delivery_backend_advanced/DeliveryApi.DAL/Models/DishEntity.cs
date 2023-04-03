using System.ComponentModel.DataAnnotations;
using delivery_backend_advanced.Models.Enums;

namespace delivery_backend_advanced.Models.Entities;

public class DishEntity
{
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }

    [Required]
    public double Price { get; set; }
    
    public string? Description { get; set; }
    
    public double? AverageRating { get; set; }
    
    [Required]
    public bool IsVegetarian { get; set; }

    [Required]
    public byte[] Photo { get; set; } = Array.Empty<byte>();
    
    [Required]
    public DishCategory Category { get; set; }

    public List<RatingEntity> Ratings { get; set; } = new();
}