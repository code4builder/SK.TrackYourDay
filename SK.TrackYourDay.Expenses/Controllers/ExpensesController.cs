using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SK.TrackYourDay.Expenses.Data;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using System.Security.Claims;
using SK.TrackYourDay.UseCases.Expenses.Services;
using SK.TrackYourDay.Expenses.Data.Services;
using AutoMapper;
using SK.TrackYourDay.UseCases.DTOs;

namespace SK.TrackYourDay.Expenses.Controllers
{
    public class ExpensesController : Controller
    {
        IHttpContextAccessor _httpContextAccessor;
        private ExpensesService _expensesService;
        private ExpensesHandler _expensesHandler;
        private PaymentMethodsService _paymentMethodsService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public ExpensesController(ExpensesService expensesService, PaymentMethodsService paymentMethodsService,
            IHttpContextAccessor httpContextAccessor, ExpensesHandler expensesHandler, IMapper mapper, ILogger<AccountController> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _expensesService = expensesService;
            _expensesHandler = expensesHandler;
            _paymentMethodsService = paymentMethodsService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string sortBy, string searchString, int pageNumber, int pageSize)
        {
            _logger.LogInformation("GetAll Expenses triggered");

            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var _role = HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            var expensesDTO = await _expensesService.GetAllExpensesDTOAsync(_userId, _role, sortBy, searchString, pageNumber, pageSize);

            var expensesVM = _mapper.Map<IEnumerable<ExpenseVM>>(expensesDTO);

            return View(expensesVM);
        }

        //GET-Create - Creating View
        public IActionResult Create()
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var ExpenseCategoriesDropDown = _expensesHandler.GetExpenseCategoriesDropDown(_userId);
            ViewBag.ExpenseCategoriesDropDown = ExpenseCategoriesDropDown;

            var PaymentMethodsDropDown = _expensesHandler.GetPaymentMethodsDropDown(_userId);
            ViewBag.PaymentMethodsDropDown = PaymentMethodsDropDown;

            return View();
        }

        //POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseVM expenseVM)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                if (ModelState.IsValid)
                {
                    var expenseDTO = _mapper.Map<ExpenseDTO>(expenseVM);
                    await _expensesService.AddExpenseAsync(expenseDTO, _userId);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                throw new Exception();
                //return BadRequest();
            }

            return View(expenseVM);
        }

        // GET-Delete - Creating View
        public async Task<IActionResult> Delete(int? id)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var expenseDTO = await _expensesService.GetExpenseDTOByIdAsync((int)id, _userId);
            var expenseVM = _mapper.Map<ExpenseVM>(expenseDTO);

            return View(expenseVM);
        }

        // POST-Delete
        [HttpPost]
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

            var expenseDTO = await _expensesService.GetExpenseDTOByIdAsync((int)id, _userId);
            if (expenseDTO == null)
            {
                return NotFound();
            }
            var expenseVM = _mapper.Map<ExpenseVM>(expenseDTO);

            var ExpenseCategoriesDropDown = _expensesHandler.GetExpenseCategoriesDropDown(_userId);
            ViewBag.ExpenseCategoriesDropDown = ExpenseCategoriesDropDown;

            var PaymentMethodsDropDown = _expensesHandler.GetPaymentMethodsDropDown(_userId);
            ViewBag.PaymentMethodsDropDown = PaymentMethodsDropDown;

            return View(expenseVM);
        }

        //POST-Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ExpenseVM expenseVM)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                var expenseDTO = _mapper.Map<ExpenseDTO>(expenseVM);
                await _expensesService.UpdateExpenseById(expenseVM.Id, expenseDTO, _userId);
                return RedirectToAction("Index");
            }

            return View(expenseVM);
        }

        public IActionResult AddFriend()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFriend(FriendVM friendVM)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _expensesService.AddFriendAsync(currentUserId, friendVM.Email);


                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
