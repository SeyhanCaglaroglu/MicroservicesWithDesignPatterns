using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaStateMachineWorkerService;
using SagaStateMachineWorkerService.Models;
using Shared.Bus;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(cfg =>
{
    // Saga State Machine ve Entity Framework Repository Yapýlandýrmasý
    cfg.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>().EntityFrameworkRepository(opt =>
    {
        opt.AddDbContext<DbContext, OrderStateDbContext>((provider, dbContextOptionsBuilder) =>
        {
            // SQL Server veritabaný baðlantýsý ve migration assembly yapýlandýrmasý
            dbContextOptionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"), m =>
            {
                m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
            });
        });
    });

    // RabbitMQ Mesaj Kuyruk Sistemi Yapýlandýrmasý
    cfg.UsingRabbitMq((context, rabbitCfg) =>
    {
        // RabbitMQ sunucu baðlantýsý
        rabbitCfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

        // Saga için mesaj kuyruðu yapýlandýrmasý
        rabbitCfg.ReceiveEndpoint(RabbitMQSettings.OrderSaga, e => 
        {
            // Saga repository'nin yapýlandýrýlmasý:
            // - OrderSaga kuyruðuna gelen mesajlarý dinler
            // - Gelen mesajlarý OrderStateInstance ile iliþkilendirir
            // - Ýlgili saga durumunu veritabanýnda saklar ve günceller
            e.ConfigureSaga<OrderStateInstance>(context);
        });
    });
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
