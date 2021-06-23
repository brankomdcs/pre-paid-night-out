using System;

namespace Transaction.Models
{
    public class PaymentTransaction
    {
        public int UserId { get; set; }
        public string AccountTo { get; set; }
        public decimal Amount { get; set; }
        public DateTime Time { get; set; }
    }
}
