using System.ComponentModel.DataAnnotations;

namespace AuthApi.DAL.Entities;

public class CustomerEntity
{
    public Guid Id { get; set; }
    
    public string? Address { get; set; }
    
    [Required]
    public AppUser User { get; set; }
}