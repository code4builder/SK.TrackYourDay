﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SK.TrackYourDay.Expenses.Data.Paging;
using SK.TrackYourDay.Expenses.Data.Services;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using SK.TrackYourDay.UseCases.DTOs;
using SK.TrackYourDay.UseCases.Expenses.Services;
using System.Security.Claims;
using static SK.TrackYourDay.Expenses.Models.ViewModels.TotalsVM;

namespace SK.TrackYourDay.Expenses.Controllers;

public class ExpensesController : Controller
{
    IHttpContextAccessor _httpContextAccessor;
    private ExpensesService _expensesService;
    private ExpensesHandler _expensesHandler;
    private ExpenseCategoriesService _expenseCategoriesService;
    private readonly IMapper _mapper;
    private readonly ILogger<AccountController> _logger;

    public ExpensesController(ExpensesService expensesService, ExpenseCategoriesService expenseCategoriesService,
        IHttpContextAccessor httpContextAccessor, ExpensesHandler expensesHandler, IMapper mapper, ILogger<AccountController> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _expensesService = expensesService;
        _expensesHandler = expensesHandler;
        _expenseCategoriesService = expenseCategoriesService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string sortBy, string searchString, int? pageNumber = 1, int pageSize = 10)
    {
        _logger.LogInformation("GetAllExpenses triggered");

        var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var _role = HttpContext.User.FindFirst(ClaimTypes.Role).Value;

        var expensesDTO = await _expensesService.GetAllExpensesDTOCache(_userId, _role, sortBy, searchString, pageNumber, pageSize);

        if (expensesDTO is null)
        {
            expensesDTO = await _expensesService.GetAllExpensesDTOAsync(_userId, _role, sortBy, searchString, pageNumber, pageSize);
            _expensesService.SetAllExpensesDTOToCache("expenses", expensesDTO);
        }

        var expensesVM = _mapper.Map<IEnumerable<ExpenseVM>>(expensesDTO);

        var ExpenseCategoriesDropDown = _expensesHandler.GetExpenseCategoriesDropDown(_userId);
        ViewBag.ExpenseCategoriesDropDown = ExpenseCategoriesDropDown;
        var PaymentMethodsDropDown = _expensesHandler.GetPaymentMethodsDropDown(_userId);
        ViewBag.PaymentMethodsDropDown = PaymentMethodsDropDown;

        // Pagination
        var paginatedExpensesVM = PaginatedList<ExpenseVM>.Create(expensesVM.AsQueryable(), pageNumber, pageSize);

        return View(paginatedExpensesVM);
    }

    //GET-Create - Creating View
    public IActionResult Create()
    {
        _logger.LogInformation("CreateExpense triggered");

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
                _logger.LogInformation($"The new expense {expenseVM.ExpenseName} was created");
                TempData["success"] = "Expense created successfully";
                return RedirectToAction("Index");
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = ex?.Message;
            throw new Exception($"{ex}. Can't create new expense by user: {User.Identity.Name}");
            //return BadRequest();
        }

        return View(expenseVM);
    }

    // GET-Delete - Creating View
    public async Task<IActionResult> Delete(int? id)
    {
        _logger.LogInformation("DeleteExpense triggered");

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
        _logger.LogInformation($"The expense with {id} was deleted");
        TempData["success"] = "Expense deleted successfully";

        return RedirectToAction("Index");
    }

    // GET-Update - Creating View
    public async Task<IActionResult> Update(int? id)
    {
        _logger.LogInformation("UpdateExpense triggered");

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

        expenseVM.ExpenseCategory = _expensesHandler.GetExpenseCategoryIdFromExpenseVM(expenseVM).ToString();
        expenseVM.PaymentMethod = _expensesHandler.GetPaymentMethodIdFromExpenseVM(expenseVM).ToString();

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
            await _expensesService.UpdateExpenseByIdAsync(expenseVM.Id, expenseDTO, _userId);
            _logger.LogInformation($"The expense with {expenseVM.Id} was updated");
            TempData["success"] = "Expense updated successfully";

            return RedirectToAction("Index");
        }
        else
        {
            TempData["error"] = "Something wrong. Update wasn't successful";
        }

        return View(expenseVM);
    }

    public IActionResult AddFriend()
    {
        _logger.LogInformation("AddFriend triggered");

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
            _logger.LogInformation($"The friend {friendVM.Email} was added");
            TempData["success"] = "Friend added successfully";

            return RedirectToAction("Index", "Home");
        }
        else
        {
            TempData["error"] = "Something wrong. Friend wasn't added";
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> FilterExpenses(FilterVM filterVM, int? pageNumber = 1, int pageSize = 10)
    {
        _logger.LogInformation("FilterExpenses triggered");

        var filteredExpensesVM = new List<ExpenseVM>();

        var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var _role = HttpContext.User.FindFirst(ClaimTypes.Role).Value;

        decimal totalAmount = 0;

        if (ModelState.IsValid)
        {
            var filterDTO = _mapper.Map<FilterDTO>(filterVM);

            var filteredExpensesDTO = await _expensesService.FilterExpensesAsync(_userId, _role, filterDTO);

            filteredExpensesVM = _mapper.Map<List<ExpenseVM>>(filteredExpensesDTO);

            _logger.LogInformation("FilterExpenses list received");

            totalAmount = _expensesService.GetTotalAmount(filteredExpensesDTO);
        }

        var paginatedExpensesVM = PaginatedList<ExpenseVM>.Create(filteredExpensesVM.AsQueryable(), pageNumber, pageSize);
        ViewBag.TotalAmount = totalAmount;

        var ExpenseCategoriesDropDown = _expensesHandler.GetExpenseCategoriesDropDown(_userId);
        ViewBag.ExpenseCategoriesDropDown = ExpenseCategoriesDropDown;
        var PaymentMethodsDropDown = _expensesHandler.GetPaymentMethodsDropDown(_userId);
        ViewBag.PaymentMethodsDropDown = PaymentMethodsDropDown;

        return View("Index", paginatedExpensesVM);
    }

    [HttpGet]
    public async Task<IActionResult> Totals()
    {
        _logger.LogInformation("Totals triggered");

        var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

        var expensesDTO = await _expensesService.GetAllExpensesDTOAsync(_userId, "User", null, null, null, null);
        var categoriesDTO = await _expenseCategoriesService.GetAllExpenseCategoriesDTOAsync(_userId);

        TotalsDTO totalsDTO = await _expensesService.GetExpensesTotals(_userId, expensesDTO.ToList(), categoriesDTO);
         
        TotalsVM totalsVM = ConvertTotalsToVM(totalsDTO);

        return View(totalsVM);
    }

    [HttpGet]
    public IActionResult LoadExpensesFromExcel()
    {
        _logger.LogInformation("LoadExpenses view triggered");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoadExpensesFromExcel(IFormFile fileInput)
    {
        _logger.LogInformation("LoadExpenses triggered");

        var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

        if (fileInput == null || fileInput.Length == 0)
        {
            TempData["error"] = "File is empty";
            return View();
        }

        string message = await _expensesService.LoadExpensesFromExcelAsync(_userId, fileInput);
        if (message == "Ok")
            TempData["success"] = "Expenses loaded successfully";
        else
            TempData["error"] = message;

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult ExportToCSV()
    {
        _logger.LogInformation("ExportToCSV view triggered");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExportToCSV(FilterToCsvVM filterVM)
    {
        _logger.LogInformation("ExportToCSV triggered");

        if (string.IsNullOrEmpty(filterVM.FilePath))
        {
            TempData["error"] = "Path to file is empty";
            return View();
        }

        var filteredExpensesVM = new List<ExpenseVM>();

        var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var _role = HttpContext.User.FindFirst(ClaimTypes.Role).Value;

        if (ModelState.IsValid)
        {
            filterVM.DateFrom = filterVM.DateFrom ?? DateTime.MinValue;
            filterVM.DateTo = filterVM.DateTo ?? DateTime.MaxValue;

            var filterDTO = _mapper.Map<FilterDTO>(filterVM);

            var filteredExpensesDTO = await _expensesService.FilterExpensesAsync(_userId, _role, filterDTO);

            _logger.LogInformation("Filtered expenses list received");

            string message = await _expensesService.ExportExpensesToCSVAsync(filterVM.FilePath, filteredExpensesDTO);
            if (message == "Ok")
                TempData["success"] = "Expenses export to CSV file successfully";
            else
                TempData["error"] = message;
        }

        return RedirectToAction("Index");
    }

    private TotalsVM ConvertTotalsToVM(TotalsDTO totalsDTO)
    {
        TotalsVM totalsVM = new TotalsVM();
        totalsVM.AllExpenses = totalsDTO.AllExpenses;
        totalsVM.YearExpenses = new List<YearExpensesVM>();
        foreach (var year in totalsDTO.YearExpenses)
        {
            YearExpensesVM yearExpenses = new YearExpensesVM() { Year = year.Year };
            // assign value from dictionary DTO to dictionary VM
            yearExpenses.YearExpensesByCategory = year.YearExpensesByCategory.Select(x => new KeyValuePair<ExpenseCategoryVM, decimal>(_mapper.Map<ExpenseCategoryVM>(x.Key), x.Value)).ToDictionary(x => x.Key, x => x.Value);
            yearExpenses.IrregularYearExpensesByCategory = year.IrregularYearExpensesByCategory.Select(x => new KeyValuePair<ExpenseCategoryVM, decimal>(_mapper.Map<ExpenseCategoryVM>(x.Key), x.Value)).ToDictionary(x => x.Key, x => x.Value);
            yearExpenses.RegularYearExpensesByCategory = year.RegularYearExpensesByCategory.Select(x => new KeyValuePair<ExpenseCategoryVM, decimal>(_mapper.Map<ExpenseCategoryVM>(x.Key), x.Value)).ToDictionary(x => x.Key, x => x.Value);
            yearExpenses.YearTotal = year.YearTotal;

            for (int i = 0; i < 12; i++)
            {
                if (year.monthExpensesYear[i] == null) continue;

                MonthExpensesVM monthExpenses = new MonthExpensesVM();
                monthExpenses.MonthName = year.monthExpensesYear[i].MonthName;
                monthExpenses.MonthExpensesByCategory = year.monthExpensesYear[i].MonthExpensesByCategory.Select(x => new KeyValuePair<ExpenseCategoryVM, decimal>(_mapper.Map<ExpenseCategoryVM>(x.Key), x.Value)).ToDictionary(x => x.Key, x => x.Value);
                monthExpenses.IrregularMonthExpensesByCategory = year.monthExpensesYear[i].IrregularMonthExpensesByCategory.Select(x => new KeyValuePair<ExpenseCategoryVM, decimal>(_mapper.Map<ExpenseCategoryVM>(x.Key), x.Value)).ToDictionary(x => x.Key, x => x.Value);
                monthExpenses.RegularMonthExpensesByCategory = year.monthExpensesYear[i].RegularMonthExpensesByCategory.Select(x => new KeyValuePair<ExpenseCategoryVM, decimal>(_mapper.Map<ExpenseCategoryVM>(x.Key), x.Value)).ToDictionary(x => x.Key, x => x.Value);
                monthExpenses.MonthTotal = year.monthExpensesYear[i].MonthTotal;
                monthExpenses.IrregularAmount = year.monthExpensesYear[i].IrregularAmount;

                yearExpenses.monthExpensesYear[i] = monthExpenses;
            }

            totalsVM.YearExpenses.Add(yearExpenses);
        }

        return totalsVM;
    }
}
