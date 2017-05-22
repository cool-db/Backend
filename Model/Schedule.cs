using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class Schedule
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Content { get; set; }
        
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
        
        public bool RepeatDaily { get; set; }
        
        public bool RepeatWeekly { get; set; }

        [StringLength(40)]
        public string Location { get; set; }

        public int CreatorId { get; set; }
        public int ProjectId { get; set; }
        public virtual User Creator { get; set; }
        public virtual Project Project { get; set; }
    }
}