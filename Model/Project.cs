using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public int OwnerId { get; set; }
        public virtual User Owner { get; set; }
    }
}