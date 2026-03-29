using MasterDB.Entity;
using Microsoft.EntityFrameworkCore;

namespace MasterDB.ModelBuilders
{
    public class GroupChatUserBuilder : IMasterDbBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<GroupChatUser>();

            entity.HasKey(x => new { x.GroupChatId, x.UserId });

            entity.HasOne(x => x.User)
                .WithMany(y => y.GroupMemberships)
                .HasForeignKey(x => x.UserId);

            entity.HasOne(x => x.GroupChat)
               .WithMany(y => y.Members)
               .HasForeignKey(x => x.GroupChatId);
        }
    }
}
