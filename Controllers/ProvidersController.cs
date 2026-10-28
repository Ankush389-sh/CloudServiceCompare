using CloudCompare.Data;
using CloudCompare.Filters;
using CloudCompare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudCompare.Controllers
{
    public class ProvidersController : Controller
    {
        private readonly AppDbContext _db;
        public ProvidersController(AppDbContext db) { _db = db; }

        // READ (public)
        public async Task<IActionResult> Index()
        {
            var list = await _db.Providers
                .Include(p => p.Services).ThenInclude(s => s.Reviews)
                .OrderBy(p => p.Name).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var p = await _db.Providers
                .Include(x => x.Services).ThenInclude(s => s.Reviews)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return NotFound();
            return View(p);
        }

        // CREATE (admin)
        [AdminAuth]
        public IActionResult Create() => View(new Provider { FoundedYear = 2010 });

        [AdminAuth, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Headquarters,Website,FoundedYear,Description")] Provider provider)
        {
            if (await _db.Providers.AnyAsync(p => p.Name == provider.Name))
                ModelState.AddModelError("Name", "A provider with this name already exists.");

            if (!ModelState.IsValid) return View(provider);

            _db.Providers.Add(provider);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Provider added successfully.";
            return RedirectToAction(nameof(Index));
        }

        // UPDATE (admin)
        [AdminAuth]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var p = await _db.Providers.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [AdminAuth, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Headquarters,Website,FoundedYear,Description")] Provider provider)
        {
            if (id != provider.Id) return NotFound();
            var existing = await _db.Providers.FindAsync(id);
            if (existing == null) return NotFound();

            if (await _db.Providers.AnyAsync(p => p.Name == provider.Name && p.Id != id))
                ModelState.AddModelError("Name", "Another provider already uses this name.");

            if (!ModelState.IsValid) return View(provider);

            existing.Name = provider.Name;
            existing.Headquarters = provider.Headquarters;
            existing.Website = provider.Website;
            existing.FoundedYear = provider.FoundedYear;
            existing.Description = provider.Description;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Provider updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // DELETE (admin)
        [AdminAuth]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var p = await _db.Providers.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [AdminAuth, HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var p = await _db.Providers.FindAsync(id);
            if (p == null) return NotFound();

            if (await _db.CloudServices.AnyAsync(s => s.ProviderId == id))
            {
                TempData["Error"] = "Cannot delete: this provider still has services. Delete its services first.";
                return RedirectToAction(nameof(Index));
            }

            _db.Providers.Remove(p);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Provider deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
