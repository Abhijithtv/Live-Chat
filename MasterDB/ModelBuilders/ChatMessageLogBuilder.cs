using MasterDB.Entity;
using Microsoft.EntityFrameworkCore;

namespace MasterDB.ModelBuilders
{
    public class ChatMessageLogBuilder : IMasterDbBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ChatMessageLog>();

            entity.HasKey(x => x.LogId);

            entity.Property(x => x.LogId)
                .UseIdentityColumn();

            //chatmsglog N : users 1 (an user can have many msg log)
            entity.HasOne(x => x.FromUser)
                .WithMany(y => y.SentMessages)
                .HasForeignKey(x => x.FromUserId);

            entity.HasOne(x => x.ToUser)
                .WithMany(y => y.ReceivedMessages)
                .HasForeignKey(x => x.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            //N log can point to one chat Message
            entity.HasOne(x => x.ChatMessage)
                .WithMany(m => m.ChatMessageLogs)
                .HasForeignKey(x => x.ChatMessageId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(x => x.CreatedOnUTC)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
