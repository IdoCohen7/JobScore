using JobScoreServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScoreServer.Data.Configurations
{
    public class JobDescriptionConfiguration : IEntityTypeConfiguration<JobDescription>
    {
        public void Configure(EntityTypeBuilder<JobDescription> builder)
        {
            builder.ToTable("JobDescriptions", 
                t => t.HasCheckConstraint("CK_JobDescription_Score", "Score >= 0 AND Score <= 100"));

            builder.HasKey(j => j.Id);

            builder.Property(j => j.JobId)
                .IsRequired();

            builder.Property(j => j.UserId)
                .IsRequired();

            builder.Property(j => j.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(j => j.Content)
                .IsRequired()
                .HasMaxLength(10000);

            builder.Property(j => j.Score)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.HasMany(j => j.Violations)
                .WithOne(v => v.JobDescription)
                .HasForeignKey(v => v.JobDescriptionId)
                .OnDelete(DeleteBehavior.Cascade);









        }
    }
}
