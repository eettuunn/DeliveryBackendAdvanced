using AutoMapper;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Entities;
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
            restDtos[i].menus = restEntities[i]
                .Menus
                .Select(menu => menu.Name)
                .ToList();
        }

        return restDtos;
    }

    public async Task<RestaurantDetailsDto> GetRestaurantDetails(Guid restaurantId, Guid? menuId)
    {
        var rest = await _context
            .Restaurants
            .Where(r => r.Id == restaurantId)
            .Include(r => r.Menus)
            .FirstOrDefaultAsync();
        
        var restDto = _mapper.Map<RestaurantDetailsDto>(rest);
        if (menuId == null)
        {
            restDto.menu = _mapper.Map<MenuDto>(rest.Menus[0]);
        }
        else
        {
            restDto.menu = _mapper.Map<MenuDto>(rest
                .Menus
                .Where(menu => menu.Id == menuId));
        }
        
        return restDto;
    }
}