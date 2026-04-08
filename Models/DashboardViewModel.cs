namespace BudgetTracker.Models
{
    // This view model holds all the data the Dashboard page needs to display
    public class DashboardViewModel
    {
        // The sum of all transaction amounts across every category
        public decimal TotalSpending { get; set; }

        // How much was spent per category — key is the category name, value is the total spent
        public Dictionary<string, decimal> SpendingByCategory { get; set; } = new Dictionary<string, decimal>();

        // One row per budget showing the planned amount and how much has actually been spent
        public List<BudgetSummary> BudgetSummaries { get; set; } = new List<BudgetSummary>();
    }

    // Represents a single row in the budget progress table
    public class BudgetSummary
    {
        // The name of the budget (e.g. "Monthly Groceries")
        public string Name { get; set; } = string.Empty;

        // How much money was planned for this budget
        public decimal BudgetAmount { get; set; }

        // How much has actually been spent in this budget's category
        public decimal SpentAmount { get; set; }
    }
}
