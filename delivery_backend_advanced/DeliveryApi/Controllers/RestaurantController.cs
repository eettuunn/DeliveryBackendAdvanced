using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Services.Interfaces;
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
    public async Task<List<RestaurantListElementDto>> GetAllRestaurants()
    {
        return await _restaurantService.GetRestaurantList();
    }

    /// <summary>
    /// Get details of restaurant
    /// </summary>
    [HttpGet]
    [Route("{restaurantId}")]
    public async Task<RestaurantDetailsDto> GetRestaurantDetails(Guid restaurantId, string? name)
    {
        return await _restaurantService.GetRestaurantDetails(restaurantId, name);
    }
    
    /// <summary>
    /// Get restaurant's orders
    /// </summary>
    [HttpGet]
    [Route("orders")]
    public async Task<OrdersPageDto> GetRestaurantOrders([FromQuery] OrderQueryModel query)
    {
        // return await _restaurantService.GetRestaurantOrders(restaurantId, query);
        query.role = "manager";
        return await _orderService.GetOrders(query);
    }
}