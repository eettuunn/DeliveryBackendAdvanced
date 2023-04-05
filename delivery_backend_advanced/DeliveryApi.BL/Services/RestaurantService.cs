using AutoMapper;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace delivery_backend_advanced.Services;

public class RestaurantService : IRestaurantService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public RestaurantService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<RestaurantListElementDto>> GetRestaurantList()
    {
        var restEntities = await _context
            .Restaurants
            .Include(r => r.Menus)
            .ToListAsync();

        List<RestaurantListElementDto> restDtos = _mapper.Map<List<RestaurantListElementDto>>(restEntities);
        for (int i = 0; i < restEntities.Count; i++)
        {
            restDtos[i].menu = _mapper.Map<List<MenuShortDto>>(restEntities[i]
                .Menus
                .ToList());
        }

        return restDtos;
    }

    public async Task<RestaurantDetailsDto> GetRestaurantDetails(Guid restaurantId, Guid? menuId)
    {
        var rest = await _context
            .Restaurants
            .Where(r => r.Id == restaurantId)
            .Include(r => r.Menus)
            .ThenInclude(menu => menu.Dishes)
            .FirstOrDefaultAsync() ?? throw new CantFindByIdException("restaurant", restaurantId);
        
        var restDto = _mapper.Map<RestaurantDetailsDto>(rest);
        if (menuId == null)
        {
            restDto.menu = _mapper.Map<MenuDto>(rest
                .Menus
                .FirstOrDefault(menu => menu.IsMain));
        }
        else
        {
            restDto.menu = _mapper.Map<MenuDto>(rest
                .Menus
                .FirstOrDefault(menu => menu.Id == menuId)) ??
                            throw new CantFindByIdException("menu", menuId);
        }
        
        return restDto;
    }

    public async Task<List<OrderListElementDto>> GetRestaurantOrders(Guid restaurantId)
    {
        //todo: sorting, filters and search
        var restaurant = await _context
                             .Restaurants
                             .FirstOrDefaultAsync(r => r.Id == restaurantId) ??
                         throw new CantFindByIdException("restaurant", restaurantId);
        
        
        var orderEntities = await _context
            .Orders
            .Include(order => order.Restaurant)
            .Include(order => order.Dishes)
            .ThenInclude(dish => dish.Dish)
            .Where(order => order.Restaurant.Id == restaurantId && (order.Status == OrderStatus.Created ||
                                                                    order.Status == OrderStatus.Kitchen ||
                                                                    order.Status == OrderStatus.Packaging))
            .ToListAsync();
        
        List<OrderListElementDto> orderDtos = _mapper.Map<List<OrderListElementDto>>(orderEntities);
        
        return orderDtos;
    }
}