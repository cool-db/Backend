using System;

namespace Backend.Model
{
    public class TaskOperation
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public DateTime Time { get; set; }
        public string Content { get; set; }

        public virtual User Task { get; set; }
        public virtual User User { get; set; }
    }
}