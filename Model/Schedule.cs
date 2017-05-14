using System;

namespace Backend.Model
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Boolean RepeatDaily { get; set; }
        public Boolean RepeatWeekly { get; set; }
        public string Location { get; set; }
        public int CreatorId { get; set; }
        public int ProjectId { get; set; }
    }
}