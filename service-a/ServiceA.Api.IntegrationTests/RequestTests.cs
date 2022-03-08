using System.Threading.Tasks;
using Dapr.Client;
using FluentAssertions;
using FluentAssertions.Extensions;
using Flurl.Http;
using Hypothesist;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Wrapr;
using Xunit;
using Xunit.Abstractions;

namespace ServiceA.Api.IntegrationTests;

public class RequestTests
{
    private readonly ITestOutputHelper _output;

    public RequestTests(ITestOutputHelper output) => 
        _output = output;

    [Fact]
    public async Task New()
    {
        var handler = Substitute.For<IRequests>();
        handler
            .New(Arg.Any<Request>())
            .Returns(info => info.Arg<Request>() with { Id = "1234" });
        
        await using var app = Startup.App(builder => builder.Services.AddSingleton(handler));
        app.Urls.Add("http://localhost:1234");
        await app.StartAsync();

         var response = await "http://localhost:1234/api/request"
             .PostJsonAsync( new { Resource = "something" })
             .ReceiveJson<Request>();

         response
             .Id
             .Should()
             .Be("1234");
    }
    
    [Fact]
    public async Task Get()
    {
        var handler = Substitute.For<IRequests>();
        handler
            .Get("1234")
            .Returns(new Request("1234"));
        
        await using var app = Startup.App(builder => builder.Services.AddSingleton(handler));
        app.Urls.Add("http://localhost:1234");
        await app.StartAsync();

        var response = await "http://localhost:1234/api/request/1234"
            .GetAsync()
            .ReceiveJson<Request>();

        response
            .Id
            .Should()
            .Be("1234");
    }
    
    [Fact]
    public async Task Approved()
    {
        var hypothesis = Hypothesis.For<string>()
            .Any(x => x == "1234");
        
        var handler = Substitute.For<IRequests>();
        handler
            .When(x => x.Approved(Arg.Any<string>()))
            .Do( info =>  hypothesis.Test(info.Arg<string>()));
            
        
        await using var app = Startup.App(builder => builder.Services.AddSingleton(handler));
        app.Urls.Add("http://localhost:1234");
        await app.StartAsync();

        await using var sidecar = new Sidecar("service-a", _output.ToLogger<Sidecar>());
        await sidecar.Start(with => with
            .AppPort(1234)
            .DaprGrpcPort(3002)
            .ComponentsPath("components"));

        var client = new DaprClientBuilder()
            .UseGrpcEndpoint("http://localhost:3002")
            .Build();

        await client.PublishEventAsync("my-pubsub", "requests/approved", new { Id = "1234" });

        await hypothesis.Validate(5.Seconds());
    }
}