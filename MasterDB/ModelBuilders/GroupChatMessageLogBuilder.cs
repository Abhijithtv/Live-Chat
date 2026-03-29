using MasterDB.Entity;
using Microsoft.EntityFrameworkCore;

namespace MasterDB.ModelBuilders
{
    public class GroupChatMessageLogBuilder : IMasterDbBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<GroupChatMessageLog>();

            entity.HasKey(x => x.LogId);

            entity.Property(x => x.LogId)
                .UseIdentityColumn();

            //group chat 1 -> N group chat msg logs
            entity.HasOne(x => x.ToGroup)
                .WithMany(x => x.GroupChatMessageLogs)
                .HasForeignKey(x => x.ToGroupId);


            entity.HasOne(x => x.FromUser)
                 .WithMany(y => y.SentMessagesInGroup)
                .HasForeignKey(x => x.FromUserId);

            entity.HasOne(x => x.ChatMessage)
                .WithMany(x => x.GroupChatMessageLogs)
                .HasForeignKey(x => x.ChatMessageId);


            entity.Property(x => x.CreatedOnUTC)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
