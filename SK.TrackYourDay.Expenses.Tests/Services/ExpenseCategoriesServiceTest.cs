using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Infrastructure.DataAccess;
using SK.TrackYourDay.UseCases.DTOs;
using SK.TrackYourDay.UseCases.Expenses.Services;

namespace SK.TrackYourDay.Expenses.Tests.Services
{
    public class ExpenseCategoriesServiceTest
    {
        #region Private Fields and Setup
        private static DbContextOptions<ApplicationDbContext> dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "ExpensesDbTest")
        .Options;

        ApplicationDbContext _context;
        ExpenseCategoriesService _expenseCategoriesService;

        [OneTimeSetUp]
        public void Setup()
        {
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureCreated();

            SeedDatabase();

            _expenseCategoriesService = new ExpenseCategoriesService(_context, null);
        }

        #endregion

        [Test, Order(1)]
        public async Task GetAllExpenseCategoriesDTOAsync_ShouldReturnEmptyList_WhenNoExpenseCategoriesExist()
        {
            // Arrange
            var userId = "UserId3";

            // Act
            var result = await _expenseCategoriesService.GetAllExpenseCategoriesDTOAsync(userId);

            // Assert
            result.Should().BeEmpty();
        }

        [Test, Order(2)]
        public async Task GetAllExpenseCategoriesDTOAsync_ShouldReturnSortedExpenseCategories_WhenExpenseCategoriesExist()
        {
            // Arrange
            var userId = "UserId1";

            // Act
            var result = await _expenseCategoriesService.GetAllExpenseCategoriesDTOAsync(userId);

            // Assert: 3 expense categories for user 1 and 1 for user 2
            result.Should().HaveCount(4);
            result[0].Name.Should().Be("Bills");
            result[1].Name.Should().Be("Car");
        }

        [Test, Order(3)]
        public async Task GetExpenseCategoriesDTOByUserId_ShouldReturnEmptyList_WhenNoExpenseCategoriesExist()
        {
            // Arrange
            var userId = "UserId3";

            // Act
            var result = await _expenseCategoriesService.GetExpenseCategoriesDTOByUserIdAsync(userId);

            // Assert
            result.Should().BeEmpty();
        }

        [Test, Order(4)]
        public async Task GetExpenseCategoriesDTOByUserId_ShouldReturnExpenseCategoriesDTO_WhenExpenseCategoriesExist()
        {
            // Arrange
            var userId = "UserId1";

            // Act
            var result = await _expenseCategoriesService.GetExpenseCategoriesDTOByUserIdAsync(userId);

            // Assert
            result.Should().HaveCount(3);
        }

        [Test, Order(5)]
        public async Task GetExpenseCategoryByIdAsync_ShouldReturnNull_WhenExpenseCategoryDoesNotExist()
        {
            // Arrange
            var id = 10;

            // Act
            var result = await _expenseCategoriesService.GetExpenseCategoryByIdAsync(id);

            // Assert
            result.Should().BeNull();
        }

        [Test, Order(6)]
        public async Task GetExpenseCategoryByIdAsync_ShouldReturnExpenseCategory_WhenExpenseCategoryExists()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _expenseCategoriesService.GetExpenseCategoryByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Groceries");
        }

        [Test, Order(7)]
        public async Task CreateExpenseCategoryAsync_ShouldCreateExpenseCategory()
        {
            // Arrange
            var expenseCategoryDTO = new ExpenseCategoryDTO { Name = "New Category" };
            var userId = "UserId4";

            // Act
            await _expenseCategoriesService.CreateExpenseCategoryAsync(expenseCategoryDTO, userId);

            // Assert
            var createdExpenseCategory = await _context.ExpenseCategories.FirstOrDefaultAsync(x => x.UserId == userId && x.Name == expenseCategoryDTO.Name);
            createdExpenseCategory.Should().NotBeNull();
            createdExpenseCategory.Name.Should().Be(expenseCategoryDTO.Name);
            createdExpenseCategory.UserId.Should().Be(userId);

            // Restore _context
            _context.ExpenseCategories.Remove(createdExpenseCategory);
            await _context.SaveChangesAsync();
        }

        [Test, Order(8)]
        public async Task DeleteExpenseCategoryAsync_ExpenseCategoryDoesNotExist()
        {
            // Arrange
            var expenseCategoryId = 10;

            // Act
            await _expenseCategoriesService.DeleteExpenseCategoryByIdAsync(expenseCategoryId);

            // Assert
            _context.ExpenseCategories.Should().HaveCount(4);
        }

        [Test, Order(9)]
        public async Task DeleteExpenseCategoryAsync_ShouldDeleteExpenseCategory()
        {
            // Arrange
            var expenseCategoryId = 3;

            // Act
            await _expenseCategoriesService.DeleteExpenseCategoryByIdAsync(expenseCategoryId);

            // Assert
            _context.ExpenseCategories.Should().HaveCount(3);
        }

        [Test, Order(10)]
        public async Task UpdateExpenseCategoryByIdAsync_ShouldUpdateExpenseCategory_WhenExpenseCategoryExists()
        {
            // Arrange
            var expenseCategoryId = 1;
            var expenseCategoryDTO = new ExpenseCategoryDTO { Name = "Updated Category" };

            // Act
            var updatedCategory = await _expenseCategoriesService.UpdateExpenseCategoryByIdAsync(expenseCategoryId, expenseCategoryDTO);

            // Assert
            updatedCategory.Should().NotBeNull();
            updatedCategory.Name.Should().Be(expenseCategoryDTO.Name);

            var updatedExpenseCategory = await _context.ExpenseCategories.FindAsync(expenseCategoryId);
            updatedExpenseCategory.Should().NotBeNull();
            updatedExpenseCategory.Name.Should().Be(expenseCategoryDTO.Name);
        }

        [Test, Order(11)]
        public async Task UpdateExpenseCategoryByIdAsync_ShouldReturnNull_WhenExpenseCategoryDoesNotExist()
        {
            // Arrange
            var id = 10;
            var expenseCategoryDTO = new ExpenseCategoryDTO { Name = "Updated Category" };

            // Act
            var resultExpenseCategory = await _expenseCategoriesService.UpdateExpenseCategoryByIdAsync(id, expenseCategoryDTO);

            // Assert
            resultExpenseCategory.Should().BeNull();
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            _context.Database.EnsureDeleted();
        }

        private void SeedDatabase()
        {
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
                },
                new ExpenseCategory()
                {
                    Id = 3,
                    Name = "Bills",
                    UserId = "UserId1"
                },
                new ExpenseCategory()
                {
                    Id = 4,
                    Name = "Cinema",
                    UserId = "UserId2"
                },
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
                },
                new ApplicationUser()
                {
                    Id = "UserId4",
                    UserName = "User4",
                    Email = "user4@mail.com",
                    EmailConfirmed = true,
                    FirstName = "Gavin",
                    LastName = "Belson"
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

            var userRelations = new List<User_Relation>()
            {
                new User_Relation()
                {
                    Id = "Id1",
                    User1Id = "UserId1",
                    User2Id = "UserId2"
                }
            };

            _context.ExpenseCategories.AddRange(expenseCategories);
            _context.Users.AddRange(users);
            _context.Roles.AddRange(roles);
            _context.User_Relations.AddRange(userRelations);

            _context.SaveChanges();
        }
    }
}
