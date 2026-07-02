using System;

namespace KFH.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public decimal CurrentBalance { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; } = "Active"; // Active / Blocked
    }
}
