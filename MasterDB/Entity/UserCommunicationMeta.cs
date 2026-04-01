namespace MasterDB.Entity
{
    //az function do the handling (only for p2p)
    public class UserCommunicationsMeta
    {
        public Guid UserId1 { get; set; }
        public User User1 { get; set; }
        public Guid UserId2 { get; set; }
        public User User2 { get; set; }
        public int CurrentSequenceNumber { get; set; }
        public DateTime StartedOnUTC { get; set; }
        public DateTime LastUpdatedOnUTC { get; set; }
    }
}
