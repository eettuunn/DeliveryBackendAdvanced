using AuthApi.Common.Dtos;

namespace AuthApi.Common.Interfaces;

public interface IAuthService
{
    public Task<TokenPairDto> RegisterUser(RegisterUserDto registerUserDto);
    public Task<TokenPairDto> LoginUser(LoginUserDto loginUserDto);
}