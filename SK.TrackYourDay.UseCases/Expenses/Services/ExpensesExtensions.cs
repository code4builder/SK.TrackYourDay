﻿using SK.TrackYourDay.UseCases.DTOs;

namespace SK.TrackYourDay.UseCases.Expenses.Services
{
    internal static class ExpensesExtensions
    {
        internal static IEnumerable<ExpenseDTO> FilterByDateRange(this IEnumerable<ExpenseDTO> expenses, FilterDTO filterDTO)
        {
            DateTime? DateTimeIncludesLastDay = filterDTO.DateTo;

            if (filterDTO.DateTo.Value.Hour == 0 && filterDTO.DateTo.Value.Minute == 0 && filterDTO.DateTo.Value.Second == 0)
                DateTimeIncludesLastDay = filterDTO.DateTo?.AddHours(23).AddMinutes(59).AddSeconds(59);

            return expenses.Where(x => x.Date >= filterDTO.DateFrom && x.Date <= DateTimeIncludesLastDay);
        }

        internal static IEnumerable<ExpenseDTO> FilterByExpenseName(this IEnumerable<ExpenseDTO> expenses, FilterDTO filterDTO)
        {
            if (!string.IsNullOrEmpty(filterDTO.ExpenseName))
                return expenses.Where(x => (x.ExpenseName?.ToLower() ?? "").Contains(filterDTO.ExpenseName.ToLower()));

            return expenses;
        }

        internal static IEnumerable<ExpenseDTO> FilterByDescription(this IEnumerable<ExpenseDTO> expenses, FilterDTO filterDTO)
        {
            if (!string.IsNullOrEmpty(filterDTO.Description))
                return expenses.Where(x => (x.Description?.ToLower() ?? "").Contains(filterDTO.Description.ToLower()));

            return expenses;
        }

        internal static IEnumerable<ExpenseDTO> FilterByPaymentMethod(this IEnumerable<ExpenseDTO> expenses, string requestedPaymentMethodName)
        {
            if (!string.IsNullOrEmpty(requestedPaymentMethodName))
                return expenses.Where(x => x.PaymentMethod.ToString() == requestedPaymentMethodName);

            return expenses;
        }

        internal static IEnumerable<ExpenseDTO> FilterByExpenseCategory(this IEnumerable<ExpenseDTO> expenses, string requestedExpenseCategoryName)
        {
            if (!string.IsNullOrEmpty(requestedExpenseCategoryName))
                return expenses.Where(x => x.ExpenseCategory.ToString() == requestedExpenseCategoryName);

            return expenses;
        }

        internal static IEnumerable<ExpenseDTO> FilterByAmountRange(this IEnumerable<ExpenseDTO> expenses, FilterDTO filterDTO)
        {
            if (filterDTO.AmountFrom != 0)
                expenses.Where(x => x.Amount >= filterDTO.AmountFrom);

            if (filterDTO.AmountTo != 0)
                expenses.Where(x => x.Amount <= filterDTO.AmountTo);

            return expenses;
        }

        internal static IEnumerable<ExpenseDTO> FilterByPaymentRegularity(this IEnumerable<ExpenseDTO> expenses, FilterDTO filterDTO)
        {
            if (filterDTO.IrregularPayment is true && filterDTO.RegularPayment is true)
                return expenses;
            else if (filterDTO.IrregularPayment is true && filterDTO.RegularPayment is false)
                return expenses.Where(x => x.IrregularPayment);
            else if (filterDTO.IrregularPayment is false && filterDTO.RegularPayment is true)
                return expenses.Where(x => x.IrregularPayment is false);
            
            return new List<ExpenseDTO>();
        }


    }
}
