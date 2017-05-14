﻿using System;

namespace Backend.Model
{
    public class ProjectUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public int Permission { get; set; }

        public virtual User User { get; set; }
        public virtual User Project { get; set; }
    }
}