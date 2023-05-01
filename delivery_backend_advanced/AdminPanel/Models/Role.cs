using AuthApi.Common.Enums;

namespace AdminPanel.Models;

public class Role
{
    public Guid id { get; set; }
    
    public UserRole name { get; set; }
}