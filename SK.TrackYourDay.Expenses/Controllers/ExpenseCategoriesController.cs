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
        private readonly ILogger<AccountController> _logger;

        public ExpenseCategoriesController(ExpenseCategoriesService expenseCategoriesService, IMapper mapper, ILogger<AccountController> logger)
        {
            _expenseCategoriesService = expenseCategoriesService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("GetAllExpenseCategories triggered");

            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var expenseCategoriesDTO = await _expenseCategoriesService.GetAllExpenseCategoriesDTOAsync(_userId);
            var expenseCategoriesVM = _mapper.Map<IEnumerable<ExpenseCategoryVM>>(expenseCategoriesDTO);
            return View(expenseCategoriesVM);
        }

        //GET-Create - Creating View
        public IActionResult Create()
        {
            _logger.LogInformation("CreateExpenseCategory triggered");

            return View();
        }

        //POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseCategoryVM expenseCategoryVM)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                var expenseCategoryDTO = _mapper.Map<ExpenseCategoryDTO>(expenseCategoryVM);
                await _expenseCategoriesService.CreateExpenseCategoryAsync(expenseCategoryDTO, _userId);
                _logger.LogInformation($"The new expense category {expenseCategoryVM.Name} was created");

                return RedirectToAction("Index");
            }

            return View(expenseCategoryVM);
        }

        // GET-Delete - Creating View
        public async Task<IActionResult> Delete(int? id)
        {
            _logger.LogInformation("DeleteExpenseCategory triggered");

            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var expenseCategoryDTO = await _expenseCategoriesService.GetExpenseCategoryDTOByIdAsync((int)id);
                var expenseCategoryVM = _mapper.Map<ExpenseCategoryVM>(expenseCategoryDTO);
                return View(expenseCategoryVM);
            }
            catch (Exception)
            {
                _logger.LogError("ExpenseCategory not found");
                return NotFound();
            }
        }

        // POST-Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id != null)
            {
                await _expenseCategoriesService.DeleteExpenseCategoryByIdAsync((int)id);
                _logger.LogInformation($"The expense category with {id} was deleted");
            }

            return RedirectToAction("Index");
        }

        // GET-Update - Creating View
        public async Task<IActionResult> Update(int? id)
        {
            _logger.LogInformation("UpdateExpenseCategory triggered");

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var expenseCategoryDTO = await _expenseCategoriesService.GetExpenseCategoryDTOByIdAsync((int)id);
            if (expenseCategoryDTO == null)
            {
                return NotFound();
            }
            var expensecategoryVM = _mapper.Map<ExpenseCategoryVM>(expenseCategoryDTO);
            if (expensecategoryVM == null)
            {
                return NotFound();
            }
            return View(expensecategoryVM);
        }

        //POST-Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ExpenseCategoryVM expenseCategoryVM)
        {
            if (ModelState.IsValid)
            {
                var expenseCategoryDTO = _mapper.Map<ExpenseCategoryDTO>(expenseCategoryVM);
                await _expenseCategoriesService.UpdateExpenseCategoryByIdAsync(expenseCategoryDTO.Id, expenseCategoryDTO);
                _logger.LogInformation($"The expense category with {expenseCategoryVM.Id} was updated");

                return RedirectToAction("Index");
            }

            return View(expenseCategoryVM);
        }
    }
}
