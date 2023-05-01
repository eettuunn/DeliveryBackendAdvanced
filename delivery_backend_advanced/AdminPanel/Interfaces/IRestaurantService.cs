using AdminPanel.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AdminPanel.Interfaces;

public interface IRestaurantService
{
    public Task CreateRestaurant(CreateRest rest, ModelStateDictionary modelState);
    public Task<List<RestaurantListElement>> GetRestaurantList();
    public Task DeleteRest(Guid Id);
    public Task EditRest(Guid Id, EditRest editRest, ModelStateDictionary modelState);
    public Task<RestInfo> GetRestInfo(Guid id);
}