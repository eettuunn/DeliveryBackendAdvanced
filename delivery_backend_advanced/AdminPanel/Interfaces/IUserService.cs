using AdminPanel.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AdminPanel.Interfaces;

public interface IUserService
{
    public Task<List<UserListElement>> GetUsers();
    public Task<UserInfo> GetUserInfo(Guid id);
    public Task EditUser(EditUser editUser, ModelStateDictionary modelState);
}