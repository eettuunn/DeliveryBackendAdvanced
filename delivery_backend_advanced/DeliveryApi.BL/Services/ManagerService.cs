using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Entities;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApi.BL.Services;

public class ManagerService : IManagerService
{
    private readonly AppDbContext _context;

    public ManagerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateMenu(CreateMenuDto createMenuDto)
    {
        //todo: manager cant create menu for not his rest
        var restaurantEntity = await _context
                                   .Restaurants
                                   .FirstOrDefaultAsync(r => r.Id == createMenuDto.restaurantId) ??
                               throw new CantFindByIdException("restaurant", createMenuDto.restaurantId);
        
        MenuEntity menuEntity = new MenuEntity()
        {
            Id = new Guid(),
            IsMain = false,
            Name = createMenuDto.name,
            Restaurant = restaurantEntity
        };

        await _context.Menus.AddAsync(menuEntity);
        await _context.SaveChangesAsync();
    }
}