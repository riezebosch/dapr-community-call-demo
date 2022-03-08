using Dapr;

namespace ServiceA.Api;

public static class Startup
{
    public static Task Start(params string[] args) => 
        App(Builder(args)).RunAsync();

    public static WebApplication App(Action<WebApplicationBuilder> configure)
    {
        var builder = Builder();
        configure(builder);
        
        return App(builder);
    }

    private static WebApplication App(WebApplicationBuilder builder)
    {
        var app = builder.Build();
        app.MapGet("api/request/{id}", 
            async (string id, IRequests handler) => Results.Ok(await handler.Get(id)));
        app.MapPost("api/request", 
            async (Request request, IRequests handler) => Results.Ok(await handler.New(request)));
        app.MapPost("api/request/approve", 
            [Topic("my-pubsub", "requests/approved")] async (Approved request, IRequests handler) => await handler.Approved(request.Id));
        
        app.UseRouting()
            .UseCloudEvents()
            .UseEndpoints(endpoints => endpoints.MapSubscribeHandler());
        
        return app;
    }

    private static WebApplicationBuilder Builder(params string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder
            .Services
            .AddSingleton<IRequests, Handler>()
            .AddSingleton<IStore, Stores.InMemory.Store>()
            .AddSingleton<IPublisher, Publishers.Dapr.Publisher>()
            .AddDaprClient();
        
        return builder;
    }
}