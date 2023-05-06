using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SK.TrackYourDay.Domain.Models;

namespace SK.TrackYourDay.Infrastructure.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User_Relation>()
                .HasKey(e => new { e.User1Id, e.User2Id });

            modelBuilder.Entity<User_Relation>()
                .HasOne(e => e.User1)
                .WithMany(e => e.RelationTo)
                .HasForeignKey(e => e.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User_Relation>()
                .HasOne(e => e.User2)
                .WithMany(e => e.RelationFrom)
                .HasForeignKey(e => e.User2Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExpenseCategory>()
                .HasOne<ApplicationUser>(ec => ec.User)
                .WithMany(u => u.expenseCategories)
                .HasForeignKey(ec => ec.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<User_Relation> User_Relations { get; set; }

        public DbSet<Expense> Expenses { get; private set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; private set; }
        public DbSet<PaymentMethod> PaymentMethods { get; private set; }
    }
}
