using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEase.Services;
using EventEase.Models;
using System;

namespace EventEase.Controllers
{
    public class VenueController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAzureBlobStorage _blobService;

        public VenueController(ApplicationDbContext context, IAzureBlobStorage blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        public async Task<IActionResult> Index(string search)
        {
            var venues = from v in _context.Venues select v;

            if (!string.IsNullOrEmpty(search))
                venues = venues.Where(v => v.VenueName.Contains(search));

            ViewData["Alert"] = TempData["Alert"];
            ViewData["AlertType"] = TempData["AlertType"];
            return View(await venues.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Venue venue, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var url = await _blobService.UploadFileAsync(ImageFile);
                    venue.ImageUrl = url;
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["Alert"] = "Venue created successfully.";
                TempData["AlertType"] = "success";
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FindAsync(id);
            return venue == null ? NotFound() : View(venue);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Venue venue, IFormFile ImageFile)
        {
            if (id != venue.VenueID) return NotFound();

            var existingVenue = await _context.Venues.AsNoTracking().FirstOrDefaultAsync(v => v.VenueID == id);
            if (existingVenue == null) return NotFound();

            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var url = await _blobService.UploadFileAsync(ImageFile);
                    venue.ImageUrl = url;
                }
                else
                {
                    venue.ImageUrl = existingVenue.ImageUrl;
                }

                _context.Update(venue);
                await _context.SaveChangesAsync();
                TempData["Alert"] = "Venue updated successfully.";
                TempData["AlertType"] = "success";
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        public async Task<IActionResult> Details(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            return venue == null ? NotFound() : View(venue);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues
                .Include(v => v.Events)
                .Include(v => v.Bookings)
                .FirstOrDefaultAsync(v => v.VenueID == id);

            return venue == null ? NotFound() : View(venue);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var venue = await _context.Venues
                .Include(v => v.Events)
                .Include(v => v.Bookings)
                .FirstOrDefaultAsync(v => v.VenueID == id);

            if (venue == null) return NotFound();

            if (venue.Events.Any() || venue.Bookings.Any())
            {
                TempData["Alert"] = "Cannot delete venue with linked events or bookings.";
                TempData["AlertType"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();

            TempData["Alert"] = "Venue deleted successfully.";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }
    }
}
