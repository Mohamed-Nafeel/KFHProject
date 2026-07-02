using System;

namespace KFH.Models
{
    public class TransferRequest
    {
        public int Id { get; set; }
        public int SourceAccountId { get; set; }
        public int DestinationAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; } = "Pending"; // Pending / Completed / Failed

        // Navigation properties
        public Account? SourceAccount { get; set; }
        public Account? DestinationAccount { get; set; }
    }
}
