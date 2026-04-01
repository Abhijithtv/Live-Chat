namespace MasterDB.Entity
{
    //from client to socket server - sent event
    public class ClientMessageTracker
    {
        public int TrackId { get; set; }
        public Guid ClientTransactionId { get; set; }
        public Guid UserId { get; set; }
        public Guid ToUserId { get; set; }
        public bool IsFromGroup { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOnUTC { get; set; }
    }
}
