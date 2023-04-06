using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/cook")]
public class CookController : ControllerBase
{
    private readonly ICookService _cookService;

    public CookController(ICookService cookService)
    {
        _cookService = cookService;
    }

    /// <summary>
    /// Get list of cook's orders
    /// </summary>
    [HttpGet]
    [Route("orders")]
    public void GetCookOrders()
    {
        
    }
    
    //todo: хз
    /*/// <summary>
    /// Get list of available orders
    /// </summary>
    [HttpGet]
    [Route("orders/available")]
    public void GetAvailableOrders()
    {
        
    }*/

    /// <summary>
    /// Change status of order when cooked or packaged
    /// </summary>
    [HttpPatch]
    [Route("{orderId}")]
    public async Task ChangeOrderStatus(Guid orderId)
    {
        await _cookService.ChangeOrderStatus(orderId);
    }

    /// <summary>
    /// Cook takes order
    /// </summary>
    [HttpPost]
    [Route("{orderId}")]
    public async Task TakeOrder(Guid orderId)
    {
        await _cookService.TakeOrder(orderId);
    }
}