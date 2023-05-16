using System.ComponentModel.DataAnnotations;

namespace delivery_backend_advanced.Models.Entities;

public class Customer
{
    public Guid Id { get; set; }
    
    public string? Address { get; set; }
    
    public List<OrderEntity> Orders { get; set; }
    
    public List<DishBasketEntity> DishesInBasket { get; set; }
    
    public List<RatingEntity> Ratings { get; set; }
}