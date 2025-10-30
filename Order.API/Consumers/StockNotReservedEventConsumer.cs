using MassTransit;
using Order.API.Models;
using Shared.Bus;

namespace Order.API.Consumers
{
    public class StockNotReservedEventConsumer(AppDbContext appDbContext, ILogger<StockNotReservedEventConsumer> logger) : IConsumer<StockNotReservedEvent>
    {
        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            var order = await appDbContext.Orders.FindAsync(context.Message.OrderId);

            if(order != null)
            {
                order.Status = OrderStatus.Fail;
                order.FailMessage = context.Message.Message;
                await appDbContext.SaveChangesAsync();

                logger.LogInformation($"Order with Id : {context.Message.OrderId} has been marked as Failed due to stock reservation failure.");
            }
            else
            {
                logger.LogError($"Order with Id : {context.Message.OrderId} not found while processing StockNotReservedEvent.");
            }
        }
    }
}
