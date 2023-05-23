using AutoMapper;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Entities;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace delivery_backend_advanced.Services;

public class RestaurantService : IRestaurantService
{
    private readonly BackendDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public RestaurantService(BackendDbContext context, IMapper mapper, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<RestListPageDto> GetRestaurantList(string? search, int? page)
    {
        var restEntities = await _context
            .Restaurants
            .Include(r => r.Menus)
            .Where(r => r.Name.Contains(search) || search == null)
            .ToListAsync();

        
        var curPage = page ?? 1;
        var pageRestsCount = _configuration.GetValue<double>("PageSize");
        var restCount = restEntities.Count;
        var dishesSkip = (int)((page - 1) * pageRestsCount);
        var dishesTake = (int)Math.Min(restCount - (curPage - 1) * pageRestsCount, pageRestsCount);
        var pageCount = (int)Math.Ceiling(restEntities.Count / pageRestsCount);
        pageCount = pageCount == 0 ? 1 : pageCount;
        restEntities = restEntities
            .Skip(dishesSkip)
            .Take(dishesTake)
            .ToList();
        if (page > pageCount || page < 1)
        {
            throw new BadRequestException("Incorrect current page");
        }
        
        
        List<RestaurantListElementDto> restDtos = _mapper.Map<List<RestaurantListElementDto>>(restEntities);
        for (int i = 0; i < restEntities.Count; i++)
        {
            restDtos[i].menu = _mapper.Map<List<MenuShortDto>>(restEntities[i]
                .Menus
                .ToList());
        }

        
        var pageInfo = new PaginationDto()
        {
            current = curPage,
            count = pageCount,
            size = restEntities.Count
        };
        RestListPageDto restPage = new RestListPageDto()
        {
            rests = restDtos,
            pagination = pageInfo
        };
        
        return restPage;
    }

    public async Task<RestaurantPageDto> GetRestaurantDetails(Guid restaurantId, DishesQueryModel query)
    {
        var rest = await _context
            .Restaurants
            .Where(r => r.Id == restaurantId)
            .Include(r => r.Menus)
            .ThenInclude(menu => menu.Dishes)
            .FirstOrDefaultAsync() ?? throw new CantFindByIdException("restaurant", restaurantId);
        
        var restDto = _mapper.Map<RestaurantDetailsDto>(rest);
        restDto.menuNames = rest.Menus.Select(menu => menu.Name).ToList();
        restDto.menu = rest.Menus.Count != 0 ? GetMenu(query.menu, rest) : null;

        RestaurantPageDto restaurantPage = new RestaurantPageDto();
        if (restDto.menu != null)
        {
            var page = query.page ?? 1;
            var pageDishesCount = _configuration.GetValue<double>("PageSize");
            var dishCountInMenu = restDto.menu.dishes.Count;
            var dishesSkip = (int)((page - 1) * pageDishesCount);
            var dishesTake = (int)Math.Min(dishCountInMenu - (page - 1) * pageDishesCount, pageDishesCount);


            var dishes = restDto.menu.dishes;
            SortAndFilterDishes(ref dishes, query);
            restDto.menu.dishes = dishes;

            var pageCount = (int)Math.Ceiling(restDto.menu.dishes.Count / pageDishesCount);
            pageCount = pageCount == 0 ? 1 : pageCount;
            restDto.menu.dishes = restDto.menu.dishes
                .Skip(dishesSkip)
                .Take(dishesTake)
                .ToList();
            if (page > pageCount || page < 1)
            {
                throw new BadRequestException("Incorrect current page");
            }

            var pageInfo = new PaginationDto()
            {
                current = page,
                count = pageCount,
                size = restDto.menu.dishes.Count
            };
            restaurantPage = new RestaurantPageDto()
            {
                restaurantDetails = restDto,
                pagination = pageInfo
            };
        }
        else
        {
            restaurantPage.restaurantDetails = restDto;
        }

        return restaurantPage;
    }



    private void SortAndFilterDishes(ref List<DishListElementDto> dishes, DishesQueryModel query)
    {
        if (query.sort != null)
        {
            switch (query.sort)
            {
                case DishSort.NameAsc:
                    dishes = dishes.OrderBy(dish => dish.name).ToList();
                    break;
                case DishSort.NameDesc:
                    dishes = dishes.OrderByDescending(dish => dish.name).ToList();
                    break;
                case DishSort.PriceAsc:
                    dishes = dishes.OrderBy(dish => dish.price).ToList();
                    break;
                case DishSort.PriceDesc:
                    dishes = dishes.OrderByDescending(dish => dish.price).ToList();
                    break;
                case DishSort.RatingAsc:
                    var dishesWithRating = dishes
                        .OrderBy(dish => dish.averageRating)
                        .Where(dish => dish.averageRating != null)
                        .ToList();
                    var dishesWithoutRating = dishes
                        .Where(dish => dish.averageRating == null)
                        .ToList();
            
                    dishesWithRating.AddRange(dishesWithoutRating);
                    dishes = dishesWithRating;
                    break;
                case DishSort.RatingDesc:
                    dishes = dishes.OrderByDescending(dish => dish.averageRating).ToList();
                    break;
            }
        }
        
        if (query.isVegetarian != null)
        {
            dishes = dishes.Where(dish => dish.isVegetarian == query.isVegetarian).ToList();
        }

        if (query.categories.Count != 0)
        {
            dishes = dishes.Where(dish => query.categories.Contains(dish.category)).ToList();
        }
    }

    private MenuDto? GetMenu(string menuName, RestaurantEntity restaurantEntity)
    {
        var menuDto = new MenuDto();
        if (menuName == null)
        {
            menuDto = _mapper.Map<MenuDto>(restaurantEntity
                .Menus
                .FirstOrDefault(menu => menu.IsMain));
        }
        else
        {
            var restEntity = restaurantEntity.Menus.FirstOrDefault(menu => menu.Name == menuName);
            menuDto = restEntity == null
                ? _mapper.Map<MenuDto>(restaurantEntity
                    .Menus
                    .FirstOrDefault(menu => menu.IsMain))
                : _mapper.Map<MenuDto>(restEntity);
        }

        return menuDto;
    }
}