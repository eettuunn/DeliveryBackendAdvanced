﻿using AutoMapper;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Entities;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApi.BL.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public OrderService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task CreateOrder(CreateOrderDto orderDto)
    {
        //todo: maybe do create orders for all rests at once
        var orderDishes = await _context
            .DishesInBasket
            .Include(dib => dib.Dish)
            .Where(dib => !dib.IsInOrder)
            .ToListAsync();
        var restaurantEntity = await _context
                                   .Restaurants
                                   .FirstOrDefaultAsync(r => r.Id == orderDto.restaurantId) ??
                               throw new CantFindByIdException("restaurant", orderDto.restaurantId);

        OrderEntity newOrder = new OrderEntity()
        {
            Id = new Guid(),
            DeliveryTime = orderDto.deliveryTime ?? DateTime.UtcNow.AddHours(1),
            OrderTime = DateTime.UtcNow,
            Price = orderDishes.Sum(dib => dib.Dish.Price * dib.Amount),
            Address = orderDto.address,
            Status = OrderStatus.Created,
            Restaurant = restaurantEntity,
            Dishes = orderDishes,
            Number = await _context.Orders.CountAsync() + 1
        };
        
        orderDishes.Select(dib =>
        {
            dib.IsInOrder = true;
            return dib;
        }).ToList();


        await _context.Orders.AddAsync(newOrder);
        await _context.SaveChangesAsync();
    }

    public async Task<List<OrderListElementDto>> GetUserOrders()
    {
        //todo: sorting, filters, search and user
        var orderEntities = await _context
            .Orders
            .Include(order => order.Restaurant)
            .Include(order => order.Dishes)
            .ThenInclude(dish => dish.Dish)
            .ToListAsync();
        
        List<OrderListElementDto> orderDtos = _mapper.Map<List<OrderListElementDto>>(orderEntities);

        return orderDtos;
    }

    public async Task<OrderDto> GetOrderDetails(Guid orderId)
    {
        //todo: sorting, filters, search and user
        var orderEntity = await _context
            .Orders
            .Include(order => order.Restaurant)
            .Include(order => order.Dishes)
            .ThenInclude(dish => dish.Dish)
            .FirstOrDefaultAsync(order => order.Id == orderId) ?? throw new CantFindByIdException("order", orderId);
        
        OrderDto orderDto = _mapper.Map<OrderDto>(orderEntity);
        orderDto.dishes = _mapper.Map<List<DishInOrderDto>>(orderEntity.Dishes.Select(d => d.Dish));
        for (int i = 0; i < orderEntity.Dishes.Count; i++)
        {
            orderDto.dishes[i].amount = orderEntity.Dishes[i].Amount;
        }
        
        return orderDto;
    }

    public async Task CancelOrder(Guid orderId)
    {
        //todo: also add for cura
        var orderEntity = await _context
            .Orders
            .FirstOrDefaultAsync(order => order.Id == orderId) ?? throw new CantFindByIdException("order", orderId);
        if (orderEntity.Status != OrderStatus.Created)
        {
            throw new ConflictException("You cant cancel order, that is already taken by cook");
        }
        orderEntity.Status = OrderStatus.Canceled;

        await _context.SaveChangesAsync();
    }
}