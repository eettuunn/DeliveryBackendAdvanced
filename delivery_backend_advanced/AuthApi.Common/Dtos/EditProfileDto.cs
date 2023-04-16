using AuthApi.Common.Enums;

namespace AuthApi.Common.Dtos;

public class EditProfileDto
{
    public DateTime? birthDate { get; set; }
    
    public Gender? gender { get; set; }
    
    public string? email { get; set; }
    
    public string? userName { get; set; }
    
    public string? phoneNumber { get; set; }
}