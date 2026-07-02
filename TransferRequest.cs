using System.Security.Principal;

namespace KFH
{
    public class TransferRequest
    {
        public int SourceAccount { get; set; }
        public int DestinationAccount { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; } = "pending";
    }
}
