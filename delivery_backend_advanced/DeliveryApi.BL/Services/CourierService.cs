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
    private readonly IMessageProducer _messageProducer;

    public CourierService(BackendDbContext context, IMessageProducer messageProducer)
    {
        _context = context;
        _messageProducer = messageProducer;
    }

    public async Task SetOrderDelivered(Guid orderId, UserInfoDto userInfoDto)
    {
        var orderEntity = await _context
            .Orders
            .Include(order => order.Courier)
            .Include(order => order.Customer)
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
        
        SendOrderStatusChangedMessage(orderEntity);
    }

    public async Task TakeOrder(Guid orderId, UserInfoDto userInfoDto)
    {
        var courier = await CreateCourierIfNull(userInfoDto);
        
        var orderEntity = await _context
            .Orders
            .Include(order => order.Customer)
            .FirstOrDefaultAsync(order => order.Id == orderId) ?? throw new CantFindByIdException("order", orderId);

        if (orderEntity.Status != OrderStatus.Packaging)
        {
            throw new ConflictException("Courier only can take order with status Packaging");
        }
        
        orderEntity.Status = OrderStatus.Delivery;
        courier.Orders.Add(orderEntity);

        await _context.SaveChangesAsync();
        
        SendOrderStatusChangedMessage(orderEntity);
    }

    public async Task CancelOrder(Guid orderId, UserInfoDto userInfoDto)
    {
        var order = await _context
            .Orders
            .Include(order => order.Courier)
            .Include(order => order.Customer)
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
        
        SendOrderStatusChangedMessage(order);
    }
    
    
    
    private async Task<Courier> CreateCourierIfNull(UserInfoDto userInfoDto)
    {
        var courier = await _context
            .Couriers
            .Include(cour => cour.Orders)
            .FirstOrDefaultAsync(c => c.Id == userInfoDto.id);
        if (courier == null)
        {
            var newCourier = new Courier()
            {
                Id = userInfoDto.id,
            };
            
            await _context.Couriers.AddAsync(newCourier);
            await _context.SaveChangesAsync();
            courier = newCourier;
        }

        return courier;
    }
    
    private void SendOrderStatusChangedMessage(OrderEntity orderEntity)
    {
        var orderStatusMessage = new OrderStatusMessage
        {
            OrderId = orderEntity.Id,
            UserId = orderEntity.Customer.Id,
            Status = NotificationStatus.New,
            Text = $@"Order number {orderEntity.Number} for address {orderEntity.Address} is on {orderEntity.Status.ToString()}"
        };
        
        _messageProducer.SendMessage(orderStatusMessage);
    }
}