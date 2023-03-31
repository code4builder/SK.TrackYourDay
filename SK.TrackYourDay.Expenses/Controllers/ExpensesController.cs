using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SK.TrackYourDay.Expenses.Data;
using SK.TrackYourDay.Expenses.Data.Services;
using SK.TrackYourDay.Expenses.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using System.Security.Claims;

namespace SK.TrackYourDay.Expenses.Controllers
{
    public class ExpensesController : Controller
    {
        IHttpContextAccessor _httpContextAccessor;
        private ExpensesService _expensesService;
        private PaymentMethodsService _paymentMethodsService;
        private readonly string _role;
        public ExpensesController(ExpensesService expensesService, PaymentMethodsService paymentMethodsService, 
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _expensesService = expensesService;
            _paymentMethodsService = paymentMethodsService;
            _role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value;
        }

        [HttpGet]
        public IActionResult Index(string sortBy, string searchString, int pageNumber)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var objList = _expensesService.GetAllExpensesVM(_userId, _role, sortBy, searchString, pageNumber);
            return View(objList);
        }

        //GET-Create - Creating View
        public IActionResult Create()
        {
            var ExpenseCategoriesDropDown = _expensesService.GetExpenseCategoriesDropDown();
            ViewBag.ExpenseCategoriesDropDown = ExpenseCategoriesDropDown;

            var PaymentMethodsDropDown = _expensesService.GetPaymentMethodsDropDown();
            ViewBag.PaymentMethodsDropDown = PaymentMethodsDropDown;

            return View();
        }

        //POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ExpenseVM expense)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (ModelState.IsValid)
            {
                _expensesService.AddExpense(expense, _userId);
                return RedirectToAction("Index");
            }

            return View(expense);
        }

        // GET-Delete - Creating View
        public IActionResult Delete(int? id)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                var expense = _expensesService.GetExpenseVMById((int)id, _userId);
                return View(expense);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST-Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if(id != null)
                _expensesService.DeleteExpenseById((int)id);

            return RedirectToAction("Index");
        }

        // GET-Update - Creating View
        public IActionResult Update(int? id)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var expense = _expensesService.GetExpenseVMById((int)id, _userId);
            if (expense == null)
            {
                return NotFound();
            }

            var ExpenseCategoriesDropDown = _expensesService.GetExpenseCategoriesDropDown();
            ViewBag.ExpenseCategoriesDropDown = ExpenseCategoriesDropDown;

            var PaymentMethodsDropDown = _expensesService.GetPaymentMethodsDropDown();
            ViewBag.PaymentMethodsDropDown = PaymentMethodsDropDown;

            return View(expense);
        }

        //POST-Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ExpenseVM expense)
        {
            if (ModelState.IsValid)
            {
                _expensesService.UpdateExpenseById(expense.Id, expense);
                return RedirectToAction("Index");
            }

            return View(expense);
        }
    }
}
