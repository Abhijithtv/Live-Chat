namespace MasterDB.Entity
{
    //created by az function
    public class GroupChatMessageLog
    {
        public int LogId { get; set; }
        public Guid ToGroupId { get; set; }
        public GroupChat ToGroup { get; set; }
        public Guid FromUserId { get; set; }
        public User FromUser { get; set; }
        public ChatMessage ChatMessage { get; set; }
        public Guid ChatMessageId { get; set; }
        public DateTime CreatedOnUTC { get; set; }
    }
}
