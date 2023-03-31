using Microsoft.AspNetCore.Mvc;
using SK.TrackYourDay.Expenses.Data.Services;
using SK.TrackYourDay.Expenses.Models.ViewModels;

namespace SK.TrackYourDay.Expenses.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.LoginAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Expenses");
                }
                ModelState.AddModelError("", "Invalid login attempt");
            }

            return View(model);
        }

        public async Task<IActionResult> Register()
        {
            await _accountService.CreateRolesAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
               var result = await _accountService.CreateUserAsync(model);

                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");

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
            await _accountService.LogoutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
