using AutoMapper;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Dtos;
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

    //todo: check for user
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

    //todo: take only users dishes
    public async Task<BasketDto> GetUserBasket()
    {
        var dishesInBasket = await _context
            .DishesInBasket
            .Include(dib => dib.Dish)
            .Include(dib => dib.Restaurant)
            .Where(dib => !dib.IsInOrder)
            .ToListAsync();
        
        List<RestaurantBasketDto> restaurants = FillRestaurantsInBasket(dishesInBasket);
        BasketDto basketDto = new BasketDto()
        {
            restaurants = restaurants,
            totalBasketPrice = GetBasketTotalPrice(restaurants)
        };

        return basketDto;
    }

    private List<RestaurantBasketDto> FillRestaurantsInBasket(List<DishBasketEntity> dishesInBasket)
    {
        List<RestaurantBasketDto> restaurants = new();
        
        foreach (var dish in dishesInBasket)
        {
            var dishBasketDto = _mapper.Map<DishBasketDto>(dish.Dish);
            dishBasketDto.amount = dish.Amount;
            dishBasketDto.totalPrice = dishBasketDto.amount * dishBasketDto.price;
            
            int index = restaurants.FindIndex(r => r.id == dish.Restaurant.Id);
            if (index >= 0)
            {
                restaurants[index].dishesInBasket.Add(dishBasketDto);
            }
            else
            {
                RestaurantBasketDto restaurantBasketDto = new RestaurantBasketDto()
                {
                    id = dish.Restaurant.Id,
                    name = dish.Restaurant.Name
                };
                restaurantBasketDto.dishesInBasket.Add(dishBasketDto);
                restaurants.Add(restaurantBasketDto);
            }
        }

        return restaurants;
    }

    private int GetBasketTotalPrice(List<RestaurantBasketDto> restaurantDtos)
    {
        int totalPrice = 0;
        foreach (var r in restaurantDtos)
        {
            totalPrice += r.dishesInBasket.Sum(d => d.totalPrice);
        }

        return totalPrice;
    }
}