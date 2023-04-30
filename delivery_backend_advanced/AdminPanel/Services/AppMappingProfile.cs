using AdminPanel.Models;
using AutoMapper;
using delivery_backend_advanced.Models.Entities;

namespace AdminPanel.Services;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        CreateMap<RestaurantEntity, RestaurantListElement>();
    }
}