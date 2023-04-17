using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/cook")]
public class CookController : ControllerBase
{
    private readonly ICookService _cookService;
    private readonly IOrderService _orderService;

    public CookController(ICookService cookService, IOrderService orderService)
    {
        _cookService = cookService;
        _orderService = orderService;
    }

    /// <summary>
    /// Get list of orders for cook (current == true => taken, else => available)
    /// </summary>
    [HttpGet]
    [Route("orders")]
    public async Task<OrdersPageDto> GetCookOrders([FromQuery] OrderQueryModel query)
    {
        return await _orderService.GetOrders(query, UserRole.Cook);
    }

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