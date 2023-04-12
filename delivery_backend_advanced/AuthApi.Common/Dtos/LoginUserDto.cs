using System.ComponentModel.DataAnnotations;

namespace AuthApi.Common.Dtos;

public class LoginUserDto
{
    [Required(ErrorMessage = "email in required")]
    public string email { get; set; }
    
    [Required(ErrorMessage = "password is required")]
    public string password { get; set; }
}