using System.Threading.Tasks;
using FluentAssertions.Extensions;
using Hypothesist;
using MassTransit;
using Xunit;

namespace ServiceB.IntegrationTests;

public class ConsumerTests : IClassFixture<RabbitMqContainer>
{
    private readonly RabbitMqContainer _container;

    public ConsumerTests(RabbitMqContainer container) => 
        _container = container;

    [Fact]
    public async Task RequestApprove()
    {
        // Arrange
        var hypothesis = Hypothesis
            .For<Approved>()
            .Any(x => x.Id == "1234");
        
        var bus = Bus.Factory
            .CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(_container.ConnectionString);
                cfg.ReceiveEndpoint("serviceb-requests", 
                    x => x.Consumer<Consumer>());
                
                cfg.ReceiveEndpoint("serviceb-request-approved", 
                    x => x.Consumer(hypothesis.AsConsumer));
            });

        await bus.StartAsync();
        
        // Act
        await bus.Publish(new Request("1234"));

        // Assert
        await hypothesis.Validate(5.Seconds());
    }
}