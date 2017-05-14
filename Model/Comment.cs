using System;

namespace Backend.Model
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public int UserId { get; set; }
        public int TaskId { get; set; }
    }
}