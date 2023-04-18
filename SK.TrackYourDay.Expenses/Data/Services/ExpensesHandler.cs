using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using SK.TrackYourDay.Infrastructure.DataAccess;
using SK.TrackYourDay.UseCases.DTOs;

namespace SK.TrackYourDay.Expenses.Data.Services
{

    public class ExpensesHandler
    {
        private ApplicationDbContext _context;
        public ExpensesHandler(ApplicationDbContext context)
        {
            _context = context;
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

        public static ExpenseDTO ConvertExpenseVMToDto(ExpenseVM expenseVM)
        {
            var expense = new ExpenseDTO
            {
                Id = expenseVM.Id,
                ExpenseName = expenseVM.ExpenseName,
                ExpenseCategory = expenseVM.ExpenseCategory,
                PaymentMethod = expenseVM.PaymentMethod,
                Amount = expenseVM.Amount,
                Date = expenseVM.Date,
                Description = expenseVM.Description
            };
            return expense;
        }
        public static ExpenseVM ConvertExpenseDtoToVM(ExpenseDTO expenseDTO)
        {
            var expenseVM = new ExpenseVM
            {
                Id = expenseDTO.Id,
                ExpenseName = expenseDTO.ExpenseName,
                ExpenseCategory = expenseDTO.ExpenseCategory,
                PaymentMethod = expenseDTO.PaymentMethod,
                Amount = expenseDTO.Amount,
                Date = expenseDTO.Date,
                Description = expenseDTO.Description
            };
            return expenseVM;
        }
    }
}

