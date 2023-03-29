using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace SK.TrackYourDay.Expenses.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }

        // Navigation Properties
        public List<User_User> TeamMates { get; set; }
    }
}
