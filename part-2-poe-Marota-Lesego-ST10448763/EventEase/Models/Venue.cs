using System.ComponentModel.DataAnnotations;
using EventEase.Models;

namespace EventEase.Models
{
    public class Venue
    {
        public int VenueID { get; set; }

        [Required, MaxLength(100)]
        public string VenueName { get; set; } = string.Empty;

        public string? Location { get; set; }

        public int Capacity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string AvailabilityStatus { get; set; } = "Available";

        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
