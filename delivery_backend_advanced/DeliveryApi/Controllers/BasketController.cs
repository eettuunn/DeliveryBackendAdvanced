using System.Net;
using System.Security.Claims;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/basket")]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;

    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }

    /// <summary>
    /// Get user's basket
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<BasketDto> GetUserBasket()
    {
        var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        return await _basketService.GetUserBasket(userId);
    }
    
    /// <summary>
    /// Add dish to basket
    /// </summary>
    [HttpPost]
    [Authorize]
    [Route("{dishId}/{restaurantId}")]
    public async Task AddDishToBasket(Guid dishId, Guid restaurantId)
    {
        var userInfo = GetCustomerInfo(HttpContext.User);
        await _basketService.AddDishToBasket(dishId, restaurantId, userInfo);
    }
    
    /// <summary>
    /// Reduce number of dishes in basket
    /// </summary>
    [HttpPatch]
    [Authorize]
    [Route("reduce/{dishBasketId}")]
    public async Task ReduceDishNumber(Guid dishBasketId)
    {
        var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        await _basketService.ReduceDishInBasket(dishBasketId, userId);
    }

    /// <summary>
    /// Delete dish from basket completely 
    /// </summary>
    [HttpDelete]
    [Authorize]
    [Route("delete/{dishBasketId}")]
    public async Task DeleteDishFromBasket(Guid dishBasketId)
    {
        var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        await _basketService.DeleteDishFromBasket(dishBasketId, userId);
    }



    private CustomerInfoDto GetCustomerInfo(ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userAddress = user.FindFirst("address")?.Value;
        var userInfo = new CustomerInfoDto()
        {
            id = Guid.Parse(userId),
            address = userAddress
        };

        return userInfo;
    }
}