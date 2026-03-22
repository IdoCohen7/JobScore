using JobScoreServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScoreServer.Data.Configurations
{
    public class BuzzwordConfiguration : IEntityTypeConfiguration<Buzzword>
    {
        public void Configure(EntityTypeBuilder<Buzzword> builder)
        {
            builder.ToTable("Buzzwords");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(b => b.Name)
                .IsUnique();
        }
    }
}
