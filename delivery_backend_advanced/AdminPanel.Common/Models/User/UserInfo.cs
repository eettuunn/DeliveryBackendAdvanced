using AuthApi.Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminPanel._Common.Models.User;

public class UserInfo
{
    public Guid id { get; set; }
    
    public DateTime birthDate { get; set; }
    
    public Gender gender { get; set; }
    
    public string email { get; set; }
    
    public string userName { get; set; }
    
    public string phoneNumber { get; set; }

    public List<Role> roles { get; set; } = new();

    public List<SelectListItem> restaurantIds { get; set; } = new();

    public string? selectedRestaurantValue { get; set; }

    public string? address { get; set; }

}