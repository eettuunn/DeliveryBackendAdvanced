using System.ComponentModel.DataAnnotations;

namespace AdminPanel._Common.Models.Auth;

public class Login
{
    [Required]
    public string email { get; set; }
    
    [Required]
    public string password { get; set; }
}