using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThreadIt.Data;
using ThreadIt.Models;

namespace ThreadIt.Controllers {
    public class UsersController : Controller {
        private readonly ApplicationDbContext db;
        private readonly UserManager<AppUser> _userManager;

        public UsersController(ApplicationDbContext context, UserManager<AppUser> userManager) {
            db = context;
            _userManager = userManager;
        }

        public IActionResult Index() {
            List<AppUser> users = db.AppUsers
                .Include("Threads")
                .Include("Comments")
                .ToList();

            ViewBag.userManager = _userManager;
            ViewBag.usersList = users;

            if (TempData["info"] != null) {
                ViewBag.info = TempData["info"];
            }

            return View();
        }

        public IActionResult Show(string? id) {
            AppUser requestedUser = db.AppUsers
                .Include("Threads")
                .Include("Comments")
                .Where(u => u.Id == id || u.UserName == id)
                .FirstOrDefault();

            if (requestedUser != null) {
                return View(requestedUser);
            }

            return RedirectToAction("Index");
        }

        public IActionResult UserShow(string? id) {
            AppUser? user = db.Users.Find(id);
            if(user == null) {
                TempData["info"] = "UserShow: Could not find specified user.";
                return RedirectToAction("Index");
            }

            List<Models.Thread> userThreads = db.Threads.Where(t => t.UserId == id).ToList();
            ViewBag.threads = userThreads;

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string? id) {
            AppUser requestedUser = db.Users.Include("Comments").Include("Threads").Where(u => u.Id == id || u.UserName == id).FirstOrDefault();
            List<IdentityUserRole<string>> userRoles = db.UserRoles.Where(ur => ur.UserId == id).ToList();
            List<IdentityRole> allRoles = db.Roles.ToList();

            ViewBag.userRoles = userRoles;
            ViewBag.allRoles = allRoles;

            return View(requestedUser);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(string? id, string? selectedRole) {
            // Removes the old roles that a user had
            var oldRoles = db.UserRoles.Where(ur => ur.UserId == id);
            db.UserRoles.RemoveRange(oldRoles);
            db.SaveChanges();

            // Updates it to the newly selected role
            AppUser? user = db.Users.Find(id);
            if(user == null) {
                TempData["info"] = "Could not find the following user.";
                return RedirectToAction("Index");
            }

            var role = db.Roles.Find(selectedRole);
            if (role == null) {
                TempData["info"] = "Could not find the selected role.";
                return RedirectToAction("Index");
            }

            var userRole = new IdentityUserRole<string> {
                UserId = id,
                RoleId = selectedRole
            };

            db.UserRoles.Add(userRole);
            db.SaveChanges();

            TempData["info"] = "Changed role of " + user.UserName + " successfully.";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult UpdateProfile()
        {
            if (!(User.IsInRole("Admin") || User.IsInRole("User") ))
            {
                //redirect to login page
                TempData["REDIRECT"] = "/Users/UpdateProfile";
                TempData["REDIRECTINFO"] = "You need to be logged in to modify your profile!";
                return Redirect("/Identity/Account/Login");
            }
            var id = _userManager.GetUserId(User);
            AppUser? user = db.Users.FirstOrDefault(u => u.Id == id);

            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateProfile(AppUser? edited) {
            // Ignore subs because it can be null
            ModelState.Remove("Subs");

            if(ModelState.IsValid) {
                var user = db.Users.FirstOrDefault(u => u.Id == _userManager.GetUserId(User));
                var checkUsername = db.Users.FirstOrDefault(u => u.UserName == edited.UserName);
                
                if (checkUsername == null) {
                    user.UserName = edited.UserName;
                    user.Descriere = edited.Descriere;

                    db.SaveChanges();

                    TempData["Editare"] = "User edited successfully.";
                    return Redirect("/Users/Show/" + _userManager.GetUserId(User));
                }
            
                TempData["Editare"] = "UserName is already taken.";
            }

            TempData["Editare"] = "Could not update user.";
            return View(edited);
        }
    }
}
