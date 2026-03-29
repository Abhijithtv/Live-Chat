namespace MasterDB.Entity
{
    public class User
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }


        #region ER
        public ICollection<ChatMessageLog> SentMessages { get; set; }
        public ICollection<ChatMessageLog> ReceivedMessages { get; set; }
        public ICollection<GroupChatMessageLog> SentMessagesInGroup { get; set; }
        public ICollection<GroupChatUser> GroupMemberships { get; set; }
        #endregion
    }
}
