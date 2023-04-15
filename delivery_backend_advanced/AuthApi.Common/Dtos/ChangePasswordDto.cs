using System.ComponentModel.DataAnnotations;

namespace AuthApi.Common.Dtos;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "oldPassword is required")]
    public string oldPassword { get; set; }

    [Required(ErrorMessage = "newPassword is required")]
    public string newPassword { get; set; }
}