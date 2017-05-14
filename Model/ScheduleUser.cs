using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class ScheduleUser
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [Required]
        public int ScheduleId { get; set; }

        public virtual User User { get; set; }
        public virtual Schedule Schedule { get; set; }
    }
}