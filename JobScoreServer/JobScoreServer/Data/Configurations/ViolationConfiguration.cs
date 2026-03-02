using JobScoreServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScoreServer.Data.Configurations
{
    public class ViolationConfiguration : IEntityTypeConfiguration<Violation>
    {
        public void Configure(EntityTypeBuilder<Violation> builder)
        {
            builder.ToTable("Violations");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.RuleId)
                .IsRequired();

            builder.Property(v => v.JobDescriptionId)
                .IsRequired();

            builder.Property(v => v.Impact)
                .IsRequired();

        }
    }
}
