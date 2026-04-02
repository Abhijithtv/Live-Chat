
namespace ChatCommon.DTO
{
    public class FwdMessageDTO
    {
        public Guid SenderId { get; set; }
        public Guid GroupId { get; set; }
        public Guid TransactionID { get; set; }
        public int SequenceNumber { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOnUTC { get; set; }
    }
}
