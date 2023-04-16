namespace delivery_backend_advanced.Models.Dtos;

public class RestListPageDto
{
    public List<RestaurantListElementDto> rests { get; set; } = new();
    
    public PaginationDto pagination { get; set; }
}