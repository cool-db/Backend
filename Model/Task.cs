using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class Task
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Content { get; set; }

        public DateTime Ddl { get; set; }

        public bool State { get; set; }

        [Required]
        public int ProgressId { get; set; }


        public int ExecutorId { get; set; }

        public virtual Progress Progress { get; set; }
        public virtual User Executor { get; set; }
    }
}