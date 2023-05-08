using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using SK.TrackYourDay.Infrastructure.DataAccess;
using SK.TrackYourDay.UseCases.DTOs;
using SK.TrackYourDay.UseCases.Expenses.Services;

namespace SK.TrackYourDay.Expenses.Data.Services
{
    public class ExpensesHandler
    {
        private ApplicationDbContext _context;
        private ExpenseCategoriesService _expenseCategoriesService;
        private PaymentMethodsService _paymentMethodsService;
        public ExpensesHandler(ApplicationDbContext context, 
            ExpenseCategoriesService expenseCategoriesService, 
            PaymentMethodsService paymentMethodsService)
        {
            _context = context;
            _expenseCategoriesService = expenseCategoriesService;
            _paymentMethodsService = paymentMethodsService;
        }

        public IEnumerable<SelectListItem> GetExpenseCategoriesDropDown(string userId)
        {
            var expenseCategories = _expenseCategoriesService.GetAllExpenseCategoriesDTOAsync(userId).Result;

            IEnumerable<SelectListItem> expenseCategoriesDropDown = expenseCategories
                .Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });

            return expenseCategoriesDropDown;
        }

        public IEnumerable<SelectListItem> GetPaymentMethodsDropDown(string userId)
        {
            var paymentMethods = _paymentMethodsService.GetAllPaymentMethodsDTOAsync(userId).Result;

            IEnumerable<SelectListItem> paymentMethodsDropDown = paymentMethods
                    .Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });

            return paymentMethodsDropDown;
        }
    }
}

