
using ChatCommon.DTO;
using MasterDB;
using MasterDB.Entity;
using Microsoft.EntityFrameworkCore;

namespace ChatServer.Services
{
    public class GroupService(MasterDBContext masterDBContext)
    {
        public async Task JoinGroup(Guid groupId, Guid userId)
        {
            var exists = await masterDBContext.GroupChat
                .AnyAsync(x => x.GroupChatId == groupId);

            if (!exists)
                throw new InvalidOperationException("Group not found");

            var membership = new GroupChatUser
            {
                GroupChatId = groupId,
                UserId = userId
            };

            masterDBContext.GroupChatUser.Add(membership);

            try
            {
                await masterDBContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException("Already a member");
            }
        }

        internal async Task CreateGroup(GroupDTO groupInfo)
        {
            var group = new GroupChat
            {
                GroupChatId = Guid.NewGuid(),
                GroupChatName = groupInfo.Name,
                GroupChatDescription = groupInfo.Description,
            };
            masterDBContext.GroupChat.Add(group);
            await masterDBContext.SaveChangesAsync();
        }

        internal GroupChat? GetGroup(Guid groupId)
        {
            return masterDBContext.GroupChat.Where(x => x.GroupChatId == groupId)
                .Include(y => y.Members)
                .FirstOrDefault();
        }
    }
}
