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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User_User>()
                .HasOne(b => b.User1)
                .WithMany(ba => ba.TeamMates)
                .HasForeignKey(bi => bi.User1Id);

            modelBuilder.Entity<User_User>()
                .HasOne(b => b.User2)
                .WithMany(ba => ba.TeamMates)
                .HasForeignKey(bi => bi.User2Id);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<User_User> TeamMates { get; set; }

        public DbSet<Expense> Expenses { get; private set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; private set; }
        public DbSet<PaymentMethod> PaymentMethods { get; private set; }
    }
}
