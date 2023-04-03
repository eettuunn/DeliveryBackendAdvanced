namespace delivery_backend_advanced.Models.Dtos;

public class DishBasketDto
{
    public Guid id { get; set; }
    
    public string name { get; set; }
    
    public int price { get; set; }
    
    public int totalPrice { get; set; }
    
    public int amount { get; set; }
    
    public string? image { get; set; }
}