using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [Required]
        public DateTime Date { get; set; }

        [ForeignKey("WalkerId")]
        public Walker? Walker { get; set; }

        public string Display => $"{Date:dd.MM.yyyy} ({Date.DayOfWeek}): {StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
    }
}