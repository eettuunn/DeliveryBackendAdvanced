using AutoMapper;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
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


    public async Task TakeOrder(Guid orderId)
    {
        var orderEntity = await _context
            .Orders
            .FirstOrDefaultAsync(order => order.Id == orderId) ?? throw new CantFindByIdException("order", orderId);

        if (orderEntity.Status != OrderStatus.Created)
        {
            throw new ConflictException("Cook can take only orders with status created");
        }
        //cook.Orders.Add(orderEntity);

        orderEntity.Status = OrderStatus.Kitchen;
        await _context.SaveChangesAsync();
    }

    public async Task ChangeOrderStatus(Guid orderId)
    {
        var orderEntity = await _context
            .Orders
            .FirstOrDefaultAsync(order => order.Id == orderId) ?? throw new CantFindByIdException("order", orderId);
        if (orderEntity.Status > OrderStatus.Packaging || orderEntity.Status == OrderStatus.Created)
        {
            throw new ConflictException("Order is out of kitchen");
        }

        orderEntity.Status += 1;

        await _context.SaveChangesAsync();
    }
}