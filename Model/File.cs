using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class File
    {
        public int Id { get; set; }

        [StringLength(40)]
        public string Name { get; set; }

        //todo
        public string Content { get; set; }

        [Required]
        public DateTime UploadTime { get; set; }

        [Required]
        public int UploaderId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public virtual User UpLoader { get; set; }
        public virtual Project Project { get; set; }
    }
}