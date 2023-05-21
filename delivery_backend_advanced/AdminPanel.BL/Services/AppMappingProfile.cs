using AdminPanel._Common.Models.Restaurant;
using AdminPanel._Common.Models.User;
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
        CreateMap<EditUser, UserInfo>();
    }
}