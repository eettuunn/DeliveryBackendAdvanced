using AdminPanel.Interfaces;
using AdminPanel.Models;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Services;

public class RestaurantService : IRestaurantService
{
    private readonly BackendDbContext _context;

    public RestaurantService(BackendDbContext context)
    {
        _context = context;
    }

    public async Task CreateRestaurant(CreateRest rest, ModelStateDictionary modelState)
    {
        var newRest = new RestaurantEntity
        {
            Id = new Guid(),
            Name = rest.Name
        };

        if (await _context.Restaurants.AnyAsync(r => r.Name == rest.Name))
        {
            modelState.AddModelError(nameof(rest.Name), $"Restaurant with name {rest.Name} already exists");
        }
        else
        {
            await _context.Restaurants.AddAsync(newRest);
            await _context.SaveChangesAsync();   
        }
    }
}