using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Infrastructure.DataAccess;
using SK.TrackYourDay.UseCases.DTOs;
using SK.TrackYourDay.UseCases.Expenses.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SK.TrackYourDay.Expenses.Tests
{
    [TestFixture()]
    public class ExpenseCategoriesServiceTest
    {
        private Mock<ApplicationDbContext> _dbContextMock;
        private ExpenseCategoriesService _expenseCategoriesService;

        [SetUp]
        public void Setup()
        {
            _dbContextMock = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _expenseCategoriesService = new ExpenseCategoriesService(_dbContextMock.Object);
        }

        [Test()]
        public async Task GetAllExpenseCategoriesDTOAsyncTest()
        {
            // Arrange
            var userId = "testUserId";
            var expenseCategories = new List<ExpenseCategoryDTO>
            {
                new ExpenseCategoryDTO { Name = "Category B" },
                new ExpenseCategoryDTO { Name = "Category C" },
                new ExpenseCategoryDTO { Name = "Category A" }
            };
            var friendsExpenseCategories = new List<ExpenseCategoryDTO>
            {
                new ExpenseCategoryDTO { Name = "Friend's Category B" },
                new ExpenseCategoryDTO { Name = "Friend's Category A" }
            };

            var mockExpenseCategoryService = new Mock<ExpenseCategoriesService>();


            mockExpenseCategoryService.Setup(x => x.GetExpenseCategoriesDTOByUserId(userId))
                .ReturnsAsync(expenseCategories);
            mockExpenseCategoryService.Setup(x => x.GetFriendsExpenseCategoriesAsync(userId))
                .ReturnsAsync(friendsExpenseCategories);

            // Act
            var result = await _expenseCategoriesService.GetAllExpenseCategoriesDTOAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeInAscendingOrder(ec => ec.Name);
            result.Should().Contain(expenseCategories);
            result.Should().Contain(friendsExpenseCategories);
            result.Should().HaveCount(expenseCategories.Count + friendsExpenseCategories.Count);
        }

        [Test]
        public async Task GetAllExpenseCategoriesDTOAsync_WhenNoExpenseCategoriesExist_ReturnsEmptyList()
        {
            // Arrange
            var userId = "user1";
            var expenseCategories = new List<ExpenseCategory>();

            _dbContextMock.Setup(db => db.ExpenseCategories).Returns(MockDbSet(expenseCategories));

            // Act
            var result = await _expenseCategoriesService.GetAllExpenseCategoriesDTOAsync(userId);

            // Assert
            result.Should().BeEmpty();
        }

        // Helper method to create a mock DbSet from a list of entities
        private static DbSet<T> MockDbSet<T>(List<T> entities) where T : class
        {
            var queryableEntities = entities.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableEntities.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableEntities.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableEntities.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableEntities.GetEnumerator());

            return dbSetMock.Object;
        }
    }
}
