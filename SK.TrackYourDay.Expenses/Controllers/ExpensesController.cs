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

        //GET-Create - Creating View
        public IActionResult Create()
        {
            return View();
        }

        //POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Expense expense)
        {
            if (ModelState.IsValid)
            {
                _db.Expenses.Add(expense);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(expense);
        }

        // GET-Delete - Creating View
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var expense = _db.Expenses.FirstOrDefault(x => x.Id == id);
            if (expense == null)
            {
                return NotFound();
            }
            return View(expense);
        }

        // POST-Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var expense = _db.Expenses.FirstOrDefault(x => x.Id == id);

            if (expense == null)
            {
                return NotFound();
            }
            _db.Expenses.Remove(expense);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET-Update - Creating View
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var expense = _db.Expenses.FirstOrDefault(x => x.Id == id);
            if (expense == null)
            {
                return NotFound();
            }
            return View(expense);
        }

        //POST-Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Expense expense)
        {
            if (ModelState.IsValid)
            {
                _db.Expenses.Update(expense);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(expense);
        }

    }
}
