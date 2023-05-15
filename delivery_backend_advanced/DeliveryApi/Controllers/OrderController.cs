using System.Net;
using System.Security.Claims;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/order")]
[Authorize]
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
            var userInfo = GetCustomerInfo(HttpContext.User);
            await _orderService.CreateOrder(orderDto, userInfo);
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
        var userInfo = GetCustomerInfo(HttpContext.User);
        await _orderService.CancelOrder(orderId, userInfo);
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
            var userInfo = GetCustomerInfo(HttpContext.User);
            await _orderService.RepeatOrder(repeatOrderDto, orderId, userInfo);
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
        var userInfo = GetCustomerInfo(HttpContext.User);
        return await _orderService.GetOrderDetails(orderId, userInfo);
    }

    /// <summary>
    /// Get list of orders
    /// </summary>
    [HttpGet]
    public async Task<OrdersPageDto> GetListOfOrders([FromQuery] OrderQueryModel query)
    {
        var userInfo = GetCustomerInfo(HttpContext.User);
        return await _orderService.GetOrders(query, UserRole.Customer, userInfo);
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