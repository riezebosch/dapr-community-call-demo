using System.Threading.Tasks;
using MassTransit;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Xunit;

namespace ServiceB.Tests;

public class ConsumerTests
{
    [Fact]
    public async Task Approve()
    {
        var context = Substitute.For<ConsumeContext<Request>>();
        context.Message.Returns(new Request("1234"));
        
        var handler = new Consumer();
        
        await handler.Consume(context);
        await context
            .Received()
            .Publish(new Approved("1234"));
    }
}