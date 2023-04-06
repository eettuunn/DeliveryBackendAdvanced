﻿using AutoMapper;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApi.BL.Services;

public class CookService : ICookService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CookService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task TakeOrder(Guid orderId)
    {
        var orderEntity = await _context
            .Orders
            .FirstOrDefaultAsync(order => order.Id == orderId) ?? throw new CantFindByIdException("order", orderId);
        
        //cook.Orders.Add(orderEntity);

        orderEntity.Status = OrderStatus.Kitchen;
        await _context.SaveChangesAsync();
    }
}