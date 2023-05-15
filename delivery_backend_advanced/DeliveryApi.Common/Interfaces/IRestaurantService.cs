using delivery_backend_advanced.Models.Dtos;
using Microsoft.AspNetCore.Http;

namespace delivery_backend_advanced.Services.Interfaces;

public interface IRestaurantService
{
    public Task<RestListPageDto> GetRestaurantList(string? search, int? page);

    public Task<RestaurantPageDto> GetRestaurantDetails(Guid restaurantId, DishesQueryModel query);
}