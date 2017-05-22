using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Cache;

namespace Backend.Model
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        public string Password { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
        
        [StringLength(40)]
        public string Address { get; set; }

        public bool? Gender { get; set; }

        [StringLength(20)]
        public string Phonenumber { get; set; }

        [StringLength(20)]
        public string Job { get; set; }

        [StringLength(20)]
        public string Website { get; set; }

        public DateTime? Birthday { get; set; }
    }
}