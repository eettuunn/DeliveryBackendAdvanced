using AdminPanel.Models;

namespace AdminPanel.Interfaces;

public interface IUserService
{
    public Task<List<UserListElement>> GetUsers();
}