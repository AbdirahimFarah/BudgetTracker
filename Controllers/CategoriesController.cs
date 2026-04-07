using BudgetTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Controllers
{
    public class CategoriesController : Controller
    {
        // Public static list so other controllers (like TransactionsController) can read the categories
        // "static" means the list is shared across all requests and persists while the app is running
        public static List<Category> _categories = new List<Category>
        {
            // Some sample data to start with
            new Category { Id = 1, Name = "Groceries" },
            new Category { Id = 2, Name = "Utilities" },
            new Category { Id = 3, Name = "Entertainment" }
        };

        // GET: /Categories
        // Shows all categories in a list
        public IActionResult Index()
        {
            return View(_categories);
        }

        // GET: /Categories/Create
        // Shows the empty form to add a new category
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Categories/Create
        // Receives the form data and adds the category to the list
        [HttpPost]
        public IActionResult Create(Category category)
        {
            // Check that all required fields are valid
            if (!ModelState.IsValid)
            {
                // If validation failed, return to the form so the user can fix errors
                return View(category);
            }

            // Assign a new Id (just use current count + 1 for now)
            category.Id = _categories.Count + 1;

            // Add the new category to our in-memory list
            _categories.Add(category);

            // Redirect back to the Index page to see the updated list
            return RedirectToAction("Index");
        }
    }
}
