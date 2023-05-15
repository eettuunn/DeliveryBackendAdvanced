using delivery_backend_advanced.Models.Dtos;

namespace delivery_backend_advanced.Services.Interfaces;

public interface IDishService
{
    public Task<DishDetailsDto> GetDishDetails(Guid dishId);
    public Task<bool> CheckAbilityToRate(Guid dishId, UserInfoDto userInfoDto);
    public Task RateDish(Guid dishId, int value, UserInfoDto userInfoDto);
}