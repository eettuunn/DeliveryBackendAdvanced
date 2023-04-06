using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/restaurant")]
public class RestaurantController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    /// <summary>
    /// Get list of restaurants
    /// </summary>
    [HttpGet]
    public async Task<List<RestaurantListElementDto>> GetAllRestaurants()
    {
        return await _restaurantService.GetRestaurantList();
    }

    /// <summary>
    /// Get details of restaurant
    /// </summary>
    [HttpGet]
    [Route("{restaurantId}")]
    public async Task<RestaurantDetailsDto> GetRestaurantDetails(Guid restaurantId, Guid? menuId)
    {
        return await _restaurantService.GetRestaurantDetails(restaurantId, menuId);
    }
    
    /// <summary>
    /// Get restaurant's orders
    /// </summary>
    [HttpGet]
    [Route("{restaurantId}/orders")]
    public async Task<OrdersPageDto> GetRestaurantOrders(Guid restaurantId, [FromQuery] OrderQueryModel query)
    {
        return await _restaurantService.GetRestaurantOrders(restaurantId, query);
    }
}