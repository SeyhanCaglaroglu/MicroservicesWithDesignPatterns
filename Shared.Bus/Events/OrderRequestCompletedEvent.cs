using Shared.Bus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Bus.Events
{
    public class OrderRequestCompletedEvent : IOrderRequestCompletedEvent
    {
        public int OrderId { get; set; }
    }
}
