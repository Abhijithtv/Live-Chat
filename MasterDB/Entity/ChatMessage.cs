namespace MasterDB.Entity
{
    public class ChatMessage
    {
        public Guid MessageId { get; set; }
        public int SequenceNumber { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public ICollection<ChatMessageLog> ChatMessageLogs { get; set; }
        public ICollection<GroupChatMessageLog> GroupChatMessageLogs { get; set; }

    }
}
