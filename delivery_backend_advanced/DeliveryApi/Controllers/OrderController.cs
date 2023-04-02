using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/order")]
public class OrderController : ControllerBase
{
    /// <summary>
    /// Create order from the dishes in basket
    /// </summary>
    [HttpPost]
    public void CreateOrder()
    {
        
    }

    /// <summary>
    /// Cancel order
    /// </summary>
    [HttpPatch]
    [Route("cancel/{orderId}")]
    public void CancelOrder(Guid orderId)
    {
        
    }
    
    /// <summary>
    /// Repeat order
    /// </summary>
    [HttpPost]
    [Route("repeat/{orderId}")]
    public void RepeatOrder()
    {
        
    }

    /// <summary>
    /// Get order details
    /// </summary>
    [HttpGet]
    [Route("{orderId}")]
    public void GetOrderDetails()
    {
        
    }

    /// <summary>
    /// Get list of orders
    /// </summary>
    [HttpGet]
    public void GetListOfOrders()
    {
        
    }
}