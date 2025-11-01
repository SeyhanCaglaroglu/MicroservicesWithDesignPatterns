using Shared.Bus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Bus.Events
{
    public class OrderCreatedRequestEvent : IOrderCreatedRequestEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; } = null!;
        public List<OrderItemMessage> OrderItems { get; set; } = null!;

        public PaymentMessage Payment { get; set; } = null!;    
    }
}
