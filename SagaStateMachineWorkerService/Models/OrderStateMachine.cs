using MassTransit;
using Shared.Bus;
using Shared.Bus.Events;
using Shared.Bus.Interfaces;
using Shared.Bus.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaStateMachineWorkerService.Models
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; set; } = default!;
        public Event<IStockReservedEvent> StockReservedEvent { get; set; } = default!;
        public Event<IStockNotReservedEvent> StockNotReservedEvent { get; set; } = default!;
        public Event<IPaymentCompletedEvent> PaymentCompletedEvent { get; set; } = default!;
        public Event<IPaymentFailedEvent> PaymentFailedEvent { get; set; } = default!;

        public State OrderCreated { get; private set; } = default!;
        public State StockReserved { get; private set; } = default!;
        public State StockNotReserved { get; private set; } = default!;
        public State PaymentCompleted { get; private set; } = default!;
        public State PaymentFailed { get; private set; } = default!;
        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => OrderCreatedRequestEvent, x => x.CorrelateById<int>(x => x.OrderId, y => y.Message.OrderId).SelectId(context => Guid.CreateVersion7()));

            Event(() => StockReservedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));
            Event(() => StockNotReservedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));

            Event(() => PaymentCompletedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));
            Event(() => PaymentFailedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));


            Initially(When(OrderCreatedRequestEvent).Then(context =>
            {
                context.Saga.BuyerId = context.Message.BuyerId;
                context.Saga.OrderId = context.Message.OrderId;
                context.Saga.CreatedDate = DateTime.Now;
                context.Saga.CardName = context.Message.Payment.CardName;
                context.Saga.CardNumber = context.Message.Payment.CardNumber;
                context.Saga.CVV = context.Message.Payment.CVV;
                context.Saga.Expiration = context.Message.Payment.Expiration;
                context.Saga.TotalPrice = context.Message.Payment.TotalPrice;

            }).Then(context => { Console.WriteLine($"OrderCreatedRequestEvent before : {context.Saga}"); })
              .TransitionTo(OrderCreated)
              .Then(context => { Console.WriteLine($"OrderCreatedRequestEvent after : {context.Saga}"); })
              .Publish(context => new OrderCreatedEvent(context.Saga.CorrelationId) { OrderItems = context.Message.OrderItems }));


            During(OrderCreated, When(StockReservedEvent).TransitionTo(StockReserved).Send(new Uri($"queue:{RabbitMQSettings.PaymentStockReservedRequestQueueName}"), context =>
                        new StockReservedRequestPayment(context.Saga.CorrelationId)
                        {
                            OrderItems = context.Message.OrderItems,
                            PaymentMessage = new PaymentMessage()
                            {
                                CardName = context.Saga.CardName,
                                CardNumber = context.Saga.CardNumber,
                                CVV = context.Saga.CVV,
                                Expiration = context.Saga.Expiration,
                                TotalPrice = context.Saga.TotalPrice
                            },
                            BuyerId = context.Saga.BuyerId

                        }).Then(context => { Console.WriteLine($"StockReservedEvent After : {context.Saga}"); })
                        , When(StockNotReservedEvent).TransitionTo(StockNotReserved).Publish(context => new OrderRequestFailedEvent() { OrderId = context.Saga.OrderId, Reason = context.Message.Reason })
                        .Then(context => { Console.WriteLine($"StockNotReservedEvent After : {context.Saga}"); }));




            During(StockReserved,
                When(PaymentCompletedEvent)
                    .TransitionTo(PaymentCompleted)
                    .Publish(context => new OrderRequestCompletedEvent() { OrderId = context.Saga.OrderId })
                    .Then(context => { Console.WriteLine($"PaymentCompletedEvent After : {context.Saga}"); })
                    .Finalize()
                ,When(PaymentFailedEvent)
                    .TransitionTo(PaymentFailed)
                    .Publish(context => new OrderRequestFailedEvent() { OrderId = context.Saga.OrderId, Reason = context.Message.Reason })
                    .Send(new Uri($"queue:{RabbitMQSettings.StockRollbackRequestMessageQueueName}"), context =>
                        new StockRollbackMessage()
                        {
                            OrderItems = context.Message.OrderItems
                        })
                    .Then(context => { Console.WriteLine($"PaymentFailedEvent After : {context.Saga}"); }));

            SetCompletedWhenFinalized();
        }
    }
}
