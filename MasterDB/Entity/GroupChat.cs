namespace MasterDB.Entity
{
    //created by az function for group chat
    public class GroupChat
    {
        public Guid GroupChatId { get; set; }
        public string GroupChatName { get; set; }
        public string GroupChatDescription { get; set; }
        public DateTime StartedOn { get; set; }
        public int CurrentSequenceNumber { get; set; }
        public ICollection<GroupChatUser> Members { get; set; }
        public ICollection<GroupChatMessageLog> GroupChatMessageLogs { get; set; }
    }
}
