using System.Security.AccessControl;
using AdminPanel.Interfaces;
using AdminPanel.Models;
using AuthApi.Common.Enums;
using AuthApi.DAL;
using AuthApi.DAL.Entities;
using AutoMapper;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace AdminPanel.Services;

public class UserService : IUserService
{
    private readonly AuthDbContext _authDbContext;
    private readonly BackendDbContext _backendDbContext;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public UserService(AuthDbContext authDbContext, IMapper mapper, UserManager<AppUser> userManager, BackendDbContext backendDbContext)
    {
        _authDbContext = authDbContext;
        _mapper = mapper;
        _userManager = userManager;
        _backendDbContext = backendDbContext;
    }

    public async Task<List<UserListElement>> GetUsers()
    {
        var users = await _authDbContext
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
        var user = await _authDbContext
            .Users
            .FirstOrDefaultAsync(user => user.Id == id.ToString()) 
                   ?? throw new CantFindByIdException("user", id);

        var userInfo = _mapper.Map<UserInfo>(user);
        userInfo.roles = await GetUserRoles(userInfo.id);

        var customer = await _backendDbContext
            .Customers
            .FirstOrDefaultAsync(c => c.Id == id);
        var cook = await _backendDbContext
            .Cooks
            .Include(c => c.Restaurant)
            .FirstOrDefaultAsync(c => c.Id == id);
        var manager = await _backendDbContext
            .Managers
            .Include(m => m.Restaurant)
            .FirstOrDefaultAsync(m => m.Id == id);
        userInfo.address = customer.Address;
        if (manager != null)
        {
            userInfo.restaurantId = manager.Restaurant.Id;
        }
        else
        {
            if (cook != null)
            {
                userInfo.restaurantId = cook.Restaurant.Id;
            }
            else
            {
                userInfo.restaurantId = null;
            }
        }
        
        return userInfo;
    }

    public async Task EditUser(EditUser editUser, ModelStateDictionary modelState)
    {
        var user = await _authDbContext
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

        await ChangeUserRoles(user, editUser.roles, editUser.address);
        
        if (editUser.restaurantId != null)
        {
            await AddRestaurantsToRoles(editUser);
        }

        await _authDbContext.SaveChangesAsync();
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
        var roles = await _authDbContext
            .Roles
            .ToListAsync();
        var userRolesId = await _authDbContext
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
        if (await _authDbContext.Users.AnyAsync(user => user.Email == editUser.email && user.Id != editUser.id.ToString()))
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

    private async Task ChangeUserRoles(AppUser user, List<Role> roles, string address)
    {
        var prevUserRoles = await _authDbContext
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
                            User = user,
                            Address = address
                        };
                        await _authDbContext.Customers.AddAsync(customer);
                        break;
                    
                    case UserRole.Cook:
                        var cook = new CookEntity()
                        {
                            Id = new Guid(),
                            User = user
                        };
                        await _authDbContext.Cooks.AddAsync(cook);
                        break;
                    
                    case UserRole.Courier:
                        var courier = new CourierEntity()
                        {
                            Id = new Guid(),
                            User = user
                        };
                        await _authDbContext.Couriers.AddAsync(courier);
                        break;
                    
                    case UserRole.Manager:
                        var manager = new ManagerEntity()
                        {
                            Id = new Guid(),
                            User = user
                        };
                        await _authDbContext.Managers.AddAsync(manager);
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
            var appRole = await _authDbContext.Roles.FirstOrDefaultAsync(ar => ar.Id == role.RoleId);
            var roleEnum = Enum.Parse(typeof(UserRole), appRole.Name);
            switch (roleEnum)
            {
                case UserRole.Cook:
                    var cook = await _authDbContext
                        .Cooks
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(cook => cook.User.Id == user.Id);
                    await _userManager.RemoveFromRoleAsync(user, UserRole.Cook.ToString());
                    _authDbContext.Cooks.Remove(cook);
                    await _authDbContext.SaveChangesAsync();
                    break;
                
                case UserRole.Customer:
                    var customer = await _authDbContext
                        .Customers
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(c => c.User.Id == user.Id);
                    await _userManager.RemoveFromRoleAsync(user, UserRole.Customer.ToString());
                    _authDbContext.Customers.Remove(customer);
                    await _authDbContext.SaveChangesAsync();
                    break;
                
                case UserRole.Courier:
                    var courier = await _authDbContext
                        .Couriers
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(c => c.User.Id == user.Id);
                    await _userManager.RemoveFromRoleAsync(user, UserRole.Courier.ToString());
                    _authDbContext.Couriers.Remove(courier);
                    await _authDbContext.SaveChangesAsync();
                    break;
                
                case UserRole.Manager:
                    var manager = await _authDbContext
                        .Managers
                        .Include(m => m.User)
                        .FirstOrDefaultAsync(m => m.User.Id == user.Id);
                    
                    await _userManager.RemoveFromRoleAsync(user, UserRole.Manager.ToString());
                    _authDbContext.Managers.Remove(manager);
                    await _authDbContext.SaveChangesAsync();
                    break;
            }
        }
    }

    private async Task DeleteRolesEntities(AppUser user)
    {
        if (user.Manager != null)
        {
            var manager = await _authDbContext
                .Managers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.User == user);
            _authDbContext.Managers.Remove(manager);
        }
        if (user.Courier != null)
        {
            var courier = await _authDbContext
                .Couriers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User == user);
            _authDbContext.Couriers.Remove(courier);
        }
        if (user.Customer != null)
        {
            var customer = await _authDbContext
                .Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User == user);
            _authDbContext.Customers.Remove(customer);
        }
        if (user.Cook != null)
        {
            var cook = await _authDbContext
                .Cooks
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User == user);
            _authDbContext.Cooks.Remove(cook);
        }

        await _authDbContext.SaveChangesAsync();
    }

    private async Task AddRestaurantsToRoles(EditUser editUser)
    {
        var restaurant = await _backendDbContext
            .Restaurants
            .FirstOrDefaultAsync(r => r.Id == editUser.restaurantId);
        if (editUser.roles.Any(r => r.name == UserRole.Manager))
        {
            var manager = await _backendDbContext
                .Managers
                .FirstOrDefaultAsync(m => m.Id == editUser.id);
            manager.Restaurant = restaurant;
        }
        if (editUser.roles.Any(r => r.name == UserRole.Cook))
        {
            var cook = await _backendDbContext
                .Cooks
                .FirstOrDefaultAsync(c => c.Id == editUser.id);
            cook.Restaurant = restaurant;
        }

        await _backendDbContext.SaveChangesAsync();
    }
}