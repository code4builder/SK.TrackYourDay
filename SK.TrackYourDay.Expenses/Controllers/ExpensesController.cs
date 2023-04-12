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

        public ExpensesController(ExpensesService expensesService, PaymentMethodsService paymentMethodsService,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _expensesService = expensesService;
            _paymentMethodsService = paymentMethodsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string sortBy, string searchString, int pageNumber, int pageSize)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var _role = HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            var objList = await _expensesService.GetAllExpensesVMAsync(_userId, _role, sortBy, searchString, pageNumber, pageSize);

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
        public async Task<IActionResult> Create(ExpenseVM expense)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                if (ModelState.IsValid)
                {
                    await _expensesService.AddExpenseAsync(expense, _userId);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                throw new Exception();
                //return BadRequest();
            }

            return View(expense);
        }

        // GET-Delete - Creating View
        public async Task<IActionResult> Delete(int? id)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var expense = await _expensesService.GetExpenseVMByIdAsync((int)id, _userId);

            return View(expense);
        }

        // POST-Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id != null)
                await _expensesService.DeleteExpenseByIdAsync((int)id);

            return RedirectToAction("Index");
        }

        // GET-Update - Creating View
        public async Task<IActionResult> Update(int? id)
        {
            throw new Exception("This is an exception that will be handled by middleware");

            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var expense = await _expensesService.GetExpenseVMByIdAsync((int)id, _userId);
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
        public async Task<IActionResult> Update(ExpenseVM expense)
        {
            if (ModelState.IsValid)
            {
                await _expensesService.UpdateExpenseById(expense.Id, expense);
                return RedirectToAction("Index");
            }

            return View(expense);
        }
    }
}
