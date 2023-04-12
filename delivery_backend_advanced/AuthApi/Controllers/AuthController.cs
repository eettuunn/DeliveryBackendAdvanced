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
            await _authService.RegisterUser(registerUserDto);
            return Ok();
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
            await _authService.LoginUser(loginUserDto);
            return Ok();
        }
        else
        {
            return BadRequest(ModelState);
        }
    }
}