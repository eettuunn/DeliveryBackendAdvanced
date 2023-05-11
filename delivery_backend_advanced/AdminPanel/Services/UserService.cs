using System.Security.AccessControl;
using AdminPanel.Interfaces;
using AdminPanel.Models;
using AuthApi.Common.Enums;
using AuthApi.DAL;
using AuthApi.DAL.Entities;
using AutoMapper;
using delivery_backend_advanced.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace AdminPanel.Services;

public class UserService : IUserService
{
    private readonly AuthDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public UserService(AuthDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
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

    public async Task<UserInfo> GetUserInfo(Guid id)
    {
        var user = await _context
            .Users
            .FirstOrDefaultAsync(user => user.Id == id.ToString()) 
                   ?? throw new CantFindByIdException("user", id);

        var userInfo = _mapper.Map<UserInfo>(user);
        userInfo.roles = await GetUserRoles(userInfo.id);
        
        return userInfo;
    }

    public async Task EditUser(EditUser editUser, ModelStateDictionary modelState)
    {
        var user = await _context
            .Users
            .FirstOrDefaultAsync(user => user.Id == editUser.id.ToString());
        if (!await CheckEditValidation(editUser, modelState))
        {
            return;
        }

        user.Email = editUser.email ?? user.Email;
        user.PhoneNumber = editUser.phoneNumber ?? user.PhoneNumber;
        user.BirthDate = editUser.birthDate.ToUniversalTime();
        user.UserName = editUser.userName ?? user.UserName;
        user.Gender = editUser.gender;

        await ChangeUserRoles(user, editUser.roles);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteUser(Guid id)
    {
        var user = await _userManager
            .Users
            .FirstOrDefaultAsync(u => u.Id == id.ToString());
        var roles = await _userManager.GetRolesAsync(user);
        await DeleteRolesEntities(user);
        
        await _userManager.RemoveFromRolesAsync(user, roles);
        await _userManager.DeleteAsync(user);
    }


    
    
    private async Task<List<Role>> GetUserRoles(Guid userId)
    {
        var roles = await _context
            .Roles
            .ToListAsync();
        var userRolesId = await _context
            .UserRoles
            .Where(role => role.UserId == userId.ToString())
            .ToListAsync();
        
        var userRoles = new List<Role>();
        foreach (var role in roles)
        {
            var userRole = _mapper.Map<Role>(role);
            if (userRolesId.Any(r => r.RoleId == role.Id))
            {
                userRole.selected = true;
            }
            else
            {
                userRole.selected = false;
            }
            
            userRoles.Add(userRole);
        }

        return userRoles;
    }

    private async Task<bool> CheckEditValidation(EditUser editUser, ModelStateDictionary modelState)
    {
        bool alright = true;
        if (await _context.Users.AnyAsync(user => user.Email == editUser.email && user.Id != editUser.id.ToString()))
        {
            modelState.AddModelError(nameof(editUser.email), $"User with email {editUser.email} already exists");
            alright = false;
        }

        if (DateTime.UtcNow.Year - editUser.birthDate.Year is < 7 or > 80)
        {
            modelState.AddModelError(nameof(editUser.birthDate), $"User birthDate must be in range 7 to 80");
            alright = false;
        }

        return alright;
    }

    private async Task ChangeUserRoles(AppUser user, List<Role> roles)
    {
        var prevUserRoles = await _context
            .UserRoles
            .Where(r => r.UserId == user.Id)
            .ToListAsync();
        foreach (var prR in prevUserRoles.ToList())
        {
            var role = roles.FirstOrDefault(r => r.id.ToString() == prR.RoleId);
            if (role.selected)
            {
                roles.Remove(role);
                prevUserRoles.Remove(prR);
            }
        }

        await DeletePrevRoles(prevUserRoles, user);
        
        foreach (var role in roles)
        {
            if (prevUserRoles.All(r => r.RoleId != role.id.ToString()) && role.selected)
            {
                switch (role.name)
                {
                    case UserRole.Customer:
                        var customer = new CustomerEntity()
                        {
                            Id = new Guid(),
                            User = user
                        };
                        await _context.Customers.AddAsync(customer);
                        break;
                    case UserRole.Cook:
                        var cook = new CookEntity()
                        {
                            Id = new Guid(),
                            User = user
                        };
                        await _context.Cooks.AddAsync(cook);
                        break;
                    case UserRole.Courier:
                        var courier = new CourierEntity()
                        {
                            Id = new Guid(),
                            User = user
                        };
                        await _context.Couriers.AddAsync(courier);
                        break;
                    case UserRole.Manager:
                        var manager = new ManagerEntity()
                        {
                            Id = new Guid(),
                            User = user
                        };
                        await _context.Managers.AddAsync(manager);
                        break;
                }

                await _userManager.AddToRoleAsync(user, role.name.ToString());
            }
        }
    }

    private async Task DeletePrevRoles(List<IdentityUserRole<string>> prevRoles, AppUser user)
    {
        foreach (var role in prevRoles)
        {
            var appRole = await _context.Roles.FirstOrDefaultAsync(ar => ar.Id == role.RoleId);
            var roleEnum = Enum.Parse(typeof(UserRole), appRole.Name);
            switch (roleEnum)
            {
                case UserRole.Cook:
                    var cook = await _context
                        .Cooks
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(cook => cook.User.Id == user.Id);
                    
                    await _userManager.RemoveFromRoleAsync(user, UserRole.Cook.ToString());
                    _context.Cooks.Remove(cook);
                    await _context.SaveChangesAsync();
                    break;
                case UserRole.Customer:
                    var customer = await _context
                        .Customers
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(c => c.User.Id == user.Id);
                    
                    await _userManager.RemoveFromRoleAsync(user, UserRole.Customer.ToString());
                    _context.Customers.Remove(customer);
                    await _context.SaveChangesAsync();
                    break;
                case UserRole.Courier:
                    var courier = await _context
                        .Couriers
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(c => c.User.Id == user.Id);
                    
                    await _userManager.RemoveFromRoleAsync(user, UserRole.Courier.ToString());
                    _context.Couriers.Remove(courier);
                    await _context.SaveChangesAsync();
                    break;
                case UserRole.Manager:
                    var manager = await _context
                        .Managers
                        .Include(m => m.User)
                        .FirstOrDefaultAsync(m => m.User.Id == user.Id);
                    
                    await _userManager.RemoveFromRoleAsync(user, UserRole.Manager.ToString());
                    _context.Managers.Remove(manager);
                    await _context.SaveChangesAsync();
                    break;
            }
        }
    }

    private async Task DeleteRolesEntities(AppUser user)
    {
        if (user.Manager != null)
        {
            var manager = await _context
                .Managers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.User == user);
            _context.Managers.Remove(manager);
        }
        if (user.Courier != null)
        {
            var courier = await _context
                .Couriers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User == user);
            _context.Couriers.Remove(courier);
        }
        if (user.Customer != null)
        {
            var customer = await _context
                .Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User == user);
            _context.Customers.Remove(customer);
        }
        if (user.Cook != null)
        {
            var cook = await _context
                .Cooks
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User == user);
            _context.Cooks.Remove(cook);
        }

        await _context.SaveChangesAsync();
    }
}