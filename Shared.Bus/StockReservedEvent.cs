using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Bus
{
    public class StockReservedEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; } = null!;
        public PaymentMessage PaymentMessage { get; set; } = null!;
        public List<OrderItemMessage> OrderItems { get; set; } = null!;
    }
}
