using System.ComponentModel.DataAnnotations;

namespace delivery_backend_advanced.Models.Entities;

public class RestaurantEntity
{
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }

    public List<OrderEntity> Orders { get; set; } = new();

    public List<MenuEntity> Menus { get; set; } = new();

    public List<Cook> Cooks { get; set; } = new();

    public List<Manager> Managers { get; set; } = new();
}