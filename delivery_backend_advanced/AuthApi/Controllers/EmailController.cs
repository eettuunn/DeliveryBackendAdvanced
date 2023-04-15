using System.Security.Claims;
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

    [HttpGet]
    public async Task ConfirmEmail(string email, string code)
    {
        await _emailService.ConfirmEmail(email, code);
    }

    [HttpPost]
    [Authorize]
    [Route("confirm")]
    public async Task ConfirmEmailAfterRegistration()
    {
        var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        await _emailService.SendConfirmationEmail(HttpContext.Request, Url, email);
    }
}