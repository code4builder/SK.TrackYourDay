using System.ComponentModel;

namespace SK.TrackYourDay.Expenses.Models.ViewModels
{
    public class PaymentMethodVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DisplayName("Created by")]
        public string User { get; set; } = "Unknown";
    }
}
