namespace ChatCommon.Models
{
    public class UserSentMsgAck
    {
        public Guid TransactionId { get; set; }
        public string Status { get; set; }

        public string Msg { get; set; }
    }
}
