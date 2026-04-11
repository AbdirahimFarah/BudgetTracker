using System.Collections.Generic;
using System.Linq;
using BudgetTracker.Controllers;
using BudgetTracker.Data;
using BudgetTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BudgetTracker.Tests.Controllers
{
    // Tests for CategoriesController
    //
    // Same Moq approach as TransactionsControllerTests:
    // we fake the database so no real database is needed.
    public class CategoriesControllerTests
    {
        // Helper: builds a fake DbSet<T> backed by an in-memory List.
        // Wires up the IQueryable interface so LINQ methods like .ToList() work.
        private static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();

            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            return mockSet;
        }

        // Helper: creates a Mock<AppDbContext> with empty options.
        private static Mock<AppDbContext> CreateMockContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().Options;
            return new Mock<AppDbContext>(options);
        }

        // -------------------------------------------------------------------------
        // Test 1: Index() should return a ViewResult
        // -------------------------------------------------------------------------
        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var mockCategories = CreateMockDbSet(new List<Category>());
            var mockContext = CreateMockContext();
            mockContext.Setup(c => c.Categories).Returns(mockCategories.Object);

            var controller = new CategoriesController(mockContext.Object);

            // Act
            var result = controller.Index();

            // Assert: action returned a view
            Assert.IsType<ViewResult>(result);
        }

        // -------------------------------------------------------------------------
        // Test 2: Create POST with valid data should save and redirect to Index
        // -------------------------------------------------------------------------
        [Fact]
        public void Create_Post_ValidData_SavesAndRedirects()
        {
            // Arrange
            var mockCategories = CreateMockDbSet(new List<Category>());
            var mockContext = CreateMockContext();
            mockContext.Setup(c => c.Categories).Returns(mockCategories.Object);
            mockContext.Setup(c => c.SaveChanges()).Returns(1);

            var controller = new CategoriesController(mockContext.Object);

            var category = new Category { Name = "Food" };

            // Act
            var result = controller.Create(category);

            // Assert: redirected back to the list
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            // Verify Add() and SaveChanges() were each called exactly once
            mockCategories.Verify(m => m.Add(It.IsAny<Category>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        // -------------------------------------------------------------------------
        // Test 3: Create POST with invalid model state should return the Create view
        //         and must NOT save anything to the database
        // -------------------------------------------------------------------------
        [Fact]
        public void Create_Post_InvalidModelState_ReturnsView()
        {
            // Arrange
            // CategoriesController does not access the database in the invalid path,
            // but we still need a mock context to construct the controller
            var mockCategories = CreateMockDbSet(new List<Category>());
            var mockContext = CreateMockContext();
            mockContext.Setup(c => c.Categories).Returns(mockCategories.Object);

            var controller = new CategoriesController(mockContext.Object);
            // Simulate a failed form submission (e.g. the Name field was left blank)
            controller.ModelState.AddModelError("Name", "Name is required");

            var category = new Category { Name = "" };

            // Act
            var result = controller.Create(category);

            // Assert: stayed on the form
            Assert.IsType<ViewResult>(result);

            // Nothing should have been written to the database
            mockCategories.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }
    }
}
