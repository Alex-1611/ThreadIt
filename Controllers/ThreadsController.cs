using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using ThreadIt.Data;
using ThreadIt.Models;

namespace ThreadIt.Controllers
{
    public class ThreadsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<AppUser> userManager;
        public ThreadsController(ApplicationDbContext DB, UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
            db = DB;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Show(int id)
        {
            var thr = db.Threads.Include("Comments").Include("User").Include("Comments.User").FirstOrDefault(t => t.Id == id);
            ViewBag.thread = thr;
            return View();
        }

        [Authorize(Roles = "User, Admin") ]
        [HttpGet]
        public IActionResult New()
        {
            var categories = db.Categories.ToList();
            ViewBag.categories = categories;
            return View();
        }

        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public IActionResult New(Models.Thread thr)
        {
            //make thr.UserId get the id of the current user
            thr.UserId = userManager.GetUserId(User);
            thr.CreateTime = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Threads.Add(thr);
                db.SaveChanges();
                return RedirectToAction("Show", new { id = thr.Id });
            }
            return View(thr);
        }

    }
}
