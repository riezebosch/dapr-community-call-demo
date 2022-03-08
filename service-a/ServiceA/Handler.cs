namespace ServiceA;

public class Handler: IRequests
{
    private readonly IStore _store;
    private readonly IPublisher _publisher;

    public Handler(IStore store, IPublisher publisher)
    {
        _store = store;
        _publisher = publisher;
    }

    public async Task<Request> New(Request request)
    {
        request = await _store.New(request);
        await _publisher.New(request);
        
        return request;
    }

    public async Task Approved(string id) => 
        await _store.Update(await _store.Get(id) with { Status = Status.Approved });

    public Task<Request> Get(string id) => 
        _store.Get(id);
}