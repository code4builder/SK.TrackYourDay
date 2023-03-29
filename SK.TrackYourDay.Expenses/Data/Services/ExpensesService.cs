using Microsoft.AspNetCore.Mvc.Rendering;
using MyBooks.Data.Paging;
using SK.TrackYourDay.Expenses.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using System;
using System.Security.Policy;

namespace SK.TrackYourDay.Expenses.Data.Services
{
    public class ExpensesService
    {
        private ApplicationDbContext _context;

        public ExpensesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ExpenseVM> GetAllExpensesVM(string sortBy, string searchString, int? pageNumber)
        {
            var expenses = _context.Expenses.OrderByDescending(e => e.Date).ToList();
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
                    var expenseVM = ConvertExpenseToVM(expense);
                    
                    expensesVM.Add(expenseVM);
                }
            }
            return expensesVM;
        }

        public ExpenseVM GetExpenseVMById(int id)
        {
            var expense = _context.Expenses.FirstOrDefault(x => x.Id == id);
            var expenseVM = ConvertExpenseToVM(expense);
            return expenseVM;
        }

        public void AddExpense(ExpenseVM expenseVM)
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
                UserId = expenseVM.UserId
            };

            _context.Expenses.Add(expense);
            _context.SaveChanges();

            // TODO: add to another related tables
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
                _expense.UserId = expenseVM.UserId;

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

        public ExpenseVM ConvertExpenseToVM(Expense expense)
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
                UserId = expense.UserId
            };
            return expenseVM;
        }

        public static Expense ConvertVMToExpense(ExpenseVM expenseVM)
        {
            var expense = new Expense()
            {
                Id = expenseVM.Id,
                ExpenseName = expenseVM.ExpenseName,
                Description = expenseVM.Description,
                Amount = expenseVM.Amount,
                ExpenseCategory = (ExpenseCategory)Enum.Parse(typeof(ExpenseCategory), expenseVM.ExpenseCategory),
                PaymentMethod = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), expenseVM.PaymentMethod),
                Date = expenseVM.Date,
                UserId = expenseVM.UserId
            };
            return expense;
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

        public Expense GetExpenseById(int id) => _context.Expenses.FirstOrDefault(x => x.Id == id);

        public List<Expense> GetExpensesByDate(DateTime date) => _context.Expenses.Where(x => x.Date == date).ToList();

        public List<Expense> GetExpensesByMonth(DateTime date) => _context.Expenses.Where(x => x.Date.Month == date.Month).ToList();

        public List<Expense> GetExpensesByYear(DateTime date) => _context.Expenses.Where(x => x.Date.Year == date.Year).ToList();

        public List<Expense> GetExpensesByUserId(int userId) => _context.Expenses.Where(x => x.UserId == userId).ToList();
    }
}

