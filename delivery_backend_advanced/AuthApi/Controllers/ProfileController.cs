using System.Security.Claims;
using AuthApi.Common.Dtos;
using AuthApi.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[Route("profile")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    /// <summary>
    /// Change password
    /// </summary>
    [HttpPut]
    [Authorize]
    [Route("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        if (ModelState.IsValid)
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            await _profileService.ChangePassword(changePasswordDto, userEmail);
            return Ok();
        }
        else
        {
            return BadRequest(ModelState);
        }
    }
    
    /// <summary>
    /// Change password if forgot it
    /// </summary>
    [HttpPut]
    [Route("password/forgot")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPassword)
    {
        if (ModelState.IsValid)
        {
            await _profileService.ForgotPassword(forgotPassword, HttpContext.Request, Url);
            return Ok();
        }
        else
        {
            return BadRequest(ModelState);
        }
    }
    
    /// <summary>
    /// Endpoint for link in 'forgot password' email
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ProfileDto> GetProfile()
    {
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        return await _profileService.GetProfile(userEmail);
    }
    
    /// <summary>
    /// Endpoint for link in 'forgot password' email
    /// </summary>
    [HttpGet]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("password/forgot")]
    public async Task ChangeForgotPassword(string email, string password)
    {
        await _profileService.ChangeForgotPassword(email, password);
    }
}