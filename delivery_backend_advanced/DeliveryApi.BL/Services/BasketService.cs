using AutoMapper;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Entities;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApi.BL.Services;

public class BasketService : IBasketService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public BasketService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task AddDishToBasket(Guid dishId, Guid restaurantId)
    {
        var dishEntity = await _context
            .Dishes
            .FirstOrDefaultAsync(dish => dish.Id == dishId) ??
                         throw new CantFindByIdException("dish", dishId);
        
        var restaurantEntity = await _context
            .Restaurants
            .FirstOrDefaultAsync(r => r.Id == restaurantId) ?? 
                               throw new CantFindByIdException("restaurant", restaurantId);
        
        var dishBasketEntity = await _context
            .DishesInBasket
            .Include(dib => dib.Dish)
            .Include(dib => dib.Restaurant)
            .Where(dib => dib.Restaurant == restaurantEntity && dib.Dish == dishEntity && !dib.IsInOrder)
            .FirstOrDefaultAsync();
        
        
        if (dishBasketEntity != null)
        {
            dishBasketEntity.Amount++;
        }
        else
        {
            dishBasketEntity = new DishBasketEntity()
            {
                Id = new Guid(),
                Dish = dishEntity,
                Amount = 1,
                Restaurant = restaurantEntity,
                IsInOrder = false
            };
            await _context.DishesInBasket.AddAsync(dishBasketEntity);
        }

        await _context.SaveChangesAsync();
    }
}