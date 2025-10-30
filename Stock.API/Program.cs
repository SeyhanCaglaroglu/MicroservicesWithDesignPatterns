using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Bus;
using Stock.API.Consumers;
using Stock.API.Extensions;
using Stock.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedEventConsumer>();
    x.AddConsumer<PaymentFailedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));


        cfg.ReceiveEndpoint(RabbitMQSettings.StockOrderCreatedQueueName, e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
        });

        cfg.ReceiveEndpoint(RabbitMQSettings.StockPaymentFailedEventQueueName, e =>
        {
            e.ConfigureConsumer<PaymentFailedEventConsumer>(context);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.AddSeedDataExt().ContinueWith(x =>
{
    Console.WriteLine(x.IsFaulted ? x.Exception?.Message : "Seed data has been saved successfully");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
