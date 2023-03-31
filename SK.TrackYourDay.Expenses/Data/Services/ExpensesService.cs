using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyBooks.Data.Paging;
using SK.TrackYourDay.Expenses.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using System;
using System.Security.Claims;
using System.Security.Policy;

namespace SK.TrackYourDay.Expenses.Data.Services
{
    public class ExpensesService
    {
        private ApplicationDbContext _context;
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;

        public ExpensesService(ApplicationDbContext context,
            UserManager<ApplicationUser> _userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = _userManager;
            _signInManager = signInManager;
        }

        public IEnumerable<ExpenseVM> GetAllExpensesVM(string userId, string role, string sortBy, string searchString, int? pageNumber)
        {
            List<Expense> expenses;

            if (role == RoleVM.User)
                expenses = GetExpensesByUserId(userId).OrderByDescending(e => e.Date).ToList();
            else
                expenses = _context.Expenses.OrderByDescending(e => e.Date).ToList();

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        expenses = expenses.OrderByDescending(p => p.ExpenseName).ToList();
                        break;
                    default:
                        break;
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                expenses = expenses.Where(p => p.ExpenseName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }

            // Paging
            int pageSize = 5;
            expenses = PaginatedList<Expense>.Create(expenses.AsQueryable(), pageNumber ?? 1, pageSize);

            var expensesVM = new List<ExpenseVM>();

            if (expenses.Any())
            {
                foreach (var expense in expenses)
                {

                    var expenseVM = ConvertExpenseToVM(expense, expense.UserId);

                    expensesVM.Add(expenseVM);
                }
            }
            return expensesVM;
        }

        public List<Expense> GetExpensesByUserId(string userId)
        {
            if (_context.Expenses.Any())
                return _context.Expenses.Where(e => e.UserId == userId).ToList();
            else
                return new List<Expense>();
        }

        public ExpenseVM GetExpenseVMById(int expenseId, string userId)
        {
            var expense = _context.Expenses.FirstOrDefault(x => x.Id == expenseId);
            var expenseVM = ConvertExpenseToVM(expense, userId);
            return expenseVM;
        }

        public void AddExpense(ExpenseVM expenseVM, string userId)
        {
            var expense = new Expense()
            {
                Id = expenseVM.Id,
                ExpenseName = expenseVM.ExpenseName,
                Description = expenseVM.Description,
                Amount = expenseVM.Amount,
                ExpenseCategory = _context.ExpenseCategories.FirstOrDefault(ec => ec.Id == Int32.Parse(expenseVM.ExpenseCategory)),
                PaymentMethod = _context.PaymentMethods.FirstOrDefault(pm => pm.Id == Int32.Parse(expenseVM.PaymentMethod)),
                Date = expenseVM.Date,
                UserId = userId
            };

            _context.Expenses.Add(expense);
            _context.SaveChanges();
        }

        public Expense UpdateExpenseById(int id, ExpenseVM expenseVM)
        {
            var _expense = _context.Expenses.FirstOrDefault(expense => expense.Id == id);
            if (_expense != null)
            {
                _expense.ExpenseName = expenseVM.ExpenseName;
                _expense.Description = expenseVM.Description;
                _expense.Amount = expenseVM.Amount;
                _expense.ExpenseCategory = _context.ExpenseCategories.FirstOrDefault(ec => ec.Id == Int32.Parse(expenseVM.ExpenseCategory));
                _expense.PaymentMethod = _context.PaymentMethods.FirstOrDefault(pm => pm.Id == Int32.Parse(expenseVM.PaymentMethod));
                _expense.Date = expenseVM.Date;

                _context.SaveChanges();
            }
            return _expense;
        }
        public void DeleteExpenseById(int id)
        {
            var _expense = _context.Expenses.FirstOrDefault(x => x.Id == id);
            if (_expense != null)
            {
                _context.Expenses.Remove(_expense);
                _context.SaveChanges();
            }
        }

        public ExpenseVM ConvertExpenseToVM(Expense expense, string userId)
        {
            var expenseVM = new ExpenseVM()
            {
                Id = expense.Id,
                ExpenseName = expense.ExpenseName,
                Description = expense.Description,
                Amount = expense.Amount,
                ExpenseCategory = _context.ExpenseCategories.FirstOrDefault(ec => ec.Id == expense.ExpenseCategoryId).Name.ToString(),
                PaymentMethod = _context.PaymentMethods.FirstOrDefault(pm => pm.Id == expense.PaymentMethodId).Name.ToString(),
                Date = expense.Date,
                UserName = GetFullUserName(userId)
            };
            return expenseVM;
        }

        public IEnumerable<SelectListItem> GetExpenseCategoriesDropDown()
        {
            IEnumerable<SelectListItem> expenseCategoriesDropDown = _context
                    .ExpenseCategories.Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });

            return expenseCategoriesDropDown;
        }

        public IEnumerable<SelectListItem> GetPaymentMethodsDropDown()
        {
            IEnumerable<SelectListItem> paymentMethodsDropDown = _context
                    .PaymentMethods.Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });

            return paymentMethodsDropDown;
        }

        public string GetFullUserName(string userId)
        {
            var _user = _context.Users.FirstOrDefault(u => u.Id == userId);
            return _user.FirstName + " " + _user.LastName;
        }

        public Expense GetExpenseById(int id) => _context.Expenses.FirstOrDefault(x => x.Id == id);

        public List<Expense> GetExpensesByDate(DateTime date) => _context.Expenses.Where(x => x.Date == date).ToList();

        public List<Expense> GetExpensesByMonth(DateTime date) => _context.Expenses.Where(x => x.Date.Month == date.Month).ToList();

        public List<Expense> GetExpensesByYear(DateTime date) => _context.Expenses.Where(x => x.Date.Year == date.Year).ToList();

        public List<Expense> GetExpensesByUserId(int userId) => _context.Expenses.Where(x => x.UserId.ToString() == userId.ToString()).ToList();
    }
}

