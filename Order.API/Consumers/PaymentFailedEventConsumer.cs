using MassTransit;
using Order.API.Models;
using Shared.Bus;

namespace Order.API.Consumers
{
    public class PaymentFailedEventConsumer(AppDbContext appDbContext, ILogger<PaymentFailedEventConsumer> logger) : IConsumer<PaymentFailedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var order = await appDbContext.Orders.FindAsync(context.Message.OrderId);

            if(order != null)
            {
                order.Status = OrderStatus.Fail;
                order.FailMessage = context.Message.Message;
                await appDbContext.SaveChangesAsync();

                logger.LogInformation($"Order with Id : {order.Id} has been updated to PaymentFailed status.");
            }

            else
            {
                logger.LogWarning($"Order with Id : {context.Message.OrderId} not found.");
            }
        }
    }
}
