using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class Task
    {
        public int Id { get; set; }

        [StringLength(40)]
        public string Name { get; set; }

//        [StringLength(40)]
        public string Content { get; set; }

        public DateTime Ddl { get; set; }

        public Boolean State { get; set; }

//        [Required]
        public int ProgressId { get; set; }
//        [Required]
        public int ExecutorId { get; set; }

        public virtual Progress Progress { get; set; }
        public virtual User Executor { get; set; }
    }
}