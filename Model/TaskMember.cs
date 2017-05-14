using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class TaskMember
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int TaskId { get; set; }

        public virtual User User { get; set; }
        public virtual Task Task { get; set; }
    }
}