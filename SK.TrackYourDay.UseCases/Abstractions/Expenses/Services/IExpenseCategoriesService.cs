using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.UseCases.DTOs;

namespace SK.TrackYourDay.UseCases.Abstractions.Expenses.Services
{
    public interface IExpenseCategoriesService
    {
        Task<List<ExpenseCategoryDTO>> GetAllExpenseCategoriesDTOAsync(string userId);
        Task<List<ExpenseCategoryDTO>> GetExpenseCategoriesDTOByUserId(string userId);
        Task<ExpenseCategory> GetExpenseCategoryByIdAsync(int id);
        Task<ExpenseCategoryDTO> GetExpenseCategoryDTOByIdAsync(int id);
        Task CreateExpenseCategoryAsync(ExpenseCategoryDTO expenseCategoryDTO, string userId);
        Task<ExpenseCategory> UpdateExpenseCategoryByIdAsync(int id, ExpenseCategoryDTO expenseCategoryDTO);
        Task DeleteExpenseCategoryByIdAsync(int id);
        Task<List<ExpenseCategoryDTO>> GetFriendsExpenseCategoriesAsync(string userId);
        ExpenseCategoryDTO ConvertExpenseCategoryToDTO(ExpenseCategory expenseCategory, string userId);
    }
}
