using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetWalk.Models
{
    public class AvailabilitySlot
    {
        [Key]
        public int Id { get; set; }

        public int WalkerId { get; set; }

        [Required]
        public DayOfWeek Day { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [ForeignKey("WalkerId")]
        public Walker? Walker { get; set; }

        public string Display => $"{Day}: {StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
    }
}