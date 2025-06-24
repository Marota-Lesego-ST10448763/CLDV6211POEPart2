using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using System;

namespace EventEase.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search, int? eventTypeId, DateTime? startDate, DateTime? endDate, string availability)
        {
            var bookings = _context.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search) && int.TryParse(search, out int bookingID))
            {
                bookings = bookings.Where(b => b.BookingID == bookingID);
            }

            if (eventTypeId.HasValue)
            {
                bookings = bookings.Where(b => b.Event.EventTypeID == eventTypeId.Value);
            }

            if (startDate.HasValue)
            {
                bookings = bookings.Where(b => b.BookingDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                bookings = bookings.Where(b => b.BookingDate <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(availability))
            {
                bookings = bookings.Where(b => b.Venue.AvailabilityStatus == availability);
            }

            ViewBag.EventTypes = new SelectList(await _context.EventTypes.ToListAsync(), "EventTypeID", "Name");
            ViewBag.AvailabilityOptions = new SelectList(new[] { "Available", "Unavailable" });

            ViewData["Alert"] = TempData["Alert"];
            ViewData["AlertType"] = TempData["AlertType"];
            return View(await bookings.ToListAsync());
        }

        // ✅ NEW: GET: Booking/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Events = new SelectList(await _context.Events.ToListAsync(), "EventID", "EventName");
            ViewBag.Venues = new SelectList(await _context.Venues.ToListAsync(), "VenueID", "VenueName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            ViewBag.Events = new SelectList(await _context.Events.ToListAsync(), "EventID", "EventName", booking.EventID);
            ViewBag.Venues = new SelectList(await _context.Venues.ToListAsync(), "VenueID", "VenueName", booking.VenueID);

            bool isDuplicate = await _context.Bookings.AnyAsync(b =>
                b.VenueID == booking.VenueID &&
                b.BookingDate.Date == booking.BookingDate.Date);

            if (isDuplicate)
            {
                ModelState.AddModelError(string.Empty, "This venue has already been booked on the selected date.");
                return View(booking);
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["Alert"] = "Booking created successfully.";
                TempData["AlertType"] = "success";
                return RedirectToAction(nameof(Index));
            }

            return View(booking);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();
            ViewBag.Events = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
            ViewBag.Venues = new SelectList(_context.Venues, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.BookingID)
                return NotFound();

            ViewBag.Events = new SelectList(await _context.Events.ToListAsync(), "EventID", "EventName", booking.EventID);
            ViewBag.Venues = new SelectList(await _context.Venues.ToListAsync(), "VenueID", "VenueName", booking.VenueID);

            bool isDuplicate = await _context.Bookings.AnyAsync(b =>
                b.VenueID == booking.VenueID &&
                b.BookingDate.Date == booking.BookingDate.Date &&
                b.BookingID != booking.BookingID);

            if (isDuplicate)
            {
                ModelState.AddModelError(string.Empty, "This venue has already been booked on the selected date.");
                return View(booking);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["Alert"] = "Booking updated successfully.";
                    TempData["AlertType"] = "success";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Bookings.Any(b => b.BookingID == booking.BookingID))
                        return NotFound();
                    throw;
                }
            }

            return View(booking);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var booking = await _context.Bookings.Include(b => b.Event).Include(b => b.Venue).FirstOrDefaultAsync(m => m.BookingID == id);
            return booking == null ? NotFound() : View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["Alert"] = "Booking deleted successfully.";
                TempData["AlertType"] = "success";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Consolidated(string search, int? eventTypeId, DateTime? startDate, DateTime? endDate, string availability)
        {
            var bookings = _context.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                bookings = bookings.Where(b =>
                    b.BookingID.ToString().Contains(search) ||
                    b.Event.EventName.Contains(search));
            }

            if (eventTypeId.HasValue)
            {
                bookings = bookings.Where(b => b.Event.EventTypeID == eventTypeId.Value);
            }

            if (startDate.HasValue)
            {
                bookings = bookings.Where(b => b.BookingDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                bookings = bookings.Where(b => b.BookingDate <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(availability))
            {
                bookings = bookings.Where(b => b.Venue.AvailabilityStatus == availability);
            }

            ViewBag.EventTypes = new SelectList(await _context.EventTypes.ToListAsync(), "EventTypeID", "Name");
            ViewBag.AvailabilityOptions = new SelectList(new[] { "Available", "Unavailable" });

            return View(await bookings.ToListAsync());
        }
    }
}
