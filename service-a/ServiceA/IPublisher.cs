namespace ServiceA;

public interface IPublisher
{
    Task New(Request request);
}