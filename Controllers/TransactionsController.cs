using BudgetTracker.Data;
using BudgetTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Controllers
{
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
    }
}
