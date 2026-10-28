using CloudCompare.Data;
using CloudCompare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudCompare.Controllers
{
    public class CompareController : Controller
    {
        private const string Key = "CompareIds";
        private const int MaxItems = 3;
        private readonly AppDbContext _db;
        public CompareController(AppDbContext db) { _db = db; }

        // State management: the compare list is kept in Session
        private List<int> GetIds()
        {
            var s = HttpContext.Session.GetString(Key);
            if (string.IsNullOrEmpty(s)) return new List<int>();
            return s.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
        }

        private void SaveIds(List<int> ids) => HttpContext.Session.SetString(Key, string.Join(",", ids));

        private IActionResult Back(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction("Index", "Services");
        }

        public async Task<IActionResult> Index()
        {
            var ids = GetIds();
            var services = await _db.CloudServices
                .Include(s => s.Provider).Include(s => s.Reviews)
                .Where(s => ids.Contains(s.Id)).ToListAsync();

            // keep the order in which the user added them
            var ordered = ids.Select(i => services.FirstOrDefault(s => s.Id == i)).Where(s => s != null).ToList();
            return View(ordered);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id, string returnUrl)
        {
            var svc = await _db.CloudServices.FindAsync(id);
            if (svc == null) return NotFound();

            var ids = GetIds();
            if (ids.Contains(id))
            {
                TempData["Error"] = "This service is already in your compare list.";
            }
            else if (ids.Count >= MaxItems)
            {
                TempData["Error"] = $"You can compare up to {MaxItems} services at a time.";
            }
            else
            {
                if (ids.Count > 0)
                {
                    var first = await _db.CloudServices.FindAsync(ids[0]);
                    if (first != null && first.Category != svc.Category)
                    {
                        TempData["Error"] = $"Compare services of the same category only (current list: {first.Category}).";
                        return Back(returnUrl);
                    }
                }
                ids.Add(id);
                SaveIds(ids);
                TempData["Success"] = $"{svc.Name} added to compare list.";
            }
            return Back(returnUrl);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            var ids = GetIds();
            ids.Remove(id);
            SaveIds(ids);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(Key);
            return RedirectToAction(nameof(Index));
        }
    }
}
