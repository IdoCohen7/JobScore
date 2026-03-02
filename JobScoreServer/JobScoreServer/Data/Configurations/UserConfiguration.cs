using JobScoreServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScoreServer.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);

            // making sure email is unique
            builder.HasIndex(x => x.Email)
                .IsUnique();

            // for max length after hashing
            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.IsAdmin)
                .IsRequired();

            builder.HasMany(x => x.JobDescriptions)
                .WithOne(j => j.User)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.NoAction);


        }
    }
}
