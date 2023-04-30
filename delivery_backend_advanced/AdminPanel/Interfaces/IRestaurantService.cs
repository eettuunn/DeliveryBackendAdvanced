using AdminPanel.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AdminPanel.Interfaces;

public interface IRestaurantService
{
    public Task CreateRestaurant(CreateRest rest, ModelStateDictionary modelState);
}