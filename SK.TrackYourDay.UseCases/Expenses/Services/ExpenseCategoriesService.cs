using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Infrastructure.DataAccess;
using SK.TrackYourDay.UseCases.Abstractions.Expenses.Services;
using SK.TrackYourDay.UseCases.DTOs;

namespace SK.TrackYourDay.UseCases.Expenses.Services
{
    public class ExpenseCategoriesService : IExpenseCategoriesService
    {
        private ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public ExpenseCategoriesService(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Get all expense categories for the current user and his friends by UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The list of <see cref="ExpenseCategoryDTO"/> for current user and his friends</returns>
        public async Task<List<ExpenseCategoryDTO>> GetAllExpenseCategoriesDTOAsync(string userId)
        {
            if (_context.ExpenseCategories.Any())
            {
                var expenseCategories = await GetExpenseCategoriesDTOByUserIdAsync(userId);
                var friendsExpenseCategories = await GetFriendsExpenseCategoriesAsync(userId);
                expenseCategories.AddRange(friendsExpenseCategories);

                return expenseCategories.OrderBy(ec => ec.Name).ToList();
            }
            else
               return new List<ExpenseCategoryDTO>();
        }

        /// <summary>
        /// Get all expense categories for the current user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The list of <see cref="ExpenseCategoryDTO"/> only for current user</returns>
        public async Task<List<ExpenseCategoryDTO>> GetExpenseCategoriesDTOByUserIdAsync(string userId)
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

        /// <summary>
        /// Get an ExpenseCategory by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The retrieved <see cref="ExpenseCategoryDTO"/> by Id</returns>
        public async Task<ExpenseCategoryDTO> GetExpenseCategoryDTOByIdAsync(int id)
        {
            var expensecategory = await GetExpenseCategoryByIdAsync(id);
            var expensecategoryDTO = ConvertExpenseCategoryToDTO(expensecategory, expensecategory.UserId);
            return expensecategoryDTO;
        }

        /// <summary>
        /// Create a new ExpenseCategory
        /// </summary>
        /// <param name="expenseCategoryDTO"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Delete ExpenseCategory by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteExpenseCategoryByIdAsync(int id)
        {
            var _expenseCategory = await GetExpenseCategoryByIdAsync(id);
            if (_expenseCategory != null)
            {
                _context.ExpenseCategories.Remove(_expenseCategory);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Updates ExpenseCategory by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="expenseCategoryDTO"></param>
        /// <returns>The updated <see cref="ExpenseCategory"/></returns>
        public async Task<ExpenseCategory> UpdateExpenseCategoryByIdAsync(int id, ExpenseCategoryDTO expenseCategoryDTO)
        {
            var _expenseCategory = await GetExpenseCategoryByIdAsync(id);
            if (_expenseCategory != null)
                _expenseCategory.Name = expenseCategoryDTO.Name;

            await _context.SaveChangesAsync();

            return _expenseCategory;
        }

        /// <summary>
        /// Get all expense categories for the current user's friends
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The list of <see cref="ExpenseCategoryDTO"/> only for current user's friends</returns>
        public async Task<List<ExpenseCategoryDTO>> GetFriendsExpenseCategoriesAsync(string userId)
        {
            var expenseService = new ExpensesService(_context, _memoryCache);
            var friends = await expenseService.GetFriendsListAsync(userId);

            var friendsExpenseCategoriesDTO = new List<ExpenseCategoryDTO>();
            foreach (var friend in friends)
            {
                var expenseCategoriesDTO = await GetExpenseCategoriesDTOByUserIdAsync(friend.Id);
                friendsExpenseCategoriesDTO.AddRange(expenseCategoriesDTO);
            }
            return friendsExpenseCategoriesDTO;
        }

        /// <summary>
        /// Converts <see cref="ExpenseCategory"/> to <see cref="ExpenseCategoryDTO"/>
        /// </summary>
        /// <param name="expenseCategory"></param>
        /// <param name="userId"></param>
        /// <returns>Converted <see cref="ExpenseCategoryDTO"/></returns>
        public ExpenseCategoryDTO ConvertExpenseCategoryToDTO(ExpenseCategory expenseCategory, string userId)
        {
            var expenseService = new ExpensesService(_context, _memoryCache);
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
