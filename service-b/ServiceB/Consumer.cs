using MassTransit;

namespace ServiceB;

public class Consumer : IConsumer<Request>
{
    public Task Consume(ConsumeContext<Request> context) => 
        context.Publish(new Approved(context.Message.Id));
}