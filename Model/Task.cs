using System;

namespace Backend.Model
{
    public class Task
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime Ddl { get; set; }
        public Boolean State { get; set; }
        public int ProgressId { get; set; }
        public int ExecutorId { get; set; }
    }
}