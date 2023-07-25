using Microsoft.EntityFrameworkCore;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.UseCases.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK.TrackYourDay.UseCases.Abstractions.Expenses.Services
{
    public interface IExpensesService
    {
        Task<IEnumerable<ExpenseDTO>> GetAllExpensesDTOAsync(string userId, string role, string sortBy,
                                                                    string searchString, int? pageNumber, int? pageSize);
        Task<List<ExpenseDTO>> GetExpensesDTOByUserId(string userId);
        Task<ExpenseDTO> GetExpenseDTOByIdAsync(int expenseId, string userId);
        Task AddExpenseAsync(ExpenseDTO expenseDTO, string userId);
        Task<Expense> UpdateExpenseById(int id, ExpenseDTO expenseDTO, string userId);
        Task DeleteExpenseByIdAsync(int id);
        ExpenseDTO ConvertExpenseToDTO(Expense expense, string userId);
        List<ExpenseDTO> ConvertListExpensesToDTO(List<Expense> expenses);
        string GetFullUserName(string userId);
        Expense GetExpenseById(int id);
        List<Expense> GetExpensesByDate(DateTime date);
        List<Expense> GetExpensesByMonth(DateTime date);
        List<Expense> GetExpensesByYear(DateTime date);
        Task<bool> CheckExpenseCategorySelected(ExpenseDTO expenseDTO, string userId);
        Task<bool> CheckPaymentMethodSelected(ExpenseDTO expenseDTO, string userId);
        Task AddFriendAsync(string currentUserId, string friendEmail);
        Task<List<ExpenseDTO>> GetFriendsExpenses(string userId);
        Task<List<ApplicationUser>> GetFriendsList(string userId);
    }
}
