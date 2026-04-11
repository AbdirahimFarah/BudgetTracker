using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BudgetTracker.Models;
using Xunit;

namespace BudgetTracker.Tests.Models
{
    // Tests for the Transaction model
    // We check that validation rules (like [Required] and [Range]) work correctly
    public class TransactionTests
    {
        // Helper method: runs the data annotations on a model and returns any errors
        // This simulates what ASP.NET Core does when it validates a form submission
        private static List<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            Validator.TryValidateObject(model, context, results, validateAllProperties: true);
            return results;
        }

        // A transaction with all required fields filled in correctly should pass validation
        [Fact]
        public void ValidTransaction_ShouldPassValidation()
        {
            var transaction = new Transaction
            {
                Description = "Grocery shopping",
                Amount = 75.50m,
                Date = DateTime.Today
            };

            var errors = ValidateModel(transaction);

            // Expect no validation errors
            Assert.Empty(errors);
        }

        // A transaction with no description should fail validation
        [Fact]
        public void Transaction_MissingDescription_ShouldFailValidation()
        {
            var transaction = new Transaction
            {
                Description = null!, // deliberately missing
                Amount = 50.00m,
                Date = DateTime.Today
            };

            var errors = ValidateModel(transaction);

            // Should have at least one error about the missing Description
            Assert.NotEmpty(errors);
        }

        // Amount must be at least 0.01 — zero is not allowed
        [Fact]
        public void Transaction_ZeroAmount_ShouldFailValidation()
        {
            var transaction = new Transaction
            {
                Description = "Test",
                Amount = 0m, // zero is outside the allowed range
                Date = DateTime.Today
            };

            var errors = ValidateModel(transaction);

            Assert.NotEmpty(errors);
        }

        // Amount must not exceed 1,000,000
        [Fact]
        public void Transaction_AmountOverLimit_ShouldFailValidation()
        {
            var transaction = new Transaction
            {
                Description = "Test",
                Amount = 1_000_001m, // over the max
                Date = DateTime.Today
            };

            var errors = ValidateModel(transaction);

            Assert.NotEmpty(errors);
        }

        // Description longer than 200 characters should fail
        [Fact]
        public void Transaction_DescriptionTooLong_ShouldFailValidation()
        {
            var transaction = new Transaction
            {
                // 201 characters — one over the limit
                Description = new string('x', 201),
                Amount = 10m,
                Date = DateTime.Today
            };

            var errors = ValidateModel(transaction);

            Assert.NotEmpty(errors);
        }

        // CategoryId and CategoryName are optional — a transaction without them should still be valid
        [Fact]
        public void Transaction_NoCategoryId_ShouldPassValidation()
        {
            var transaction = new Transaction
            {
                Description = "Cash withdrawal",
                Amount = 100m,
                Date = DateTime.Today,
                CategoryId = null,
                CategoryName = null
            };

            var errors = ValidateModel(transaction);

            Assert.Empty(errors);
        }
    }
}
