using AutoMapper;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApi.BL.Services;

public class DishService : IDishService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public DishService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DishDetailsDto> GetDishDetails(Guid dishId)
    {
        var dishEntity = await _context
            .Dishes
            .Include(dish => dish.Ratings)
            .FirstOrDefaultAsync(dish => dish.Id == dishId) ?? throw new CantFindByIdException("dish", dishId);

        DishDetailsDto dishDto = _mapper.Map<DishDetailsDto>(dishEntity);

        return dishDto;
    }
}