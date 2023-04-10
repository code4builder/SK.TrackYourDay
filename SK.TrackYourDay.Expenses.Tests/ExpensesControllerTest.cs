using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SK.TrackYourDay.Expenses.Controllers;
using SK.TrackYourDay.Expenses.Data;
using SK.TrackYourDay.Expenses.Data.Services;
using SK.TrackYourDay.Expenses.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using System;
using System.Security.Claims;
using System.Security.Policy;

namespace SK.TrackYourDay.Expenses.Tests
{
    public class ExpensesControllerTest
    {
        private static DbContextOptions<ApplicationDbContext> dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "ExpensesDbTest")
        .Options;

        ApplicationDbContext _context;
        ExpensesService _expensesService;
        ExpensesController _expensesController;
        PaymentMethodsService _paymentMethodsService;

        [OneTimeSetUp]
        public void Setup()
        {
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureCreated();

            SeedDatabase();

            _expensesService = new ExpensesService(_context, null, null);
            _expensesController = new ExpensesController(_expensesService, null, null);

            _expensesController.ControllerContext = new ControllerContext();
            _expensesController.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "UserId1"),
                    new Claim(ClaimTypes.Name, "user1@mail.com"),
                    new Claim(ClaimTypes.Email, "user1@mail.com"),
                    new Claim(ClaimTypes.Role, "Admin")
                }))
            };
        }


        [Test, Order(1)]
        public void HTTPGET_GetAllExpenses_WithSortBySearchStringPageNumber_ReturnOk_Test()
        {
            IActionResult actionResult = _expensesController.Index("name_desc", "name", 1, 5).Result;

            Assert.That(actionResult, Is.TypeOf<ViewResult>());

            var actionResultData = (actionResult as ViewResult).Model as List<ExpenseVM>;

            Assert.That(actionResultData.First().ExpenseName, Is.EqualTo("Expense name 6"));
            Assert.That(actionResultData.Count, Is.EqualTo(5));
        }

        [Test, Order(2)]
        public void HTTPGET_GetAllExpenses_WithSortBySearchStringPageNumber_ReturnVoidList_Test()
        {
            IActionResult actionResult = _expensesController.Index("", "wrong", 1, 5).Result;
            var actionResultData = (actionResult as ViewResult).Model as List<ExpenseVM>;
            
            Assert.That(actionResult, Is.TypeOf<ViewResult>());
            Assert.That(actionResultData.Count, Is.EqualTo(0));
        }

        [Test, Order(3)]
        public void HTTPPOST_CreateExpense_ReturnsCreated_Test()
        {
            var expenseVM = new ExpenseVM()
            {
                Id = 7,
                ExpenseName = "Name expense 7",
                Description = "Description expense 7",
                Amount = 70,
                Date = DateTime.Now.AddDays(-3),
                ExpenseCategory = "1",
                PaymentMethod = "1",
                UserName = "John SmithUser"
            };

            var actionResult = _expensesController.Create(expenseVM).Result;

            Assert.That(actionResult, Is.TypeOf<RedirectToActionResult>());

            IActionResult actionResultIndex = _expensesController.Index("","",1,10).Result;
            var actionResultData = (actionResultIndex as ViewResult).Model as List<ExpenseVM>;

            Assert.That(actionResultData.Count(), Is.EqualTo(7));
        }

        [Test, Order(4)]
        public void HTTPPOST_CreateExpense_ReturnsBadRequest_Test()
        {
            var expenseVM = new ExpenseVM()
            {
                Id = 7,
                ExpenseName = "Name expense 7",
                Description = "Description expense 7",
                Amount = 70,
                Date = DateTime.Now.AddDays(-3),
                ExpenseCategory = "3",
                PaymentMethod = "1",
                UserName = "John SmithUser"
            };

            var actionResult = _expensesController.Create(expenseVM).Result;

            Assert.That(actionResult, Is.TypeOf<RedirectToActionResult>());

            IActionResult actionResultIndex = _expensesController.Index("", "", 1, 10).Result;
            var actionResultData = (actionResultIndex as ViewResult).Model as List<ExpenseVM>;

            Assert.That(actionResultData.Count(), Is.EqualTo(6));
        }

        [Test, Order(5)]
        public void HTTPPOST_UpdateExpense_ReturnsUpdated_Test()
        {
            var expenseVM = new ExpenseVM()
            {
                Id = 4,
                ExpenseName = "Name expense 4 Updated",
                Description = "Description expense 4 Updated",
                Amount = 50,
                Date = DateTime.Now.AddDays(-3),
                ExpenseCategory = "1",
                PaymentMethod = "1",
                UserName = "John SmithUser"
            };

            var actionResult = _expensesController.Update(expenseVM).Result;
            Assert.That(actionResult, Is.TypeOf<RedirectToActionResult>());
            Assert.That(_context.Expenses.FirstOrDefault(x => x.Id == 4).ExpenseName, Is.EqualTo("Name expense 4 Updated"));
            Assert.That(_context.Expenses.FirstOrDefault(x => x.Id == 4).Amount, Is.EqualTo(50));
        }

        [Test, Order(6)]
        public void HTTPPOST_UpdateExpense_ReturnsBadRequest_Test()
        {
            var expenseVM = new ExpenseVM()
            {
                Id = 4,
                ExpenseName = "Name expense 4 Updated",
                Description = "Description expense 4 Updated",
                Amount = 50,
                Date = DateTime.Now.AddDays(-3),
                ExpenseCategory = "5",
                PaymentMethod = "1",
                UserName = "John SmithUser"
            };

            var actionResult = _expensesController.Update(expenseVM).Result;
            Assert.That(actionResult, Is.TypeOf<BadRequestResult>());
            
        }

        [Test, Order(7)]
        public void HTTPPOST_DeleteExpense_ReturnsDeleted_Test()
        {
            var actionResult = _expensesController.DeletePost(4).Result;

            Assert.That(actionResult, Is.TypeOf<RedirectToActionResult>());

            IActionResult actionResultIndex = _expensesController.Index("", "", 1, 10).Result;
            var actionResultData = (actionResultIndex as ViewResult).Model as List<ExpenseVM>;

            Assert.That(actionResultData.Count(), Is.EqualTo(5));
        }

        [Test, Order(8)]
        public void HTTPPOST_DeleteExpense_ReturnsBadRequest_Test()
        {
            var actionResult = _expensesController.DeletePost(10).Result;

            Assert.That(actionResult, Is.TypeOf<RedirectToActionResult>());

            IActionResult actionResultIndex = _expensesController.Index("", "", 1, 10).Result;
            var actionResultData = (actionResultIndex as ViewResult).Model as List<ExpenseVM>;

            Assert.That(actionResultData.Count(), Is.EqualTo(5));
        }

        private void SeedDatabase()
        {
            var expenses = new List<Expense>()
            {
                new Expense()
                {
                    Id = 1,
                    ExpenseName = "Expense name 1",
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
                    ExpenseName = "Expense name 2",
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
                    ExpenseName = "Expense name 3",
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
                    ExpenseName = "Expense name 4",
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
                    ExpenseName = "Expense name 5",
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
                    ExpenseName = "Expense name 6",
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