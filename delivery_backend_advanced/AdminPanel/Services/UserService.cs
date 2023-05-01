using System.Security.AccessControl;
using AdminPanel.Interfaces;
using AdminPanel.Models;
using AuthApi.Common.Enums;
using AuthApi.DAL;
using AuthApi.DAL.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Services;

public class UserService : IUserService
{
    private readonly AuthDbContext _context;
    private readonly IMapper _mapper;

    public UserService(AuthDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<UserListElement>> GetUsers()
    {
        var users = await _context
            .Users
            .ToListAsync();

        var usersDisplay = _mapper.Map<List<UserListElement>>(users);
        foreach (var user in usersDisplay)
        {
            user.roles = await GetUserRoles(user.id);
        }
        
        return usersDisplay;
    }



    private async Task<List<Role>> GetUserRoles(Guid userId)
    {
        var rolesId = await _context
            .UserRoles
            .Where(role => role.UserId == userId.ToString())
            .ToListAsync();
        
        var roles = new List<Role>();
        foreach (var roleId in rolesId)
        {
            var identityRole = await _context
                .Roles
                .FirstOrDefaultAsync(r => r.Id == roleId.RoleId);
            var role = _mapper.Map<Role>(identityRole);
            
            roles.Add(role);
        }

        return roles;
    }
}