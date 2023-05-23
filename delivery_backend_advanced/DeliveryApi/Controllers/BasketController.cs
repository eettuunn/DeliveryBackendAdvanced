using System.Net;
using System.Security.Claims;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Policies;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/basket")]
[Authorize]
[Authorize(Policy = PolicyNames.Ban)]
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
    public async Task<BasketDto> GetUserBasket()
    {
        var userInfo = GetCustomerInfo(HttpContext.User);
        return await _basketService.GetUserBasket(userInfo);
    }
    
    /// <summary>
    /// Add dish to basket
    /// </summary>
    [HttpPost]
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
    [Route("reduce/{dishBasketId}")]
    public async Task ReduceDishNumber(Guid dishBasketId)
    {
        var userInfo = GetCustomerInfo(HttpContext.User);
        await _basketService.ReduceDishInBasket(dishBasketId, userInfo);
    }

    /// <summary>
    /// Delete dish from basket completely 
    /// </summary>
    [HttpDelete]
    [Route("delete/{dishBasketId}")]
    public async Task DeleteDishFromBasket(Guid dishBasketId)
    {
        var userInfo = GetCustomerInfo(HttpContext.User);
        await _basketService.DeleteDishFromBasket(dishBasketId, userInfo);
    }



    private UserInfoDto GetCustomerInfo(ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userAddress = user.FindFirst("address")?.Value;
        var userInfo = new UserInfoDto()
        {
            id = Guid.Parse(userId),
            address = userAddress
        };

        return userInfo;
    }
}