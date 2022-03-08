namespace ServiceA;

public interface IStore
{
    Task<Request> Get(string id);
    Task<Request> New(Request request);
    Task<Request> Update(Request request);
}