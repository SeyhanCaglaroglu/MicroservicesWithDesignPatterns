using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Bus
{
    public class RabbitMQSettings
    {
        public const string OrderSaga = "order-saga-queue";
        public const string StockOrderCreatedQueueName = "stock-order-created-queue";
        public const string StockReservedEventQueueName = "stock-reserved-queue";
        public const string OrderPaymentCompletedEventQueueName = "order-payment-completed-event-queue";
        public const string OrderPaymentFailedEventQueueName = "order-payment-failed-event-queue";
        public const string OrderStockNotReservedEventQueueName = "order-stock-not-reserved-event-queue";
        public const string StockPaymentFailedEventQueueName = "stock-payment-failed-event-queue";
        public const string PaymentStockReservedRequestQueueName = "payment-stock-reserved-request-queue";
        public const string OrderRequestCompletedEventQueueName = "order-request-completed-event-queue";
        public const string OrderRequestFailedEventQueueName = "order-request-failed-event-queue";
        public const string StockRollbackRequestMessageQueueName = "stock-rollback-request-message-queue";
    }
}
