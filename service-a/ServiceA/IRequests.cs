namespace ServiceA;

public interface IRequests
{
    Task<Request> New(Request request);
    Task Approved(string id);
    Task<Request> Get(string id);
}