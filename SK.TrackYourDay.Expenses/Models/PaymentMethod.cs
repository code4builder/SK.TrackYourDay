using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SK.TrackYourDay.Expenses.Models
{
    public class PaymentMethod
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Payment Method")]
        [Required]
        public string Name { get; set; }
        public PaymentMethod() { }

        //Card,
        //Cash,
        //BankTransfer,
        //Other
    }
}