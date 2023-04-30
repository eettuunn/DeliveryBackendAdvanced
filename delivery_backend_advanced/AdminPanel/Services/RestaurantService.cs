using AdminPanel.Interfaces;
using AdminPanel.Models;
using AutoMapper;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Services;

public class RestaurantService : IRestaurantService
{
    private readonly BackendDbContext _context;
    private readonly IMapper _mapper;

    public RestaurantService(BackendDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task CreateRestaurant(CreateRest rest, ModelStateDictionary modelState)
    {
        var newRest = new RestaurantEntity
        {
            Id = new Guid(),
            Name = rest.name
        };

        if (await _context.Restaurants.AnyAsync(r => r.Name == rest.name))
        {
            modelState.AddModelError(nameof(rest.name), $"Restaurant with name {rest.name} already exists");
        }
        else
        {
            await _context.Restaurants.AddAsync(newRest);
            await _context.SaveChangesAsync();   
        }
    }

    public async Task<List<RestaurantListElement>> GetRestaurantList()
    {
        var rests = await _context
            .Restaurants
            .ToListAsync();

        var displayRests = _mapper.Map<List<RestaurantListElement>>(rests);

        return displayRests;
    }
}