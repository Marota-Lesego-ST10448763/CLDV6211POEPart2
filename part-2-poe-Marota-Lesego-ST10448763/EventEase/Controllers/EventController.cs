using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEase.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace EventEase.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAzureBlobStorage _blobService;

        public EventController(ApplicationDbContext context, IAzureBlobStorage blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        public async Task<IActionResult> Index(string search)
        {
            var events = _context.Events.Include(e => e.Venue).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                events = events.Where(e =>
                    e.EventName.Contains(search) ||
                    // e.Type.Contains(search) ||  // Comment out this reference
                    e.Venue.VenueName.Contains(search));
            }

            ViewData["Alert"] = TempData["Alert"];
            ViewData["AlertType"] = TempData["AlertType"];
            return View(await events.ToListAsync());
        }

        // GET: Event/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.VenueID = new SelectList(await _context.Venues.ToListAsync(), "VenueID", "VenueName");
            return View();
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            ViewBag.VenueID = new SelectList(await _context.Venues.ToListAsync(), "VenueID", "VenueName", @event.VenueID);

            Console.WriteLine("POST Create called");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
                return View(@event);
            }

            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            Console.WriteLine("Event saved");
            TempData["Alert"] = "Event creation successfully.";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        // GET: Event/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            ViewBag.VenueID = new SelectList(await _context.Venues.ToListAsync(), "VenueID", "VenueName", @event.VenueID);
            return View(@event);
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event @event, IFormFile ImageFile)
        {
            if (id != @event.EventID)
                return NotFound();

            var existing = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.EventID == id);
            if (existing == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.VenueID = new SelectList(await _context.Venues.ToListAsync(), "VenueID", "VenueName", @event.VenueID);
                return View(@event);
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var url = await _blobService.UploadFileAsync(ImageFile);
                @event.ImageUrl = url;
            }
            else
            {
                @event.ImageUrl = existing.ImageUrl;
            }

            _context.Update(@event);
            await _context.SaveChangesAsync();
            TempData["Alert"] = "Event update successfully.";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        // GET: Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.Include(e => e.Venue).FirstOrDefaultAsync(m => m.EventID == id);
            return @event == null ? NotFound() : View(@event);
        }

        // GET: Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.Include(e => e.Venue).FirstOrDefaultAsync(m => m.EventID == id);
            return @event == null ? NotFound() : View(@event);
        }

        // POST: Event/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.Include(e => e.Bookings).FirstOrDefaultAsync(e => e.EventID == id);
            if (@event == null) return NotFound();

            if (@event.Bookings.Any())
            {
                TempData["Alert"] = "Unable to delete event with active bookings.";
                TempData["AlertType"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            TempData["Alert"] = "Event deletion successfully.";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }
    }
}
