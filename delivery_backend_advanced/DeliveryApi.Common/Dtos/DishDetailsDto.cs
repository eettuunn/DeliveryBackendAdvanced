using delivery_backend_advanced.Models.Enums;

namespace delivery_backend_advanced.Models.Dtos;

public class DishDetailsDto
{
    public Guid id { get; set; }
    
    public string name { get; set; }
    
    public double price { get; set; }
    
    public string? description { get; set; }
    
    public bool isVegetarian { get; set; }
    
    public double? averageRating { get; set; }

    public byte[] photo { get; set; } = Array.Empty<byte>();
    
    public DishCategory category { get; set; }
}