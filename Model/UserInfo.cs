using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Model
{
    public class UserInfo
    {
        [Key,ForeignKey("User")]
        public int UserId { get; set; }
        
        [MaxLength(20)]
        public string Name { get; set; }
        
        [MaxLength(40)]
        public string Address { get; set; }

        public bool? Gender { get; set; }

        [MaxLength(20)]
        public string Phonenumber { get; set; }

        [MaxLength(40)]
        public string Job { get; set; }

        [MaxLength(40)]
        public string Website { get; set; }

        public DateTime? Birthday { get; set; }
        
        public virtual User User { get; set; }

    }
}