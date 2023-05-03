using System.ComponentModel.DataAnnotations;

namespace SK.TrackYourDay.Expenses.Models.ViewModels
{
    public class FriendVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
