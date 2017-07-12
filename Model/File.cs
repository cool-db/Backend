using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Model
{
    public class File
    {
        public File()
        {
            Tasks = new List<Task>();
        }

        
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        public byte[] Content { get; set; }

        public DateTime UploadTime { get; set; }

        
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        
        public virtual User User { get; set; } //上传文件者
        public virtual Project Project { get; set; }
        
        public virtual ICollection<Task> Tasks { get; set; } //依附的任务

    }
}