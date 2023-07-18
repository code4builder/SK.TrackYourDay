using Microsoft.AspNetCore.Identity;

namespace SK.TrackYourDay.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool HasUserRelations { get; set; }

        // Navigation Properties
        public virtual ICollection<User_Relation> RelationFrom { get; set; }
        public virtual ICollection<User_Relation> RelationTo { get; set; }
        public List<ExpenseCategory> expenseCategories { get; set; }
        public List<PaymentMethod> paymentMethods { get; set; }
    }
}
