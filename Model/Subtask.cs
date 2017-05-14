using System;

namespace Backend.Model
{
    public class Subtask
    {
        public int SubtaskId { get; set; }
        public string Content { get; set; }
        public Boolean State { get; set; }
        public int ExecutorId { get; set; }
        public int TaskId { get; set; }
    }
}