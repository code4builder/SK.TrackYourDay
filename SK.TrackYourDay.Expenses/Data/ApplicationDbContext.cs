﻿using Microsoft.EntityFrameworkCore;
using SK.TrackYourDay.Expenses.Models;

namespace SK.TrackYourDay.Expenses.Data
{
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Expense> Expenses { get; set; }
    }
}