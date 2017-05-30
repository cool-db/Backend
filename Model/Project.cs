using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.ClientServices.Providers;

namespace Backend.Model
{
    public class Project
    {
        public Project()
        {
            Progresses = new List<Progress>();
            Schedules = new List<Schedule>();
            Users = new List<User>();
            Files = new List<File>();
            ProjectOperations = new List<ProjectOperation>();
        }
        
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Description { get; set; }

        public int UserId { get; set; }//创建者
//        public User User { get; set; }
        
        public virtual ICollection<Progress> Progresses { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; } 
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<File> Files { get; set; }
        public virtual ICollection<ProjectOperation> ProjectOperations { get; set; }
        
    }
}