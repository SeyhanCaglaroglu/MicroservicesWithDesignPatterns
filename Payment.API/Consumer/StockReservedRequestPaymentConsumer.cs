using MassTransit;
using Shared.Bus.Events;
using Shared.Bus.Interfaces;

namespace Payment.API.Consumer
{
    public class StockReservedRequestPaymentConsumer(ILogger<StockReservedRequestPaymentConsumer> logger, IPublishEndpoint publishEndpoint) : IConsumer<IStockReservedRequestPayment>
    {
        public async Task Consume(ConsumeContext<IStockReservedRequestPayment> context)
        {
            decimal balance = 3000m;

            if (context.Message.PaymentMessage.TotalPrice < balance)
            {
                logger.LogInformation($"Payment succeeded for BuyerId: {context.Message.BuyerId}, Amount: {context.Message.PaymentMessage.TotalPrice}");

                await publishEndpoint.Publish(new PaymentCompletedEvent(context.Message.CorrelationId));
            }
            else
            {
                logger.LogWarning($"Payment failed for BuyerId: {context.Message.BuyerId}, Amount: {context.Message.PaymentMessage.TotalPrice}");

                await publishEndpoint.Publish(new PaymentFailedEvent(context.Message.CorrelationId)
                {
                    Reason = "Insufficient balance",
                    OrderItems = context.Message.OrderItems
                });
            }
        }
    }
}
