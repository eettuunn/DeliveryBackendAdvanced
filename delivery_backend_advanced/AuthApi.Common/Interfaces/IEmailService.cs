using AuthApi.Common.Dtos;

namespace AuthApi.Common.Interfaces;

public interface IEmailService
{
    public Task SendEmailAsync(SendEmailDto emailDto);
}