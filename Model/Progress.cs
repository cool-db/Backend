using System;

namespace Backend.Model
{
    public class Progress
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int ProjectId { get; set; }
    }
}