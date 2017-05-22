using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Content { get; set; }

        public DateTime Time { get; set; }

        public int UserId { get; set; }
        public int TaskId { get; set; }

        public virtual User User { get; set; }
        public virtual Task Task { get; set; }
    }
}