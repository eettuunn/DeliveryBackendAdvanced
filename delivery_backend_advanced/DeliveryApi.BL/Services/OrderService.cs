﻿using AutoMapper;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Entities;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DeliveryApi.BL.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public OrderService(AppDbContext context, IMapper mapper, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task CreateOrder(CreateOrderDto orderDto)
    {
        if (orderDto.deliveryTime < DateTime.UtcNow.AddHours(1) && orderDto.deliveryTime != null)
        {
            throw new BadRequestException("delivery time must be more, then now + 1 hour");
        }
        
        //todo: maybe do create orders for all rests at once
        var orderDishes = await _context
            .DishesInBasket
            .Include(dib => dib.Dish)
            .Where(dib => !dib.IsInOrder)
            .ToListAsync();
        if (orderDishes.Count == 0)
        {
            throw new ConflictException("There are no dishes in basket");
        }
        
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

    public async Task<OrdersPageDto> GetOrders(OrderQueryModel query, UserRole role)
    {
        var orderEn = OrdersRoleDepend(role, query.current);
        
        var page = query.page ?? 1;
        var pageOrderCount = _configuration.GetValue<double>("PageSize");
        var orderCountInRest = await _context.Orders.CountAsync();
        var ordersSkip = (int)((page - 1) * pageOrderCount);
        var ordersTake = (int)Math.Min(orderCountInRest - (page - 1) * pageOrderCount, pageOrderCount);

        var orderList = SortAndFilterOrders(orderEn, query, role);
        
        var pageCount = (int)Math.Ceiling(orderList.Count / pageOrderCount);
        pageCount = pageCount == 0 ? 1 : pageCount;
        orderList = orderList
            .Skip(ordersSkip)
            .Take(ordersTake)
            .ToList();
        if (page > pageCount || page < 1)
        {
            throw new BadRequestException("Incorrect current page");
        }

        
        List<OrderListElementDto> orderDtos = _mapper.Map<List<OrderListElementDto>>(orderList);

        
        var pageInfo = new PaginationDto()
        {
            current = page,
            count = pageCount,
            size = orderList.Count
        };
        OrdersPageDto ordersPage = new OrdersPageDto()
        {
            orders = orderDtos,
            pagination = pageInfo
        };
        
        
        return ordersPage;
    }

    public async Task<OrderDto> GetOrderDetails(Guid orderId)
    {
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

    public async Task RepeatOrder(RepeatOrderDto repOrder, Guid orderId)
    {
        if (repOrder.deliveryTime < DateTime.UtcNow.AddHours(1) && repOrder.deliveryTime != null)
        {
            throw new BadRequestException("delivery time must be more, then now + 1 hour");
        }
        
        var prevOrder = await _context
            .Orders
            .Include(order => order.Dishes)
            .Include(order => order.Restaurant)
            .FirstOrDefaultAsync(order => order.Id == orderId) ?? throw new CantFindByIdException("order", orderId);

        OrderEntity newOrder = new OrderEntity()
        {
            Id = new Guid(),
            DeliveryTime = repOrder.deliveryTime ?? DateTime.UtcNow.AddHours(1),
            OrderTime = DateTime.UtcNow,
            Price = prevOrder.Price,
            Address = repOrder.address,
            Status = OrderStatus.Created,
            Number = await _context.Orders.CountAsync() + 1,
            Restaurant = prevOrder.Restaurant,
            Dishes = prevOrder.Dishes
        };

        await _context.Orders.AddAsync(newOrder);
        await _context.SaveChangesAsync();
    }



    private IEnumerable<OrderEntity> OrdersRoleDepend(UserRole role, bool current)
    {
        IEnumerable<OrderEntity> orders = Enumerable.Empty<OrderEntity>();
        switch (role)
        {
            case UserRole.Customer:
                orders = _context
                    .Orders
                    //.Where(order => order.Customer.Id == userId)
                    .Where(order => current == false || order.Status != OrderStatus.Canceled && order.Status != OrderStatus.Delivered)
                    .AsEnumerable();
                break;
            case UserRole.Manager:
                // var restaurantId = user.Restaurant.Id;
                orders = _context
                    .Orders
                    // .Where(order => order.Restaurant.Id == restaurantId)
                    .AsEnumerable();
                break;
            case UserRole.Cook:
                orders = _context
                    .Orders
                    .Where(
                        order => !current &&
                                 order.Status == OrderStatus.Created || current/* && cook.Orders.Contains(order)*/)
                    .AsEnumerable();
                break;
            case UserRole.Courier:
                orders = _context
                    .Orders
                    .Where(
                        order => !current &&
                                 order.Status == OrderStatus.Delivery || current/* && courier.Orders.Contains(order)*/)
                    .AsEnumerable();
                break;
        }

        return orders;
    }
    
    private List<OrderEntity> SortAndFilterOrders(IEnumerable<OrderEntity> orders, OrderQueryModel query, UserRole role)
    {
        if (query.sort != null)
        {
            switch (query.sort)
            {
                case OrderSort.CreateAsc:
                    orders = orders.OrderBy(order => order.OrderTime).ToList();
                    break;
                case OrderSort.CreateDesc:
                    orders = orders.OrderByDescending(order => order.OrderTime).ToList();
                    break;
                case OrderSort.DeliveryAsc:
                    orders = orders.OrderBy(order => order.DeliveryTime).ToList();
                    break;
                case OrderSort.DeliveryDesc:
                    orders = orders.OrderByDescending(order => order.DeliveryTime).ToList();
                    break;
            }
        }
        
        if (query.search != null)
        {
            orders = orders.Where(order => order.Number.ToString().Contains(query.search.ToString())).ToList();
        }

        if (query.statuses.Count != 0 && role == UserRole.Manager)
        {
            orders = orders.Where(order => query.statuses.Contains(order.Status)).ToList();
        }

        return orders.ToList();
    }
}