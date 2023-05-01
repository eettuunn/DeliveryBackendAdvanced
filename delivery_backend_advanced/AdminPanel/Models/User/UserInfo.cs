using AuthApi.Common.Enums;

namespace AdminPanel.Models;

public class UserInfo
{
    public Guid id { get; set; }
    
    public DateTime birthDate { get; set; }
    
    public Gender gender { get; set; }
    
    public string email { get; set; }
    
    public string userName { get; set; }
    
    public string phoneNumber { get; set; }

    public List<Role> roles { get; set; } = new();
}