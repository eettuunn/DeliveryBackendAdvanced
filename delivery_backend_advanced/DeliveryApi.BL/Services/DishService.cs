using AutoMapper;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Entities;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApi.BL.Services;

public class DishService : IDishService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public DishService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DishDetailsDto> GetDishDetails(Guid dishId)
    {
        var dishEntity = await _context
            .Dishes
            .Include(dish => dish.Ratings)
            .FirstOrDefaultAsync(dish => dish.Id == dishId) ?? throw new CantFindByIdException("dish", dishId);

        DishDetailsDto dishDto = _mapper.Map<DishDetailsDto>(dishEntity);

        return dishDto;
    }

    public async Task<bool> CheckAbilityToRate(Guid dishId)
    {
        //todo: do when add users
        var dishEntity = await _context
            .Dishes
            .Where(dish => dish.Id == dishId)
            .FirstOrDefaultAsync() ?? throw new CantFindByIdException("dish", dishId);
        
        var dishBasketEntities = await _context
            .DishesInBasket
            .Include(dish => dish.Dish)
            .Where(dish => dish.Dish.Id == dishId)
            .ToListAsync();
        if (dishBasketEntities.Count == 0)
        {
            return false;
        }

        var userOrders = await _context
            .Orders
            /*.Include(order => order.Customer)
            .Where(order => order.Customer.Email == email)*/
            .Include(order => order.Dishes)
            .ThenInclude(dish => dish.Dish)
            .Where(order => order.Status == OrderStatus.Delivered)
            .ToListAsync();

        return CheckDishInOrder(userOrders, dishBasketEntities);
    }

    
    // auxiliary
    private bool CheckDishInOrder(List<OrderEntity> orders, List<DishBasketEntity> dishes)
    {
        foreach (var order in orders)
        {
            foreach (var dish in dishes)
            {
                if (order.Dishes.Contains(dish))
                {
                    return true;
                }
            }
        }

        return false;
    }
}