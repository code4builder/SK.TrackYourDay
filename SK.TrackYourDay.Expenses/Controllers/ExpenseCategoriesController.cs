using Microsoft.AspNetCore.Mvc;
using SK.TrackYourDay.Expenses.Data;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using SK.TrackYourDay.UseCases.Expenses.Services;
using System.Security.Claims;
using AutoMapper;
using SK.TrackYourDay.UseCases.DTOs;

namespace SK.TrackYourDay.Expenses.Controllers
{
    public class ExpenseCategoriesController : Controller
    {
        private ExpenseCategoriesService _expenseCategoriesService;
        private readonly IMapper _mapper;
        public ExpenseCategoriesController(ExpenseCategoriesService expenseCategoriesService, IMapper mapper)
        {
            _expenseCategoriesService = expenseCategoriesService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var objList = _expenseCategoriesService.GetAllExpenseCategoriesDTOAsync(_userId);
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
        public IActionResult Create(ExpenseCategoryVM expenseCategoryVM)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (ModelState.IsValid)
            {
                var expenseCategoryDTO = _mapper.Map<ExpenseCategoryDTO>(expenseCategoryVM);
                _expenseCategoriesService.CreateExpenseCategory(expenseCategoryDTO, _userId);
                return RedirectToAction("Index");
            }

            return View(expenseCategoryVM);
        }

        // GET-Delete - Creating View
        public IActionResult Delete(int? id)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
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
