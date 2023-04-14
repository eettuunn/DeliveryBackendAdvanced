using System.ComponentModel.DataAnnotations;

namespace AuthApi.Common.Dtos;

public class TokenPairDto
{
    [Required(ErrorMessage = "field accessToken is required")]
    public string accessToken { get; set; }
    
    [Required(ErrorMessage = "field refreshToken is required")]

    public string refreshToken { get; set; }
}