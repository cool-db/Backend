using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class ProjectOperation
    {
        public int Id { get; set; }


        public int ProjectId { get; set; }

        public int UserId { get; set; }

        public DateTime Time { get; set; }

        public string Content { get; set; }

        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
    }
}