using System.Net.Mime;
using CloudEventify.MassTransit;
using MassTransit;
using ServiceB;

var bus = Bus.Factory
    .CreateUsingRabbitMq(cfg =>
    {
        cfg.Host("amqp://localhost:8395");
        cfg.ReceiveEndpoint("serviceb-requests", x =>
        {
            x.Consumer<Consumer>();
            x.Bind("requests");

            x.UseCloudEvents()
                .WithContentType(new ContentType("text/plain"));
        });
        
        cfg.Message<Approved>(x => 
            x.SetEntityName("requests/approved"));

        cfg.UseCloudEvents();
    });

await bus.StartAsync();
Console.ReadLine();