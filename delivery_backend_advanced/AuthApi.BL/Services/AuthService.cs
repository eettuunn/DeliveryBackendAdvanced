using AuthApi.Common.Dtos;
using AuthApi.Common.Enums;
using AuthApi.Common.Interfaces;
using AuthApi.DAL;
using AuthApi.DAL.Entities;
using AutoMapper;
using delivery_backend_advanced.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.BL.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AuthDbContext _context;
    private readonly IMapper _mapper;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthService(AuthDbContext context, UserManager<AppUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;
    }

    public async Task RegisterUser(RegisterUserDto registerUserDto)
    {
        var newUser = _mapper.Map<AppUser>(registerUserDto);
        var result = await _userManager.CreateAsync(newUser, registerUserDto.password);

        if (!result.Succeeded)
        {
            var errorsStrings = result.Errors.Select(e => e.Description.ToString()).ToList();
            throw new AuthErrorsException(errorsStrings);
        }

        var findUser = await _context
            .Users
            .FirstOrDefaultAsync(user => user.Email == registerUserDto.email);
        if (findUser == null)
        {
            throw new NotFoundException($"User with email {registerUserDto.email} not found");
        }

        if (!await _roleManager.RoleExistsAsync(UserRole.Customer.ToString()))
        {
            var role = new IdentityRole();
            role.Name = UserRole.Customer.ToString();
            await _roleManager.CreateAsync(role);
        }
        await _userManager.AddToRoleAsync(findUser, UserRole.Customer.ToString());
    }
}