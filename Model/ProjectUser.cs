using System;

namespace Backend.Model
{
    public class ProjectUser
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public int Permission { get; set; }
    }
}