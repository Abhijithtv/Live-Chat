namespace MasterDB.Entity
{
    public class ClientMessageTracker
    {
        public int TrackId { get; set; }
        public Guid ClientTransactionId { get; set; }
        public Guid UserId { get; set; }

        public Guid ToUserId { get; set; }

        public bool IsFromGroup { get; set; }
        public string Status { get; set; }
    }
}
