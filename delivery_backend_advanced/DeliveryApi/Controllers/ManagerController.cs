using System.Security.Claims;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Policies;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/manager")]
[Authorize]
[Authorize(Roles = "Manager")]
[Authorize(Policy = PolicyNames.Ban)]
public class ManagerController : ControllerBase
{
    private readonly IManagerService _managerService;

    public ManagerController(IManagerService managerService)
    {
        _managerService = managerService;
    }

    /// <summary>
    /// Create menu of dishes for restaurant
    /// </summary>
    [HttpPost]
    [Route("menu/new")]
    public async Task<IActionResult> CreateMenu([FromBody] CreateMenuDto createMenuDto)
    {
        if (createMenuDto.restaurantId == Guid.Empty)
        {
            ModelState.AddModelError("restaurantId", "restaurantId field is required");
        }
        if (ModelState.IsValid)
        {
            var managerInfo = GetManagerInfo(HttpContext.User);
            await _managerService.CreateMenu(createMenuDto, managerInfo);
            return Ok();
        }
        else
        {
            return BadRequest(ModelState);
        }
    }

    /// <summary>
    /// Add dish to menu
    /// </summary>
    [HttpPut]
    [Route("menu/{menuId}/{dishId}")]
    public async Task AddDishToMenu(Guid menuId, Guid dishId)
    {
        var managerInfo = GetManagerInfo(HttpContext.User);
        await _managerService.AddDishToMenu(menuId, dishId, managerInfo);
    }
    
    /// <summary>
    /// Delete dish from menu
    /// </summary>
    [HttpDelete]
    [Route("menu/{menuId}/{dishId}")]
    public async Task DeleteDishMenu(Guid menuId, Guid dishId)
    {
        var managerInfo = GetManagerInfo(HttpContext.User);
        await _managerService.DeleteDishFromMenu(menuId, dishId, managerInfo);
    }
    
    /// <summary>
    /// Delete menu
    /// </summary>
    [HttpDelete]
    [Route("menu/{menuId}")]
    public async Task DeleteMenu(Guid menuId)
    {
        var managerInfo = GetManagerInfo(HttpContext.User);
        await _managerService.DeleteMenu(menuId, managerInfo);
    }
    
    
    
    /// <summary>
    /// Set main menu by Id
    /// </summary>
    [HttpPut]
    [Route("{restaurantId}/menu/{menuId}")]
    public async Task SetMenuMain(Guid restaurantId, Guid menuId)
    {
        var managerInfo = GetManagerInfo(HttpContext.User);
        await _managerService.SetMenuMain(restaurantId, menuId, managerInfo);
    }
    
    /// <summary>
    /// Edit menu information
    /// </summary>
    [HttpPut]
    [Route("menu/{menuId}")]
    public async Task EditMenu(Guid menuId, [FromBody] EditMenuDto editMenuDto)
    {
        var managerInfo = GetManagerInfo(HttpContext.User);
        await _managerService.EditMenu(menuId, editMenuDto, managerInfo);
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