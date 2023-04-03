namespace delivery_backend_advanced.Models.Dtos;

public class BasketDto
{
    public List<RestaurantBasketDto> restaurants { get; set; } = new();
    
    public int totalBasketPrice { get; set; }
}