using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApi.BL.Services;

public class CourierService : ICourierService
{
    private readonly BackendDbContext _context;

    public CourierService(BackendDbContext context)
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

        if (orderEntity.Status != OrderStatus.Delivery)
        {
            throw new ConflictException("Courier only can take order with status Delivery");
        }
        
        // courier.Orders.Add(orderEntity);
        orderEntity.Status = OrderStatus.Delivery;

        await _context.SaveChangesAsync();
    }

    public async Task CancelOrder(Guid orderId)
    {
        var order = await _context
            .Orders
            .FirstOrDefaultAsync(or => or.Id == orderId) ?? throw new CantFindByIdException("order", orderId);
        if (order.Status != OrderStatus.Delivery)
        {
            throw new ConflictException("Can't cancel order, that dont on delivery by courier");
        }

        order.Status = OrderStatus.Canceled;
        await _context.SaveChangesAsync();
    }
}