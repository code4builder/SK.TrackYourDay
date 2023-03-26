using SK.TrackYourDay.Expenses.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using System;

namespace SK.TrackYourDay.Expenses.Data.Services
{
    public class ExpensesService
    {
        private ApplicationDbContext _context;

        public ExpensesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Expense> GetAllExpenses() => _context.Expenses.ToList();
        public IEnumerable<ExpenseVM> GetAllExpensesVM()
        {
            var expenses = _context.Expenses.ToList();
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

        public void AddExpense(ExpenseVM expense)
        {
            var _expense = ConvertVMToExpense(expense);
            
            _context.Expenses.Add(_expense);
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

        public static ExpenseVM ConvertExpenseToVM(Expense expense)
        {
            var expenseVM = new ExpenseVM()
            {
                Id = expense.Id,
                ExpenseName = expense.ExpenseName,
                Description = expense.Description,
                Amount = expense.Amount,
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
                Date = expenseVM.Date,
                UserId = expenseVM.UserId
            };
            return expense;
        }

        public Expense GetExpenseById(int id) => _context.Expenses.FirstOrDefault(x => x.Id == id);

        public List<Expense> GetExpensesByDate(DateTime date) => _context.Expenses.Where(x => x.Date == date).ToList();

        public List<Expense> GetExpensesByMonth(DateTime date) => _context.Expenses.Where(x => x.Date.Month == date.Month).ToList();

        public List<Expense> GetExpensesByYear(DateTime date) => _context.Expenses.Where(x => x.Date.Year == date.Year).ToList();

        public List<Expense> GetExpensesByUserId(int userId) => _context.Expenses.Where(x => x.UserId == userId).ToList();
    }
}

