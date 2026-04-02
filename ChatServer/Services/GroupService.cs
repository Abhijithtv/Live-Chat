
using ChatCommon.DTO;
using ChatServer.Handlers.SocketHandler;
using ChatServer.Managers;
using MasterDB;
using MasterDB.Entity;
using Microsoft.EntityFrameworkCore;

namespace ChatServer.Services
{
    public class GroupService(MasterDBContext masterDBContext,
        MessageFwdHandler messageFwdHandler,
        GroupMembersManager groupMembersManager)
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

        internal async Task CreateGroup(GroupChatDTO groupInfo)
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

        internal GroupChatDTO GetGroup(Guid groupId)
        {
            var group = masterDBContext.GroupChat.Where(x => x.GroupChatId == groupId)
                .Include(y => y.Members)
                .ThenInclude(x => x.User)
                .FirstOrDefault();

            if (group == null)
            {
                throw new InvalidOperationException("group not found");
            }

            return new GroupChatDTO
            {
                Name = group.GroupChatName,
                Description = group.GroupChatDescription,
                Memebers = group.Members.Select(x => new UserDTO
                {
                    UserEmail = x.User.UserEmail,
                    UserName = x.User.UserName
                }).ToList()
            };
        }

        public async Task SendMessageAsync(ChatMessageOpsDTO chatMessageOpsDTO)
        {
            var msgLog = await masterDBContext
                .GroupChatMessageLog
                .Include(x => x.ChatMessage)
                .Where(x => x.ChatMessageId == chatMessageOpsDTO.MessageId)
                .FirstAsync();

            var members = groupMembersManager.GetMembersByGroupId(msgLog.ToGroupId);

            await messageFwdHandler.FwdAsync(msgLog, members, chatMessageOpsDTO.TransactionId);
        }
    }
}
