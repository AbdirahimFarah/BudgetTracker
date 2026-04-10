using BudgetTracker.Data;
using BudgetTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Controllers
{
    // [Authorize] means only logged-in users can access any action in this controller
    [Authorize]
    public class BudgetsController : Controller
    {
        // AppDbContext gives us access to the database tables
        private readonly AppDbContext _db;

        // ASP.NET Core will automatically inject AppDbContext here when the controller is created
        public BudgetsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Budgets
        // Shows all budgets in a list
        public IActionResult Index()
        {
            // Fetch all budgets from the database
            var budgets = _db.Budgets.ToList();
            return View(budgets);
        }

        // GET: /Budgets/Create
        // Shows the empty form to add a new budget
        [HttpGet]
        public IActionResult Create()
        {
            // Pass the list of categories from the database so we can show a dropdown
            ViewBag.Categories = _db.Categories.ToList();
            return View();
        }

        // POST: /Budgets/Create
        // Receives the form data and saves the budget to the database
        [HttpPost]
        public IActionResult Create(Budget budget)
        {
            // Check that all required fields are valid
            if (!ModelState.IsValid)
            {
                // If validation failed, pass categories again so the dropdown still works
                ViewBag.Categories = _db.Categories.ToList();
                return View(budget);
            }

            // Look up the category name based on the selected CategoryId and save it on the budget
            if (budget.CategoryId.HasValue)
            {
                var selectedCategory = _db.Categories
                    .FirstOrDefault(c => c.Id == budget.CategoryId.Value);

                // If we found a matching category, store its name
                if (selectedCategory != null)
                {
                    budget.CategoryName = selectedCategory.Name;
                }
            }

            // Add the new budget to the database
            // EF Core automatically assigns the Id (no need to set it manually)
            _db.Budgets.Add(budget);
            _db.SaveChanges(); // Write the changes to the database

            // Redirect back to the Index page to see the updated list
            return RedirectToAction("Index");
        }
    }
}
