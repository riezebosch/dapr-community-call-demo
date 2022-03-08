using MassTransit;
using ServiceB;

var bus = Bus.Factory
    .CreateUsingRabbitMq(cfg =>
    {
        cfg.Host("amqp://localhost:8395");
        cfg.ReceiveEndpoint("serviceb-requests", x =>
        {
            x.Consumer<Consumer>();
        });
    });

await bus.StartAsync();
Console.ReadLine();