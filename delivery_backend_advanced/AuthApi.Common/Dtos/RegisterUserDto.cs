using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using AuthApi.Common.Enums;

namespace AuthApi.Common.Dtos;

public class RegisterUserDto
{
    [Required(ErrorMessage = "userName is required")]
    public string userName { get; set; }
    
    [Required(ErrorMessage = "birthDate is required")]
    public DateTime birthDate { get; set; }

    [Required(ErrorMessage = "gender is required")]
    public Gender gender { get; set; }

    [Required(ErrorMessage = "phoneNumber is required")]
    public string phoneNumber { get; set; }
    
    [Required(ErrorMessage = "email is required")]
    public string email { get; set; }

    [Required(ErrorMessage = "password is required")] 
    public string password { get; set; }
}