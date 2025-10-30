using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Dtos;
using Order.API.Models;
using Shared.Bus;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(AppDbContext appDbContext, IPublishEndpoint publishEndpoint) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderCreateDto)
        {
            var order = new Models.Order
            {
                BuyerId = orderCreateDto.BuyerId,
                CreatedDate = DateTime.Now,
                Address = new Address
                {
                    Line = orderCreateDto.Address.Line,
                    Province = orderCreateDto.Address.Province,
                    District = orderCreateDto.Address.District
                },
                Items = orderCreateDto.orderItems.Select(oi => new OrderItem
                {
                    ProductId = oi.ProductId,
                    Count = oi.Count,
                    Price = oi.Price
                }).ToList(),            
                Status = OrderStatus.Suspend
            };


            await appDbContext.Orders.AddAsync(order);
            await appDbContext.SaveChangesAsync();


            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                BuyerId = order.BuyerId,
                OrderItems = order.Items.Select(oi => new OrderItemMessage
                {
                    ProductId = oi.ProductId,
                    Count = oi.Count,
                }).ToList(),
                Payment = new PaymentMessage
                {
                    CardName = orderCreateDto.payment.CardName,
                    CardNumber =  orderCreateDto.payment.CardNumber,
                    Expiration = orderCreateDto.payment.Expiration,
                    CVV = orderCreateDto.payment.CVV,
                    TotalPrice = order.Items.Sum(i => i.Price * i.Count)
                }
            };


            await publishEndpoint.Publish(orderCreatedEvent);

            return Ok();
        }
    }
}
