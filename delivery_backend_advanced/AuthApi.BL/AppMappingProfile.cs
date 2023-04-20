using AuthApi.Common.Dtos;
using AuthApi.DAL.Entities;
using AutoMapper;

namespace AuthApi.BL;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        CreateMap<RegisterUserDto, AppUser>();
        CreateMap<RegisterUserDto, CustomerEntity>();
        CreateMap<AppUser, TokenUserDto>();
        CreateMap<AppUser, ProfileDto>(); 
    }
}