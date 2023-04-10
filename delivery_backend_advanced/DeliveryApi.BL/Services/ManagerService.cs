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
        if(await _context.Menus.AnyAsync(menu => menu.Name == createMenuDto.name))
        {
            throw new BadRequestException("Menu with this name already exists");
        }
        
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

    public async Task AddDishToMenu(Guid menuId, Guid dishId)
    {
        var dishEntity = await _context
            .Dishes
            .FirstOrDefaultAsync(dish => dish.Id == dishId) ?? throw new CantFindByIdException("dish", dishId);
        var menuEntity = await _context
            .Menus
            .FirstOrDefaultAsync(menu => menu.Id == menuId) ?? throw new CantFindByIdException("menu", menuId);

        if (menuEntity.Dishes.Contains(dishEntity))
        {
            throw new ConflictException($"Dish with id {dishId} is already in menu");
        }
        
        menuEntity.Dishes.Add(dishEntity);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteDishFromMenu(Guid menuId, Guid dishId)
    {
        var menuEntity = await _context
            .Menus
            .FirstOrDefaultAsync(menu => menu.Id == menuId) ?? throw new CantFindByIdException("menu", menuId);
        var dishEntity = await _context
            .Dishes
            .FirstOrDefaultAsync(dish => dish.Id == dishId) ?? throw new CantFindByIdException("dish", dishId);

        if (!menuEntity.Dishes.Contains(dishEntity))
        {
            throw new ConflictException($"There is no dish with id {dishId}");
        }
        
        menuEntity.Dishes.Remove(dishEntity);

        await _context.SaveChangesAsync();
    }

    public async Task SetMenuMain(Guid restaurantId, Guid menuId)
    {
        var menuEntity = await _context
            .Menus
            .FirstOrDefaultAsync(menu => menu.Id == menuId) ?? throw new CantFindByIdException("menu", menuId);
        var restEntity = await _context
            .Restaurants
            .Include(rest => rest.Menus)
            .FirstOrDefaultAsync(rest => rest.Id == restaurantId);
        var prevMainMenu = restEntity.Menus.FirstOrDefault(menu => menu.IsMain);

        prevMainMenu.IsMain = false;
        menuEntity.IsMain = true;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteMenu(Guid menuId)
    {
        var menuEntity = await _context
            .Menus
            .FirstOrDefaultAsync(menu => menu.Id == menuId) ?? throw new CantFindByIdException("menu", menuId);

        _context.Menus.Remove(menuEntity);
        await _context.SaveChangesAsync();
    }
}