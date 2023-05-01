using AdminPanel.Interfaces;
using AdminPanel.Models;
using AutoMapper;
using delivery_backend_advanced.Exceptions;
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

    public async Task DeleteRest(Guid Id)
    {
        var rest = await _context
            .Restaurants
            .FirstOrDefaultAsync(r => r.Id == Id) 
                   ?? throw new CantFindByIdException("rest", Id);

        _context.Remove(rest);
        await _context.SaveChangesAsync();
    }

    public async Task EditRest(Guid Id, EditRest editRest, ModelStateDictionary modelState)
    {
        if (await _context.Restaurants.AnyAsync(r => r.Name == editRest.name && r.Id != Id))
        {
            modelState.AddModelError(nameof(editRest.name), $"Restaurant with name {editRest.name} already exists");
            return;
        }

        var rest = await _context
            .Restaurants
            .FirstOrDefaultAsync(r => r.Id == Id) ?? throw new CantFindByIdException("restaurant", Id);

        rest.Name = editRest.name ?? rest.Name;

        await _context.SaveChangesAsync();
    }

    public async Task<RestInfo> GetRestInfo(Guid id)
    {
        var rest = await _context
                       .Restaurants
                       .FirstOrDefaultAsync(r => r.Id == id) 
                   ?? throw new CantFindByIdException("rest", id);

        var restInfo = _mapper.Map<RestInfo>(rest);
        return restInfo;
    }
}