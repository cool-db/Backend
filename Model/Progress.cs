using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class Progress
    {
        public int Id { get; set; }

//        [StringLength(40)]
        public string Name { get; set; }

        [Required]
        public int Order { get; set; }
        [Required]
        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }
    }
}