using AutoMapper;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Entities;

namespace delivery_backend_advanced.Models;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        CreateMap<RestaurantEntity, RestaurantListElementDto>();
        CreateMap<MenuEntity, MenuDto>().ReverseMap();
        CreateMap<DishEntity, DishListElementDto>();
        CreateMap<RestaurantEntity, RestaurantDetailsDto>().ForMember(rest => rest.menu, opt => opt.Ignore());
        CreateMap<MenuEntity, MenuShortDto>();
        CreateMap<OrderEntity, OrderDto>();
        CreateMap<DishEntity, DishInOrderDto>();
        CreateMap<DishBasketEntity, DishInOrderDto>();
        CreateMap<DishEntity, DishDetailsDto>();
    }
}