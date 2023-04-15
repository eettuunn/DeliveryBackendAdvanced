using System.ComponentModel.DataAnnotations;

namespace AuthApi.Common.Dtos;

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "email is required")]
    public string email { get; set; }
    
    [Required(ErrorMessage = "password is required")]
    public string password { get; set; }
}