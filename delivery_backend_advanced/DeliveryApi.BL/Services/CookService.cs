using AutoMapper;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Entities;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApi.BL.Services;

public class CookService : ICookService
{
    private readonly BackendDbContext _context;
    private readonly IMessageProducer _messageProducer;

    public CookService(BackendDbContext context, IMessageProducer messageProducer)
    {
        _context = context;
        _messageProducer = messageProducer;
    }


    public async Task TakeOrder(Guid orderId, UserInfoDto userInfoDto)
    {
        var cook = await CreateCookIfNull(userInfoDto);
        
        var orderEntity = await _context
            .Orders
            .FirstOrDefaultAsync(order => order.Id == orderId) ?? throw new CantFindByIdException("order", orderId);
        
        if(orderEntity.Restaurant != cook.Restaurant)
        {
            throw new BadRequestException("Cook can take order only from his restaurant");
        }
        if (orderEntity.Status != OrderStatus.Created)
        {
            throw new ConflictException("Cook can take only orders with status created");
        }
        
        orderEntity.Status = OrderStatus.Kitchen;
        cook.Orders.Add(orderEntity);
        
        await _context.SaveChangesAsync();
        
        SendOrderStatusChangedMessage(orderEntity);
    }

    public async Task ChangeOrderStatus(Guid orderId, UserInfoDto userInfoDto)
    {
        var orderEntity = await _context
            .Orders
            .Include(order => order.Cook)
            .FirstOrDefaultAsync(order => order.Id == orderId) ?? throw new CantFindByIdException("order", orderId);

        if (orderEntity.Cook.Id != userInfoDto.id)
        {
            throw new BadRequestException("This is not your order");
        }
        if (orderEntity.Status > OrderStatus.Packaging || orderEntity.Status == OrderStatus.Created)
        {
            throw new ConflictException("Order is out of kitchen");
        }

        orderEntity.Status += 1;

        await _context.SaveChangesAsync();
        
        SendOrderStatusChangedMessage(orderEntity);
    }
    
    
    
    private async Task<Cook> CreateCookIfNull(UserInfoDto userInfoDto)
    {
        var cook = await _context
            .Cooks
            .Include(cook => cook.Orders)
            .FirstOrDefaultAsync(c => c.Id == userInfoDto.id);
        if (cook == null)
        {
            var newCook = new Cook()
            {
                Id = userInfoDto.id,
            };
            
            await _context.Cooks.AddAsync(newCook);
            await _context.SaveChangesAsync();
            cook = newCook;
        }

        return cook;
    }

    private void SendOrderStatusChangedMessage(OrderEntity orderEntity)
    {
        var orderStatusMessage = new OrderStatusMessage
        {
            orderId = orderEntity.Id,
            newStatus = orderEntity.Status,
            address = orderEntity.Address,
            number = orderEntity.Number
        };
        
        _messageProducer.SendMessage(orderStatusMessage);
    }
}