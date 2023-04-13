using AuthApi.Common.Dtos;
using AuthApi.Common.Interfaces;
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

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
    {
        if (ModelState.IsValid)
        {
            var tokenPair = await _authService.RegisterUser(registerUserDto);
            return Ok(tokenPair);
        }
        else
        {
            return BadRequest(ModelState);
        }
    }
    
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
}