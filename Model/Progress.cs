using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class Progress
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        public int Order { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
    }
}