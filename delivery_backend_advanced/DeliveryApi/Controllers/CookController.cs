using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/cook")]
public class CookController : ControllerBase
{
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
    public void ChangeOrderStatus(Guid orderId)
    {
        
    }

    /// <summary>
    /// Cook takes order
    /// </summary>
    [HttpPost]
    [Route("{orderId}")]
    public void TakeOrder(Guid orderId)
    {
        
    }
}