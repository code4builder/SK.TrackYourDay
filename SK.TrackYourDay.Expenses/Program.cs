using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using Serilog;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Expenses.Data.Services;
using SK.TrackYourDay.Expenses.DbInitializer;
using SK.TrackYourDay.Expenses.Exceptions;
using SK.TrackYourDay.Infrastructure.DataAccess;
using SK.TrackYourDay.UseCases.Expenses.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using SK.TrackYourDay.Expenses.Middleware;

namespace SK.TrackYourDay.Expenses
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                Log.Information("Starting the Web Host");

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

                builder.Services.AddAutoMapper(typeof(Program).Assembly);

                builder.Host.UseSerilog();

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

                app.UseSerilogRequestLogging();

                app.UseRouting();

                // use dbInitializer
                using (var scope = app.Services.CreateScope())
                {
                    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                    dbInitializer.Initialize();
                }

                app.UseAuthentication();
                app.UseAuthorization();

                app.UseMiddleware<LogUserNameMiddleware>();

                // Exception Handling
                app.ConfigureBuildInExceptionHandler();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}