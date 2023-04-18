using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SK.TrackYourDay.Expenses.Data;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using System.Security.Claims;
using SK.TrackYourDay.UseCases.Expenses.Services;
using SK.TrackYourDay.Expenses.Data.Services;

namespace SK.TrackYourDay.Expenses.Controllers
{
    public class ExpensesController : Controller
    {
        IHttpContextAccessor _httpContextAccessor;
        private ExpensesService _expensesService;
        private ExpensesHandler _expensesHandler;
        private PaymentMethodsService _paymentMethodsService;

        public ExpensesController(ExpensesService expensesService, PaymentMethodsService paymentMethodsService,
            IHttpContextAccessor httpContextAccessor, ExpensesHandler expensesHandler)
        {
            _httpContextAccessor = httpContextAccessor;
            _expensesService = expensesService;
            _expensesHandler = expensesHandler;
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
            var ExpenseCategoriesDropDown = _expensesHandler.GetExpenseCategoriesDropDown();
            ViewBag.ExpenseCategoriesDropDown = ExpenseCategoriesDropDown;

            var PaymentMethodsDropDown = _expensesHandler.GetPaymentMethodsDropDown();
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
                    var expenseDTO = ExpensesHandler.ConvertExpenseVMToDto(expense);
                    await _expensesService.AddExpenseAsync(expenseDTO, _userId);
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
        public async Task<IActionResult> Delete(int id)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var expense = await _expensesService.GetExpenseVMByIdAsync(id, _userId);

            return View(expense);
        }

        // POST-Delete
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (id != null)
                await _expensesService.DeleteExpenseByIdAsync(id);

            return RedirectToAction("Index");
        }

        // GET-Update - Creating View
        public async Task<IActionResult> Update(int? id)
        {
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

            var ExpenseCategoriesDropDown = _expensesHandler.GetExpenseCategoriesDropDown();
            ViewBag.ExpenseCategoriesDropDown = ExpenseCategoriesDropDown;

            var PaymentMethodsDropDown = _expensesHandler.GetPaymentMethodsDropDown();
            ViewBag.PaymentMethodsDropDown = PaymentMethodsDropDown;

            return View(expense);
        }

        //POST-Update
        [HttpPatch]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ExpenseVM expense)
        {
            if (ModelState.IsValid)
            {
                var expenseDTO = ExpensesHandler.ConvertExpenseVMToDto(expense);
                await _expensesService.UpdateExpenseById(expense.Id, expenseDTO);
                return RedirectToAction("Index");
            }

            return View(expense);
        }
    }
}
