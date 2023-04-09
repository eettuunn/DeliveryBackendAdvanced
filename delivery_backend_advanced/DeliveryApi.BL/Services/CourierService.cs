using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApi.BL.Services;

public class CourierService : ICourierService
{
    private readonly AppDbContext _context;

    public CourierService(AppDbContext context)
    {
        _context = context;
    }

    public async Task SetOrderDelivered(Guid orderId)
    {
        //todo: find this order in courier orders
        var orderEntity = await _context
            .Orders
            .FirstOrDefaultAsync(order => order.Id == orderId) ?? throw new CantFindByIdException("order", orderId);

        orderEntity.Status = OrderStatus.Delivered;

        await _context.SaveChangesAsync();
    }

    public async Task TakeOrder(Guid orderId)
    {
        var orderEntity = await _context
            .Orders
            .FirstOrDefaultAsync(order => order.Id == orderId) ?? throw new CantFindByIdException("order", orderId);

        // courier.Orders.Add(orderEntity);
        orderEntity.Status = OrderStatus.Delivery;

        await _context.SaveChangesAsync();
    }
}