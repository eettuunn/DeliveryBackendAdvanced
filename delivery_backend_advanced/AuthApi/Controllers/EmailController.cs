using System.Security.Claims;
using AuthApi.Common.Dtos;
using AuthApi.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[Route("email")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    /// <summary>
    /// Endpoint for link in 'email confirm' email
    /// </summary>
    [HttpGet]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task ConfirmEmail(string email, string code)
    {
        await _emailService.ConfirmEmail(email, code);
    }

    /// <summary>
    /// Send confirm email after registration
    /// </summary>
    [HttpPost]
    [Authorize]
    [Route("confirm")]
    public async Task ConfirmEmailAfterRegistration()
    {
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        var emailDto = new SendEmailDto()
        {
            email = userEmail,
            subject = "Confirm email",
            message = "Для подтверждения почты перейдите по ссылке: "
        };
        await _emailService.SendConfirmationEmail(HttpContext.Request, Url, emailDto);
    }
}