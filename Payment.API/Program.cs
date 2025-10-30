using MassTransit;
using Payment.API.Consumer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Mass Transit Configuration
builder.Services.AddMassTransit(x=>
{
    x.AddConsumer<StockReservedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

        cfg.ReceiveEndpoint(Shared.Bus.RabbitMQSettings.StockReservedEventQueueName, e =>
        {
            e.ConfigureConsumer<StockReservedEventConsumer>(context);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
