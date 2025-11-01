using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Bus.Events;
using Shared.Bus.Interfaces;
using Stock.API.Models;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer(AppDbContext appDbContext, ILogger<OrderCreatedEventConsumer> logger, IPublishEndpoint publishEndpoint) : IConsumer<IOrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<IOrderCreatedEvent> context)
        {
            var stockResult = new List<bool>();

            foreach (var item in context.Message.OrderItems)
            {
                stockResult.Add(
                    await appDbContext.Stocks.AnyAsync(x => x.ProductId == item.ProductId && x.Count > item.Count));
            }

            if (stockResult.All(x => x.Equals(true)))
            {
                foreach (var item in context.Message.OrderItems)
                {
                    var stock = await appDbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                    if (stock != null)
                    {
                        stock.Count -= item.Count;
                    }

                    await appDbContext.SaveChangesAsync();
                }

                logger.LogInformation($"Stock was reserved for CorrelationId Id :{context.Message.CorrelationId}");


                StockReservedEvent stockReservedEvent = new(context.Message.CorrelationId)
                {
                    OrderItems = context.Message.OrderItems
                };

                await publishEndpoint.Publish(stockReservedEvent);
            }

            else
            {
                await publishEndpoint.Publish(new StockNotReservedEvent(context.Message.CorrelationId)
                {
                    Reason = "Not enough stock"
                });

                logger.LogInformation($"Stock was not reserved for CorrelationId Id :{context.Message.CorrelationId}");
            }
        }
    }
}
