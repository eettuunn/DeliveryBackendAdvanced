namespace delivery_backend_advanced.Models.Dtos;

public class RestaurantBasketDto
{
    public Guid id { get; set; }
    
    public string name { get; set; }

    public List<DishBasketDto> dishesInBasket { get; set; } = new();
}