using AdminPanel.Models;
using AuthApi.DAL;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Entities;
using AdminPanel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Controllers;

public class RestaurantController : Controller
{
    private readonly BackendDbContext _context;
    private readonly IRestaurantService _restaurantService;

    public RestaurantController(BackendDbContext context, IRestaurantService restaurantService)
    {
        _context = context;
        _restaurantService = restaurantService;
    }

    public IActionResult Index()
    {
        ViewBag.hi = "sssss";
        return View();
    }

    public IActionResult CreateRest()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateRest(CreateRest rest)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }

        await _restaurantService.CreateRestaurant(rest, ModelState);

        if(!ModelState.IsValid)
        {
            return View(rest);
        }
        
        return RedirectToAction("Index");
    }
}