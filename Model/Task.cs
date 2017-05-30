using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Backend.Model
{
    public class Task
    {
        public Task()
        {
            Users = new List<User>();
            Comments = new List<Comment>();
            Subtasks = new List<Subtask>();
            TaskOperations = new List<TaskOperation>();
            Files = new List<File>();

        }
        
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Content { get; set; }

        public DateTime Ddl { get; set; }

        public bool State { get; set; }
        
        public int ProgressId { get; set; }
        public int OwnerId { get; set; }
        public virtual Progress Progress { get; set; }

        public virtual ICollection<User> Users{ get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Subtask> Subtasks { get; set; }
        public virtual ICollection<TaskOperation> TaskOperations { get; set; }
        public virtual ICollection<File> Files { get; set; }

    }
}