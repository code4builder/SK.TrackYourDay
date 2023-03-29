using Microsoft.AspNetCore.Mvc;
using SK.TrackYourDay.Expenses.Data.Services;

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

        public IActionResult Register()
        {
            return View();
        }
    }
}
