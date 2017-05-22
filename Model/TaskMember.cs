using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class TaskMember
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int TaskId { get; set; }

        public virtual User User { get; set; }
        public virtual Task Task { get; set; }
    }
}