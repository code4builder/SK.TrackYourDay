using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SK.TrackYourDay.UseCases.DTOs
{
    public class ExpenseDTO
    {
        public int Id { get; set; }

        public string ExpenseName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public string ExpenseCategory { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public string UserName { get; set; } = "Unknown";

        public bool IrregularPayment { get; set; }
    }
}
