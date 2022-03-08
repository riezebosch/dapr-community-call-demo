using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ServiceA.Tests;

public class HandlerTests
{
    [Fact]
    public async Task New()
    {
        var request = new Request("0");

        var store = Substitute.For<IStore>();
        store.New(request).Returns(request);
        
        var publisher = Substitute.For<IPublisher>();
        
        var handler = new Handler(store, publisher);
        await handler.New(request);

        await store
            .Received()
            .New(request);
        await publisher
            .Received()
            .New(request);
    }
    
    [Fact]
    public async Task Approve()
    {
        var request = new Request("1234");

        var store = Substitute.For<IStore>();
        store.Get("1234").Returns(request);

        var handler = new Handler(store, Substitute.For<IPublisher>());
        await handler.Approved("1234");

        await store
            .Received()
            .Update(request with { Status = Status.Approved });
    }
    
    [Fact]
    public async Task Get()
    {
        // Arrange
        var store = Substitute.For<IStore>();
        store.Get("1234").Returns(new Request("1234"));
        
        // Act
        var handler = new Handler(store, Substitute.For<IPublisher>());
        var (id, _) = await handler.Get("1234");
        
        // Assert
        id.Should().Be("1234");
    }
}