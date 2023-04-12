using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Expenses.Controllers;
using SK.TrackYourDay.Expenses.Data;
using SK.TrackYourDay.Expenses.Data.Services;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using System;
using System.Security.Policy;

namespace SK.TrackYourDay.Expenses.Tests
{
    public class ExpensesServiceTest
    {
        #region Private Fields and Setup
        private static DbContextOptions<ApplicationDbContext> dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "ExpensesDbTest")
        .Options;

        ApplicationDbContext _context;
        ExpensesService _expensesService;

        [OneTimeSetUp]
        public void Setup()
        {
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureCreated();

            SeedDatabase();

            _expensesService = new ExpensesService(_context, null, null);
        }

        #endregion

        [Test, Order(1)]
        public void GetAllExpensesVMAsync_WithNoSort_NoSearchString_NoPageNumber_RoleAdmin_Test()
        {
            var result = _expensesService.GetAllExpensesVMAsync("UserId1", RoleVM.Admin, "", "", null, null).Result;
            var expenseNameFirstElement = "Name expense 1";

            Assert.That(result.Count, Is.EqualTo(6));
            Assert.That(result.Sum(e => e.Amount), Is.EqualTo(210));
            Assert.That(result.ElementAt(0).ExpenseName, Is.EqualTo(expenseNameFirstElement));
        }

        [Test, Order(2)]
        public void GetAllExpensesVMAsync_NoSort_NoSearchString_NoPageNumber_RoleUser_Test()
        {
            var result = _expensesService.GetAllExpensesVMAsync("UserId2", RoleVM.User, "", "", null, null).Result;

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result.Sum(e => e.Amount), Is.EqualTo(150));
        }

        [Test, Order(3)]
        public void GetAllExpensesVMAsync_NoSort_NoSearchString_WithPageNumber_RoleAdmin_Test()
        {
            var result = _expensesService.GetAllExpensesVMAsync("UserId1", RoleVM.Admin, "", "", 1, 5).Result;

            Assert.That(result.Count, Is.EqualTo(5));
            Assert.That(result.Sum(e => e.Amount), Is.EqualTo(150));
        }

        [Test, Order(4)]
        public void GetAllExpensesVMAsync_NoSort_WithSearchString_NoPageNumber_RoleUser_Test()
        {
            var result = _expensesService.GetAllExpensesVMAsync("UserId2", RoleVM.User, "", "Name expense 5", null, null).Result;
            var expenseName = "Name expense 5";

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.ElementAt(0).ExpenseName, Is.EqualTo(expenseName));
        }

        [Test, Order(5)]
        public void GetAllExpensesVMAsync_WithSort_NoSearchString_NoPageNumber_RoleAdmin_Test()
        {
            var result = _expensesService.GetAllExpensesVMAsync("UserId1", RoleVM.Admin, "name_desc", "", null, null).Result;
            var expenseNameFirstElement = "Name expense 6";

            Assert.That(result.ElementAt(0).ExpenseName, Is.EqualTo(expenseNameFirstElement));
        }

        [Test, Order(6)]
        public void GetExpensesByUserId_Test()
        {
            var result = _expensesService.GetExpensesByUserId("UserId2");

            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test, Order(7)]
        public void GetExpenseVMByIdAsync_Test()
        {
            var result = _expensesService.GetExpenseVMByIdAsync(5, "UserId2").Result;
            var expenseVM = new ExpenseVM()
            {
                Id = 5,
                ExpenseName = "Name expense 5",
                Description = "Description expense 5",
                Amount = 50,
                Date = DateTime.Now,
                ExpenseCategory = "Groceries",
                PaymentMethod = "Cash",
                UserName = "John SmithUser"
            };

            Assert.That(result.ExpenseName, Is.EqualTo("Name expense 5"));
            Assert.That(result.ExpenseCategory, Is.EqualTo("Groceries"));
            Assert.That(result.PaymentMethod, Is.EqualTo("Cash"));
            Assert.That(result.UserName, Is.EqualTo("John SmithUser"));
        }

        [Test, Order(8)]
        public void AddExpenseAsync_Test()
        {
            var expenseVM = new ExpenseVM()
            {
                Id = 5,
                ExpenseName = "Name expense 5",
                Description = "Description expense 5",
                Amount = 50,
                Date = DateTime.Now,
                ExpenseCategory = "Groceries",
                PaymentMethod = "Cash",
                UserName = "John SmithUser"
            };

            var result = _expensesService.AddExpenseAsync(expenseVM, "UserId2");

            Assert.That(result, Is.Not.Null);
        }

        [Test, Order(9)]
        public void UpdateExpenseById_Test()
        {
            var expenseVM = new ExpenseVM()
            {
                Id = 5,
                ExpenseName = "Name expense 5",
                Description = "Description expense 5",
                Amount = 50,
                Date = DateTime.Now,
                ExpenseCategory = "1",
                PaymentMethod = "1",
                UserName = "John SmithUser"
            };

            var result = _expensesService.UpdateExpenseById(5, expenseVM).Result;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ExpenseName, Is.EqualTo("Name expense 5"));
            Assert.That(result.ExpenseCategoryId, Is.EqualTo(1));
            Assert.That(result.PaymentMethodId, Is.EqualTo(1));
        }

        [Test, Order(10)]
        public void DeleteExpenseByIdAsync_Test()
        {
            var result = _expensesService.DeleteExpenseByIdAsync(5);

            Assert.That(_context.Expenses.Count, Is.EqualTo(5));
        }

        [Test, Order(11)]
        public void ConvertExpenseToVM_Test()
        {
            var expense = new Expense()
            {
                Id = 1,
                ExpenseName = "Name expense 1",
                Description = "Description expense 1",
                Amount = 10,
                Date = DateTime.Now,
                ExpenseCategoryId = 1,
                PaymentMethodId = 1,
                UserId = "UserId1"
            };
            var result = _expensesService.ConvertExpenseToVM(expense, "UserId1");

            var expenseCategory = "Groceries";
            var paymentMethod = "Cash";
            var fullName = "James BondAdmin";

            Assert.That(result.ExpenseCategory, Is.EqualTo(expenseCategory));
            Assert.That(result.PaymentMethod, Is.EqualTo(paymentMethod));
            Assert.That(result.UserName, Is.EqualTo(fullName));
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            _context.Database.EnsureDeleted();
        }

        private void SeedDatabase()
        {
            var expenses = new List<Expense>()
            {
                new Expense()
                {
                    Id = 1,
                    ExpenseName = "Name expense 1",
                    Description = "Description expense 1",
                    Amount = 10,
                    Date = DateTime.Now,
                    ExpenseCategoryId = 1,
                    PaymentMethodId = 1,
                    UserId = "UserId1"
                },
                new Expense()
                {
                    Id = 2,
                    ExpenseName = "Name expense 2",
                    Description = "Description expense 2",
                    Amount = 20,
                    Date = DateTime.Now.AddDays(-1),
                    ExpenseCategoryId = 1,
                    PaymentMethodId = 1,
                    UserId = "UserId1"
                },
                new Expense()
                {
                    Id = 3,
                    ExpenseName = "Name expense 3",
                    Description = "Description expense 3",
                    Amount = 30,
                    Date = DateTime.Now.AddDays(-2),
                    ExpenseCategoryId = 1,
                    PaymentMethodId = 1,
                    UserId = "UserId1"
                },
                new Expense()
                {
                    Id = 4,
                    ExpenseName = "Name expense 4",
                    Description = "Description expense 4",
                    Amount = 40,
                    Date = DateTime.Now.AddDays(-3),
                    ExpenseCategoryId = 1,
                    PaymentMethodId = 1,
                    UserId = "UserId2"
                },
                new Expense()
                {
                    Id = 5,
                    ExpenseName = "Name expense 5",
                    Description = "Description expense 5",
                    Amount = 50,
                    Date = DateTime.Now.AddDays(-4),
                    ExpenseCategoryId = 1,
                    PaymentMethodId = 1,
                    UserId = "UserId2"
                },
                new Expense()
                {
                    Id = 6,
                    ExpenseName = "Name expense 6",
                    Description = "Description expense 6",
                    Amount = 60,
                    Date = DateTime.Now.AddDays(-5),
                    ExpenseCategoryId = 1,
                    PaymentMethodId = 1,
                    UserId = "UserId2"
                }
            };

            var expenseCategories = new List<ExpenseCategory>()
            {
                new ExpenseCategory()
                {
                    Id = 1,
                    Name = "Groceries"
                },
                new ExpenseCategory()
                {
                    Id = 2,
                    Name = "Car"
                }
            };

            var paymentMethods = new List<PaymentMethod>()
            {
                new PaymentMethod()
                {
                    Id = 1,
                    Name = "Cash"
                },
                new PaymentMethod()
                {
                    Id = 2,
                    Name = "Credit card"
                }
            };

            var users = new List<ApplicationUser>()
            {
                new ApplicationUser()
                {
                    Id = "UserId1",
                    UserName = "User1",
                    Email = "user1@mail.com",
                    EmailConfirmed = true,
                    FirstName = "James",
                    LastName = "BondAdmin"
                },
                new ApplicationUser()
                {
                    Id = "UserId2",
                    UserName = "User2",
                    Email = "user2@mail.com",
                    EmailConfirmed = true,
                    FirstName = "John",
                    LastName = "SmithUser"
                }
            };

            var roles = new List<IdentityRole>()
            {
                new IdentityRole()
                {
                    Id = "RoleId1",
                    Name = "Admin"
                },
                new IdentityRole()
                {
                    Id = "RoleId2",
                    Name = "User"
                }
            };

            var userRoles = new List<IdentityUserRole<string>>()
            {
                new IdentityUserRole<string>()
                {
                    UserId = "UserId1",
                    RoleId = "RoleId1"
                },
                new IdentityUserRole<string>()
                {
                    UserId = "UserId2",
                    RoleId = "RoleId2",
                }
            };

            _context.Expenses.AddRange(expenses);
            _context.ExpenseCategories.AddRange(expenseCategories);
            _context.PaymentMethods.AddRange(paymentMethods);
            _context.Users.AddRange(users);
            _context.Roles.AddRange(roles);

            _context.SaveChanges();
        }
    }
}