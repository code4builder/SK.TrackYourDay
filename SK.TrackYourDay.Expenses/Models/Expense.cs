﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
        public decimal Amount { get; set; }

        //public ExpenseCategory Category { get; set; }
        public string Date { get; set; }
        //public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
