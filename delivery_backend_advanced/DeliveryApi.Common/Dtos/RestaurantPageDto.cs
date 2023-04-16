namespace delivery_backend_advanced.Models.Dtos;

public class RestaurantPageDto
{
    public RestaurantDetailsDto restaurantDetails { get; set; } = new();
    
    public PaginationDto pagination { get; set; }
}