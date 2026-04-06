using MasterDB;
using Microsoft.Extensions.Caching.Memory;

namespace ChatServer.Managers
{
    public class GroupMembersManager(MasterDBContext masterDBContext, IMemoryCache cache)
    {
        private static readonly MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5), // TTL
            SlidingExpiration = TimeSpan.FromMinutes(2),               // refresh if accessed
            Size = 1
        };

        public List<Guid> GetMembersByGroupId(Guid groupId)
        {
            if (cache.TryGetValue(groupId, out List<Guid> members))
            {
                return members;
            }

            var res = masterDBContext
                .GroupChatUser
                .Where(x => x.GroupChatId == groupId)
                .Select(x => x.UserId)
                .ToList();

            cache.Set(groupId, res, _cacheOptions);
            return res;
        }

        //todo - call it from group join controller
        public void AddMemberToGroup(Guid groupId, Guid userId)
        {
        }

        //todo - call it from group remove controller
        public void RemoveMemberFromGroup(Guid groupId, Guid userId)
        {

        }
    }
}
