using BudgetTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Controllers
{
    public class BudgetsController : Controller
    {
        // Static list to store budgets in memory (no database yet)
        // "static" means the list is shared across all requests and persists while the app is running
        private static List<Budget> _budgets = new List<Budget>
        {
            // Some sample data to start with
            new Budget { Id = 1, Name = "Monthly Groceries", Amount = 400.00m, CategoryId = 1, CategoryName = "Groceries", StartDate = new DateTime(2026, 4, 1), EndDate = new DateTime(2026, 4, 30) },
            new Budget { Id = 2, Name = "Utilities Budget", Amount = 200.00m, CategoryId = 2, CategoryName = "Utilities", StartDate = new DateTime(2026, 4, 1), EndDate = new DateTime(2026, 4, 30) }
        };

        // GET: /Budgets
        // Shows all budgets in a list
        public IActionResult Index()
        {
            return View(_budgets);
        }

        // GET: /Budgets/Create
        // Shows the empty form to add a new budget
        [HttpGet]
        public IActionResult Create()
        {
            // Pass the list of categories to the view so we can show a dropdown
            ViewBag.Categories = CategoriesController._categories;
            return View();
        }

        // POST: /Budgets/Create
        // Receives the form data and adds the budget to the list
        [HttpPost]
        public IActionResult Create(Budget budget)
        {
            // Check that all required fields are valid
            if (!ModelState.IsValid)
            {
                // If validation failed, pass categories again so the dropdown still works
                ViewBag.Categories = CategoriesController._categories;
                return View(budget);
            }

            // Look up the category name based on the selected CategoryId and save it on the budget
            if (budget.CategoryId.HasValue)
            {
                var selectedCategory = CategoriesController._categories
                    .FirstOrDefault(c => c.Id == budget.CategoryId.Value);

                // If we found a matching category, store its name
                if (selectedCategory != null)
                {
                    budget.CategoryName = selectedCategory.Name;
                }
            }

            // Assign a new Id (just use current count + 1 for now)
            budget.Id = _budgets.Count + 1;

            // Add the new budget to our in-memory list
            _budgets.Add(budget);

            // Redirect back to the Index page to see the updated list
            return RedirectToAction("Index");
        }
    }
}
