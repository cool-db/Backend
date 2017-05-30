using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class ProjectOperation
    {
        public int Id { get; set; }

        public DateTime Time { get; set; }

        [Required]
        [MaxLength(200)]
        public string Content { get; set; }
        
        public int ProjectId { get; set; }
        public int UserId { get; set; } //操作者

        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
    }
}