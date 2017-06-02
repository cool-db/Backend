using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Backend.Model
{
    public class Progress
    {
        public Progress()
        {
            Tasks = new List<Task>();
        }
        
        
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        public int Order { get; set; }
        
        public int OwnerId { get; set; }
        
        public int ProjectId { get; set; }
        
        public virtual Project Project { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}