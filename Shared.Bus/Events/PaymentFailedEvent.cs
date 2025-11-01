using Shared.Bus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Bus.Events
{
    public class PaymentFailedEvent : IPaymentFailedEvent
    {
        public string Reason { get; set; } = null!;
        public List<OrderItemMessage> OrderItems { get; set; } = new();

        public Guid CorrelationId { get; }

        public PaymentFailedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}
