using System.Net;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/order")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Create order from the dishes in basket
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto orderDto)
    {
        if (orderDto.restaurantId == Guid.Empty)
        {
            ModelState.AddModelError("restaurantId", "restaurantId field is required");
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        else
        {
            await _orderService.CreateOrder(orderDto);
            return Ok();
        }
    }

    /// <summary>
    /// Cancel order
    /// </summary>
    [HttpPut]
    [Route("cancel/{orderId}")]
    public async Task CancelOrder(Guid orderId)
    {
        await _orderService.CancelOrder(orderId);
    }
    
    /// <summary>
    /// Repeat order
    /// </summary>
    [HttpPost]
    [Route("repeat/{orderId}")]
    public async Task<IActionResult> RepeatOrder([FromBody] RepeatOrderDto repeatOrderDto, Guid orderId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        else
        {
            await _orderService.RepeatOrder(repeatOrderDto, orderId);
            return Ok();
        }
    }

    /// <summary>
    /// Get order details
    /// </summary>
    [HttpGet]
    [Route("{orderId}")]
    public async Task<OrderDto> GetOrderDetails(Guid orderId)
    {
        return await _orderService.GetOrderDetails(orderId);
    }

    /// <summary>
    /// Get list of orders
    /// </summary>
    [HttpGet]
    public async Task<OrdersPageDto> GetListOfOrders([FromQuery] OrderQueryModel query)
    {
        return await _orderService.GetOrders(query);
    }
}