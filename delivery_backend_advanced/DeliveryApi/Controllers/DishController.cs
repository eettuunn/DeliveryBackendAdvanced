using System.ComponentModel.DataAnnotations;
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
    public async Task<bool> CheckAbilityToRate(Guid dishId)
    {
        return await _dishService.CheckAbilityToRate(dishId);
    }

    /// <summary>
    /// Rate dish
    /// </summary>
    [HttpPost]
    [Route("{dishId}/rating")]
    public async Task<IActionResult> RateDish(Guid dishId, [Range(1, 10)] int value)
    {
        if (ModelState.IsValid)
        {
            await _dishService.RateDish(dishId, value);
            return Ok();
        }
        else
        {
            return BadRequest(ModelState);
        }
    }
}