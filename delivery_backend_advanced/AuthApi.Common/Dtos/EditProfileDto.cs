using System.ComponentModel.DataAnnotations;
using AuthApi.Common.Enums;

namespace AuthApi.Common.Dtos;

public class EditProfileDto
{
    public DateTime? birthDate { get; set; }
    
    public Gender? gender { get; set; }
    
    [EmailAddress]
    [RegularExpression(@"[a-zA-Z]+\w*@[a-zA-Z]+\.[a-zA-Z]+", ErrorMessage = "Invalid email address")]
    public string? email { get; set; }
    
    public string? userName { get; set; }
    
    public string? phoneNumber { get; set; }
    
    public string? address { get; set; }
}