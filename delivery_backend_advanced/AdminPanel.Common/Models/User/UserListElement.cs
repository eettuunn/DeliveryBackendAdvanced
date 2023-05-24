using AuthApi.Common.Enums;

namespace AdminPanel._Common.Models.User;

public class UserListElement
{
    public Guid id { get; set; }
    
    public DateTime birthDate { get; set; }
    
    public Gender gender { get; set; }
    
    public string email { get; set; }
    
    public string userName { get; set; }
    
    public string phoneNumber { get; set; }

    public List<Role> roles { get; set; } = new();
    
    public bool isBanned { get; set; }

}