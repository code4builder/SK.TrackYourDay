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
                var friendsExpenseCategories = await GetFriendsExpenseCategoriesAsync(userId);
                expenseCategories.AddRange(friendsExpenseCategories);

                return expenseCategories.OrderBy(ec => ec.Name).ToList();
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
                    var expenseCategoryDTO = ConvertExpenseCategoryToDTO(expenseCategory, userId);
                    expenseCategoriesDTO.Add(expenseCategoryDTO);
                }
                return expenseCategoriesDTO;
            }
            else
                return new List<ExpenseCategoryDTO>();
        }

        public async Task<ExpenseCategory> GetExpenseCategoryByIdAsync(int id) => await _context.ExpenseCategories.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<ExpenseCategoryDTO> GetExpenseCategoryDTOByIdAsync(int id)
        {
            var expensecategory = await GetExpenseCategoryByIdAsync(id);
            var expensecategoryDTO = ConvertExpenseCategoryToDTO(expensecategory, expensecategory.UserId);
            return expensecategoryDTO;
        }

        public async Task CreateExpenseCategoryAsync(ExpenseCategoryDTO expenseCategoryDTO, string userId)
        {
            var expenseCategory = new ExpenseCategory()
            {
                Name = expenseCategoryDTO.Name,
                UserId = userId
            };

            await _context.ExpenseCategories.AddAsync(expenseCategory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteExpenseCategoryByIdAsync(int id)
        {
            var _expenseCategory = await GetExpenseCategoryByIdAsync(id);
            if (_expenseCategory != null)
            {
                _context.ExpenseCategories.Remove(_expenseCategory);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ExpenseCategory> UpdateExpenseCategoryByIdAsync(int id, ExpenseCategoryDTO expenseCategoryDTO)
        {
            var _expenseCategory = await GetExpenseCategoryByIdAsync(id);
            if (_expenseCategory != null)
                _expenseCategory.Name = expenseCategoryDTO.Name;

            await _context.SaveChangesAsync();

            return _expenseCategory;
        }

        public async Task<List<ExpenseCategoryDTO>> GetFriendsExpenseCategoriesAsync(string userId)
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

        public ExpenseCategoryDTO ConvertExpenseCategoryToDTO(ExpenseCategory expenseCategory, string userId)
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
