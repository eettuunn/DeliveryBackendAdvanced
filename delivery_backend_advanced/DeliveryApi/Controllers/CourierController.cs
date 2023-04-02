using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/courier")]
public class CourierController : ControllerBase
{
    /// <summary>
    /// Get list of courier's orders
    /// </summary>
    [HttpGet]
    [Route("orders")]
    public void GetCourierOrders()
    {
        
    }
    
    /// <summary>
    /// Get list of available orders
    /// </summary>
    [HttpGet]
    [Route("orders/available")]
    public void GetAvailableOrders()
    {
        
    }

    /// <summary>
    /// Change order status when delivered it
    /// </summary>
    [HttpPatch]
    [Route("{orderId}")]
    public void SetOrderStatusDelivered()
    {
        
    }

    /// <summary>
    /// Courier takes order
    /// </summary>
    [HttpPost]
    [Route("{orderId}")]
    public void TakeOrder()
    {
        
    }
}