using BudgetTracker.Data;
using BudgetTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Controllers
{
    // [Authorize] means only logged-in users can access any action in this controller
    [Authorize]
    public class TransactionsController : Controller
    {
        // AppDbContext gives us access to the database tables
        private readonly AppDbContext _db;

        // ASP.NET Core will automatically inject AppDbContext here when the controller is created
        public TransactionsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Transactions
        // Shows all transactions in a list
        public IActionResult Index()
        {
            // Fetch all transactions from the database
            var transactions = _db.Transactions.ToList();
            return View(transactions);
        }

        // GET: /Transactions/Create
        // Shows the empty form to add a new transaction
        [HttpGet]
        public IActionResult Create()
        {
            // Pass the list of categories from the database so we can show a dropdown
            ViewBag.Categories = _db.Categories.ToList();
            return View();
        }

        // POST: /Transactions/Create
        // Receives the form data and saves the transaction to the database
        [HttpPost]
        public IActionResult Create(Transaction transaction)
        {
            // Check that all required fields are valid
            if (!ModelState.IsValid)
            {
                // If validation failed, pass categories again so the dropdown still works
                ViewBag.Categories = _db.Categories.ToList();
                return View(transaction);
            }

            // Look up the category name based on the selected CategoryId and save it on the transaction
            if (transaction.CategoryId.HasValue)
            {
                var selectedCategory = _db.Categories
                    .FirstOrDefault(c => c.Id == transaction.CategoryId.Value);

                // If we found a matching category, store its name
                if (selectedCategory != null)
                {
                    transaction.CategoryName = selectedCategory.Name;
                }
            }

            // Add the new transaction to the database
            // EF Core automatically assigns the Id (no need to set it manually)
            _db.Transactions.Add(transaction);
            _db.SaveChanges(); // Write the changes to the database

            // Redirect back to the Index page to see the updated list
            return RedirectToAction("Index");
        }

        // GET: /Transactions/Edit/5
        // Shows the edit form pre-filled with the existing transaction data
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Find the transaction in the database by its id
            var transaction = _db.Transactions.FirstOrDefault(t => t.Id == id);

            // If no transaction was found with that id, return a 404 page
            if (transaction == null)
            {
                return NotFound();
            }

            // Pass categories so the dropdown still works
            ViewBag.Categories = _db.Categories.ToList();
            return View(transaction);
        }

        // POST: /Transactions/Edit/5
        // Receives the updated form data and saves it to the database
        [HttpPost]
        public IActionResult Edit(Transaction transaction)
        {
            // Check that all required fields are valid
            if (!ModelState.IsValid)
            {
                // Pass categories again so the dropdown still works
                ViewBag.Categories = _db.Categories.ToList();
                return View(transaction);
            }

            // Look up the category name based on the selected CategoryId
            if (transaction.CategoryId.HasValue)
            {
                var selectedCategory = _db.Categories
                    .FirstOrDefault(c => c.Id == transaction.CategoryId.Value);

                if (selectedCategory != null)
                {
                    transaction.CategoryName = selectedCategory.Name;
                }
            }
            else
            {
                // Clear the category name if no category was selected
                transaction.CategoryName = null;
            }

            // Tell EF Core this transaction has been changed so it updates the database row
            _db.Transactions.Update(transaction);
            _db.SaveChanges(); // Write the changes to the database

            return RedirectToAction("Index");
        }

        // POST: /Transactions/Delete/5
        // Deletes the transaction with the given id from the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            // Find the transaction to delete
            var transaction = _db.Transactions.FirstOrDefault(t => t.Id == id);

            // If no transaction was found, return a 404 page
            if (transaction == null)
            {
                return NotFound();
            }

            // Remove the transaction from the database and save
            _db.Transactions.Remove(transaction);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
