using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThreadIt.Data;
using ThreadIt.Models;

namespace ThreadIt.Controllers {
    public class CategoriesController : Controller {
        private readonly ApplicationDbContext db;
        private readonly UserManager<AppUser> _userManager;

        public CategoriesController(ApplicationDbContext context, UserManager<AppUser> userManager) {
            db = context;
            _userManager = userManager;
        }

        public IActionResult Show(int? id) {
            if (id == null) return RedirectToAction("Index");
            var category = db.Categories.FirstOrDefault(c => c.Id == id);

            ViewBag.usermanager = _userManager;
            var followedCats = db.Subscribes.Where(s => s.UserId == _userManager.GetUserId(User)).Select(s => s.CategoryId).ToList();
            ViewBag.followedCats = followedCats;

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
            var categories = db.Categories.Include("Threads").ToList();
            ViewBag.Categories = categories;
            ViewBag.usermanager = _userManager;

            var followedCats = db.Subscribes.Where(s => s.UserId == _userManager.GetUserId(User)).Select(s => s.CategoryId).ToList();
            ViewBag.followedCats = followedCats;

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

            db.Categories.Remove(category);
            db.SaveChanges();

            TempData["CategoryDelete"] = "Category deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
