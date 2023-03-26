using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SK.TrackYourDay.Expenses.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Expense")]
        [Required]
        public string ExpenseName { get; set; }
        public string? Description { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0!")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        //public ExpenseCategory Category { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        //public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public int UserId { get; set; }

        public Expense() { }

        public Expense(int id, string expenseName, string? description, decimal amount, DateTime date, int userId)
        {
            id = Id;
            expenseName = ExpenseName;
            description = Description;
            amount = Amount;
            date = Date;
            userId = UserId;
        }
    }
}
