using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetWalk.Models
{
    public class Walker : User
    {
        [MaxLength(100)]
        public string Location { get; set; } = string.Empty;

        public decimal HourlyRate { get; set; }

        public double Rating { get; set; }

        public bool IsAvailable { get; set; } = true;

        public List<Walk> AssignedWalks { get; set; } = new List<Walk>();
        public List<Review> Reviews { get; set; } = new List<Review>();

        public Walker()
        {
            UserType = "Walker";
        }

        public double CalculateAverageRating()
        {
            if (Reviews == null || Reviews.Count == 0)
                return 0;

            return Reviews.Average(r => r.Rating);
        }
    }
}
