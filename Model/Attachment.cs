using System;

namespace Backend.Model
{
    public class Attachment
    {
        public int Id { get; set; }

        public int FileId { get; set; }

        public int TaskId { get; set; }

        public virtual File File { get; set; }
        public virtual Task Task { get; set; }
    }
}