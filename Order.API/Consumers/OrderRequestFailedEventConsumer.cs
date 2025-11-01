using MassTransit;
using Order.API.Models;
using Shared.Bus.Interfaces;

namespace Order.API.Consumers
{
    public class OrderRequestFailedEventConsumer(AppDbContext appDbContext, ILogger<OrderRequestFailedEventConsumer> logger) : IConsumer<IOrderRequestFailedEvent>
    {

        public async Task Consume(ConsumeContext<IOrderRequestFailedEvent> context)
        {
            var order = await appDbContext.Orders.FindAsync(context.Message.OrderId);

            if (order != null)
            {
                order.Status = OrderStatus.Fail;
                order.FailMessage = context.Message.Reason;

                await appDbContext.SaveChangesAsync();

                logger.LogInformation("Order with Id {OrderId} marked as Failed. Reason: {Reason}", context.Message.OrderId, context.Message.Reason);
            }
            else
            {
                logger.LogWarning("Order with Id {OrderId} not found.", context.Message.OrderId);
            }
        }
    }
}
