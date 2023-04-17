using System.ComponentModel.DataAnnotations;

namespace AuthApi.Common.Dtos;

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "email is required")]
    [EmailAddress]
    [RegularExpression(@"[a-zA-Z]+\w*@[a-zA-Z]+\.[a-zA-Z]+", ErrorMessage = "Invalid email address")]
    public string email { get; set; }
    
    [Required(ErrorMessage = "password is required")]
    [MinLength(8)]
    public string password { get; set; }
}