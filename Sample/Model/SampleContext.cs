using System.Data.Entity;

namespace Backend.Sample.Model
{
    public class SampleContext : DbContext
    {
        public SampleContext() : base("Sample")
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<SampleContext>());
        }

        public DbSet<SampleData> Datas { get; set; }
        public DbSet<SampleFile> Files { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("SAMPLE");
        }
    }
}