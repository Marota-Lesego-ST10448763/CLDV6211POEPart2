using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEase.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index(string search, int? eventTypeId, DateTime? startDate, DateTime? endDate, string availability)
        {
            var events = _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .AsQueryable();

            // 🔍 Keyword Search
            if (!string.IsNullOrEmpty(search))
            {
                events = events.Where(e =>
                    e.EventName.Contains(search) ||
                    e.EventType.Name.Contains(search) ||
                    e.Venue.VenueName.Contains(search));
            }

            // 🔍 Filter: Event Type
            if (eventTypeId.HasValue)
            {
                events = events.Where(e => e.EventTypeID == eventTypeId.Value);
            }

            // 🔍 Filter: Start Date
            if (startDate.HasValue)
            {
                events = events.Where(e => e.EventDateTime >= startDate.Value);
            }

            // 🔍 Filter: End Date
            if (endDate.HasValue)
            {
                events = events.Where(e => e.EventDateTime <= endDate.Value);
            }

            // 🔍 Filter: Venue Availability
            if (!string.IsNullOrEmpty(availability))
            {
                events = events.Where(e => e.Venue.AvailabilityStatus == availability);
            }

            // Dropdown ViewBags for filters
            ViewBag.EventTypes = new SelectList(await _context.EventTypes.ToListAsync(), "EventTypeID", "Name");
            ViewBag.AvailabilityOptions = new SelectList(new[] { "Available", "Unavailable" });

            ViewData["Alert"] = TempData["Alert"];
            ViewData["AlertType"] = TempData["AlertType"];
            return View(await events.ToListAsync());
        }


        // GET: Event/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.EventTypeID = new SelectList(await _context.EventTypes.ToListAsync(), "EventTypeID", "Name");
            ViewBag.VenueID = new SelectList(await _context.Venues.ToListAsync(), "VenueID", "VenueName");
            return View();
        }


        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            ViewBag.VenueID = new SelectList(await _context.Venues.ToListAsync(), "VenueID", "VenueName", @event.VenueID);
            ViewBag.EventTypeID = new SelectList(await _context.EventTypes.ToListAsync(), "EventTypeID", "Name", @event.EventTypeID);

            if (!ModelState.IsValid)
            {
                return View(@event);
            }

            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            TempData["Alert"] = "Event created successfully.";
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
            ViewBag.EventTypeID = new SelectList(await _context.EventTypes.ToListAsync(), "EventTypeID", "Name", @event.EventTypeID);
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
                ViewBag.EventTypeID = new SelectList(await _context.EventTypes.ToListAsync(), "EventTypeID", "Name", @event.EventTypeID );
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
            TempData["Alert"] = "Event updated successfully.";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        // GET: Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventID == id);
            return @event == null ? NotFound() : View(@event);
        }

        // GET: Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventID == id);
            return @event == null ? NotFound() : View(@event);
        }

        // POST: Event/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.EventID == id);
            if (@event == null) return NotFound();

            if (@event.Bookings.Any())
            {
                TempData["Alert"] = "Unable to delete event with active bookings.";
                TempData["AlertType"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            TempData["Alert"] = "Event deleted successfully.";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }
    }
}
