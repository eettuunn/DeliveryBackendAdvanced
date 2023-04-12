using System.ComponentModel.DataAnnotations;
using AuthApi.Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.DAL.Entities;

public class AppUser : IdentityUser
{
    public Guid Id { get; set; }
    
    public string FullName { get; set; }
    
    public DateTime BirthDate { get; set; }
    
    public Gender Gender { get; set; }
    
    public string Phone { get; set; }
    
    [EmailAddress]
    public string Email { get; set; }

    public List<UserRole> Roles { get; set; } = new();
}