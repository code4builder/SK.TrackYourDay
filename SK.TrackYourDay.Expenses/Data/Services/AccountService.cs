﻿using Microsoft.AspNetCore.Identity;
using SK.TrackYourDay.Expenses.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;

namespace SK.TrackYourDay.Expenses.Data.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _db;
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;

        public AccountService(ApplicationDbContext db, UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        internal async Task<SignInResult> LoginAsync(LoginVM model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            return result;
        }

        internal async Task CreateRolesAsync()
        {
            if (!_roleManager.RoleExistsAsync(RoleVM.Admin).GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole(RoleVM.Admin));
                await _roleManager.CreateAsync(new IdentityRole(RoleVM.User));
            }
        }

        internal async Task<IdentityResult> CreateUserAsync(RegisterVM model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.RoleName);
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return result;
        }

        internal async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
