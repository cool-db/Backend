using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class Schedule
    {
        public int Id { get; set; }

//        [StringLength(40)]
        public string Name { get; set; }
//        [StringLength(200)]
        public string Content { get; set; }
//        [Required]
        public DateTime StartTime { get; set; }


        public DateTime EndTime { get; set; }
        public Boolean RepeatDaily { get; set; }
        public Boolean RepeatWeekly { get; set; }

//        [StringLength(80)]
        public string Location { get; set; }

//        [Required]
        public int CreatorId { get; set; }
//        [Required]
        public int ProjectId { get; set; }

        public virtual User Creator { get; set; }
        public virtual Project Project { get; set; }
    }
}