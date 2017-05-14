using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class Project
    {
        public int Id { get; set; }

        [StringLength(40)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Discription { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public virtual User Owner { get; set; }

    }
}