﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using SK.TrackYourDay.Infrastructure.DataAccess;

namespace SK.TrackYourDay.Expenses.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {

            }

            if (_db.Roles.Any(r => r.Name == RoleVM.Admin))
                return;


            _roleManager.CreateAsync(new IdentityRole(RoleVM.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(RoleVM.User)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "adminSergey",
                Email = "info@bim-s.it",
                EmailConfirmed = true,
                FirstName = "Sergey",
                LastName = "Admin"
            }, "boBBy03@").GetAwaiter().GetResult();

            ApplicationUser user = _db.Users.FirstOrDefault(u => u.Email == "info@bim-s.it");
            _userManager.AddToRoleAsync(user, RoleVM.Admin).GetAwaiter().GetResult();
        }
    }
}
