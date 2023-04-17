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

    public async Task RateDish(Guid dishId, int value)
    {
        if (value is < 1 or > 10)
        {
            throw new BadRequestException("Rating value must be in range from 1 to 10");
        }
        var canRate = await CheckAbilityToRate(dishId);

        if (canRate)
        {
            var dishEntity = await _context
                .Dishes
                .Include(dish => dish.Ratings)
                .FirstOrDefaultAsync(dish => dish.Id == dishId) ?? throw new CantFindByIdException("dish", dishId);
          
            //todo: add change rating when user already rated this dish
            
            RatingEntity ratingEntity = new RatingEntity()
            {
                Id = new Guid(),
                Value = value,
                Dish = dishEntity
            };

            await _context.Ratings.AddAsync(ratingEntity);
            RecalculateAverageRating(ref dishEntity);
            await _context.SaveChangesAsync();
        }
        else throw new ConflictException("You can't rate this dish");
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

    private void RecalculateAverageRating(ref DishEntity dishEntity)
    {
        int sum = dishEntity.Ratings.Sum(r => r.Value);
        int count = dishEntity.Ratings.Count;

        dishEntity.AverageRating = Math.Round((double)sum / count, 1);
    }
}