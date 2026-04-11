using Microsoft.EntityFrameworkCore;
using BudgetTracker.Models;

namespace BudgetTracker.Data
{
    // AppDbContext is the main class that connects our app to the database.
    // It inherits from DbContext which is provided by Entity Framework Core.
    public class AppDbContext : DbContext
    {
        // The constructor receives options (like the connection string) and passes them to DbContext
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet represents a table in the database.
        // Each DbSet maps to a table named after the property (e.g. "Transactions").

        // Table for all income and expense transactions
        // virtual allows Moq to override these properties in unit tests
        public virtual DbSet<Transaction> Transactions { get; set; }

        // Table for spending categories (e.g. Food, Rent, Entertainment)
        public virtual DbSet<Category> Categories { get; set; }

        // Table for budget plans
        public virtual DbSet<Budget> Budgets { get; set; }
    }
}
