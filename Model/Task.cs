using System;

namespace Backend.Model
{
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime Ddl { get; set; }
        public Boolean State { get; set; }
        public int ProgressId { get; set; }
        public int ExecutorId { get; set; }

        public virtual Progress Progress { get; set; }
        public virtual User Executor { get; set; }
    }
}