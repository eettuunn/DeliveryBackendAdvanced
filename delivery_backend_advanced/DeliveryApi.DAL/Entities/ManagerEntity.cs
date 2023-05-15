using System.ComponentModel.DataAnnotations;

namespace delivery_backend_advanced.Models.Entities;

public class ManagerEntity
{
    public Guid Id { get; set; }
    
    public RestaurantEntity? Restaurant { get; set; }
}