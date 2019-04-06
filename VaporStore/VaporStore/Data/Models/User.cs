using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(3), MaxLength(20)]
        public string Username { get; set; }

        [Required]
        [RegularExpression(@"[A-Z]{1}[a-z]*\s{1}[A-Z]{1}[a-z]*")]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [Range(typeof(int), "3", "103")]
        public int Age { get; set; }

        public ICollection<Card> Cards { get; set; }
    }
}
