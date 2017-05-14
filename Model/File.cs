using System;

namespace Backend.Model
{
    public class File
    {
        public int FileId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime UploadTime { get; set; }
        public int UploaderId { get; set; }
        public int ProjectId { get; set; }
    }
}