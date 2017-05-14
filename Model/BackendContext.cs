using System.Data.Entity;

namespace Backend.Model
{
    public class BackendContext : DbContext
    {
        public BackendContext() : base("Backend")
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<BackendContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("BACKEND");
        }

        public DbSet<Attachment>           Attachments { get; set; }
        public DbSet<Comment>              Comments { get; set; }
        public DbSet<File>                 File { get; set; }
        public DbSet<Progress>             Progresses { get; set; }
        public DbSet<Project>              Projects { get; set; }
        public DbSet<ProjectOperation>     ProjectOperations { get; set; }
        public DbSet<ProjectUser>          ProjectUsers { get; set; }
        public DbSet<Schedule>             Schedules { get; set; }
        public DbSet<ScheduleUser>         ScheduleUsers { get; set; }
        public DbSet<Subtask>              Subtasks { get; set; }
        public DbSet<Task>                 Tasks { get; set; }
        public DbSet<TaskMember>           TaskMembers { get; set; }
        public DbSet<TaskOperation>        TaskOperations { get; set; }
        public DbSet<User>                 Users { get; set; }
    }
}