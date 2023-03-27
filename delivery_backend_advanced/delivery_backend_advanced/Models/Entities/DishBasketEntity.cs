using System.ComponentModel.DataAnnotations;

namespace delivery_backend_advanced.Models.Entities;

public class DishBasketEntity
{
    public Guid Id { get; set; }

    // [Required]
    // public CustomerEntity Customer { get; set; }
    
    [Required]
    public DishEntity Dish { get; set; }

    [Required]
    public int Amount { get; set; }
    
    //todo: мб можно придумать другую реализацию хранения блюд в заказе
    [Required]
    public bool IsInOrder { get; set; }
}