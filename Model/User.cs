using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Model
{
    public class User
    {
        public User()
        {
            Projects = new List<Project>();
            Tasks = new List<Task>();
            Comments = new List<Comment>();
            Schedules = new List<Schedule>();
            Subtasks = new List<Subtask>();
            UserPermissons = new List<UserPermisson>();
            GenerateToken();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        [Index]
        public string Email { get; set; }

        [Required]
        [MaxLength(20)]
        public string Password { get; set; }

        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Token { get; set; }

        public virtual UserInfo UserInfo { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<Subtask> Subtasks { get; set; }
        public virtual ICollection<UserPermisson> UserPermissons { get; set; }

        public void GenerateToken()
        {
            var str = "salt" + Id + DateTime.Now;
            var md5Hash = MD5.Create();
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sBuilder = new StringBuilder();
            foreach (var t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }
            Token = sBuilder.ToString();
        }

        public bool Authorize(int id, string token)
        {
            using (var context = new BackendContext())
            {
                return context.Users.Any(user => user.Id == id && user.Token == token);
            }
        }
    }
}