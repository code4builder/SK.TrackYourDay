using Microsoft.EntityFrameworkCore;
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

        public async Task<List<ExpenseCategoryDTO>> GetAllExpenseCategoriesDTOAsync(string userId)
        {
            if (_context.ExpenseCategories.Any())
            {
                var expenseCategories = await GetExpenseCategoriesDTOByUserId(userId);
                var friendsExpenseCategories = await GetFriendsExpenseCategories(userId);
                expenseCategories.AddRange(friendsExpenseCategories);
                return expenseCategories;
            }
            else
               return new List<ExpenseCategoryDTO>();
        }

        public async Task<List<ExpenseCategoryDTO>> GetExpenseCategoriesDTOByUserId(string userId)
        {
            if (_context.ExpenseCategories.Any())
            {
                var expenseCategories = await _context.ExpenseCategories.Where(e => e.UserId == userId).ToListAsync();

                var expenseCategoriesDTO = new List<ExpenseCategoryDTO>();
                foreach (var expenseCategory in expenseCategories)
                {
                    var expenseCategoryDTO = ConvertExpenseToDTO(expenseCategory, userId);
                    expenseCategoriesDTO.Add(expenseCategoryDTO);
                }
                return expenseCategoriesDTO;
            }
            else
                return new List<ExpenseCategoryDTO>();
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

        public async Task<List<ExpenseCategoryDTO>> GetFriendsExpenseCategories(string userId)
        {
            var expenseService = new ExpensesService(_context);
            var friends = expenseService.GetFriendsList(userId);

            var friendsExpenseCategoriesDTO = new List<ExpenseCategoryDTO>();
            foreach (var friend in friends)
            {
                var expenseCategoriesDTO = await GetExpenseCategoriesDTOByUserId(friend.Id);
                friendsExpenseCategoriesDTO.AddRange(expenseCategoriesDTO);
            }
            return friendsExpenseCategoriesDTO;
        }

        public ExpenseCategoryDTO ConvertExpenseToDTO(ExpenseCategory expenseCategory, string userId)
        {
            var expenseService = new ExpensesService(_context);
            try
            {
                var expenseCategoryDTO = new ExpenseCategoryDTO()
                {
                    Id = expenseCategory.Id,
                    Name = expenseCategory.Name,
                    User = expenseService.GetFullUserName(userId)
                };
                return expenseCategoryDTO;
            }
            catch (Exception)
            {
                throw new Exception("Can not be converted");
            }
        }
    }
}
