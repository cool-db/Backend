using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using Backend.Biz;

namespace Backend.Model
{
    public class BackendContext : DbContext
    {
        public BackendContext() : base("Backend")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<BackendContext>());
            Database.Log = Helper.Log;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("BACKEND");
            base.OnModelCreating(modelBuilder); //todo
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("\nEntity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<File> File { get; set; }
        public DbSet<Progress> Progresses { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectOperation> ProjectOperations { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Subtask> Subtasks { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskOperation> TaskOperations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet UserInfoes { get; set; }
    }
}