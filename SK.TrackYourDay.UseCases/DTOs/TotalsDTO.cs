using System.Diagnostics;

namespace SK.TrackYourDay.UseCases.DTOs;

public class TotalsDTO
{
    public List<YearExpensesDTO> YearExpenses { get; private set; }
    public decimal AllExpenses { get; private set; }

    public TotalsDTO(List<ExpenseDTO> expenses, List<ExpenseCategoryDTO> categories)
    {
        AllExpenses = expenses.Sum(x => x.Amount);

        YearExpenses = CalculateYearTotals(expenses, categories);
    }

    private List<YearExpensesDTO> CalculateYearTotals(List<ExpenseDTO> expenses, List<ExpenseCategoryDTO> categories)
    {
        List<YearExpensesDTO> expensesByYear = new();
        var years = expenses.Select(x => x.Date.Year).Distinct().ToList();
        foreach (var year in years)
        {
            YearExpensesDTO yearExpenses = new() { Year = year };
            yearExpenses.YearExpensesByCategory = new Dictionary<ExpenseCategoryDTO, decimal>();
            yearExpenses.IrregularYearExpensesByCategory = new Dictionary<ExpenseCategoryDTO, decimal>();
            yearExpenses.RegularYearExpensesByCategory = new Dictionary<ExpenseCategoryDTO, decimal>();

            MonthExpensesDTO[] expensesByMonth = new MonthExpensesDTO[12];
            for (int i = 1; i <= 12; i++)
            {
                expensesByMonth[i - 1] = new MonthExpensesDTO();
                expensesByMonth[i - 1].MonthName = Enum.GetName(typeof(Month), i);

                expensesByMonth[i - 1].MonthExpensesByCategory = new Dictionary<ExpenseCategoryDTO, decimal>();
                expensesByMonth[i - 1].IrregularMonthExpensesByCategory = new Dictionary<ExpenseCategoryDTO, decimal>();
                expensesByMonth[i - 1].RegularMonthExpensesByCategory = new Dictionary<ExpenseCategoryDTO, decimal>();

                if (expenses.Any(x => x.Date.Month == i && x.Date.Year == year))
                {
                    expensesByMonth[i - 1].MonthTotal = expenses.Where(x => x.Date.Month == i && x.Date.Year == year).Sum(x => x.Amount);
                    yearExpenses.YearTotal += expensesByMonth[i - 1].MonthTotal;

                    foreach (var category in categories)
                    {
                        expensesByMonth[i - 1].MonthExpensesByCategory.Add(category, expenses.Where(x => x.Date.Month == i && x.Date.Year == year && x.ExpenseCategory == category.Name).Sum(x => x.Amount));
                        expensesByMonth[i - 1].IrregularMonthExpensesByCategory.Add(category, expenses.Where(x => x.Date.Month == i && x.Date.Year == year && x.ExpenseCategory == category.Name && x.IrregularPayment is true).Sum(x => x.Amount));
                        expensesByMonth[i - 1].RegularMonthExpensesByCategory.Add(category, expenses.Where(x => x.Date.Month == i && x.Date.Year == year && x.ExpenseCategory == category.Name && x.IrregularPayment is false).Sum(x => x.Amount));

                        if (yearExpenses.YearExpensesByCategory.ContainsKey(category))
                        {
                            yearExpenses.YearExpensesByCategory[category] += expensesByMonth[i - 1].MonthExpensesByCategory[category];
                            yearExpenses.IrregularYearExpensesByCategory[category] += expensesByMonth[i - 1].IrregularMonthExpensesByCategory[category];
                            yearExpenses.RegularYearExpensesByCategory[category] += expensesByMonth[i - 1].RegularMonthExpensesByCategory[category];
                        }
                        else
                        {
                            yearExpenses.YearExpensesByCategory.Add(category, expensesByMonth[i - 1].MonthExpensesByCategory[category]);
                            yearExpenses.IrregularYearExpensesByCategory.Add(category, expensesByMonth[i - 1].IrregularMonthExpensesByCategory[category]);
                            yearExpenses.RegularYearExpensesByCategory.Add(category, expensesByMonth[i - 1].RegularMonthExpensesByCategory[category]);
                        }
                    }

                    expensesByMonth[i - 1].IrregularAmount = expenses.Where(x => x.Date.Month == i && x.Date.Year == year && x.IrregularPayment is true).Sum(x => x.Amount);

                }
                else
                {
                    expensesByMonth[i - 1].MonthTotal = 0;
                    foreach (var category in categories)
                    {
                        expensesByMonth[i - 1].MonthExpensesByCategory.Add(category, 0);
                        expensesByMonth[i - 1].IrregularMonthExpensesByCategory.Add(category, 0);
                        expensesByMonth[i - 1].RegularMonthExpensesByCategory.Add(category, 0);
                    }
                }

                yearExpenses.monthExpensesYear = expensesByMonth;
            }
            expensesByYear.Add(yearExpenses);
        }

        return expensesByYear;
    }

    [DebuggerDisplay("{Year} with total: {YearTotal}")]
    public class YearExpensesDTO
    {
        public int Year { get; set; }
        public MonthExpensesDTO[] monthExpensesYear = new MonthExpensesDTO[12];
        public Dictionary<ExpenseCategoryDTO, decimal> YearExpensesByCategory { get; set; }
        public Dictionary<ExpenseCategoryDTO, decimal> IrregularYearExpensesByCategory { get; set; }
        public Dictionary<ExpenseCategoryDTO, decimal> RegularYearExpensesByCategory { get; set; }
        public decimal YearTotal { get; set; }
    }

    [DebuggerDisplay("{MonthName} with total: {MonthTotal}")]
    public class MonthExpensesDTO
    {
        public string MonthName { get; set; }
        public Dictionary<ExpenseCategoryDTO, decimal> MonthExpensesByCategory { get; set; }
        public Dictionary<ExpenseCategoryDTO, decimal> IrregularMonthExpensesByCategory { get; set; }
        public Dictionary<ExpenseCategoryDTO, decimal> RegularMonthExpensesByCategory { get; set; }

        public decimal MonthTotal { get; set; }
        public decimal IrregularAmount { get; set; }
    }

    internal enum Month
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }
}