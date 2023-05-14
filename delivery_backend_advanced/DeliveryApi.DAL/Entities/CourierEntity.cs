using System.ComponentModel.DataAnnotations;

namespace delivery_backend_advanced.Models.Entities;

public class CourierEntity
{
    public Guid Id { get; set; }
    
    public List<OrderEntity> Orders { get; set; }
}