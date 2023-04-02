using System.ComponentModel.DataAnnotations;
using delivery_backend_advanced.Models.Enums;

namespace delivery_backend_advanced.Models.Dtos;

public class DishInOrderDto
{
    public Guid id { get; set; }
    
    [Required]
    public string name { get; set; }
    
    [Required]
    public double price { get; set; }
    
    [Required]
    public bool isVegetarian { get; set; }
    
    [Required]
    public byte[] photo { get; set; }
    
    [Required]
    public DishCategory category { get; set; }
    
    [Required]
    public int amount { get; set; }
}