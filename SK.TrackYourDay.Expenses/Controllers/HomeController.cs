using Microsoft.AspNetCore.Mvc;
using SK.TrackYourDay.Domain.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace SK.TrackYourDay.Expenses.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var _user = HttpContext.User.Identity.Name;

            if (_user == null)
                return RedirectToAction("Login", "Account");
            else 
                return RedirectToAction("Index", "Expenses");
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
}