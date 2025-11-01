using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Bus.Messages;
using Stock.API.Models;

namespace Stock.API.Consumers
{
    public class StockRollbackMessageConsumer(AppDbContext appDbContext, ILogger<StockRollbackMessageConsumer> logger) : IConsumer<IStockRollbackMessage>
    {
        public async Task Consume(ConsumeContext<IStockRollbackMessage> context)
        {
            foreach (var item in context.Message.OrderItems)
            {
                var stockItem = await appDbContext.Stocks.FirstOrDefaultAsync(s => s.ProductId == item.ProductId);

                if (stockItem != null)
                {
                    stockItem.Count += item.Count;

                    await appDbContext.SaveChangesAsync();
                }
            }

            
            logger.LogInformation("Stock rollback completed for OrderItems: {@OrderItems}", context.Message.OrderItems);


        }
    }
}
