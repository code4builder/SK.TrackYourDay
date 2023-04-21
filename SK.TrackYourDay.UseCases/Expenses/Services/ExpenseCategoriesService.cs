using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Infrastructure.DataAccess;

namespace SK.TrackYourDay.UseCases.Expenses.Services
{
    public class ExpenseCategoriesService
    {
        private ApplicationDbContext _context;

        public ExpenseCategoriesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ExpenseCategory> GetAllExpenseCategories() => _context.ExpenseCategories.ToList();

        public ExpenseCategory GetExpenseCategoryById(int id) => _context.ExpenseCategories.FirstOrDefault(x => x.Id == id);

        public void CreateExpenseCategory(ExpenseCategory expenseCategory)
        {
            _context.ExpenseCategories.Add(expenseCategory);
            _context.SaveChanges();
        }

        public void DeleteExpenseCategoryById(int id)
        {
            var _expenseCategory = GetExpenseCategoryById(id);
            if (_expenseCategory != null)
            {
                _context.ExpenseCategories.Remove(_expenseCategory);
                _context.SaveChanges();
            }
        }

        public ExpenseCategory UpdateExpenseCategoryById(int id, ExpenseCategory expenseCategory)
        {
            var _expenseCategory = GetExpenseCategoryById(id);
            if (expenseCategory != null)
                _expenseCategory.Name = expenseCategory.Name;

            _context.SaveChanges();

            return _expenseCategory;
        }
    }
}
