using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class TaskOperation
    {
        public int Id { get; set; }

        [Required]
        public int TaskId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [StringLength(80)]
        public string Content { get; set; }

        public virtual User Task { get; set; }
        public virtual User User { get; set; }
    }
}