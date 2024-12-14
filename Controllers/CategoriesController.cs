using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThreadIt.Data;
using ThreadIt.Models;

namespace ThreadIt.Controllers {
    public class CategoriesController : Controller {
        private readonly ApplicationDbContext db;

        public CategoriesController(ApplicationDbContext context) {
            db = context;
        }

        public IActionResult Show(int? id) {
            if (id == null) return RedirectToAction("Index");
            var category = db.Categories.FirstOrDefault(c => c.Id == id);

            var threads = db.Threads.Where(t => t.CategoryId == id).ToList();
            ViewBag.catThreads = threads;

            return View(category);
        }

        public IActionResult Index() {
            var categories = db.Categories.ToList();
            ViewBag.Categories = categories;

            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult New() {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult New(Category category) {
            if(ModelState.IsValid) {
                db.Categories.Add(category);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            
            return View(category);
        }

        [HttpPost]
        public IActionResult Delete(int? id) {
            if (id == null) return RedirectToAction("Index");

            var category = db.Categories.FirstOrDefault(c => c.Id == id);
            if(category == null) return RedirectToAction("Index");

            //db.Categories.Remove(category);
            //db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
