using SK.TrackYourDay.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK.TrackYourDay.UseCases.DTOs
{
    public class TotalsDTO
    {
        List<ExpenseDTO> _expenses;
        List<ExpenseCategoryDTO> _categories;

        public TotalsDTO(List<ExpenseDTO> expenses, List<ExpenseCategoryDTO> categories)
        {
            _expenses = expenses;
            _categories = categories;

            AllExpenses = expenses.Sum(x => x.Amount);

            CalculateMonthTotals(_expenses);
            CalculateYearTotals(_expenses);
            CalculateCategoriesTotals(_expenses, _categories);
        }

        #region MonthExpensesProperties
        public decimal AllExpenses { get; private set; }
        public decimal ExpensesMonth01CurrentYear { get; private set; }
        public decimal ExpensesMonth02CurrentYear { get; private set; }
        public decimal ExpensesMonth03CurrentYear { get; private set; }
        public decimal ExpensesMonth04CurrentYear { get; private set; }
        public decimal ExpensesMonth05CurrentYear { get; private set; }
        public decimal ExpensesMonth06CurrentYear { get; private set; }
        public decimal ExpensesMonth07CurrentYear { get; private set; }
        public decimal ExpensesMonth08CurrentYear { get; private set; }
        public decimal ExpensesMonth09CurrentYear { get; private set; }
        public decimal ExpensesMonth10CurrentYear { get; private set; }
        public decimal ExpensesMonth11CurrentYear { get; private set; }
        public decimal ExpensesMonth12CurrentYear { get; private set; }

        public decimal ExpensesMonth01PreviousYear { get; private set; }
        public decimal ExpensesMonth02PreviousYear { get; private set; }
        public decimal ExpensesMonth03PreviousYear { get; private set; }
        public decimal ExpensesMonth04PreviousYear { get; private set; }
        public decimal ExpensesMonth05PreviousYear { get; private set; }
        public decimal ExpensesMonth06PreviousYear { get; private set; }
        public decimal ExpensesMonth07PreviousYear { get; private set; }
        public decimal ExpensesMonth08PreviousYear { get; private set; }
        public decimal ExpensesMonth09PreviousYear { get; private set; }
        public decimal ExpensesMonth10PreviousYear { get; private set; }
        public decimal ExpensesMonth11PreviousYear { get; private set; }
        public decimal ExpensesMonth12PreviousYear { get; private set; }
        #endregion

        #region YearExpensesProperties
        public decimal ExpensesCurrentYear { get; private set; }
        public decimal ExpensesPreviousYear { get; private set; }
        #endregion

        #region CategoriesExpensesPropertiesCurrentYear
        public decimal Category00Expenses { get; private set; }
        public string Category00Name { get; private set; }
        public decimal Category01Expenses { get; private set; }
        public string Category01Name { get; private set; }
        public decimal Category02Expenses { get; private set; }
        public string Category02Name { get; private set; }
        public decimal Category03Expenses { get; private set; }
        public string Category03Name { get; private set; }
        public decimal Category04Expenses { get; private set; }
        public string Category04Name { get; private set; }
        public decimal Category05Expenses { get; private set; }
        public string Category05Name { get; private set; }
        public decimal Category06Expenses { get; private set; }
        public string Category06Name { get; private set; }
        public decimal Category07Expenses { get; private set; }
        public string Category07Name { get; private set; }
        public decimal Category08Expenses { get; private set; }
        public string Category08Name { get; private set; }
        public decimal Category09Expenses { get; private set; }
        public string Category09Name { get; private set; }
        public decimal Category10Expenses { get; private set; }
        public string Category10Name { get; private set; }
        public decimal Category11Expenses { get; private set; }
        public string Category11Name { get; private set; }
        public decimal Category12Expenses { get; private set; }
        public string Category12Name { get; private set; }
        public decimal Category13Expenses { get; private set; }
        public string Category13Name { get; private set; }
        public decimal Category14Expenses { get; private set; }
        public string Category14Name { get; private set; }
        public decimal Category15Expenses { get; private set; }
        public string Category15Name { get; private set; }
        public decimal Category16Expenses { get; private set; }
        public string Category16Name { get; private set; }
        public decimal Category17Expenses { get; private set; }
        public string Category17Name { get; private set; }
        public decimal Category18Expenses { get; private set; }
        public string Category18Name { get; private set; }
        public decimal Category19Expenses { get; private set; }
        public string Category19Name { get; private set; }
        #endregion

        #region Methods
        private void CalculateMonthTotals(List<ExpenseDTO> expenses)
        {
            ExpensesMonth01CurrentYear = expenses.Where(x => x.Date.Month == 1 && x.Date.Year == DateTime.Now.Year).Sum(x => x.Amount);
            ExpensesMonth02CurrentYear = expenses.Where(x => x.Date.Month == 2 && x.Date.Year == DateTime.Now.Year).Sum(x => x.Amount);
            ExpensesMonth03CurrentYear = expenses.Where(x => x.Date.Month == 3 && x.Date.Year == DateTime.Now.Year).Sum(x => x.Amount);
            ExpensesMonth04CurrentYear = expenses.Where(x => x.Date.Month == 4 && x.Date.Year == DateTime.Now.Year).Sum(x => x.Amount);
            ExpensesMonth05CurrentYear = expenses.Where(x => x.Date.Month == 5 && x.Date.Year == DateTime.Now.Year).Sum(x => x.Amount);
            ExpensesMonth06CurrentYear = expenses.Where(x => x.Date.Month == 6 && x.Date.Year == DateTime.Now.Year).Sum(x => x.Amount);
            ExpensesMonth07CurrentYear = expenses.Where(x => x.Date.Month == 7 && x.Date.Year == DateTime.Now.Year).Sum(x => x.Amount);
            ExpensesMonth08CurrentYear = expenses.Where(x => x.Date.Month == 8 && x.Date.Year == DateTime.Now.Year).Sum(x => x.Amount);
            ExpensesMonth09CurrentYear = expenses.Where(x => x.Date.Month == 9 && x.Date.Year == DateTime.Now.Year).Sum(x => x.Amount);
            ExpensesMonth10CurrentYear = expenses.Where(x => x.Date.Month == 10 && x.Date.Year == DateTime.Now.Year).Sum(x => x.Amount);
            ExpensesMonth11CurrentYear = expenses.Where(x => x.Date.Month == 11 && x.Date.Year == DateTime.Now.Year).Sum(x => x.Amount);
            ExpensesMonth12CurrentYear = expenses.Where(x => x.Date.Month == 12 && x.Date.Year == DateTime.Now.Year).Sum(x => x.Amount);

            ExpensesMonth01PreviousYear = expenses.Where(x => x.Date.Month == 1 && x.Date.Year == DateTime.Now.Year - 1).Sum(x => x.Amount);
            ExpensesMonth02PreviousYear = expenses.Where(x => x.Date.Month == 2 && x.Date.Year == DateTime.Now.Year - 1).Sum(x => x.Amount);
            ExpensesMonth03PreviousYear = expenses.Where(x => x.Date.Month == 3 && x.Date.Year == DateTime.Now.Year - 1).Sum(x => x.Amount);
            ExpensesMonth04PreviousYear = expenses.Where(x => x.Date.Month == 4 && x.Date.Year == DateTime.Now.Year - 1).Sum(x => x.Amount);
            ExpensesMonth05PreviousYear = expenses.Where(x => x.Date.Month == 5 && x.Date.Year == DateTime.Now.Year - 1).Sum(x => x.Amount);
            ExpensesMonth06PreviousYear = expenses.Where(x => x.Date.Month == 6 && x.Date.Year == DateTime.Now.Year - 1).Sum(x => x.Amount);
            ExpensesMonth07PreviousYear = expenses.Where(x => x.Date.Month == 7 && x.Date.Year == DateTime.Now.Year - 1).Sum(x => x.Amount);
            ExpensesMonth08PreviousYear = expenses.Where(x => x.Date.Month == 8 && x.Date.Year == DateTime.Now.Year - 1).Sum(x => x.Amount);
            ExpensesMonth09PreviousYear = expenses.Where(x => x.Date.Month == 9 && x.Date.Year == DateTime.Now.Year - 1).Sum(x => x.Amount);
            ExpensesMonth10PreviousYear = expenses.Where(x => x.Date.Month == 10 && x.Date.Year == DateTime.Now.Year - 1).Sum(x => x.Amount);
            ExpensesMonth11PreviousYear = expenses.Where(x => x.Date.Month == 11 && x.Date.Year == DateTime.Now.Year - 1).Sum(x => x.Amount);
            ExpensesMonth12PreviousYear = expenses.Where(x => x.Date.Month == 12 && x.Date.Year == DateTime.Now.Year - 1).Sum(x => x.Amount);
        }

        private void CalculateYearTotals(List<ExpenseDTO> expenses)
        {
            ExpensesCurrentYear = expenses.Where(x => x.Date.Year == DateTime.Now.Year).Sum(x => x.Amount);
            ExpensesPreviousYear = expenses.Where(x => x.Date.Year == DateTime.Now.Year - 1).Sum(x => x.Amount);
        }

        private void CalculateCategoriesTotals(List<ExpenseDTO> expenses, List<ExpenseCategoryDTO> categories)
        {
            List<ExpenseDTO> expensesCurYear = expenses.Where(x => x.Date.Year == DateTime.Now.Year).ToList();
            Category00Name = categories.ElementAtOrDefault(0)?.Name;
            Category00Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(0)?.Name).Sum(x => x.Amount);
            Category01Name = categories.ElementAtOrDefault(1)?.Name;
            Category01Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(1)?.Name).Sum(x => x.Amount);
            Category02Name = categories.ElementAtOrDefault(2)?.Name;
            Category02Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(2)?.Name).Sum(x => x.Amount);
            Category03Name = categories.ElementAtOrDefault(3)?.Name;
            Category03Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(3)?.Name).Sum(x => x.Amount);
            Category04Name = categories.ElementAtOrDefault(4)?.Name;
            Category04Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(4)?.Name).Sum(x => x.Amount);
            Category05Name = categories.ElementAtOrDefault(5)?.Name;
            Category05Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(5)?.Name).Sum(x => x.Amount);
            Category06Name = categories.ElementAtOrDefault(6)?.Name;
            Category06Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(6)?.Name).Sum(x => x.Amount);
            Category07Name = categories.ElementAtOrDefault(7)?.Name;
            Category07Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(7)?.Name).Sum(x => x.Amount);
            Category08Name = categories.ElementAtOrDefault(8)?.Name;
            Category08Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(8)?.Name).Sum(x => x.Amount);
            Category09Name = categories.ElementAtOrDefault(9)?.Name;
            Category09Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(9)?.Name).Sum(x => x.Amount);
            Category10Name = categories.ElementAtOrDefault(10)?.Name;
            Category10Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(10)?.Name).Sum(x => x.Amount);
            Category11Name = categories.ElementAtOrDefault(11)?.Name;
            Category11Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(11)?.Name).Sum(x => x.Amount);
            Category12Name = categories.ElementAtOrDefault(12)?.Name;
            Category12Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(12)?.Name).Sum(x => x.Amount);
            Category13Name = categories.ElementAtOrDefault(13)?.Name;
            Category13Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(13)?.Name).Sum(x => x.Amount);
            Category14Name = categories.ElementAtOrDefault(14)?.Name;
            Category14Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(14)?.Name).Sum(x => x.Amount);
            Category15Name = categories.ElementAtOrDefault(15)?.Name;
            Category15Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(15)?.Name).Sum(x => x.Amount);
            Category16Name = categories.ElementAtOrDefault(16)?.Name;
            Category16Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(16)?.Name).Sum(x => x.Amount);
            Category17Name = categories.ElementAtOrDefault(17)?.Name;
            Category17Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(17)?.Name).Sum(x => x.Amount);
            Category18Name = categories.ElementAtOrDefault(18)?.Name;
            Category18Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(18)?.Name).Sum(x => x.Amount);
            Category19Name = categories.ElementAtOrDefault(19)?.Name;
            Category19Expenses = expensesCurYear.Where(x => x.ExpenseCategory == categories.ElementAtOrDefault(19)?.Name).Sum(x => x.Amount);
        }
        #endregion
    }
}
