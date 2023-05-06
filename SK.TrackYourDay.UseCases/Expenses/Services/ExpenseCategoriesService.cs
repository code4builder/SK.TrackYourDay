using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Infrastructure.DataAccess;
using SK.TrackYourDay.UseCases.DTOs;

namespace SK.TrackYourDay.UseCases.Expenses.Services
{
    public class ExpenseCategoriesService
    {
        private ApplicationDbContext _context;

        public ExpenseCategoriesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ExpenseCategory>> GetAllExpenseCategoriesDTOAsync(string userId)
        {
            if (_context.ExpenseCategories.Any()) 
                return _context.ExpenseCategories.Where(ec => ec.UserId == userId).ToList();
            else
               return new List<ExpenseCategory>();
        }

        public ExpenseCategory GetExpenseCategoryById(int id) => _context.ExpenseCategories.FirstOrDefault(x => x.Id == id);

        public void CreateExpenseCategory(ExpenseCategoryDTO expenseCategoryDTO, string userId)
        {
            var expenseCategory = new ExpenseCategory()
            {
                Name = expenseCategoryDTO.Name,
                UserId = userId
            };

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
