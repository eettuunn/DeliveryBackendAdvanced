using System.ComponentModel.DataAnnotations;
using AuthApi.Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.DAL.Entities;

public class AppUser : IdentityUser
{
    public Guid Id { get; set; }
    
    public DateTime BirthDate { get; set; }
    
    public Gender Gender { get; set; }
    
    public ManagerEntity? Manager { get; set; }
    
    public CourierEntity? Courier { get; set; }
    
    public CookEntity? Cook { get; set; }
    
    public CustomerEntity? Customer { get; set; }
}