using Dapr.Client;

namespace ServiceA.Publishers.Dapr;

public class Publisher : IPublisher
{
    private readonly DaprClient _client;

    public Publisher(DaprClient client) => 
        _client = client;

    public Task New(Request request) => 
        _client.PublishEventAsync("my-pubsub", "requests", request);
}