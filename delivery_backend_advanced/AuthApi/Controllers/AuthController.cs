using System.Security.Claims;
using AuthApi.Common.Dtos;
using AuthApi.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register user
    /// </summary>
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
    {
        if (ModelState.IsValid)
        {
            var tokenPair = await _authService.RegisterUser(registerUserDto, HttpContext.Request, Url);
            return Ok(tokenPair);
        }
        else
        {
            return BadRequest(ModelState);
        }
    }
    
    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDto loginUserDto)
    {
        if (ModelState.IsValid)
        {
            var tokePair = await _authService.LoginUser(loginUserDto);
            return Ok(tokePair);
        }
        else
        {
            return BadRequest(ModelState);
        }
    }

    /// <summary>
    /// Refresh token
    /// </summary>
    [HttpPost]
    [Route("refresh")]
    public async Task<TokenPairDto> RefreshToken([FromBody] TokenPairDto tokenPairDto)
    {
        return await _authService.RefreshToken(tokenPairDto);
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
            await _authService.ChangePassword(changePasswordDto, userEmail);
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
            await _authService.ForgotPassword(forgotPassword, HttpContext.Request, Url);
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
    [Route("profile")]
    public async Task<ProfileDto> GetProfile()
    {
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        return await _authService.GetProfile(userEmail);
    }
    
    /// <summary>
    /// Endpoint for link in 'forgot password' email
    /// </summary>
    [HttpGet]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("password/forgot")]
    public async Task ChangeForgotPassword(string email, string password)
    {
        await _authService.ChangeForgotPassword(email, password);
    }
}