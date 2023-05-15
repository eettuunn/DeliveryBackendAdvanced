using delivery_backend_advanced.Models.Dtos;

namespace delivery_backend_advanced.Services.Interfaces;

public interface IBasketService
{
    public Task AddDishToBasket(Guid dishId, Guid restaurantId, UserInfoDto userInfoDto);
    public Task<BasketDto> GetUserBasket(UserInfoDto userInfoDto);
    public Task DeleteDishFromBasket(Guid dishBasketId, UserInfoDto userInfoDto);
    public Task ReduceDishInBasket(Guid dishBasketId, UserInfoDto userInfoDto);
}