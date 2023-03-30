using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace SK.TrackYourDay.Expenses.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        // Navigation Properties
        public virtual ICollection<User_Relation> RelationFrom { get; set; }
        public virtual ICollection<User_Relation> RelationTo { get; set; }
    }
}
