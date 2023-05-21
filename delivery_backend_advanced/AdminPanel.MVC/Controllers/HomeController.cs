using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers;

public class HomeController : Controller
{
    public IActionResult StartPage()
    {
        return View();
    }
}