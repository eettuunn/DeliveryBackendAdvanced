﻿using AdminPanel.Interfaces;
using AdminPanel.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers;

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
}