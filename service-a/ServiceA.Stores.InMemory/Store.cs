using HashidsNet;

namespace ServiceA.Stores.InMemory;

public class Store : IStore
{
    private readonly Hashids _hashids = new();
    private Dictionary<string, Request> _requests = new();

    public Task<Request> New(Request request)
    {
        var id = _hashids.Encode(_requests.Count);
        return Task.FromResult(_requests[id] = request with { Id = id });
    }

    public Task<Request> Get(string id) => 
        Task.FromResult(_requests[id]);

    public Task<Request> Update(Request request) =>
        Task.FromResult(_requests[request.Id] = request);
}