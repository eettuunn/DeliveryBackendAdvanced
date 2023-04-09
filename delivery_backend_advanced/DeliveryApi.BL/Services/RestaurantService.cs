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
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public RestaurantService(AppDbContext context, IMapper mapper, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
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

    public async Task<RestaurantDetailsDto> GetRestaurantDetails(Guid restaurantId, string? menuName)
    {
        var rest = await _context
            .Restaurants
            .Where(r => r.Id == restaurantId)
            .Include(r => r.Menus)
            .ThenInclude(menu => menu.Dishes)
            .FirstOrDefaultAsync() ?? throw new CantFindByIdException("restaurant", restaurantId);
        
        var restDto = _mapper.Map<RestaurantDetailsDto>(rest);
        if (menuName == null)
        {
            restDto.menu = _mapper.Map<MenuDto>(rest
                .Menus
                .FirstOrDefault(menu => menu.IsMain));
        }
        else
        {
            var restEntity = rest.Menus.FirstOrDefault(menu => menu.Name == menuName);
            restDto.menu = restEntity == null
                ? _mapper.Map<MenuDto>(rest
                    .Menus
                    .FirstOrDefault(menu => menu.IsMain))
                : _mapper.Map<MenuDto>(restEntity);
        }
        
        return restDto;
    }

    /*public async Task<OrdersPageDto> GetRestaurantOrders(Guid restaurantId, OrderQueryModel query)
    {
        var restaurant = await _context
                             .Restaurants
                             .Include(r => r.Orders)
                             .ThenInclude(order => order.Dishes)
                             .ThenInclude(dib => dib.Dish)
                             .FirstOrDefaultAsync(r => r.Id == restaurantId) ??
                         throw new CantFindByIdException("restaurant", restaurantId);

        
        var page = query.page ?? 1;
        var pageOrderCount = _configuration.GetValue<double>("PageSize");
        var orderCountInRest = restaurant.Orders.Count;
        var ordersSkip = (int)((page - 1) * pageOrderCount);
        var ordersTake = (int)Math.Min(orderCountInRest - (page - 1) * pageOrderCount, pageOrderCount);
        
        var orderEntities = restaurant.Orders
            .Where(order => order.Restaurant.Id == restaurantId && (order.Status == OrderStatus.Created ||
                                                                    order.Status == OrderStatus.Kitchen ||
                                                                    order.Status == OrderStatus.Packaging))
            .ToList();
        
        
        SortAndFilterOrders(ref orderEntities, query);

        
        var pageCount = (int)Math.Ceiling(orderEntities.Count / pageOrderCount);
        pageCount = pageCount == 0 ? 1 : pageCount;
        orderEntities = orderEntities
            .Skip(ordersSkip)
            .Take(ordersTake)
            .ToList();
        if (page > pageCount || page < 1)
        {
            throw new BadRequestException("Incorrect current page");
        }

        
        List<OrderListElementDto> orderDtos = _mapper.Map<List<OrderListElementDto>>(orderEntities);

        
        var pageInfo = new PaginationDto()
        {
            current = page,
            count = pageCount,
            size = orderEntities.Count
        };
        OrdersPageDto ordersPage = new OrdersPageDto()
        {
            orders = orderDtos,
            pagination = pageInfo
        };
        
        
        return ordersPage;
    }*/



    private void SortAndFilterOrders(ref List<OrderEntity> orders, OrderQueryModel query)
    {
        if (query.sort != null)
        {
            switch (query.sort)
            {
                case OrderSort.CreateAsc:
                    orders = orders.OrderBy(order => order.OrderTime).ToList();
                    break;
                case OrderSort.CreateDesc:
                    orders = orders.OrderByDescending(order => order.OrderTime).ToList();
                    break;
                case OrderSort.DeliveryAsc:
                    orders = orders.OrderBy(order => order.DeliveryTime).ToList();
                    break;
                case OrderSort.DeliveryDesc:
                    orders = orders.OrderByDescending(order => order.DeliveryTime).ToList();
                    break;
            }
        }
        
        if (query.search != null)
        {
            orders = orders.Where(order => order.Number.ToString().Contains(query.search.ToString())).ToList();
        }

        if (query.statuses.Count != 0)
        {
            orders = orders.Where(order => query.statuses.Contains(order.Status)).ToList();
        }
    }
}