using delivery_backend_advanced.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Models.Dtos;

public class OrderQueryModel
{
    [FromQuery(Name = "search")]
    public int? search { get; set; }
    
    [FromQuery(Name = "sort")]
    public OrderSort? sort { get; set; }

    [FromQuery(Name = "status")] public List<OrderStatus> statuses { get; set; } = new();
    
    [FromQuery(Name = "page")]
    public int? page { get; set; }
}