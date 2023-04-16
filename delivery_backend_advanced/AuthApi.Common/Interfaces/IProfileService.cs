using AuthApi.Common.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Common.Interfaces;

public interface IProfileService
{
    public Task ChangePassword(ChangePasswordDto changePasswordDto, string email);
    public Task ForgotPassword(ForgotPasswordDto forgotPassword, HttpRequest request, IUrlHelper urlHelper);
    public Task ChangeForgotPassword(string email, string password);
    public Task<ProfileDto> GetProfile(string email);
}