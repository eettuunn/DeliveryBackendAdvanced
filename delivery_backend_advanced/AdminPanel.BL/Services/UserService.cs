using AdminPanel._Common.Models.User;
using AdminPanel.Interfaces;
using AuthApi.Common.Enums;
using AuthApi.DAL;
using AuthApi.DAL.Entities;
using AutoMapper;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        userInfo.restaurantIds = await GetRestaurants(userInfo.id);
        var selectedRest = userInfo.restaurantIds.FirstOrDefault(r => r.Selected);
        userInfo.selectedRestaurantValue = selectedRest?.Value;

        var customer = await _backendDbContext
            .Customers
            .FirstOrDefaultAsync(c => c.Id == id);
        userInfo.address = customer.Address;

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

        var rolesClone = new List<Role>(editUser.roles);
        await ChangeUserRoles(user, rolesClone, editUser.address);
        
        if (editUser.roles.Any(r => r.name is UserRole.Cook or UserRole.Manager))
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
                        var customerAuth = new CustomerEntity()
                        {
                            Id = new Guid(),
                            User = user,
                            Address = address
                        };
                        await _authDbContext.Customers.AddAsync(customerAuth);
                        var customerBack = new Customer()
                        {
                            Id = Guid.Parse(user.Id),
                            Address = address
                        };
                        await _backendDbContext.Customers.AddAsync(customerBack);
                        break;
                    
                    case UserRole.Cook:
                        var cookAuth = new CookEntity()
                        {
                            Id = new Guid(),
                            User = user
                        };
                        await _authDbContext.Cooks.AddAsync(cookAuth);
                        var cookBack = new Cook()
                        {
                            Id = Guid.Parse(user.Id),
                        };
                        await _backendDbContext.Cooks.AddAsync(cookBack);
                        break;
                    
                    case UserRole.Courier:
                        var courierAuth = new CourierEntity()
                        {
                            Id = new Guid(),
                            User = user
                        };
                        await _authDbContext.Couriers.AddAsync(courierAuth);
                        var courierBack = new Courier()
                        {
                            Id = Guid.Parse(user.Id),
                        };
                        await _backendDbContext.Couriers.AddAsync(courierBack);
                        break;
                    
                    case UserRole.Manager:
                        var managerAuth = new ManagerEntity()
                        {
                            Id = new Guid(),
                            User = user
                        };
                        await _authDbContext.Managers.AddAsync(managerAuth);
                        var managerBack = new Manager()
                        {
                            Id = Guid.Parse(user.Id),
                        };
                        await _backendDbContext.Managers.AddAsync(managerBack);
                        break;
                }

                await _authDbContext.SaveChangesAsync();
                await _backendDbContext.SaveChangesAsync();
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
                    var cookAuth = await _authDbContext
                        .Cooks
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(cook => cook.User.Id == user.Id);
                    var cookBack = await _backendDbContext
                        .Cooks
                        .FirstOrDefaultAsync(cook => cook.Id.ToString() == user.Id);
                    _backendDbContext.Cooks.Remove(cookBack);
                    await _userManager.RemoveFromRoleAsync(user, UserRole.Cook.ToString());
                    _authDbContext.Cooks.Remove(cookAuth);
                    break;
                
                case UserRole.Customer:
                    var customerAuth = await _authDbContext
                        .Customers
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(c => c.User.Id == user.Id);
                    var customerBack = await _backendDbContext
                        .Customers
                        .FirstOrDefaultAsync(customer => customer.Id.ToString() == user.Id);
                    _backendDbContext.Customers.Remove(customerBack);
                    await _userManager.RemoveFromRoleAsync(user, UserRole.Customer.ToString());
                    _authDbContext.Customers.Remove(customerAuth);
                    break;
                
                case UserRole.Courier:
                    var courierAuth = await _authDbContext
                        .Couriers
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(c => c.User.Id == user.Id);
                    var courierBack = await _backendDbContext
                        .Couriers
                        .FirstOrDefaultAsync(courier => courier.Id.ToString() == user.Id);
                    _backendDbContext.Couriers.Remove(courierBack);
                    await _userManager.RemoveFromRoleAsync(user, UserRole.Courier.ToString());
                    _authDbContext.Couriers.Remove(courierAuth);
                    break;
                
                case UserRole.Manager:
                    var managerAuth = await _authDbContext
                        .Managers
                        .Include(m => m.User)
                        .FirstOrDefaultAsync(m => m.User.Id == user.Id);
                    var managerBack = await _backendDbContext
                        .Managers
                        .FirstOrDefaultAsync(manager => manager.Id.ToString() == user.Id);
                    _backendDbContext.Managers.Remove(managerBack);
                    await _userManager.RemoveFromRoleAsync(user, UserRole.Manager.ToString());
                    _authDbContext.Managers.Remove(managerAuth);
                    break;
            }
            
            await _backendDbContext.SaveChangesAsync();
            await _authDbContext.SaveChangesAsync();
        }
    }

    private async Task DeleteRolesEntities(AppUser user)
    {
        if (user.Manager != null)
        {
            var managerAuth = await _authDbContext
                .Managers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.User == user);
            _authDbContext.Managers.Remove(managerAuth);
            var managerBack = await _backendDbContext
                .Managers
                .FirstOrDefaultAsync(m => m.Id.ToString() == user.Id);
            _backendDbContext.Managers.Remove(managerBack);
        }
        if (user.Courier != null)
        {
            var courierAuth = await _authDbContext
                .Couriers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User == user);
            _authDbContext.Couriers.Remove(courierAuth);
            var courierBack = await _backendDbContext
                .Couriers
                .FirstOrDefaultAsync(c => c.Id.ToString() == user.Id);
            _backendDbContext.Couriers.Remove(courierBack);
        }
        if (user.Customer != null)
        {
            var customerAuth = await _authDbContext
                .Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User == user);
            _authDbContext.Customers.Remove(customerAuth);
            var customerBack = await _backendDbContext
                .Customers
                .FirstOrDefaultAsync(c => c.Id.ToString() == user.Id);
            _backendDbContext.Customers.Remove(customerBack);
        }
        if (user.Cook != null)
        {
            var cookAuth = await _authDbContext
                .Cooks
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User == user);
            _authDbContext.Cooks.Remove(cookAuth);
            var cookBack = await _backendDbContext
                .Cooks
                .FirstOrDefaultAsync(c => c.Id.ToString() == user.Id);
            _backendDbContext.Cooks.Remove(cookBack);
        }

        await _authDbContext.SaveChangesAsync();
    }

    private async Task AddRestaurantsToRoles(EditUser editUser)
    {
        var restaurant = await _backendDbContext
            .Restaurants
            .FirstOrDefaultAsync(r => r.Id.ToString() == editUser.selectedRestaurantValue);
        if (editUser.roles.Any(r => r.name == UserRole.Manager && r.selected))
        {
            var manager = await _backendDbContext
                .Managers
                .FirstOrDefaultAsync(m => m.Id == editUser.id);
            manager.Restaurant = restaurant;
        }
        if (editUser.roles.Any(r => r.name == UserRole.Cook && r.selected))
        {
            var cook = await _backendDbContext
                .Cooks
                .FirstOrDefaultAsync(c => c.Id == editUser.id);
            cook.Restaurant = restaurant;
        }
        await _backendDbContext.SaveChangesAsync();
    }

    private async Task<List<SelectListItem>> GetRestaurants(Guid userId)
    {
        var restaurantEntities = await _backendDbContext
            .Restaurants
            .Include(r => r.Managers)
            .Include(r => r.Cooks)
            .ToListAsync();

        List<SelectListItem> rests = new List<SelectListItem>();
        foreach (var rest in restaurantEntities)
        {
            var newRest = new SelectListItem()
            {
                Value = rest.Id.ToString(),
                Text = rest.Name,
                Selected = rest.Managers.Any(m => m.Id == userId) || rest.Cooks.Any(c => c.Id == userId)
            };
            rests.Add(newRest);
        }

        return rests;
    }
}