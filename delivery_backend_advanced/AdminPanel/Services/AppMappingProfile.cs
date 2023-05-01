using AdminPanel.Models;
using AuthApi.Common.Enums;
using AuthApi.DAL.Entities;
using AutoMapper;
using delivery_backend_advanced.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Services;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        CreateMap<RestaurantEntity, RestaurantListElement>();
        CreateMap<RestaurantEntity, RestInfo>();
        CreateMap<AppUser, UserListElement>();
        CreateMap<AppUser, UserInfo>();
        CreateMap<IdentityRole, Role>();
    }
}