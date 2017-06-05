using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class Schedule
    {
        public Schedule()
        {
            Users = new List<User>();
        }
        
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Content { get; set; }
        
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
        
        public bool RepeatDaily { get; set; }
        
        public bool RepeatWeekly { get; set; }

        [MaxLength(40)]
        public string Location { get; set; }

        public int OwerId { get; set; } //创建者
        
        public int ProjectId { get; set; }
        
        public virtual Project Project { get; set; }
        public virtual ICollection<User> Users { get; set; } //项目成员
        

    }
}