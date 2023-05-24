using AdminPanel._Common.Models.User;
using AdminPanel.Interfaces;
using AutoMapper;
using delivery_backend_advanced.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers;

[Authorize]
[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
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
            var userInfo = _mapper.Map<UserInfo>(editUser);
            return View(userInfo);
        }

        await _userService.EditUser(editUser, ModelState);

        if (!ModelState.IsValid)
        {
            var userInfo = _mapper.Map<UserInfo>(editUser);
            return View(userInfo);
        }

        return RedirectToAction("UserList");
    }

    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _userService.DeleteUser(id);
        return RedirectToAction("UserList");
    }
}