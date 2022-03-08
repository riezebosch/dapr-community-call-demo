namespace ServiceA;

public record Request(string Id, Status Status = Status.Pending);