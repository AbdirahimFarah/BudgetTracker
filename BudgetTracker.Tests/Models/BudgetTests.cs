using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BudgetTracker.Models;
using Xunit;

namespace BudgetTracker.Tests.Models
{
    // Tests for the Budget model
    // We check that the validation attributes on the model work as expected
    public class BudgetTests
    {
        // Helper: runs data annotation validation on an object and returns any errors
        private static List<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            Validator.TryValidateObject(model, context, results, validateAllProperties: true);
            return results;
        }

        // A fully filled-in budget should pass without any errors
        [Fact]
        public void ValidBudget_ShouldPassValidation()
        {
            var budget = new Budget
            {
                Name = "Monthly Groceries",
                Amount = 500m,
                StartDate = new DateTime(2026, 4, 1),
                EndDate = new DateTime(2026, 4, 30)
            };

            var errors = ValidateModel(budget);

            Assert.Empty(errors);
        }

        // Name is required — leaving it blank should cause a validation error
        [Fact]
        public void Budget_EmptyName_ShouldFailValidation()
        {
            var budget = new Budget
            {
                Name = "",   // empty name is not allowed
                Amount = 200m,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30)
            };

            var errors = ValidateModel(budget);

            Assert.NotEmpty(errors);
        }

        // Name cannot be longer than 100 characters
        [Fact]
        public void Budget_NameTooLong_ShouldFailValidation()
        {
            var budget = new Budget
            {
                Name = new string('a', 101), // 101 characters — one too many
                Amount = 300m,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30)
            };

            var errors = ValidateModel(budget);

            Assert.NotEmpty(errors);
        }

        // CategoryId is optional — a budget that covers all spending (no category) should be valid
        [Fact]
        public void Budget_NoCategoryId_ShouldPassValidation()
        {
            var budget = new Budget
            {
                Name = "General Expenses",
                Amount = 1000m,
                CategoryId = null, // not linked to a specific category
                StartDate = new DateTime(2026, 4, 1),
                EndDate = new DateTime(2026, 4, 30)
            };

            var errors = ValidateModel(budget);

            Assert.Empty(errors);
        }

        // A budget amount of zero is technically allowed by the model (no [Range] attribute)
        // This test documents the current behaviour so we notice if it changes
        [Fact]
        public void Budget_ZeroAmount_ShouldPassValidation()
        {
            var budget = new Budget
            {
                Name = "Placeholder Budget",
                Amount = 0m,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1)
            };

            var errors = ValidateModel(budget);

            // No [Range] attribute on Amount means 0 is currently valid
            Assert.Empty(errors);
        }
    }
}
