using AuthApi.Common.Enums;

namespace AdminPanel._Common.Models.User;

public class Role
{
    public Guid id { get; set; }
    
    public UserRole name { get; set; }
    
    public bool selected { get; set; }

}