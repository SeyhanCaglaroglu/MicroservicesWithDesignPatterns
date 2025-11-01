using Shared.Bus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Bus.Events
{
    public class StockReservedRequestPayment : IStockReservedRequestPayment
    {
        public PaymentMessage PaymentMessage { get; set; } = null!;
        public List<OrderItemMessage> OrderItems { get; set; } = null!;
        public string BuyerId { get; set; } = null!;

        public Guid CorrelationId { get; }

        public StockReservedRequestPayment(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}
