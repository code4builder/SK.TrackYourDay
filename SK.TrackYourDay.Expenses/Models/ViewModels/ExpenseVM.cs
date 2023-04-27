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
        [Required(ErrorMessage = "Expense is required")]
        public string ExpenseName { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        [Required]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0!")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date is required")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [DisplayName("Expense Category")]
        [Required(ErrorMessage = "Category is required")]
        public string ExpenseCategory { get; set; } = "Other";

        [DisplayName("Payment Method")]
        [Required(ErrorMessage = "Payment Method is required")]
        public string PaymentMethod { get; set; } = "Other";

        [DisplayName("Created By")]
        public string UserName { get; set; } = "Unknown";
    }
}
