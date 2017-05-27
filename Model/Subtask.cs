using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class Subtask
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(80)]
        public string Content { get; set; }

        public bool State { get; set; }

        public int UserID { get; set; }
        public int TaskId { get; set; }

        public virtual User User { get; set; }
        public virtual Task Task { get; set; }
    }
}