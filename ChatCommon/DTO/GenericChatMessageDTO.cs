namespace ChatCommon.DTO
{
    public class GenericChatMessageDTO
    {
        public Guid TransactionId { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public Guid ToUserId { get; set; }
        public Guid FromUserId { get; set; }
        public bool IsFromGroup { get; set; }
    }
}
