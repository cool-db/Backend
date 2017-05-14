using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class Comment
    {
        public int Id { get; set; }

//        [StringLength(100)]
        public string Content { get; set; }

        public DateTime Time { get; set; }

//        [Required]
        public int UserId { get; set; }

//        [Required]
        public int TaskId { get; set; }

        public virtual User User { get; set; }
        public virtual Task Task { get; set; }
    }
}