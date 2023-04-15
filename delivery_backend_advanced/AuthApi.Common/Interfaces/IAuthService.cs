using AuthApi.Common.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Common.Interfaces;

public interface IAuthService
{
    public Task<TokenPairDto> RegisterUser(RegisterUserDto registerUserDto, HttpRequest httpRequest, IUrlHelper urlHelper);
    public Task<TokenPairDto> LoginUser(LoginUserDto loginUserDto);
    public Task<TokenPairDto> RefreshToken(TokenPairDto tokenPairDto);
    public Task ChangePassword(ChangePasswordDto changePasswordDto, string email);
    public Task ForgotPassword(ForgotPasswordDto forgotPassword, HttpRequest request, IUrlHelper urlHelper);
    public Task ChangeForgotPassword(string email, string password);
}