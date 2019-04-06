using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaporStore.Data.Models
{
	public class Game
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(typeof(decimal), "0.00", "792281625142643375935439503")]
        public decimal Price { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [ForeignKey("Developer")]
        public int DeveloperId { get; set; }
        public Developer Developer { get; set; } 

        [ForeignKey("Genre")]
        public int GenreId { get; set; }
        public Genre Genre { get; set; } 

        public ICollection<Purchase> Purchases { get; set; } 

        public ICollection<GameTag> GameTags { get; set; } 
    }
}
