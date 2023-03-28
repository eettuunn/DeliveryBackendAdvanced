using delivery_backend_advanced.Models.Enums;

namespace delivery_backend_advanced.Models.Dtos;

public class DishListElementDto
{
    public Guid id { get; set; }
    
    public string name { get; set; }
    
    public double price { get; set; }
    
    public bool isVegetarian { get; set; }
    
    public byte[] photo { get; set; }
    
    public DishCategory category { get; set; }
}