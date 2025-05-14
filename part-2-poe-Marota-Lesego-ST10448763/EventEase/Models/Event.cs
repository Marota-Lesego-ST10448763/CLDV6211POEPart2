using System.ComponentModel.DataAnnotations;
using EventEase.Models;

namespace EventEase.Models
{
    public class Event
    {
        public int? EventID { get; set; }

        [Required, MaxLength(100)]
        public string? EventName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string? Description { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string? Type { get; set; } = string.Empty;

        public DateTime EventDateTime { get; set; }

        public string? ImageUrl { get; set; } = string.Empty;

        public string? EventPath { get; set; } = string.Empty;

        [Required]
        public int? VenueID { get; set; }
        public Venue? Venue { get; set; } = null!;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}