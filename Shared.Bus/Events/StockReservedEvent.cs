using Shared.Bus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Bus.Events
{
    public class StockReservedEvent:IStockReservedEvent
    {
        
        public List<OrderItemMessage> OrderItems { get; set; } = null!;

        public Guid CorrelationId { get;}

        public StockReservedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}
