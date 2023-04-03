using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/dish")]
public class DishController : ControllerBase
{
    private readonly IDishService _dishService;

    public DishController(IDishService dishService)
    {
        _dishService = dishService;
    }

    /// <summary>
    /// Get dish details
    /// </summary>
    [HttpGet]
    [Route("{dishId}")]
    public async Task<DishDetailsDto> GetDishDetails(Guid dishId)
    {
        return await _dishService.GetDishDetails(dishId);
    }

    /// <summary>
    /// Check ability to rate dish
    /// </summary>
    [HttpGet]
    [Route("{dishId}/rating/check")]
    public bool CheckAbilityToRate(Guid dishId)
    {
        return true;
    }

    /// <summary>
    /// Rate dish
    /// </summary>
    [HttpPost]
    [Route("{dishId}/rating")]
    public void RateDish(Guid dishId)
    {
        
    }
}