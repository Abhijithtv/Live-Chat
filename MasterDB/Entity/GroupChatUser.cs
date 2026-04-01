namespace MasterDB.Entity
{
    //used by chat server
    public class GroupChatUser
    {
        public Guid GroupChatId { get; set; }
        public GroupChat GroupChat { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
