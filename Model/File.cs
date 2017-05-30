using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class File
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        public byte[] Content { get; set; }

        public DateTime UploadTime { get; set; }

        
        public int UserId { get; set; }
        public int TaskId { get; set; }
        
        public virtual User User { get; set; }
        public virtual Task Task { get; set; }
    }
}