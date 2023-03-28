namespace delivery_backend_advanced.Models.Dtos;

public class RestaurantListElementDto
{
    public Guid id { get; set; }
    
    public string name { get; set; }

    public List<string> menus { get; set; } = new();
}