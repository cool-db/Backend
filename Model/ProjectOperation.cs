using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class ProjectOperation
    {
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [StringLength(100)]
        public string Content { get; set; }

        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
    }
}