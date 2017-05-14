using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Cache;

namespace Backend.Model
{
    public class User
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Address { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Boolean Gender { get; set; }

        public string[] Phonenumbers { get; set; }

        public string Job { get; set; }

        public string Website { get; set; }

        public DateTime Birthday { get; set; }
    }
}