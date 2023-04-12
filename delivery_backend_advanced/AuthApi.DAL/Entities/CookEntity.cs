using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.DAL.Entities;

public class CookEntity
{
    public Guid Id { get; set; }
    
    [Required]
    public AppUser User { get; set; }
}