using Microsoft.EntityFrameworkCore;
using JobScoreServer.Models;
using JobScoreServer.Data.Configurations;

namespace JobScoreServer.Data
{
    public class DBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<JobDescription> JobDescriptions { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<Violation> Violations { get; set; }

        public DbSet<Buzzword> Buzzwords { get; set; }

        public DbSet<Chatroom> Chatrooms { get; set; }

        public DbSet<ChatroomUser> ChatroomUsers { get; set; }
        public DbSet<ChatroomMessage> ChatroomMessages { get; set; }


        public DBContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new JobDescriptionConfiguration());
            modelBuilder.ApplyConfiguration(new RuleConfiguration());
            modelBuilder.ApplyConfiguration(new ViolationConfiguration());
            modelBuilder.ApplyConfiguration(new BuzzwordConfiguration());
            modelBuilder.ApplyConfiguration(new ChatroomConfiguration());
            modelBuilder.ApplyConfiguration(new ChatroomUserConfiguration());
            modelBuilder.ApplyConfiguration(new ChatroomMessageConfiguration());

            base.OnModelCreating(modelBuilder);
        }

    }
}
