using System;

namespace Backend.Model
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
        public int OnwerId { get; set; }
    }
}