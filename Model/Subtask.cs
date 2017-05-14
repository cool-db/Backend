using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class Subtask
    {
        public int Id { get; set; }

//        [StringLength(80)]
        public string Content { get; set; }

        public Boolean State { get; set; }

//        [Required]
        public int ExecutorId { get; set; }
//        [Required]
        public int TaskId { get; set; }

        public virtual User Executor { get; set; }
        public virtual Task Task { get; set; }
    }
}