using System.ComponentModel.DataAnnotations;
using AuthApi.Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.DAL.Entities;

public class AppUser : IdentityUser
{
    public DateTime BirthDate { get; set; }
    
    public Gender Gender { get; set; }
    
    public ManagerEntity? Manager { get; set; }
    
    public CourierEntity? Courier { get; set; }
    
    public CookEntity? Cook { get; set; }
    
    public CustomerEntity? Customer { get; set; }
    
    public AdminEntity? Admin { get; set; }
    
    public string? RefreshToken { get; set; }
    
    public DateTime? RefreshTokenExpiryTime { get; set; }
}