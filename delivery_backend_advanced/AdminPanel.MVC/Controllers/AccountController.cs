using AuthApi.Common.Dtos;
using AuthApi.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers;

public class AccountController : Controller
{
    //добавил ссылку на AuthApi.BL, так как все же лучше использовать логин уже реализованный, а не новый копипастить в админку
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AccountController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
    }

    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginUserDto loginCredentials)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }

        var tokenPair = new TokenPairDto();
        try
        {
            tokenPair = await _authService.LoginUser(loginCredentials);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(nameof(loginCredentials.password), "Invalid credentials");
        }

        if (ModelState.IsValid)
        {
            //была выбрана стратегия добавления токена в кукис
            var cookieOptions = new CookieOptions();
            var expires = _configuration.GetSection("JwtConfig:AccessMinutesLifeTime").Get<int>();
            cookieOptions.Expires =
                DateTime.UtcNow.AddMinutes(expires);
            cookieOptions.Path = "/";
            Response.Cookies.Append("access_token", tokenPair.accessToken, cookieOptions);

            return RedirectToAction("StartPage", "Home");
        }
        return View();
    }
}