using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
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
        var userInfo = GetCustomerInfo(HttpContext.User);
        return await _dishService.CheckAbilityToRate(dishId, userInfo);
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
            var userInfo = GetCustomerInfo(HttpContext.User);
            await _dishService.RateDish(dishId, value, userInfo);
            return Ok();
        }
        else
        {
            return BadRequest(ModelState);
        }
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