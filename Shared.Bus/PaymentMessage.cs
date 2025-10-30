using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Bus
{
    public class PaymentMessage
    {
        public string CardName { get; set; } = null!;
        public string CardNumber { get; set; } = null!;
        public string Expiration { get; set; } = null!;
        public string CVV { get; set; } = null!;
        public decimal TotalPrice { get; set; }
    }
}
