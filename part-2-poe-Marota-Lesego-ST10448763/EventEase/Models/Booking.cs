using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventEase.Models;

namespace EventEase.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        [Required]
        public int EventID { get; set; }

        [ForeignKey("EventID")]
        public Event Event { get; set; } = default!;

        [Required]
        public int VenueID { get; set; }

        [ForeignKey("VenueID")]
        public Venue Venue { get; set; } = default!;

        [Required]
        public DateTime BookingDate { get; set; }

        // Optional convenience properties for display
        [NotMapped]
        public string Date => BookingDate.ToShortDateString();

        [NotMapped]
        public string Time => BookingDate.ToShortTimeString();
    }
}