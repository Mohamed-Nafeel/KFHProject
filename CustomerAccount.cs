using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KFH
{
    public class CustomerAccount
    {
        public int AccountNumber { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal CurrentBalance { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; } ="active";

    }
}
