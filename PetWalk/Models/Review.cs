using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PetWalk.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        public int OwnerId { get; set; }
        public int WalkerId { get; set; }
        public int WalkId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string Comment { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.Now;

        [ForeignKey("OwnerId")]
        public Owner? Owner { get; set; }

        [ForeignKey("WalkerId")]
        public Walker? Walker { get; set; }

        [ForeignKey("WalkId")]
        public Walk? Walk { get; set; }
    }
}
