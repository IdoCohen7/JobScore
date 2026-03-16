using JobScoreServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScoreServer.Data.Configurations
{
    public class ChatroomMessageConfiguration : IEntityTypeConfiguration<ChatroomMessage>
    {
        public void Configure(EntityTypeBuilder<ChatroomMessage> builder)
        {
            builder.ToTable("ChatroomMessages");

            builder.HasKey(cm => cm.Id);

            builder.Property(cm => cm.Content).IsRequired().HasMaxLength(10000);

            builder.Property(cm => cm.UserId).IsRequired();

            builder.Property(cm => cm.ChatroomId).IsRequired();

            builder.HasOne(cm => cm.User).WithMany(u => u.ChatroomMessages).HasForeignKey(cm => cm.UserId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cm => cm.Chatroom).WithMany(cr => cr.ChatroomMessages).HasForeignKey(cm => cm.ChatroomId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
