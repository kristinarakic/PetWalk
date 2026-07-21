using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PetWalk.Models
{
    public class Dog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Breed { get; set; } = string.Empty;

        public int Age { get; set; }

        public double Weight { get; set; }

        [MaxLength(200)]
        public string Note { get; set; } = string.Empty;

        public int OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public Owner? Owner { get; set; }
    }
}
