using System;
namespace Backend.Model
{
    public class TaskOperation
    {
     public   int  TaskId { get; set; }
     public    int    UserId { get; set; }
        public     DateTime   Time { get; set; }
        public    string Content { get; set; }
    }
}