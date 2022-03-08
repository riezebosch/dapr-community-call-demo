using System.Threading.Tasks;
using Dapr.Client;
using NSubstitute;
using Xunit;

namespace ServiceA.Publishers.Dapr.Tests;

public class PublisherTests
{
    [Fact]
    public async Task New()
    {
        var request = new Request("1");
        var client = Substitute.For<DaprClient>();
        
        var publisher = new Publisher(client);
        await publisher.New(request);

        await client
            .Received()
            .PublishEventAsync("my-pubsub", "requests", request);
    }
}