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

        //private List<Comment> GetComments(int id)
        //{
        //    var sql =
        //        "WITH nest_comm as (" +
        //        "SELECT *, CAST(ROW_NUMBER() OVER (ORDER BY CreateTime) AS VARCHAR) + '/' AS prec " +
        //        "FROM Comments " +
        //        "WHERE ThreadId = {0} AND Level = 0 " +
        //        "UNION ALL " +
        //        "SELECT c.*, n.prec + CAST(ROW_NUMBER() OVER (ORDER BY CreateTime) AS VARCHAR) + '/' AS prec " +
        //        "FROM nest_comm n " +
        //        "JOIN Comments c ON n.Id = ParentId " +
        //        "WHERE ThreadId = {0} AND c.Level = (n.Level + 1)) " +
        //        "SELECT Id, Content, Level, IsDeleted, CreateTime, ParentId, ThreadId, UserId " +
        //        "FROM nest_comm " +
        //        "ORDER BY prec";
        //}

        private List<Comment> GetComments(int id)
        {
            var comments = db.Comments
                .Include(c => c.User)
                .Where(c => c.ThreadId == id && c.Level == 0)
                .OrderByDescending(c => db.Comments.Count(r => r.ParentId == c.Id))
                .ThenBy(c => c.CreateTime)
                .ToList();

            List<Comment> result = new List<Comment>();

            void OrderComments(Comment com)
            {
                result.Add(com);
                var replies = db.Comments.Include(c => c.User).Where(c => c.ParentId == com.Id).OrderBy(r => r.CreateTime);
                if (replies != null)
                    foreach (var rep in replies)
                        OrderComments(rep);
            }

            foreach (var com in comments) 
                OrderComments(com);

            return result;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Show(int id)
        {
            var thr = db.Threads.Include(t => t.User).FirstOrDefault(t => t.Id == id);
            var comments = GetComments(id);
            ViewBag.thread = thr;
            ViewBag.comments = comments;
            return View();
        }

        [HttpGet]
        public IActionResult New()
        {
            if(!(User.IsInRole("Admin") || User.IsInRole("User")))
            {
                TempData["REDIRECTINFO"] = "Trebuie sa fii logat pentru a posta un Thread!";
                TempData["REDIRECT"] = "/Threads/New";
                return Redirect("/Identity/Account/Login");
            }
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
