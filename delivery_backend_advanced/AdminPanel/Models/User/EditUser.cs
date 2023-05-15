using System.ComponentModel.DataAnnotations;
using AuthApi.Common.Enums;

namespace AdminPanel.Models;

public class EditUser
{
    public Guid id { get; set; }
    
    public DateTime birthDate { get; set; }
    
    public Gender gender { get; set; }
    
    [RegularExpression(@"[a-zA-Z]+\w*@[a-zA-Z]+\.[a-zA-Z]+", ErrorMessage = "Invalid email address")]
    public string email { get; set; }
    
    public string userName { get; set; }
    
    public string phoneNumber { get; set; }

    public List<Role> roles { get; set; } = new();
    
    public Guid? restaurantId { get; set; }
    
    public string? address { get; set; }
}