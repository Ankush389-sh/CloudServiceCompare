using CloudCompare.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudCompare.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        public AccountController(IConfiguration config) { _config = config; }

        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Admin")))
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (vm.Username == _config["Admin:Username"] && vm.Password == _config["Admin:Password"])
            {
                HttpContext.Session.SetString("Admin", vm.Username);   // session state
                TempData["Success"] = "Logged in as admin.";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View(vm);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Admin");
            TempData["Success"] = "You have been logged out.";
            return RedirectToAction("Index", "Home");
        }
    }
}
