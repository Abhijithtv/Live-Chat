
namespace MasterDB.Entity
{
    //az function get triggered by queue
    public class SentEventToQueueMapping
    {
        public Guid ClientTransactionId { get; set; }
        public string AzureMessageId { get; set; }
        public string AzureCorrelationId { get; set; }
        public Guid ChatMessageId { get; set; } //todo -add fk
        public int ProcessCount { get; set; } //might have to update
    }
}
