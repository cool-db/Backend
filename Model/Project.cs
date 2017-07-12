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
            ProjectOperations = new List<ProjectOperation>();
            UserPermissons = new List<UserPermisson>();
            Files = new List<File>();
        }
        
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Description { get; set; }

        public int OwnerId { get; set; }//创建者
        
        public virtual ICollection<Progress> Progresses { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; } 
        public virtual ICollection<User> Users { get; set; } //项目成员
        public virtual ICollection<ProjectOperation> ProjectOperations { get; set; }
        public virtual ICollection<UserPermisson> UserPermissons { get; set; }
        public virtual ICollection<File> Files { get; set; }
        
    }
}