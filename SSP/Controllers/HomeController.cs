using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SSP.Models;

namespace SSP.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // Not logged in: show public homepage
        if (!User.Identity.IsAuthenticated)
        {
            return View("Index");
        }

        // Logged in: check if the user is admin or student
        var email = User.FindFirstValue(ClaimTypes.Email);

        // If the user is an admin, show the default homepage
        if (email == "admin@example.com") // Replace with actual admin email
        {
            return View("Index"); // Admin stays on the default homepage
        }

        // Students should still be able to access the public homepage if they want
        // Do not redirect them automatically
        return View("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
