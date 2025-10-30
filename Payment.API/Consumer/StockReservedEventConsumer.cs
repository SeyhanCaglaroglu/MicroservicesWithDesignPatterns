using MassTransit;
using Shared.Bus;

namespace Payment.API.Consumer
{
    public class StockReservedEventConsumer(ILogger<StockReservedEventConsumer> logger, IPublishEndpoint publishEndpoint) : IConsumer<StockReservedEvent>
    {
        public Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            var balance = 3000m;

            //Bakiye yeterli ise PaymentCompletedEvent gonder
            if (balance > context.Message.PaymentMessage.TotalPrice)
            {
                //Para Cekme islemleri yapilir
                var remainingBalance = balance - context.Message.PaymentMessage.TotalPrice;

                logger.LogInformation($"Payment Success for Buyer Id :{context.Message.BuyerId}, Remaining balance ={remainingBalance}");

                PaymentCompletedEvent paymentCompletedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    BuyerId = context.Message.BuyerId
                };
                return publishEndpoint.Publish(paymentCompletedEvent);
            }
            //Degilse PaymentFailedEvent gonder
            else
            {
                logger.LogInformation($"Payment Failed for Buyer Id :{context.Message.BuyerId}, Remaining balance ={balance}");

                PaymentFailedEvent paymentFailedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    BuyerId = context.Message.BuyerId,
                    Message = "Insufficient balance",
                    OrderItems = context.Message.OrderItems
                };
                return publishEndpoint.Publish(paymentFailedEvent);
            }
        }
    }
}
