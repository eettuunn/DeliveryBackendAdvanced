using System.Security.Claims;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Policies;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/restaurant")]
public class RestaurantController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;
    private readonly IOrderService _orderService;

    public RestaurantController(IRestaurantService restaurantService, IOrderService orderService)
    {
        _restaurantService = restaurantService;
        _orderService = orderService;
    }

    /// <summary>
    /// Get list of restaurants
    /// </summary>
    [HttpGet]
    public async Task<RestListPageDto> GetAllRestaurants(string? search, int? page)
    {
        return await _restaurantService.GetRestaurantList(search, page);
    }

    /// <summary>
    /// Get details of restaurant
    /// </summary>
    [HttpGet]
    [Route("{restaurantId}")]
    public async Task<RestaurantPageDto> GetRestaurantDetails(Guid restaurantId, DishesQueryModel query)
    {
        return await _restaurantService.GetRestaurantDetails(restaurantId, query);
    }
    
    /// <summary>
    /// Get restaurant's orders
    /// </summary>
    [HttpGet]
    [Authorize]
    [Authorize(Roles = "Manager")]
    [Authorize(Policy = PolicyNames.Ban)]
    [Route("orders")]
    public async Task<OrdersPageDto> GetRestaurantOrders([FromQuery] OrderQueryModel query)
    {
        var managerInfo = GetManagerInfo(HttpContext.User);
        return await _orderService.GetOrders(query, UserRole.Manager, managerInfo);
    }
    
    
    
    private UserInfoDto GetManagerInfo(ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userInfo = new UserInfoDto()
        {
            id = Guid.Parse(userId),
        };

        return userInfo;
    }
}