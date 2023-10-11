using Microsoft.AspNetCore.Mvc;
using Serilog;
using SK.TrackYourDay.Expenses.Data.Services;
using SK.TrackYourDay.Expenses.Models.ViewModels;

namespace SK.TrackYourDay.Expenses.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            _logger.LogInformation("Login triggered");

            if (ModelState.IsValid)
            {
                var result = await _accountService.LoginAsync(model);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Login succeded");

                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Invalid login attempt");
            }

            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            _logger.LogInformation("Register triggered");

            if (ModelState.IsValid)
            {
                var result = await _accountService.CreateUserAsync(model);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User with email {model.Email} was registered successfully");
                    return RedirectToAction("Index", "Home");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Logout triggered");

            await _accountService.LogoutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
