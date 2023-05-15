using delivery_backend_advanced.Models.Dtos;

namespace delivery_backend_advanced.Services.Interfaces;

public interface IManagerService
{
    public Task CreateMenu(CreateMenuDto createMenuDto, UserInfoDto userInfoDto);
    public Task AddDishToMenu(Guid menuId, Guid dishId, UserInfoDto userInfoDto);
    public Task DeleteDishFromMenu(Guid menuId, Guid dishId, UserInfoDto userInfoDto);
    public Task SetMenuMain(Guid restaurantId, Guid menuId, UserInfoDto userInfoDto);
    public Task DeleteMenu(Guid menuId, UserInfoDto userInfoDto);
    public Task EditMenu(Guid menuId, EditMenuDto editMenuDto, UserInfoDto userInfoDto);
}