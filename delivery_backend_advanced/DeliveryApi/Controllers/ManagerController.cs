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
    public async Task CreateMenu([FromBody] CreateMenuDto createMenuDto)
    {
        await _managerService.CreateMenu(createMenuDto);
    }

    /// <summary>
    /// Add dish to menu
    /// </summary>
    [HttpPut]
    [Route("menu/{menuId}/{dishId}")]
    public void AddDishToMenu(Guid menuId, Guid dishId)
    {
        
    }
    
    /// <summary>
    /// Delete dish from menu
    /// </summary>
    [HttpDelete]
    [Route("menu/{menuId}/{dishId}")]
    public void DeleteDishMenu(Guid menuId, Guid dishId)
    {
        
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
    public void SetMenuMain(Guid restaurantId, Guid menuId)
    {
    }
}