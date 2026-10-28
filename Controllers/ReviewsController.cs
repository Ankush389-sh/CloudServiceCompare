using CloudCompare.Data;
using CloudCompare.Filters;
using CloudCompare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudCompare.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly AppDbContext _db;
        public ReviewsController(AppDbContext db) { _db = db; }

        // CREATE (public form)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CloudServiceId,ReviewerName,Rating,Comment")] Review review)
        {
            var service = await _db.CloudServices.Include(s => s.Provider).Include(s => s.Reviews)
                              .FirstOrDefaultAsync(s => s.Id == review.CloudServiceId);
            if (service == null) return NotFound();

            if (!ModelState.IsValid)
            {
                // redisplay the details page with validation messages
                return View("~/Views/Services/Details.cshtml",
                    new ServiceDetailsViewModel { Service = service, NewReview = review });
            }

            review.CreatedOn = DateTime.Now;
            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Thanks! Your review has been posted.";
            return RedirectToAction("Details", "Services", new { id = review.CloudServiceId });
        }

        // DELETE (admin)
        [AdminAuth, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _db.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            int serviceId = review.CloudServiceId;
            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Review deleted.";
            return RedirectToAction("Details", "Services", new { id = serviceId });
        }
    }
}
