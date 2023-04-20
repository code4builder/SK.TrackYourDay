using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace SK.TrackYourDay.Expenses.Models.ViewModels
{
    public class ExpenseVM
    {
        public int Id { get; set; }

        [DisplayName("Expense")]
        [Required]
        public string ExpenseName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0!")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [DisplayName("Expense Category")]
        public string ExpenseCategory { get; set; } = string.Empty;

        [DisplayName("Payment Method")]
        public string PaymentMethod { get; set; } = string.Empty;

        [DisplayName("Created By")]
        public string UserName { get; set; } = "Unknown";
    }
}
