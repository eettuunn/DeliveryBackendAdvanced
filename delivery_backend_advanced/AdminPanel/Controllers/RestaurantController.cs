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
    private readonly IRestaurantService _restaurantService;

    public RestaurantController(IRestaurantService restaurantService)
    {
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

    public async Task<ActionResult<RestaurantListElement>> RestaurantList()
    {
        var rests = await _restaurantService.GetRestaurantList();

        return View(rests);
    }
}