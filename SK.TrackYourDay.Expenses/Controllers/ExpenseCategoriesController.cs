using Microsoft.AspNetCore.Mvc;
using SK.TrackYourDay.Expenses.Data;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using SK.TrackYourDay.UseCases.Expenses.Services;

namespace SK.TrackYourDay.Expenses.Controllers
{
    public class ExpenseCategoriesController : Controller
    {
        private ExpenseCategoriesService _expenseCategoriesService;
        public ExpenseCategoriesController(ExpenseCategoriesService expenseCategoriesService)
        {
            _expenseCategoriesService = expenseCategoriesService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var objList = _expenseCategoriesService.GetAllExpenseCategories();
            return View(objList);
        }

        //GET-Create - Creating View
        public IActionResult Create()
        {
            return View();
        }

        //POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ExpenseCategory expenseCategory)
        {
            if (ModelState.IsValid)
            {
                _expenseCategoriesService.CreateExpenseCategory(expenseCategory);
                return RedirectToAction("Index");
            }

            return View(expenseCategory);
        }

        // GET-Delete - Creating View
        public IActionResult Delete(int? id)
        {
            try
            {
                var expenseCategory = _expenseCategoriesService.GetExpenseCategoryById((int)id);
                return View(expenseCategory);
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
                _expenseCategoriesService.DeleteExpenseCategoryById((int)id);

            return RedirectToAction("Index");
        }

        // GET-Update - Creating View
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var expenseCategory = _expenseCategoriesService.GetExpenseCategoryById((int)id);
            if (expenseCategory == null)
            {
                return NotFound();
            }
            return View(expenseCategory);
        }

        //POST-Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ExpenseCategory expenseCategory)
        {
            if (ModelState.IsValid)
            {
                _expenseCategoriesService.UpdateExpenseCategoryById(expenseCategory.Id, expenseCategory);
                return RedirectToAction("Index");
            }

            return View(expenseCategory);
        }

    }
}
