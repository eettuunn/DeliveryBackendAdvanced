using System.ComponentModel.DataAnnotations;

namespace AuthApi.DAL.Entities;

public class AdminEntity
{
    public Guid Id { get; set; }
    
    [Required]
    public AppUser User { get; set; }
}