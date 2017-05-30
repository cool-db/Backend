using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class TaskOperation
    {
        public int Id { get; set; }

        public DateTime Time { get; set; }

        [Required]
        [MaxLength(100)]
        public string Content { get; set; }

        public int TaskId { get; set; }
        public int UserId { get; set; }//执行者
        
        public virtual User User { get; set; }
        public virtual Task Task { get; set; }
    }
}