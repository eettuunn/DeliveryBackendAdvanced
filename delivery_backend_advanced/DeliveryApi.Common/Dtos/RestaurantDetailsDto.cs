namespace delivery_backend_advanced.Models.Dtos;

public class RestaurantDetailsDto
{
    public Guid id { get; set; }
    
    public string name { get; set; }
    
    public MenuDto menus { get; set; }
}