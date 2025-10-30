using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Bus
{
    public class PaymentFailedEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; } = null!;
        public string Message { get; set; } = null!;

        public List<OrderItemMessage> OrderItems { get; set; } = null!;
    }
}
