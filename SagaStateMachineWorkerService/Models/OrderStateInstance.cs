using MassTransit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaStateMachineWorkerService.Models
{
    public class OrderStateInstance : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; } = null!;
        public string BuyerId { get; set; } = null!;
        public int OrderId { get; set; }

        public string CardName { get; set; } = null!;
        public string CardNumber { get; set; } = null!;
        public string Expiration { get; set; } = null!;
        public string CVV { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        public DateTime CreatedDate { get; set; }

        public override string ToString()
        {
            var properties = GetType().GetProperties();

            var sb = new StringBuilder();

            properties.ToList().ForEach(p =>
            {
                var value = p.GetValue(this, null);
                sb.AppendLine($"{p.Name}:{value}");
            });

            sb.Append("------------------------");
            return sb.ToString();
        }
    }
}
