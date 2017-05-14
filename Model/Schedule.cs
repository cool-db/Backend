using System;

namespace Backend.Model
{
    public class Schedule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Boolean RepeatDaily { get; set; }
        public Boolean RepeatWeekly { get; set; }
        public string Location { get; set; }
        public int CreatorId { get; set; }
        public int ProjectId { get; set; }

        public virtual User Creator { get; set; }
        public virtual Project Project { get; set; }
    }
}