using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ThreadIt.Data;
using ThreadIt.Models;

namespace ThreadIt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext db;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext DB)
        {
            _logger = logger;
            db = DB;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Threads/Following/?page=1&filter=1");
            }
            var categories = db.Categories
                .Select(c => new
                {
                    Category = c,
                    Threads = c.Threads.OrderBy(t => t.CreateTime).Take(3)
                })
                .ToList();
            ViewBag.Categories = categories;

            // Motor cautare
            var search = "";
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null) {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();

                // Cautarea propriuzisa
                List<int> threadIds = db.Threads.Where(
                            t => t.Title.Contains(search)
                                || t.Content.Contains(search)
                        ).Select(t => t.Id).ToList();

                List<int> commentIds = db.Comments.Where(
                            c => !c.IsDeleted 
                            && c.Content.Contains(search)
                        ).Select(c => c.ThreadId).ToList();

                List<int> allIds = threadIds.Union(commentIds).ToList();

                var threads = db.Threads
                    .Where(t => allIds.Contains(t.Id))
                    .Include("Comments")
                    .Include("User")
                    .OrderBy(t => t.CreateTime);

                ViewBag.searchThreads = threads;
                ViewBag.searchString = search;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
