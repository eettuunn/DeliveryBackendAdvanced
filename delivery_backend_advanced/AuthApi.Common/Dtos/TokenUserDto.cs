namespace AuthApi.Common.Dtos;

public class TokenUserDto
{
    public Guid id { get; set; }
    
    public string email { get; set; }
    
    public string username { get; set; }
}