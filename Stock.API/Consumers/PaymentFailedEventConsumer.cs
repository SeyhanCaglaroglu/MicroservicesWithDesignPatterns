using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Bus;
using Stock.API.Models;

namespace Stock.API.Consumers
{
    public class PaymentFailedEventConsumer(AppDbContext appDbContext, ILogger<PaymentFailedEventConsumer> logger) : IConsumer<PaymentFailedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            foreach(var item in context.Message.OrderItems)
            {
                var stock = await appDbContext.Stocks.FirstOrDefaultAsync(s => s.ProductId == item.ProductId);

                if(stock != null)
                {
                    stock.Count += item.Count;
                    appDbContext.SaveChanges();

                    logger.LogInformation($"Stock for ProductId {item.ProductId} increased by {item.Count}. New count: {stock.Count}");
                }
                else
                {
                    logger.LogWarning($"Stock record not found for ProductId {item.ProductId}. Cannot increase stock.");    
                }
            }

            
        }
    }
}
