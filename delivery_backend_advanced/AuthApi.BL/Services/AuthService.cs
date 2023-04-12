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
                           .FirstOrDefaultAsync(user => user.Email == registerUserDto.email) ??
                       throw new NotFoundException($"User with email {registerUserDto.email} not found");

        await _userManager.AddToRoleAsync(findUser, UserRole.Customer.ToString());
        await _context.SaveChangesAsync();
    }

    public async Task LoginUser(LoginUserDto loginUserDto)
    {
        var findUser = await _userManager
            .FindByEmailAsync(loginUserDto.email) ?? throw new BadRequestException("Invalid credentials");

        var isPasswordValid = await _userManager.CheckPasswordAsync(findUser, loginUserDto.password);
        if (!isPasswordValid)
        {
            throw new BadRequestException("Invalid credentials");
        }

        var user = await _context
                       .Users
                       .FirstOrDefaultAsync(user => user.Email == loginUserDto.email) ??
                   throw new BadRequestException($"Can't find user with email {loginUserDto.email}");
        
        
    }
}