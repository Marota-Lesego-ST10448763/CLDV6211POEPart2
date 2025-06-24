using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventEase.Models;

namespace EventEase.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        [MaxLength(100)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EventDateTime { get; set; }

        public string? ImageUrl { get; set; }

        public string? EventPath { get; set; }

        // Foreign key and navigation to Venue
        [Required]
        public int VenueID { get; set; }

        [ForeignKey("VenueID")]
        public Venue Venue { get; set; } = default!;

        // Foreign key and navigation to EventType
        [Required]
        public int EventTypeID { get; set; }

        [ForeignKey("EventTypeID")]
        public EventType EventType { get; set; } = default!;

        // Navigation property to Bookings
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}