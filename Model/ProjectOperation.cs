using System;

namespace Backend.Model
{
    public class ProjectOperation
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public DateTime Time { get; set; }
        public string Content { get; set; }
    }
}