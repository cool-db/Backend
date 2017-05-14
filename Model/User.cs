using System;

namespace Backend.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public Boolean Gender { get; set; }
        public string[] Phonenumbers { get; set; }
        public string Job { get; set; }
        public string Website { get; set; }
        public DateTime Birthday { get; set; }
    }
}