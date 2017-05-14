using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class ProjectUser
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int Permission { get; set; }

        public virtual User User { get; set; }
        public virtual User Project { get; set; }
    }
}