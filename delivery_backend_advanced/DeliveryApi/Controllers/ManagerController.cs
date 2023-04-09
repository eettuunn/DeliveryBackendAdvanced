using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/manager")]
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
        //todo: then there will be no rId
        if (createMenuDto.restaurantId == Guid.Empty)
        {
            ModelState.AddModelError("restaurantId", "restaurantId field is required");
        }
        if (ModelState.IsValid)
        {
            await _managerService.CreateMenu(createMenuDto);
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
        await _managerService.AddDishToMenu(menuId, dishId);
    }
    
    /// <summary>
    /// Delete dish from menu
    /// </summary>
    [HttpDelete]
    [Route("menu/{menuId}/{dishId}")]
    public async Task DeleteDishMenu(Guid menuId, Guid dishId)
    {
        await _managerService.DeleteDishFromMenu(menuId, dishId);
    }
    
    /// <summary>
    /// Delete menu
    /// </summary>
    [HttpDelete]
    [Route("menu/{menuId}")]
    public void DeleteMenu(Guid menuId)
    {
        
    }
    
    
    
    /// <summary>
    /// Set main menu by Id
    /// </summary>
    [HttpPut]
    [Route("{restaurantId}/menu/{menuId}")]
    public async Task SetMenuMain(Guid restaurantId, Guid menuId)
    {
        await _managerService.SetMenuMain(restaurantId, menuId);
    }
}