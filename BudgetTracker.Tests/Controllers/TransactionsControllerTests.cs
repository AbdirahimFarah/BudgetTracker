using System;
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
    // Tests for TransactionsController
    //
    // We use Moq to create a fake AppDbContext and fake DbSet<T>.
    // This means we never touch a real database — our tests are fast and isolated.
    public class TransactionsControllerTests
    {
        // Helper: builds a fake DbSet<T> backed by an in-memory List.
        //
        // DbSet<T> implements IQueryable<T>, so we have to wire up four properties
        // (Provider, Expression, ElementType, GetEnumerator) for LINQ methods like
        // .ToList() to work against our fake data.
        private static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();

            var mockSet = new Mock<DbSet<T>>();

            // These four setups let LINQ walk our list as if it were a real DbSet
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            return mockSet;
        }

        // Helper: creates a Mock<AppDbContext> with empty options.
        // We pass options so the DbContext constructor does not throw,
        // but we never actually connect to a database.
        private static Mock<AppDbContext> CreateMockContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().Options;
            return new Mock<AppDbContext>(options);
        }

        
        // Test 1: Index() should return a ViewResult
        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var mockTransactions = CreateMockDbSet(new List<Transaction>());
            var mockContext = CreateMockContext();
            // Tell the fake context to return our fake DbSet when Transactions is accessed
            mockContext.Setup(c => c.Transactions).Returns(mockTransactions.Object);

            var controller = new TransactionsController(mockContext.Object);

            // Act
            var result = controller.Index();

            // Assert: the action returned a view (not a redirect or an error page)
            Assert.IsType<ViewResult>(result);
            //Assert.IsType<RedirectToActionResult>(result);
        }

        // Test 2: Create POST with valid data should save and redirect to Index

        [Fact]
        public void Create_Post_ValidData_SavesAndRedirects()
        {
            // Arrange
            var mockTransactions = CreateMockDbSet(new List<Transaction>());
            var mockContext = CreateMockContext();
            mockContext.Setup(c => c.Transactions).Returns(mockTransactions.Object);
            // Set up SaveChanges so it pretends 1 row was written
            mockContext.Setup(c => c.SaveChanges()).Returns(1);

            var controller = new TransactionsController(mockContext.Object);

            // A transaction without a CategoryId keeps things simple —
            // the controller only touches _db.Transactions in this path
            var transaction = new Transaction
            {
                Description = "Lunch",
                Amount = 12.50m,
                Date = DateTime.Today
            };

            // Act
            var result = controller.Create(transaction);

            // Assert: redirected back to the list
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            // Verify that Add() and SaveChanges() were each called exactly once
            mockTransactions.Verify(m => m.Add(It.IsAny<Transaction>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        // Test 3: Create POST with invalid model state should return the Create view
        //         and must NOT save anything to the database
        [Fact]
        public void Create_Post_InvalidModelState_ReturnsView()
        {
            // Arrange
            // When validation fails, the controller calls _db.Categories.ToList() to
            // re-populate the category dropdown, so we need to fake that DbSet too
            var mockCategories = CreateMockDbSet(new List<Category>());
            var mockTransactions = CreateMockDbSet(new List<Transaction>());
            var mockContext = CreateMockContext();
            mockContext.Setup(c => c.Categories).Returns(mockCategories.Object);
            mockContext.Setup(c => c.Transactions).Returns(mockTransactions.Object);

            var controller = new TransactionsController(mockContext.Object);
            // Adding an error to ModelState simulates a failed form submission
            // (e.g. the user left the Description field blank)
            controller.ModelState.AddModelError("Description", "Description is required");

            var transaction = new Transaction { Amount = 10m, Date = DateTime.Today };

            // Act
            var result = controller.Create(transaction);

            // Assert: stayed on the form
            Assert.IsType<ViewResult>(result);

            // Nothing should have been written to the database
            mockTransactions.Verify(m => m.Add(It.IsAny<Transaction>()), Times.Never());
            mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }
    }
}
