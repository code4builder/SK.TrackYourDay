using System.ComponentModel.DataAnnotations;

namespace SK.TrackYourDay.Expenses.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public ExpenseCategory Category { get; set; }
        public string Date { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
