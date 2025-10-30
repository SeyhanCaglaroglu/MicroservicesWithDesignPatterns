using MassTransit;
using Shared.Bus;
using Stock.API.Models;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer(AppDbContext appDbContext, ILogger<OrderCreatedEventConsumer> logger, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint) : IConsumer<OrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            //Stock da ilgili urun varmi ve sayisi siparisdekinden fazlami

            var stockResult = new List<bool>();

            foreach (var item in context.Message.OrderItems)
            {
                stockResult.Add(appDbContext.Stocks.Any(s => s.ProductId == item.ProductId && s.Count >= item.Count));
            }


            //Eger fazlaysa stoktan dus ve StockReservedEvent gonder
            if (stockResult.All(x => x.Equals(true)))
            {
                foreach (var item in context.Message.OrderItems)
                {
                    var stock = appDbContext.Stocks.FirstOrDefault(s => s.ProductId == item.ProductId);
                    if (stock != null)
                    {
                        stock.Count -= item.Count;

                    }

                    await appDbContext.SaveChangesAsync();
                }

                logger.LogInformation($"Stock was reserved for Buyer Id :{context.Message.BuyerId}");


                var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StockReservedEventQueueName}"));

                StockReservedEvent stockReservedEvent = new StockReservedEvent
                {
                    OrderId = context.Message.OrderId,
                    BuyerId = context.Message.BuyerId,
                    PaymentMessage = context.Message.Payment,
                    OrderItems = context.Message.OrderItems
                };

                await sendEndpoint.Send(stockReservedEvent);
            }
            //Degilse StockNotReservedEvent gonder
            else
            {
                
                StockNotReservedEvent stockNotReservedEvent = new StockNotReservedEvent
                {
                    OrderId = context.Message.OrderId,
                    Message = "Stock not reserved"
                };

                logger.LogInformation($"Not enough stock for Buyer Id :{context.Message.BuyerId}");

                await publishEndpoint.Publish(stockNotReservedEvent);
            }
        }
    }
}
