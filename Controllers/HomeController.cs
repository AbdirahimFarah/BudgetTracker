using BudgetTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BudgetTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: /Home/Dashboard
        // Builds a summary of all spending and compares it against budgets
        public IActionResult Dashboard()
        {
            // Grab the shared data from the other controllers
            var transactions = TransactionsController._transactions;
            var budgets = BudgetsController._budgets;

            // --- Total Spending ---
            // Add up every transaction amount to get the grand total
            decimal totalSpending = transactions.Sum(t => t.Amount);

            // --- Spending by Category ---
            // Group transactions by their category name, then sum the amounts in each group
            // Transactions with no category get grouped under "Uncategorized"
            var spendingByCategory = transactions
                .GroupBy(t => t.CategoryName ?? "Uncategorized")
                .ToDictionary(
                    group => group.Key,          // key   = category name
                    group => group.Sum(t => t.Amount) // value = total spent in that category
                );

            // --- Budget Summaries ---
            // For each budget, find how much was actually spent in its category
            var budgetSummaries = new List<BudgetSummary>();

            foreach (var budget in budgets)
            {
                // Look up how much was spent in this budget's category
                // If the category has no transactions, default to 0
                decimal spentInCategory = 0;
                if (budget.CategoryName != null && spendingByCategory.ContainsKey(budget.CategoryName))
                {
                    spentInCategory = spendingByCategory[budget.CategoryName];
                }

                budgetSummaries.Add(new BudgetSummary
                {
                    Name = budget.Name,
                    BudgetAmount = budget.Amount,
                    SpentAmount = spentInCategory
                });
            }

            // Build the view model and pass it to the Dashboard view
            var viewModel = new DashboardViewModel
            {
                TotalSpending = totalSpending,
                SpendingByCategory = spendingByCategory,
                BudgetSummaries = budgetSummaries
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
