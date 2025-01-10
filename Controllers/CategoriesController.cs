using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            ViewBag.PaginationBaseUrl = "/Categories/Show/" + id + "?page";
            int threadsPerPage = 6;
            int threadsPerRow = 2; // < threadsPerPage

            var rows = Math.Ceiling((float)threadsPerPage/(float)threadsPerRow);
            ViewBag.pageRows = rows;
            ViewBag.perRow = threadsPerRow;

            var threads = db.Threads.Include("User").Where(t => t.CategoryId == id).OrderBy(t => t.CreateTime);

            int totalThreads = threads.Count();

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;

            if(!currentPage.Equals(0)) {
                offset = (currentPage - 1) * threadsPerPage;
            }

            var paginatedThreads = threads.Skip(offset).Take(threadsPerPage);
            ViewBag.lastPage = Math.Ceiling((float)totalThreads / (float)threadsPerPage);
            ViewBag.threads = paginatedThreads.ToList();

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
