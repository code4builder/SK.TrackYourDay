using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Expenses.Controllers;
using SK.TrackYourDay.Expenses.Data.Services;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using System;
using System.Security.Policy;
using SK.TrackYourDay.Infrastructure.DataAccess;
using SK.TrackYourDay.UseCases.Expenses.Services;
using FluentAssertions;
using FluentAssertions.Execution;
using SK.TrackYourDay.UseCases.DTOs;

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

            _expensesService = new ExpensesService(_context, null);
        }

        #endregion

        [Test, Order(1)]
        public void GetAllExpensesDTOAsync_WithNoSort_NoSearchString_NoPageNumber_RoleAdmin_Test()
        {
            var expenses = _expensesService.GetAllExpensesDTOAsync("UserId1", RoleVM.Admin, "", "", null, null).Result;
            var expectedExpenseNameFirstElement = "Name expense 1";

            using (new AssertionScope())
            {
                expenses.Count().Should().Be(6);
                expenses.Sum(e => e.Amount).Should().Be(210);
                expenses.ElementAt(0).ExpenseName.Should().Be(expectedExpenseNameFirstElement);
            }
        }

        [Test, Order(2)]
        public void GetAllExpensesDTOAsync_NoSort_NoSearchString_NoPageNumber_RoleUser_Test()
        {
            var expenses = _expensesService.GetAllExpensesDTOAsync("UserId2", RoleVM.User, "", "", null, null).Result;

            using (new AssertionScope())
            {
                expenses.Count().Should().Be(3);
                expenses.Sum(e => e.Amount).Should().Be(150);
            }
        }

        [Test, Order(3)]
        public void GetAllExpensesDTOAsync_NoSort_NoSearchString_WithPageNumber_RoleAdmin_Test()
        {
            var expenses = _expensesService.GetAllExpensesDTOAsync("UserId1", RoleVM.Admin, "", "", 1, 5).Result;

            using (new AssertionScope())
            {
                expenses.Count().Should().Be(5);
                expenses.Sum(e => e.Amount).Should().Be(150);
            }
        }

        [Test, Order(4)]
        public void GetAllExpensesDTOAsync_NoSort_WithSearchString_NoPageNumber_RoleUser_Test()
        {
            var expenses = _expensesService.GetAllExpensesDTOAsync("UserId2", RoleVM.User, "", "Name expense 5", null, null).Result;
            var expectedName = "Name expense 5";

            using (new AssertionScope())
            {
                expenses.Count().Should().Be(1);
                expenses.ElementAt(0).ExpenseName.Should().Be(expectedName);
            }
        }

        [Test, Order(5)]
        public void GetAllExpensesDTOAsync_WithSort_NoSearchString_NoPageNumber_RoleAdmin_Test()
        {
            var expenses = _expensesService.GetAllExpensesDTOAsync("UserId1", RoleVM.Admin, "name_desc", "", null, null).Result;
            var expectedExpenseNameFirstElement = "Name expense 6";

            expenses.ElementAt(0).ExpenseName.Should().Be(expectedExpenseNameFirstElement);
        }

        [Test, Order(6)]
        public void GetExpensesByUserId_Test()
        {
            var expenses = _expensesService.GetExpensesDTOByUserIdAsync("UserId2").Result;

            expenses.Count().Should().Be(3);
        }

        [Test, Order(7)]
        public void GetExpenseDTOByIdAsync_Test()
        {
            var expenseDTO = _expensesService.GetExpenseDTOByIdAsync(5, "UserId2").Result;

            using (new AssertionScope())
            {
                expenseDTO.ExpenseName.Should().Be("Name expense 5");
                expenseDTO.ExpenseCategory.Should().Be("Groceries");
                expenseDTO.PaymentMethod.Should().Be("Cash");
                expenseDTO.UserName.Should().Be("John SmithUser");
            }
        }

        [Test, Order(8)]
        public async Task AddExpenseAsync_Test()
        {
            var expenseDTO = new ExpenseDTO()
            {
                Id = 7,
                ExpenseName = "Name expense 7",
                Description = "Description expense 7",
                Amount = 50,
                Date = DateTime.Now,
                ExpenseCategory = "Groceries",
                PaymentMethod = "Cash",
                UserName = "John SmithUser"
            };

            var addingResult = _expensesService.AddExpenseAsync(expenseDTO, "UserId2");

            using (new AssertionScope())
            {
                addingResult.Should().NotBeNull();
                addingResult.IsCompleted.Should().BeTrue();
            }

            // Remove this element from the list
            await _expensesService.DeleteExpenseByIdAsync(7);
            _context.Expenses.Count().Should().Be(6);
        }

        [Test, Order(9)]
        public void UpdateExpenseById_Test()
        {
            var expenseDTO = new ExpenseDTO()
            {
                Id = 4,
                ExpenseName = "Name expense 4 upd",
                Description = "Description expense 4",
                Amount = 50,
                Date = DateTime.Now,
                ExpenseCategory = "2",
                PaymentMethod = "2",
                UserName = "John SmithUser"
            };

            var updateResult = _expensesService.UpdateExpenseByIdAsync(4, expenseDTO, "UserId2").Result;

            using (new AssertionScope())
            {
                updateResult.Should().NotBeNull();
                updateResult.ExpenseName.Should().Be("Name expense 4 upd");
                updateResult.ExpenseCategoryId.Should().Be(2);
                updateResult.PaymentMethodId.Should().Be(2);
            }
        }

        [Test, Order(10)]
        public void DeleteExpenseByIdAsync_Test()
        {
            _expensesService.DeleteExpenseByIdAsync(5);

            _context.Expenses.Count().Should().Be(5);
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
            var convertedExpenseDTO = _expensesService.ConvertExpenseToDTO(expense, "UserId1");

            using (new AssertionScope())
            {
                convertedExpenseDTO.Should().NotBeNull();
                convertedExpenseDTO.ExpenseCategory.Should().Be("Groceries");
                convertedExpenseDTO.PaymentMethod.Should().Be("Cash");
                convertedExpenseDTO.UserName.Should().Be("James BondAdmin");
            }
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
                    Name = "Groceries",
                    UserId = "UserId1"
                },
                new ExpenseCategory()
                {
                    Id = 2,
                    Name = "Car",
                    UserId = "UserId1"
                }
            };

            var paymentMethods = new List<PaymentMethod>()
            {
                new PaymentMethod()
                {
                    Id = 1,
                    Name = "Cash",
                    UserId = "UserId1"
                },
                new PaymentMethod()
                {
                    Id = 2,
                    Name = "Credit card",
                    UserId = "UserId1"
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