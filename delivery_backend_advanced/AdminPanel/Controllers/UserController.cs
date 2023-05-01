using AdminPanel.Interfaces;
using AdminPanel.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> UserList()
    {
        var users = await _userService.GetUsers();
        return View(users);
    }
    
    
    
    public async Task<IActionResult> EditUser(Guid Id)
    {
        var user = await _userService.GetUserInfo(Id);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(EditUser editUser)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        await _userService.EditUser(editUser, ModelState);

        if (!ModelState.IsValid)
        {
            return View();
        }

        return RedirectToAction("UserList");
    }
}