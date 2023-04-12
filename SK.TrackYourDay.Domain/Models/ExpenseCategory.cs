using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SK.TrackYourDay.Domain.Models
{
    public class ExpenseCategory
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Expense Category")]
        [Required]
        public string Name { get; set; }

        public ExpenseCategory() {}
    }
}
