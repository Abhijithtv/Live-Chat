namespace MasterDB.Entity
{
    public class ClientMessage
    {
        public Guid TransactionId { get; set; }

        public Guid ChatMessageId { get; set; }

        public Guid SenderId { get; set; }

        public string Status { get; set; }

        public DateTime LastUpdatedOnUTC { get; set; }

        public ChatMessage ChatMessage { get; set; }
    }
}
