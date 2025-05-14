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

        [Required]
        public int VenueID { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        public Event? Event { get; set; } = null!;
        public Venue? Venue { get; set; } = null!;

        [NotMapped]
        public string Date => BookingDate.ToShortDateString();

        [NotMapped]
        public string Time => BookingDate.ToShortTimeString();
    }
}