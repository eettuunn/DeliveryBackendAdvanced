using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Entities;
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

    public async Task SetOrderDelivered(Guid orderId, UserInfoDto userInfoDto)
    {
        var orderEntity = await _context
            .Orders
            .Include(order => order.Courier)
            .FirstOrDefaultAsync(order => order.Id == orderId) ?? throw new CantFindByIdException("order", orderId);

        if (orderEntity.Courier.Id != userInfoDto.id)
        {
            throw new BadRequestException("You can't deliver not your order");
        }
        if (orderEntity.Status != OrderStatus.Delivery)
        {
            throw new ConflictException("You can set status 'Delivered' only for order on delivery");
        }
        
        orderEntity.Status = OrderStatus.Delivered;

        await _context.SaveChangesAsync();
    }

    public async Task TakeOrder(Guid orderId, UserInfoDto userInfoDto)
    {
        var courier = await CreateCourierIfNull(userInfoDto);
        
        var orderEntity = await _context
            .Orders
            .FirstOrDefaultAsync(order => order.Id == orderId) ?? throw new CantFindByIdException("order", orderId);

        if (orderEntity.Status != OrderStatus.Delivery)
        {
            throw new ConflictException("Courier only can take order with status Delivery");
        }
        
        orderEntity.Status = OrderStatus.Delivery;
        courier.Orders.Add(orderEntity);

        await _context.SaveChangesAsync();
    }

    public async Task CancelOrder(Guid orderId, UserInfoDto userInfoDto)
    {
        var order = await _context
            .Orders
            .Include(order => order.Courier)
            .FirstOrDefaultAsync(or => or.Id == orderId) ?? throw new CantFindByIdException("order", orderId);

        if (order.Courier.Id != userInfoDto.id)
        {
            throw new BadRequestException("You can't cancel not your order");
        }
        if (order.Status != OrderStatus.Delivery)
        {
            throw new ConflictException("Can't cancel order, that dont on delivery by courier");
        }

        order.Status = OrderStatus.Canceled;
        await _context.SaveChangesAsync();
    }
    
    
    
    private async Task<CourierEntity> CreateCourierIfNull(UserInfoDto userInfoDto)
    {
        var courier = await _context
            .Couriers
            .Include(cour => cour.Orders)
            .FirstOrDefaultAsync(c => c.Id == userInfoDto.id);
        if (courier == null)
        {
            var newCourier = new CourierEntity()
            {
                Id = userInfoDto.id,
            };
            
            await _context.Couriers.AddAsync(newCourier);
            await _context.SaveChangesAsync();
            courier = newCourier;
        }

        return courier;
    }
}