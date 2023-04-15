using AuthApi.Common.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Common.Interfaces;

public interface IEmailService
{
    public Task SendEmailAsync(SendEmailDto emailDto);
    public Task ConfirmEmail(string email, string code);
    public Task SendConfirmationEmail(HttpRequest request, IUrlHelper urlHelper, string email);
}