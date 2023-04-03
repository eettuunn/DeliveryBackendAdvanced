using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Services.Interfaces;
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
    public async Task<BasketDto> GetUserBasket()
    {
        return await _basketService.GetUserBasket();
    }
    
    /// <summary>
    /// Add dish to basket
    /// </summary>
    [HttpPost]
    [Route("{dishId}/{restaurantId}")]
    public async Task AddDishToBasket(Guid dishId, Guid restaurantId)
    {
        await _basketService.AddDishToBasket(dishId, restaurantId);
    }
    
    /// <summary>
    /// Reduce number of dishes in basket
    /// </summary>
    [HttpDelete]
    [Route("{dishId}/{restaurantId}/reduce")]
    public void ReduceDishNumber(Guid dishId, Guid restaurantId, bool? delete = null)
    {
        
    }

    /// <summary>
    /// Delete dish from basket completely 
    /// </summary>
    [HttpDelete]
    [Route("{dishId}")]
    public void DeleteDishFromBasket(Guid dishId)
    {
        
    }
}