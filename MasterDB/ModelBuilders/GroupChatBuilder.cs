using MasterDB.Entity;
using Microsoft.EntityFrameworkCore;

namespace MasterDB.ModelBuilders
{
    public class GroupChatBuilder : IMasterDbBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<GroupChat>();

            entity.HasKey(x => x.GroupChatId);

            entity.Property(x => x.GroupChatName)
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(x => x.GroupChatDescription)
                .HasMaxLength(256)
                .IsRequired();

            //N members to N group chats
            entity.HasMany(x => x.Members)
                .WithOne(y => y.GroupChat)
                .HasForeignKey(y => y.GroupChatId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(x => x.StartedOn)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(x => x.CurrentSequenceNumber)
                 .IsRequired()
                .HasDefaultValue(0);
        }
    }
}
