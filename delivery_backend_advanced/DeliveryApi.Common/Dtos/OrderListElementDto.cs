using System.ComponentModel.DataAnnotations;
using delivery_backend_advanced.Models.Enums;

namespace delivery_backend_advanced.Models.Dtos;

public class OrderListElementDto
{
    public Guid id { get; set; }
    
    [Required]
    public DateTime orderTime { get; set; }
    
    [Required]
    public DateTime deliveryTime { get; set; }
    
    [Required]
    public double price { get; set; }
    
    [Required]
    public string address { get; set; }
    
    [Required]
    public OrderStatus status { get; set; }
    
    [Required]
    public int number { get; set; }

    // [Required]
    // public CustomerEntity customer { get; set; }
    
    //[Required]
    // public CookEntity cook { get; set; }
    
    // [Required]
    // public CourierEntity courier { get; set; }
}