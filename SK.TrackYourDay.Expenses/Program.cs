using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Expenses.Data.Services;
using SK.TrackYourDay.Expenses.DbInitializer;
using SK.TrackYourDay.Expenses.Exceptions;
using SK.TrackYourDay.Infrastructure.DataAccess;
using SK.TrackYourDay.UseCases.Expenses.Services;
using System;

namespace SK.TrackYourDay.Expenses
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                           options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                           x => x.MigrationsAssembly("SK.TrackYourDay.Infrastructure.DataAccess")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            // Configure the Services
            builder.Services.AddTransient<AccountService>();
            builder.Services.AddTransient<ExpensesService>();
            builder.Services.AddTransient<ExpensesHandler>();
            builder.Services.AddTransient<ExpenseCategoriesService>();
            builder.Services.AddTransient<PaymentMethodsService>();
            builder.Services.AddScoped<IDbInitializer, DbInitializer.DbInitializer>();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // use dbInitializer
            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                dbInitializer.Initialize();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            // Exception Handling
            app.ConfigureBuildInExceptionHandler();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}