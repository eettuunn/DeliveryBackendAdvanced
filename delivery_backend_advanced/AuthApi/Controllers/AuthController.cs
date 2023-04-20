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
    public async Task<ActionResult<TokenPairDto>> RegisterUser([FromBody] RegisterUserDto registerUserDto)
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
    public async Task<ActionResult<TokenPairDto>> LoginUser([FromBody] LoginUserDto loginUserDto)
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
    public async Task<ActionResult<TokenPairDto>> RefreshToken([FromBody] TokenPairDto tokenPairDto)
    {
        if (ModelState.IsValid)
        {
            return Ok(await _authService.RefreshToken(tokenPairDto));
        }
        else
        {
            return BadRequest(ModelState);
        }
    }

    /// <summary>
    /// Logout user
    /// </summary>
    [HttpPost]
    [Authorize]
    [Route("logout")]
    public async Task LogoutUser()
    {
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

        await _authService.LogoutUser(userEmail);
    }
}