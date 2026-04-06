namespace ChatCommon.DTO
{
    public class ChatMessageDTO
    {
        public Guid MessageId { get; set; }
        public int SequenceNumber { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }
}
