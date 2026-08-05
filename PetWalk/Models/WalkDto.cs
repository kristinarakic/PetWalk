using System;

namespace PetWalk.Models
{
    public class WalkDto
    {
        public int Id { get; set; }
        public DateTime ScheduledDate { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string WalkerName { get; set; } = string.Empty;
        public string DogName { get; set; } = string.Empty;
    }
}