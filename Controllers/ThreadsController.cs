using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using ThreadIt.Data;

namespace ThreadIt.Controllers
{
    public class ThreadsController : Controller
    {
        private readonly ApplicationDbContext db;
        public ThreadsController(ApplicationDbContext DB)
        {
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
    }
}
