using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Models
{
    // Budget model represents a planned spending limit for a time period
    public class Budget
    {
        // Unique identifier for each budget
        public int Id { get; set; }

        // Name is required and cannot be longer than 100 characters
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Amount is required — this is the planned spending limit
        [Required]
        public decimal Amount { get; set; }

        // CategoryId is optional — a budget can apply to a specific category or all spending
        public int? CategoryId { get; set; }

        // CategoryName is stored here so we don't need to look it up every time we display the budget
        public string? CategoryName { get; set; }

        // The date range this budget covers
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
