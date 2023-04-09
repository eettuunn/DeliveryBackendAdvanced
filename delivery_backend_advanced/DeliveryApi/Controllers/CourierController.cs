using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/courier")]
public class CourierController : ControllerBase
{
    private readonly IOrderService _orderService;

    public CourierController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Get list of courier's orders (current == true => taken, else => available)
    /// </summary>
    [HttpGet]
    [Route("orders")]
    public async Task<OrdersPageDto> GetCourierOrders([FromQuery] OrderQueryModel query)
    {
        query.role = "courier";
        
        return await _orderService.GetOrders(query);
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