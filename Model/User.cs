using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Cache;

namespace Backend.Model
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Boolean Gender { get; set; }

        [StringLength(30)]
        public string[] Phonenumbers { get; set; }

        [StringLength(40)]
        public string Job { get; set; }

        [StringLength(40)]
        public string Website { get; set; }

        public DateTime Birthday { get; set; }
    }
}