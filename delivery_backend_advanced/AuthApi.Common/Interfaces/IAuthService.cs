using AuthApi.Common.Dtos;

namespace AuthApi.Common.Interfaces;

public interface IAuthService
{
    public Task RegisterUser(RegisterUserDto registerUserDto);
    public Task LoginUser(LoginUserDto loginUserDto);
}