using delivery_backend_advanced.Models.Dtos;
using Microsoft.AspNetCore.Http;

namespace delivery_backend_advanced.Services.Interfaces;

public interface IRestaurantService
{
    public Task<List<RestaurantListElementDto>> GetRestaurantList();

    public Task<RestaurantDetailsDto> GetRestaurantDetails(Guid restaurantId, Guid? menuId);

    public Task<OrdersPageDto> GetRestaurantOrders(Guid restaurantId, OrderQueryModel query);
}