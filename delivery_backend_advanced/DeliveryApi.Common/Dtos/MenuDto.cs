namespace delivery_backend_advanced.Models.Dtos;

public class MenuDto
{
    public Guid id { get; set; }
    
    public string name { get; set; }

    public List<DishListElementDto> dishes { get; set; }
}