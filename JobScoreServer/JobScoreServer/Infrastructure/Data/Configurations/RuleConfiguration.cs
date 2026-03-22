using JobScoreServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScoreServer.Infrastructure.Data.Configurations
{
    public class RuleConfiguration : IEntityTypeConfiguration<Rule> 
    {
        public void Configure(EntityTypeBuilder<Rule> builder)
        {
            builder.ToTable("Rules");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(r => r.Description)
                .HasMaxLength(500);

            builder.Property(r => r.Weight)
                .IsRequired();

            builder.HasMany(r => r.Violations)
                .WithOne(v => v.Rule)
                .HasForeignKey(v => v.RuleId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
