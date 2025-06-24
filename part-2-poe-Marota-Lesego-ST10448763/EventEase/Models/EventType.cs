using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}