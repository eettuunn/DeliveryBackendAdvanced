using System.ComponentModel.DataAnnotations;

namespace delivery_backend_advanced.Models.Dtos;

public class CreateMenuDto
{
    [Required(ErrorMessage = "field restaurantId is required")]
    public Guid restaurantId { get; set; }
    
    [Required(ErrorMessage = "field name is required")]
    public string name { get; set; }
}