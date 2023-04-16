using delivery_backend_advanced.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Models.Dtos;

public class DishesQueryModel
{
    [FromQuery(Name = "categories")] 
    public List<DishCategory> categories { get; set; } = new();

    [FromQuery(Name = "menu")]
    public string? menu { get; set; } = null;
    
    [FromQuery(Name = "isVegetarian")]
    public bool? isVegetarian { get; set; }
    
    [FromQuery(Name = "sort")]
    public DishSort? sort { get; set; }
    
    [FromQuery(Name = "page")]
    public int? page { get; set; }
}