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
    private readonly BackendDbContext _context;
    private readonly IMapper _mapper;

    public DishService(BackendDbContext context, IMapper mapper)
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

    public async Task<bool> CheckAbilityToRate(Guid dishId, UserInfoDto userInfoDto)
    {
        await CreateCustomerIfNull(userInfoDto);

        if (!await _context.Dishes.AnyAsync(d => d.Id == dishId))
        {
            throw new CantFindByIdException("dish", dishId);
        }
        
        var dishBasketEntities = await _context
            .DishesInBasket
            .Where(dib => dib.Dish.Id == dishId && dib.Customer.Id == userInfoDto.id)
            .ToListAsync();
        if (dishBasketEntities.Count == 0)
        {
            return false;
        }

        var userOrders = await _context
            .Orders
            .Where(order => order.Customer.Id == userInfoDto.id)
            .Include(order => order.Dishes)
            .ThenInclude(dish => dish.Dish)
            .Where(order => order.Status == OrderStatus.Delivered)
            .ToListAsync();

        return CheckDishInOrder(userOrders, dishId);
    }

    public async Task RateDish(Guid dishId, int value, UserInfoDto userInfoDto)
    {
        var canRate = await CheckAbilityToRate(dishId, userInfoDto);
        
        if (canRate)
        {
            var customer = await CreateCustomerIfNull(userInfoDto);
            
            var dishEntity = await _context
                .Dishes
                .Include(dish => dish.Ratings)
                .FirstOrDefaultAsync(dish => dish.Id == dishId) ?? throw new CantFindByIdException("dish", dishId);

            var ratingEntity = await _context
                .Ratings
                .FirstOrDefaultAsync(rat => rat.Customer.Id == userInfoDto.id);
            if (ratingEntity == null)
            {
                ratingEntity = new RatingEntity()
                {
                    Id = new Guid(),
                    Value = value,
                    Dish = dishEntity,
                    Customer = customer
                };
            }
            else
            {
                ratingEntity.Value = value;
            }

            await _context.Ratings.AddAsync(ratingEntity);
            RecalculateAverageRating(ref dishEntity);
            await _context.SaveChangesAsync();
        }
        else throw new ConflictException("You can't rate this dish");
    }


    // auxiliary
    private bool CheckDishInOrder(List<OrderEntity> orders, Guid dishId)
    {
        foreach (var order in orders)
        {
            foreach (var dish in order.Dishes)
            {
                if (dish.Dish.Id == dishId)
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
    
    private async Task<CustomerEntity> CreateCustomerIfNull(UserInfoDto userInfoDto)
    {
        var customer = await _context
            .Customers
            .FirstOrDefaultAsync(c => c.Id == userInfoDto.id);
        if (customer == null)
        {
            var newCustomer = new CustomerEntity()
            {
                Id = userInfoDto.id,
                Address = userInfoDto.address
            };
            
            await _context.Customers.AddAsync(newCustomer);
            await _context.SaveChangesAsync();
            customer = newCustomer;
        }
        customer.Address = userInfoDto.address ?? customer.Address;

        return customer;
    }
}