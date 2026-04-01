namespace MasterDB.Entity
{
    //created by az functions
    public class ChatMessageLog
    {
        public int LogId { get; set; }
        public Guid FromUserId { get; set; }
        public User FromUser { get; set; }
        public Guid ToUserId { get; set; }
        public User ToUser { get; set; }
        public Guid ChatMessageId { get; set; }
        public ChatMessage ChatMessage { get; set; }
        public DateTime CreatedOnUTC { get; set; }
    }
}
