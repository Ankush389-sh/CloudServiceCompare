using System.Text.Json;
using CloudCompare.Data;
using CloudCompare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudCompare.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            var providers = await _db.Providers
                .Include(p => p.Services).ThenInclude(s => s.Reviews)
                .OrderBy(p => p.Name).ToListAsync();

            ViewBag.ProviderCount = providers.Count;
            ViewBag.ServiceCount = providers.Sum(p => p.Services.Count);
            ViewBag.ReviewCount = providers.Sum(p => p.ReviewCount);

            // Chart data: average starting price per category for each provider
            var cats = Categories.List;
            var datasets = providers.Select(p => new
            {
                label = p.Name,
                data = cats.Select(c =>
                {
                    var list = p.Services.Where(s => s.Category == c).ToList();
                    return list.Any() ? Math.Round((double)list.Average(s => s.StartingPrice), 2) : 0.0;
                }).ToList()
            }).ToList();
            ViewBag.ChartJson = JsonSerializer.Serialize(new { labels = cats, datasets });

            return View(providers.OrderByDescending(p => p.AvgRating).ToList());
        }

        // State management: theme preference stored in a cookie
        public IActionResult ToggleTheme(string returnUrl)
        {
            var current = Request.Cookies["theme"] ?? "light";
            Response.Cookies.Append("theme", current == "light" ? "dark" : "light",
                new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30), IsEssential = true });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction("Index");
        }

        public IActionResult Error() => Content("Something went wrong. Please go back and try again.");
    }
}
