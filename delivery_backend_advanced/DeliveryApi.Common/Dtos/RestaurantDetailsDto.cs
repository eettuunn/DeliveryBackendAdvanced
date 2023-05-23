namespace delivery_backend_advanced.Models.Dtos;

public class RestaurantDetailsDto
{
    public Guid id { get; set; }
    
    public string name { get; set; }

    public List<string> menuNames { get; set; } = new();
    
    public MenuDto? menu { get; set; }
}