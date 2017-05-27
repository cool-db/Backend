using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Cache;

namespace Backend.Model
{
    public class User
    {
        public User()
        {
            Projects = new List<Project>();
            Tasks = new List<Task>();
            Comments = new List<Comment>();
            Schedules = new List<Schedule>();
            Subtasks = new List<Subtask>();
        }
        
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Email { get; set; }

        [Required]
        [MaxLength(20)]
        public string Password { get; set; }

        [MaxLength(20)]
        [Index(IsUnique=true)]
        public string Token { get; set; }
        
        
        public virtual UserInfo UserInfo { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<Subtask> Subtasks { get; set; }

    }
}