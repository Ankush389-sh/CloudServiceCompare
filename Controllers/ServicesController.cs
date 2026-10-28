using CloudCompare.Data;
using CloudCompare.Filters;
using CloudCompare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CloudCompare.Controllers
{
    public class ServicesController : Controller
    {
        private readonly AppDbContext _db;
        public ServicesController(AppDbContext db) { _db = db; }

        // READ (public): list with search / filter / sort
        public async Task<IActionResult> Index(string search, string category, int? providerId, string sort)
        {
            var q = _db.CloudServices.Include(s => s.Provider).Include(s => s.Reviews).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(s => s.Name.Contains(search) || s.Description.Contains(search));
            if (!string.IsNullOrEmpty(category))
                q = q.Where(s => s.Category == category);
            if (providerId.HasValue)
                q = q.Where(s => s.ProviderId == providerId.Value);

            var list = await q.ToListAsync();
            list = sort switch
            {
                "price_asc" => list.OrderBy(s => s.StartingPrice).ToList(),
                "price_desc" => list.OrderByDescending(s => s.StartingPrice).ToList(),
                "sla" => list.OrderByDescending(s => s.UptimeSla).ToList(),
                "rating" => list.OrderByDescending(s => s.AvgRating).ToList(),
                _ => list.OrderBy(s => s.Category).ThenBy(s => s.Name).ToList()
            };

            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.Sort = sort;
            ViewBag.Providers = new SelectList(await _db.Providers.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", providerId);
            return View(list);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var s = await _db.CloudServices.Include(x => x.Provider).Include(x => x.Reviews)
                        .FirstOrDefaultAsync(x => x.Id == id);
            if (s == null) return NotFound();

            return View(new ServiceDetailsViewModel
            {
                Service = s,
                NewReview = new Review { CloudServiceId = s.Id }
            });
        }

        // CREATE (admin)
        [AdminAuth]
        public async Task<IActionResult> Create()
        {
            await LoadProviders();
            return View(new CloudService { UptimeSla = 99.9m, RegionCount = 1 });
        }

        [AdminAuth, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Category,ProviderId,Description,StartingPrice,FreeTier,UptimeSla,RegionCount")] CloudService service)
        {
            if (ModelState.IsValid && await _db.CloudServices.AnyAsync(s => s.Name == service.Name && s.ProviderId == service.ProviderId))
                ModelState.AddModelError("Name", "This provider already has a service with that name.");

            if (!ModelState.IsValid)
            {
                await LoadProviders();
                return View(service);
            }

            _db.CloudServices.Add(service);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Service added successfully.";
            return RedirectToAction(nameof(Index));
        }

        // UPDATE (admin)
        [AdminAuth]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var s = await _db.CloudServices.FindAsync(id);
            if (s == null) return NotFound();
            await LoadProviders();
            return View(s);
        }

        [AdminAuth, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Category,ProviderId,Description,StartingPrice,FreeTier,UptimeSla,RegionCount")] CloudService service)
        {
            if (id != service.Id) return NotFound();
            var existing = await _db.CloudServices.FindAsync(id);
            if (existing == null) return NotFound();

            if (ModelState.IsValid && await _db.CloudServices.AnyAsync(s => s.Name == service.Name && s.ProviderId == service.ProviderId && s.Id != id))
                ModelState.AddModelError("Name", "This provider already has a service with that name.");

            if (!ModelState.IsValid)
            {
                await LoadProviders();
                return View(service);
            }

            existing.Name = service.Name;
            existing.Category = service.Category;
            existing.ProviderId = service.ProviderId;
            existing.Description = service.Description;
            existing.StartingPrice = service.StartingPrice;
            existing.FreeTier = service.FreeTier;
            existing.UptimeSla = service.UptimeSla;
            existing.RegionCount = service.RegionCount;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Service updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // DELETE (admin) - reviews of the service are deleted with it
        [AdminAuth]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var s = await _db.CloudServices.Include(x => x.Provider).FirstOrDefaultAsync(x => x.Id == id);
            if (s == null) return NotFound();
            return View(s);
        }

        [AdminAuth, HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var s = await _db.CloudServices.FindAsync(id);
            if (s == null) return NotFound();

            _db.CloudServices.Remove(s);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Service deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadProviders()
        {
            ViewBag.Providers = new SelectList(await _db.Providers.OrderBy(p => p.Name).ToListAsync(), "Id", "Name");
        }
    }
}
