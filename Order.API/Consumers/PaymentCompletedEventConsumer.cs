using MassTransit;
using Order.API.Models;
using Shared.Bus;

namespace Order.API.Consumers
{
    public class PaymentCompletedEventConsumer(AppDbContext appDbContext, ILogger<PaymentCompletedEventConsumer> logger) : IConsumer<PaymentCompletedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var order = await appDbContext.Orders.FindAsync(context.Message.OrderId);

            if(order != null)
            {
                order.Status = OrderStatus.Complete;
                await appDbContext.SaveChangesAsync();
                logger.LogInformation($"Order with Id : {order.Id} has been complete successfully.");
            }
            else
            {
                logger.LogWarning($"Order with Id : {context.Message.OrderId} not found.");
            }
        }
    }
}
