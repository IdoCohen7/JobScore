using JobScoreServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScoreServer.Data.Configurations
{
    public class ChatroomConfiguration : IEntityTypeConfiguration<Chatroom>
    {
        public void Configure(EntityTypeBuilder<Chatroom> builder)
        {
            builder.ToTable("Chatrooms");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(c => c.Title)
                .IsUnique();

        }
    }
}
