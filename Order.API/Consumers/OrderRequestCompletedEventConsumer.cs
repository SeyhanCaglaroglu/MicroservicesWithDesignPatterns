using MassTransit;
using Order.API.Models;
using Shared.Bus.Interfaces;

namespace Order.API.Consumers
{
    public class OrderRequestCompletedEventConsumer(AppDbContext appDbContext, ILogger<OrderRequestCompletedEventConsumer> logger) : IConsumer<IOrderRequestCompletedEvent>
    {
        public async Task Consume(ConsumeContext<IOrderRequestCompletedEvent> context)
        {
            var order = await appDbContext.Orders.FindAsync(context.Message.OrderId);

            if (order != null)
            {
                order.Status = OrderStatus.Complete;

                await appDbContext.SaveChangesAsync();

                logger.LogInformation($"Order with Id {order.Id} has been completed.");
            }
            else
            {
                logger.LogWarning($"Order with Id {context.Message.OrderId} not found.");
            }
        }
    }
}
