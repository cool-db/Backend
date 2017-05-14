using System;

namespace Backend.Model
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
        public int OnwerId { get; set; }

        public virtual User Onwer { get; set; }

    }
}