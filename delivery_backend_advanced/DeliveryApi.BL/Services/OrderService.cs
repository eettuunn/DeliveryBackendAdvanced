using AutoMapper;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Entities;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
            Dishes = orderDishes
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
}