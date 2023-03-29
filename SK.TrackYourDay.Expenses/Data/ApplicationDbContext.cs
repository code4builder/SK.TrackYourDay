using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SK.TrackYourDay.Expenses.Models;

namespace SK.TrackYourDay.Expenses.Data
{
    public class ApplicationDbContext : IdentityDbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Expense> Expenses { get; private set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; private set; }
        public DbSet<PaymentMethod> PaymentMethods { get; private set; }
    }
}
