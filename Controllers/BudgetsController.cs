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

        // GET: /Budgets/Edit/5
        // Shows the edit form pre-filled with the existing budget data
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Find the budget in the database by its id
            var budget = _db.Budgets.FirstOrDefault(b => b.Id == id);

            // If no budget was found with that id, return a 404 page
            if (budget == null)
            {
                return NotFound();
            }

            // Pass categories so the dropdown still works
            ViewBag.Categories = _db.Categories.ToList();
            return View(budget);
        }

        // POST: /Budgets/Edit/5
        // Receives the updated form data and saves it to the database
        [HttpPost]
        public IActionResult Edit(Budget budget)
        {
            // Check that all required fields are valid
            if (!ModelState.IsValid)
            {
                // Pass categories again so the dropdown still works
                ViewBag.Categories = _db.Categories.ToList();
                return View(budget);
            }

            // Look up the category name based on the selected CategoryId
            if (budget.CategoryId.HasValue)
            {
                var selectedCategory = _db.Categories
                    .FirstOrDefault(c => c.Id == budget.CategoryId.Value);

                if (selectedCategory != null)
                {
                    budget.CategoryName = selectedCategory.Name;
                }
            }
            else
            {
                // Clear the category name if no category was selected
                budget.CategoryName = null;
            }

            // Tell EF Core this budget has been changed so it updates the database row
            _db.Budgets.Update(budget);
            _db.SaveChanges(); // Write the changes to the database

            return RedirectToAction("Index");
        }

        // POST: /Budgets/Delete/5
        // Deletes the budget with the given id from the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            // Find the budget to delete
            var budget = _db.Budgets.FirstOrDefault(b => b.Id == id);

            // If no budget was found, return a 404 page
            if (budget == null)
            {
                return NotFound();
            }

            // Remove the budget from the database and save
            _db.Budgets.Remove(budget);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
