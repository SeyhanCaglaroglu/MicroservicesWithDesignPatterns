using Shared.Bus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Bus.Events
{
    public class StockNotReservedEvent : IStockNotReservedEvent
    {
        public string Reason { get; set; } = null!;

        public Guid CorrelationId { get; }

        public StockNotReservedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}
