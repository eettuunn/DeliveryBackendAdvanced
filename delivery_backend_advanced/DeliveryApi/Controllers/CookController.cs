using System.Security.Claims;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Policies;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/cook")]
[Authorize]
[Authorize(Roles = "Cook")]
[Authorize(Policy = PolicyNames.Ban)]
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
        var cookInfo = GetCookInfo(HttpContext.User);
        return await _orderService.GetOrders(query, UserRole.Cook, cookInfo);
    }

    /// <summary>
    /// Change status of order when cooked or packaged
    /// </summary>
    [HttpPatch]
    [Route("{orderId}")]
    public async Task ChangeOrderStatus(Guid orderId)
    {
        var cookInfo = GetCookInfo(HttpContext.User);
        await _cookService.ChangeOrderStatus(orderId, cookInfo);
    }

    /// <summary>
    /// Cook takes order
    /// </summary>
    [HttpPost]
    [Route("{orderId}")]
    public async Task TakeOrder(Guid orderId)
    {
        var cookInfo = GetCookInfo(HttpContext.User);
        await _cookService.TakeOrder(orderId, cookInfo);
    }
    
    
    
    private UserInfoDto GetCookInfo(ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userInfo = new UserInfoDto()
        {
            id = Guid.Parse(userId),
        };

        return userInfo;
    }
}