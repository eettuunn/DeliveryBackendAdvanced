using AuthApi.Common.Interfaces;
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
    [Route("email/confirm")]
    public async Task ConfirmEmail(Guid userId, string code)
    {
        await _emailService.ConfirmEmail(userId, code);
    }
}