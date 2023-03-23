using Microsoft.AspNetCore.Mvc;
using SK.TrackYourDay.Expenses.Data;
using SK.TrackYourDay.Expenses.Models;

namespace SK.TrackYourDay.Expenses.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ExpensesController(ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }

        public IActionResult Index()
        {
            IEnumerable<Expense> objList = _db.Expenses;
            return View(objList);
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
