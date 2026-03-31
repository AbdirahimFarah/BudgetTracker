using BudgetTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Controllers
{
    public class TransactionsController : Controller
    {
        // Static list to store transactions in memory (no database yet)
        // "static" means the list is shared across all requests and persists while the app is running
        private static List<Transaction> _transactions = new List<Transaction>
        {
            // Some sample data to start with
            new Transaction { Id = 1, Description = "Grocery shopping", Amount = 85.50m, Date = DateTime.Now.AddDays(-3), CategoryId = 1 },
            new Transaction { Id = 2, Description = "Electric bill", Amount = 120.00m, Date = DateTime.Now.AddDays(-7), CategoryId = 2 },
            new Transaction { Id = 3, Description = "Coffee", Amount = 4.75m, Date = DateTime.Now.AddDays(-1), CategoryId = 3 }
        };

        // GET: /Transactions
        // Shows all transactions in a list
        public IActionResult Index()
        {
            return View(_transactions);
        }

        // GET: /Transactions/Create
        // Shows the empty form to add a new transaction
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Transactions/Create
        // Receives the form data and adds the transaction to the list
        [HttpPost]
        public IActionResult Create(Transaction transaction)
        {
            // Check that all required fields are valid
            if (!ModelState.IsValid)
            {
                // If validation failed, return to the form so the user can fix errors
                return View(transaction);
            }

            // Assign a new Id (just use current count + 1 for now)
            transaction.Id = _transactions.Count + 1;

            // Add the new transaction to our in-memory list
            _transactions.Add(transaction);

            // Redirect back to the Index page to see the updated list
            return RedirectToAction("Index");
        }
    }
}
