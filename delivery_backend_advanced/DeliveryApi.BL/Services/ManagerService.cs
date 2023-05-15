using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Entities;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApi.BL.Services;

public class ManagerService : IManagerService
{
    private readonly BackendDbContext _context;

    public ManagerService(BackendDbContext context)
    {
        _context = context;
    }

    public async Task CreateMenu(CreateMenuDto createMenuDto, UserInfoDto userInfoDto)
    {
        var manager = await CreateManagerIfNull(userInfoDto);
        
        if(await _context.Menus.AnyAsync(menu => menu.Name == createMenuDto.name))
        {
            throw new BadRequestException("Menu with this name already exists");
        }
        
        var restaurantEntity = await _context
                                   .Restaurants
                                   .Include(r => r.Managers)
                                   .Include(r => r.Menus)
                                   .FirstOrDefaultAsync(r => r.Id == createMenuDto.restaurantId) ??
                               throw new CantFindByIdException("restaurant", createMenuDto.restaurantId);
        if (!restaurantEntity.Managers.Contains(manager))
        {
            throw new BadRequestException("You can't create menu for not yours restaurant");
        }
        
        MenuEntity menuEntity = new MenuEntity()
        {
            Id = new Guid(),
            IsMain = false,
            Name = createMenuDto.name,
            Restaurant = restaurantEntity
        };
        if (restaurantEntity.Menus.Count == 0)
        {
            menuEntity.IsMain = true;
        }
        
        restaurantEntity.Managers.Add(manager);
        await _context.Menus.AddAsync(menuEntity);
        await _context.SaveChangesAsync();
    }

    public async Task AddDishToMenu(Guid menuId, Guid dishId, UserInfoDto userInfoDto)
    {
        await CreateManagerIfNull(userInfoDto);

        var dishEntity = await _context
            .Dishes
            .FirstOrDefaultAsync(dish => dish.Id == dishId) ?? throw new CantFindByIdException("dish", dishId);
        var menuEntity = await _context
            .Menus
            .Include(m => m.Restaurant)
            .ThenInclude(r => r.Managers)
            .FirstOrDefaultAsync(menu => menu.Id == menuId) ?? throw new CantFindByIdException("menu", menuId);

        if (menuEntity.Restaurant.Managers.All(m => m.Id != userInfoDto.id))
        {
            throw new BadRequestException("You can't add dish to menu not in your restaurant");
        }
        if (menuEntity.Dishes.Contains(dishEntity))
        {
            throw new ConflictException($"Dish with id {dishId} is already in menu");
        }
        
        menuEntity.Dishes.Add(dishEntity);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteDishFromMenu(Guid menuId, Guid dishId, UserInfoDto userInfoDto)
    {
        await CreateManagerIfNull(userInfoDto);
        
        var menuEntity = await _context
            .Menus
            .Include(m => m.Restaurant)
            .ThenInclude(r => r.Managers)
            .FirstOrDefaultAsync(menu => menu.Id == menuId) ?? throw new CantFindByIdException("menu", menuId);
        var dishEntity = await _context
            .Dishes
            .FirstOrDefaultAsync(dish => dish.Id == dishId) ?? throw new CantFindByIdException("dish", dishId);

        if (menuEntity.Restaurant.Managers.All(m => m.Id != userInfoDto.id))
        {
            throw new BadRequestException("You can't delete dish from menu not in your restaurant");
        }
        if (!menuEntity.Dishes.Contains(dishEntity))
        {
            throw new ConflictException($"There is no dish with id {dishId}");
        }
        
        menuEntity.Dishes.Remove(dishEntity);

        await _context.SaveChangesAsync();
    }

    public async Task SetMenuMain(Guid restaurantId, Guid menuId, UserInfoDto userInfoDto)
    {
        await CreateManagerIfNull(userInfoDto);
        
        var menuEntity = await _context
            .Menus
            .Include(m => m.Restaurant)
            .ThenInclude(r => r.Managers)
            .FirstOrDefaultAsync(menu => menu.Id == menuId) ?? throw new CantFindByIdException("menu", menuId);
        var restEntity = await _context
            .Restaurants
            .Include(rest => rest.Menus)
            .FirstOrDefaultAsync(rest => rest.Id == restaurantId);
        var prevMainMenu = restEntity.Menus.FirstOrDefault(menu => menu.IsMain);
        
        if (menuEntity.Restaurant.Managers.All(m => m.Id != userInfoDto.id))
        {
            throw new BadRequestException("You can't set menu main not in your restaurant");
        }
        
        if (prevMainMenu != null)
        {
            prevMainMenu.IsMain = false;
        }
        menuEntity.IsMain = true;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteMenu(Guid menuId, UserInfoDto userInfoDto)
    {
        await CreateManagerIfNull(userInfoDto);
        
        var menuEntity = await _context
            .Menus
            .Include(m => m.Restaurant)
            .ThenInclude(r => r.Managers)
            .Include(m => m.Dishes)
            .FirstOrDefaultAsync(menu => menu.Id == menuId) ?? throw new CantFindByIdException("menu", menuId);

        if (menuEntity.Restaurant.Managers.All(m => m.Id != userInfoDto.id))
        {
            throw new BadRequestException("You can't delete menu from not your restaurant");
        }

        menuEntity.Dishes = new();
        _context.Menus.Remove(menuEntity);
        await _context.SaveChangesAsync();
    }

    public async Task EditMenu(Guid menuId, EditMenuDto editMenuDto, UserInfoDto userInfoDto)
    {
        await CreateManagerIfNull(userInfoDto);
        
        if(await _context.Menus.AnyAsync(menu => menu.Name == editMenuDto.name))
        {
            throw new BadRequestException("Menu with this name already exists");
        }
        
        var menuEntity = await _context
            .Menus
            .Include(m => m.Restaurant)
            .ThenInclude(r => r.Managers)
            .FirstOrDefaultAsync(menu => menu.Id == menuId) ?? throw new CantFindByIdException("menu", menuId);
        
        if (menuEntity.Restaurant.Managers.All(m => m.Id != userInfoDto.id))
        {
            throw new BadRequestException("You can't edit menu from not your restaurant");
        }

        menuEntity.Name = editMenuDto.name ?? menuEntity.Name;
        
        await _context.SaveChangesAsync();
    }
    
    
    
    private async Task<ManagerEntity> CreateManagerIfNull(UserInfoDto userInfoDto)
    {
        var manager = await _context
            .Managers
            .FirstOrDefaultAsync(c => c.Id == userInfoDto.id);
        if (manager == null)
        {
            var newManager = new ManagerEntity()
            {
                Id = userInfoDto.id,
            };
            
            await _context.Managers.AddAsync(newManager);
            await _context.SaveChangesAsync();
            manager = newManager;
        }

        return manager;
    }
}