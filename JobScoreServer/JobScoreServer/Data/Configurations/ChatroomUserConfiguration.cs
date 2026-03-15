using JobScoreServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace JobScoreServer.Data.Configurations
{
    public class ChatroomUserConfiguration : IEntityTypeConfiguration<ChatroomUser>
    {
        public void Configure(EntityTypeBuilder<ChatroomUser> builder)
        {
            builder.ToTable("ChatroomUsers");

            builder.HasKey(cu => new { cu.UserId, cu.ChatroomId });

            builder.Property(cu => cu.UserId).IsRequired();

            builder.Property(cu => cu.ChatroomId).IsRequired();

            builder.Property(cu => cu.IsAdmin).HasDefaultValue(false).IsRequired();

            builder.HasOne(cu => cu.User)
                .WithMany(u => u.ChatroomUsers)
                .HasForeignKey(cu => cu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cu => cu.Chatroom)
                .WithMany(c => c.ChatroomUsers)
                .HasForeignKey(cu => cu.ChatroomId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
