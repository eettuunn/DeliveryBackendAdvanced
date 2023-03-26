using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/restaurant")]
public class RestaurantController : ControllerBase
{
    /// <summary>
    /// Get list of restaurants
    /// </summary>
    [HttpGet]
    public void GetAllRestaurants()
    {
        
    }

    /// <summary>
    /// Get details of restaurant
    /// </summary>
    [HttpGet]
    [Route("{restaurantId}")]
    public void GetRestaurantDetails(Guid restaurantId)
    {
        
    }
    
    /// <summary>
    /// Get restaurant's menu
    /// </summary>
    [HttpGet]
    [Route("{restaurantId}/menu")]
    public void GetRestaurantMenu(Guid restaurantId, Guid? menuId)
    {
        
    }
}