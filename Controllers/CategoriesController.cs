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

//            int _perPage = 3;

//            var articles = db.Articles.Include("Category").Include("User").OrderBy(a => a.Date);

//            if (TempData.ContainsKey("message")) {
//                ViewBag.message = TempData["message"].ToString();
//                ViewBag.Alert = TempData["messageType"];
//            }

//            int totalItems = articles.Count();



//            Dezvoltarea Aplicatiilor Web – Curs 10
//https://www.cezarabenegui.com/                                               cezara.benegui@fmi.unibuc.ro  
//            11
//            // Se preia pagina curenta din View-ul asociat 
//            // Numarul paginii este valoarea parametrului page 
//din ruta
//            // /Articles/Index?page=valoare 

//            var currentPage =
//Convert.ToInt32(HttpContext.Request.Query["page"]);

//            // Pentru prima pagina offsetul o sa fie zero 
//            // Pentru pagina 2 o sa fie 3  
//            // Asadar offsetul este egal cu numarul de articole 
//            care au fost deja afisate pe paginile anterioare
//            var offset = 0;

//            // Se calculeaza offsetul in functie de numarul 
//            paginii la care suntem
//            if (!currentPage.Equals(0)) {
//                offset = (currentPage - 1) * _perPage;
//            }

//            // Se preiau articolele corespunzatoare pentru 
//            fiecare pagina la care ne aflam
//            // in functie de offset 
//            var paginatedArticles =
//articles.Skip(offset).Take(_perPage);


//            // Preluam numarul ultimei pagini 

//            ViewBag.lastPage = Math.Ceiling((float)totalItems /
//(float)_perPage);

//            // Trimitem articolele cu ajutorul unui ViewBag 
//            catre View-ul corespunzator
//            ViewBag.Articles = paginatedArticles;

//            return View();

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
