using System.ComponentModel.DataAnnotations;

namespace delivery_backend_advanced.Models.Dtos;

public class RepeatOrderDto
{
    public DateTime? deliveryTime { get; set; }

    [Required(ErrorMessage = "address field is required")]
    public string address { get; set; }
}