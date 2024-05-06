using System.Diagnostics;

namespace SK.TrackYourDay.Expenses.Models.ViewModels;

public class TotalsVM
{
    public decimal AllExpenses { get; set; }
    public List<YearExpensesVM> YearExpenses { get; set; }

    [DebuggerDisplay("{Year} with total: {YearTotal}")]
    public class YearExpensesVM
    {
        public int Year { get; set; }
        public MonthExpensesVM[] monthExpensesYear = new MonthExpensesVM[12];
        public Dictionary<ExpenseCategoryVM, decimal> YearExpensesByCategory { get; set; }
        public Dictionary<ExpenseCategoryVM, decimal> IrregularYearExpensesByCategory { get; set; }
        public Dictionary<ExpenseCategoryVM, decimal> RegularYearExpensesByCategory { get; set; }
        public decimal YearTotal { get; set; }
    }

    [DebuggerDisplay("{MonthName} with total: {MonthTotal}")]
    public class MonthExpensesVM
    {
        public string MonthName { get; set; }
        public Dictionary<ExpenseCategoryVM, decimal> MonthExpensesByCategory { get; set; }
        public Dictionary<ExpenseCategoryVM, decimal> IrregularMonthExpensesByCategory { get; set; }
        public Dictionary<ExpenseCategoryVM, decimal> RegularMonthExpensesByCategory { get; set; }

        public decimal MonthTotal { get; set; }
        public decimal IrregularAmount { get; set; }
    }
}
