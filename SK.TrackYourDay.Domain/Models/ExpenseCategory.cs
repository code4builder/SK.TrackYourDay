using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SK.TrackYourDay.Domain.Models
{
    public class ExpenseCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Navigation Properties
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
