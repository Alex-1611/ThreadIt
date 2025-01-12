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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Show(int id)
        {
            var thr = db.Threads.Where(t => t.Id == id);
            var comments = db.Comments.Where(t => t.ThreadId == id).ToList();
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
