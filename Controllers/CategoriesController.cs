using BudgetTracker.Data;
using BudgetTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Controllers
{
    // only logged-in users can access any action in this controller
    [Authorize]
    public class CategoriesController : Controller
    {
        // AppDbContext gives us access to the database tables
        private readonly AppDbContext _db;

        // ASP.NET Core will automatically inject AppDbContext here when the controller is created
        public CategoriesController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Categories
        // Shows all categories in a list
        public IActionResult Index()
        {
            // Fetch all categories from the database
            var categories = _db.Categories.ToList();
            return View(categories);
        }

        // GET: /Categories/Create
        // Shows the empty form to add a new category
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Categories/Create
        // Receives the form data and saves the category to the database
        [HttpPost]
        public IActionResult Create(Category category)
        {
            // Check that all required fields are valid
            if (!ModelState.IsValid)
            {
                // If validation failed, return to the form so the user can fix errors
                return View(category);
            }

            // Add the new category to the database
            // EF Core automatically assigns the Id (no need to set it manually)
            _db.Categories.Add(category);
            _db.SaveChanges(); // Write the changes to the database

            // Redirect back to the Index page to see the updated list
            return RedirectToAction("Index");
        }

        // GET: /Categories/Edit/5
        // Shows the edit form pre-filled with the existing category data
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Find the category in the database by its id
            var category = _db.Categories.FirstOrDefault(c => c.Id == id);

            // If no category was found with that id, return a 404 page
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: /Categories/Edit/5
        // Receives the updated form data and saves it to the database
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            // Check that all required fields are valid
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            // Tell EF Core this category has been changed so it updates the database row
            _db.Categories.Update(category);
            _db.SaveChanges(); // Write the changes to the database

            return RedirectToAction("Index");
        }

        // POST: /Categories/Delete/5
        // Deletes the category with the given id from the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            // Find the category to delete
            var category = _db.Categories.FirstOrDefault(c => c.Id == id);

            // If no category was found, return a 404 page
            if (category == null)
            {
                return NotFound();
            }

            // Remove the category from the database and save
            _db.Categories.Remove(category);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
