namespace AuthApi.Common.Dtos;

public class TokenPairDto
{
    public string accesToken { get; set; }
    
    public string refreshToken { get; set; }
}