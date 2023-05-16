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
    private readonly IMapper _mapper;

    public CookService(BackendDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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
}