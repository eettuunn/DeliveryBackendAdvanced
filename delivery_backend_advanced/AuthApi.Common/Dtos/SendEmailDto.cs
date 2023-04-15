namespace AuthApi.Common.Dtos;

public class SendEmailDto
{
    public string email { get; set; }

    public string subject { get; set; }
    
    public string message { get; set; }
}